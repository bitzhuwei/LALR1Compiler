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

    class DivideGetTokenList : GetTokenList
    {
        public DivideGetTokenList(GetTokenList getTokenList)
        {
            foreach (var item in getTokenList)
            {
                this.TryInsert(item);
            }
        }

        internal override CodeStatement[] DumpReadToken()
        {
            List<CodeStatement> result = new List<CodeStatement>();
            if (this.Count > 0)// 有除了"//"和"/*"之外的项，例如可能是"/", "/="
            {
                OtherItemsAndNote(result);
            }
            else
            {
                OnlyNote(result);
            }
            return result.ToArray();
        }

        private static void OnlyNote(List<CodeStatement> result)
        {
            int length = 2;
            var ifStatement = new CodeConditionStatement(
                new CodeSnippetExpression(string.Format(
                    "context.NextLetterIndex + {0} <= count", length)));
            {
                var str = new CodeVariableDeclarationStatement(typeof(string), "str");
                str.InitExpression = new CodeSnippetExpression(string.Format(
                    "context.SourceCode.Substring(context.NextLetterIndex, {0});", length));
                ifStatement.TrueStatements.Add(str);
            }
            SingleLine(ifStatement);
            MultiLine(ifStatement);

            result.Add(ifStatement);
        }

        private void OtherItemsAndNote(List<CodeStatement> result)
        {
            for (int length = this[0].Value.Type.Length; length > 0; length--)
            {
                bool exists = false;
                // if (context.NextLetterIndex + {0} <= count)
                var ifStatement = new CodeConditionStatement(
                    new CodeSnippetExpression(string.Format(
                        "context.NextLetterIndex + {0} <= count", length)));
                {
                    // str = context.SourceCode.SubString(context.NextLetterIndex, {0});
                    var str = new CodeVariableDeclarationStatement(typeof(string), "str");
                    str.InitExpression = new CodeSnippetExpression(string.Format(
                        "context.SourceCode.Substring(context.NextLetterIndex, {0});", length));
                    ifStatement.TrueStatements.Add(str);
                }
                foreach (var item in this)
                {
                    // if ("xxx" == str) { ... }
                    if (item.Value.Content.Length != length) { continue; }

                    CodeStatement[] statements = item.DumpReadToken();
                    if (statements != null)
                    {
                        ifStatement.TrueStatements.AddRange(statements);
                        exists = true;
                    }
                }

                if (length == 2)// 轮到添加对注释的处理了
                {
                    SingleLine(ifStatement);
                    MultiLine(ifStatement);

                    exists = true;
                }

                if (exists)
                {
                    result.Add(ifStatement);
                }
            }
        }

        private static void MultiLine(CodeConditionStatement ifStatement)
        {
            var multiLine = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodePrimitiveExpression("/*"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeVariableReferenceExpression("str")));
            multiLine.TrueStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "SkipMultilineNote",
                    new CodeVariableReferenceExpression("context")));
            ifStatement.TrueStatements.Add(multiLine);
        }

        private static void SingleLine(CodeConditionStatement ifStatement)
        {
            var singleLine = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodePrimitiveExpression("//"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeVariableReferenceExpression("str")));
            singleLine.TrueStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "SkipSingleLineNote",
                    new CodeVariableReferenceExpression("context")));
            ifStatement.TrueStatements.Add(singleLine);
        }
    }
    class GetTokenList : OrderedCollection<CodeGetToken>
    {

        public GetTokenList() : base(Environment.NewLine) { }

        internal virtual CodeStatement[] DumpReadToken()
        {
            List<CodeStatement> result = new List<CodeStatement>();

            for (int length = this[0].Value.Type.Length; length > 0; length--)
            {
                bool exists = false;
                var ifStatement = new CodeConditionStatement(
                    new CodeSnippetExpression(string.Format(
                        "context.NextLetterIndex + {0} <= count", length)));
                {
                    var str = new CodeVariableDeclarationStatement(typeof(string), "str");
                    str.InitExpression = new CodeSnippetExpression(string.Format(
                        "context.SourceCode.Substring(context.NextLetterIndex, {0});", length));
                    ifStatement.TrueStatements.Add(str);
                }
                foreach (var item in this)
                {
                    if (item.Value.Content.Length != length) { continue; }

                    CodeStatement[] statements = item.DumpReadToken();
                    if (statements != null)
                    {
                        ifStatement.TrueStatements.AddRange(statements);
                        exists = true;
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
