using LALR1Compiler;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    static partial class Program
    {
        static List<LexiState> GetLexiStateList(this RegulationList grammar)
        {
            var result = new List<LexiState>();
            List<TreeNodeType> nodeList = grammar.GetAllTreeNodeLeaveTypes();
            foreach (var node in nodeList)
            {
                if (node.Type == "identifierLeave__")
                {
                    TryInsertGetIdentifier(result);
                }
                else if (node.Type == "numberLeave__")
                {
                    TryInsertGetNumber(result);
                }
                else if (node.Type == "constStringLeave__")
                {
                    TryInserteGetConstString(result);
                }
                else if (node.Type == "charLeave__")
                {
                    TryInserteGetChar(result);
                }
                else
                {
                    CodeGetRegularToken(result, node);
                }
            }

            return result;
        }

        private static void CodeGetRegularToken(List<LexiState> result, TreeNodeType node)
        {
            string content = node.Content;
            SourceCodeCharType charType = content[0].GetCharType();
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(charType))
                {
                    state.GetTokenList.TryInsert(new CodeGetToken(node));
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(charType);
                state.GetTokenList.TryInsert(new CodeGetToken(node));
                result.Add(state);
            }
        }

        private static void TryInserteGetChar(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.Quotation))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.Quotation);
                state.GetTokenList.TryInsert(new CodeGetChar());
                result.Add(state);
            }
        }

        private static void TryInserteGetConstString(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.DoubleQuotation))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.DoubleQuotation);
                state.GetTokenList.TryInsert(new CodeGetConstString());
                result.Add(state);
            }
        }

        private static void TryInsertGetNumber(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.Number))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.Number);
                state.GetTokenList.TryInsert(new CodeGetNumber());
                result.Add(state);
            }
        }

        private static void TryInsertGetIdentifier(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.Letter))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.Letter);
                state.CharTypeList.Add(SourceCodeCharType.UnderLine);
                state.GetTokenList.TryInsert(new CodeGetIdentifier());
                result.Add(state);
            }
        }
    }

    class LexiState
    {

        public static LexiState GetUnknownState()
        {
            var unknown = new LexiState();
            unknown.charTypeList.Add(SourceCodeCharType.Unknown);
            unknown.getTokenList.TryInsert(new CodeGetUnknown());
            return unknown;
        }


        private List<SourceCodeCharType> charTypeList = new List<SourceCodeCharType>();

        internal List<SourceCodeCharType> CharTypeList
        {
            get { return charTypeList; }
        }

        private GetTokenList getTokenList = new GetTokenList();

        internal GetTokenList GetTokenList
        {
            get { return getTokenList; }
        }

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

        public CodeMemberMethod GetMethodDefinitionStatement()
        {
            if (this.IsContextfreeGrammarKeyword()) { return null; }

            var method = new CodeMemberMethod();
            method.Name = string.Format("Get{0}", this.GetTokenName());
            method.Attributes = MemberAttributes.Family;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            {
                // var count = context.SourceCode.Length;
                var count = new CodeVariableDeclarationStatement(typeof(int), "count");
                count.InitExpression = new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("context"), "SourceCode"), "Length");
                method.Statements.Add(count);
            }
            {
                CodeStatement[] statements = this.GetTokenList.DumpReadToken();
                method.Statements.AddRange(statements);
            }
            {
                // return false;
                var returnFalse = new CodeMethodReturnStatement(new CodePrimitiveExpression(false));
                method.Statements.Add(returnFalse);
            }

            return method;
        }

        private bool IsContextfreeGrammarKeyword()
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

        public CodeStatement[] GetMethodInvokeStatement()
        {
            var statements = new CodeStatement[2];

            // gotToken = GetUnknown(result, context);
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

        private string GetTokenName()
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
                    throw new NotImplementedException();
                }
            }
            else if (this.charTypeList.Count == 1)
            {
                return this.charTypeList[0].ToString();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal bool Contains(SourceCodeCharType sourceCodeCharType)
        {
            return this.charTypeList.Contains(sourceCodeCharType);
        }
    }

    class GetTokenList : OrderedCollection<CodeGetToken>
    {

        public GetTokenList() : base(Environment.NewLine) { }

        internal CodeStatement[] DumpReadToken()
        {
            List<CodeStatement> result = new List<CodeStatement>();

            for (int length = this[0].Value.Type.Length; length > 0; length--)
            {
                bool exists = false;
                var ifStatement = new CodeConditionStatement(
                    new CodeSnippetExpression(string.Format(
                        "context.NextLetterIndex + {0} <= count", length)));
                foreach (var item in this)
                {
                    if (item.Value.Content.Length == length)
                    {
                        CodeStatement[] statements = item.DumpReadToken();
                        if (statements != null)
                        {
                            ifStatement.TrueStatements.AddRange(statements);
                            exists = true;
                        }
                    }
                }
                if (exists)
                {
                    result.Add(ifStatement);
                }
            }

            return result.ToArray();
        }
    }

    class CodeGetToken : HashCache
    {

        public CodeGetToken(TreeNodeType value)
            : base(GetUniqueString)
        {
            this.Value = value;
        }

        public TreeNodeType Value { get; set; }

        public override int CompareTo(HashCache other)
        {
            CodeGetToken obj = other as CodeGetToken;
            if ((Object)obj == null) { return -1; }
            if (obj.Value == null) { return -1; }

            return -(this.Value.Type.Length - obj.Value.Type.Length);
        }

        public virtual CodeStatement[] DumpReadToken()
        {
            List<CodeStatement> list = new List<CodeStatement>();

            {
                int length = this.Value.Content.Length;
                var str = new CodeVariableDeclarationStatement(typeof(string), "str");
                str.InitExpression = new CodeSnippetExpression(string.Format(
                    "context.SourceCode.Substring(context.NextLetterIndex, {0});", length));
                list.Add(str);
            }
            {
                var ifStatement = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodePrimitiveExpression(this.Value.Content),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodeVariableReferenceExpression("str")));
                {
                    var newTokenType = new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodeVariableReferenceExpression("result"),
                            "TokenType"),
                        new CodeObjectCreateExpression(typeof(TokenType),
                            new CodePrimitiveExpression(this.Value.Type),
                            new CodePrimitiveExpression(this.Value.Content),
                            new CodePrimitiveExpression(this.Value.Nickname)));
                    ifStatement.TrueStatements.Add(newTokenType);
                    var pointer = new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("context"), "NextLetterIndex");
                    var incresePointer = new CodeAssignStatement(
                        pointer, new CodeBinaryOperatorExpression(
                            pointer, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(3)));
                    ifStatement.TrueStatements.Add(incresePointer);
                    var returnTrue = new CodeMethodReturnStatement(new CodePrimitiveExpression(true));
                    ifStatement.TrueStatements.Add(returnTrue);
                }
                list.Add(ifStatement);
            }

            return list.ToArray();
        }

        static string GetUniqueString(HashCache cache)
        {
            CodeGetToken obj = cache as CodeGetToken;
            return obj.Dump();
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write(this.Value);
        }

    }

    class CodeGetIdentifier : CodeGetToken
    {

        public CodeGetIdentifier() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }
    }

    class CodeGetNumber : CodeGetToken
    {
        public CodeGetNumber() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }

    class CodeGetConstString : CodeGetToken
    {
        public CodeGetConstString() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }

    class CodeGetChar : CodeGetToken
    {
        public CodeGetChar() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }

    class CodeGetUnknown : CodeGetToken
    {
        public CodeGetUnknown() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }
}
