using LALR1Compiler;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    /// <summary>
    /// 词法分析器是个自动机。这个类型表示他的一个状态。一个状态会产生一个gotToken = GetXXX();及GetXXX()的定义。
    /// </summary>
    class LexiState
    {

        /// <summary>
        /// 处理注释状态的字符。
        /// </summary>
        /// <returns></returns>
        public static DivideState GetCommentState()
        {
            var slash = new LexiState();
            slash.charTypeList.Add(SourceCodeCharType.Divide);
            return new DivideState(slash);
        }

        /// <summary>
        /// 处理Unknown状态的字符。
        /// </summary>
        /// <returns></returns>
        public static LexiState GetSpaceState()
        {
            var unknown = new LexiState();
            unknown.charTypeList.Add(SourceCodeCharType.Space);
            unknown.getTokenList.TryInsert(new CodeGetSpace());
            return unknown;
        }

        /// <summary>
        /// 处理Unknown状态的字符。
        /// </summary>
        /// <returns></returns>
        public static LexiState GetUnknownState()
        {
            var unknown = new LexiState();
            unknown.charTypeList.Add(SourceCodeCharType.Unknown);
            unknown.getTokenList.TryInsert(new CodeGetUnknown());
            return unknown;
        }

        /// <summary>
        /// 哪些类型的char应该在此状态进行处理？
        /// </summary>
        protected List<SourceCodeCharType> charTypeList = new List<SourceCodeCharType>();

        /// <summary>
        /// 哪些类型的char应该在此状态进行处理？
        /// </summary>
        internal List<SourceCodeCharType> CharTypeList
        {
            get { return charTypeList; }
        }

        /// <summary>
        /// 哪些类型的单词会由此状态产生？
        /// </summary>
        protected CodeGetTokenCollection getTokenList = new CodeGetTokenCollection();

        /// <summary>
        /// 哪些类型的单词会由此状态产生？
        /// </summary>
        internal CodeGetTokenCollection GetTokenList
        {
            get { return getTokenList; }
        }

        /// <summary>
        /// charType == SourceCodeCharType.Letter|| charType == SourceCodeCharType.UnderLine
        /// <para>获取类似如上的代码</para>
        /// </summary>
        /// <returns></returns>
        public CodeExpression GetCondition()
        {
            // charType == SourceCodeCharType.Letter
            //    || charType == SourceCodeCharType.UnderLine
            var first = new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression("charType"),
               CodeBinaryOperatorType.IdentityEquality,
               new CodeSnippetExpression("SourceCodeCharType." + this.charTypeList[0]));
            for (int i = 1; i < this.charTypeList.Count; i++)
            {
                var expression = new CodeBinaryOperatorExpression(
                    first, CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression("charType"),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodeSnippetExpression("SourceCodeCharType." + this.charTypeList[i])));
                first = expression;
            }

            return first;
        }

        /// <summary>
        /// 获取GetXXX()的定义
        /// </summary>
        /// <returns></returns>
        public CodeMemberMethod GetMethodDefinitionStatement(string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            if (this.IsContextfreeGrammarKeyword()) { return null; }

            var method = new CodeMemberMethod();
            method.Name = string.Format("Get{0}", this.GetTokenName());
            method.Attributes = MemberAttributes.Family;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Token), "result"));
            method.Parameters.Add(new CodeParameterDeclarationExpression("AnalyzingContext", "context"));
            {
                // var count = context.SourceCode.Length;
                var count = new CodeVariableDeclarationStatement(typeof(int), "count");
                count.InitExpression = new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("context"), "SourceCode"), "Length");
                method.Statements.Add(count);
            }
            {
                CodeStatement[] statements = this.GetTokenList.DumpReadToken(grammarId, algorithm);
                method.Statements.AddRange(statements);
            }
            {
                // return false;
                var returnFalse = new CodeMethodReturnStatement(new CodePrimitiveExpression(false));
                method.Statements.Add(returnFalse);
            }

            return method;
        }

        /// <summary>
        /// 如果是ContextfreeGrammar的关键字，那么不需要产生其函数定义。（因为已经有固定的定义了）
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsContextfreeGrammarKeyword()
        {
            SourceCodeCharType charType = this.charTypeList[0];
            // identifier
            if (charType == SourceCodeCharType.UnderLine) { return true; }
            if (charType == SourceCodeCharType.Letter) { return true; }
            // number
            if (charType == SourceCodeCharType.Number) { return true; }
            // constString
            if (charType == SourceCodeCharType.DoubleQuotation) { return true; }
            // char
            if (charType == SourceCodeCharType.Quotation) { return true; }
            // null
            // null is not a node.

            return false;
        }

        /// <summary>
        /// gotToken = GetXXX(result, context);
        /// <para>return gotToken;</para>
        /// <para>获取类似上述的代码。</para>
        /// </summary>
        /// <returns></returns>
        public CodeStatement[] GetMethodInvokeStatement()
        {
            var statements = new CodeStatement[2];

            // gotToken = GetXXX(result, context);
            // return gotToken;
            var gotToken = new CodeVariableReferenceExpression("gotToken");
            var callMethod = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                string.Format("Get{0}", this.GetTokenName()),
                new CodeVariableReferenceExpression("result"),
                new CodeVariableReferenceExpression("context"));
            statements[0] = new CodeAssignStatement(gotToken, callMethod);
            statements[1] = new CodeMethodReturnStatement(gotToken);
            return statements;
        }

        /// <summary>
        /// 获取字符的名字，用作GetXXX()的函数名字的一部分。
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTokenName()
        {
            if (this.charTypeList.Count > 1)
            {
                if (this.charTypeList.Contains(SourceCodeCharType.UnderLine)
                    && this.charTypeList.Contains(SourceCodeCharType.Letter))
                {
                    return "Identifier";
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (this.charTypeList.Count == 1)
            {
                return this.charTypeList[0].ToString();
            }
            else
            {
                throw new Exception();
            }
        }

        internal bool Contains(SourceCodeCharType sourceCodeCharType)
        {
            return this.charTypeList.Contains(sourceCodeCharType);
        }
    }

    /// <summary>
    /// 特殊处理这个要对付注释的符号"/"
    /// </summary>
    class DivideState : LexiState
    {
        public DivideState(LexiState state)
        {
            this.charTypeList = state.CharTypeList;
            this.getTokenList = new CommentCodeGetTokenCollection(state.GetTokenList);
        }

        protected override string GetTokenName()
        {
            return ConstString2IdentifierHelper.ConstString2Identifier("/");
        }

        protected override bool IsContextfreeGrammarKeyword()
        {
            return false;
        }
    }

}
