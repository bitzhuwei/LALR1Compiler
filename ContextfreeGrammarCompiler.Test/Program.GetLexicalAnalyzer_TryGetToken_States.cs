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
                    GetPunctuation(result, node);
                }
            }

            return result;
        }

        private static void GetPunctuation(List<LexiState> result, TreeNodeType node)
        {
            string content = node.Content;
            SourceCodeCharType charType = content[0].GetCharType();
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(charType))
                {
                    state.GetTokenList.TryInsert(new GetPunctuation(content));
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(charType);
                state.GetTokenList.TryInsert(new GetPunctuation(content));
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
                state.GetTokenList.TryInsert(new GetChar());
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
                state.GetTokenList.TryInsert(new GetConstString());
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
                state.GetTokenList.TryInsert(new GetNumber());
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
                state.GetTokenList.TryInsert(new GetIdentifier());
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
            unknown.getTokenList.TryInsert(new GetUnknown());
            return unknown;
        }


        private List<SourceCodeCharType> charTypeList = new List<SourceCodeCharType>();

        internal List<SourceCodeCharType> CharTypeList
        {
            get { return charTypeList; }
        }

        private OrderedCollection<GetTokenBase> getTokenList = new OrderedCollection<GetTokenBase>(Environment.NewLine);

        internal OrderedCollection<GetTokenBase> GetTokenList
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

    abstract class GetTokenBase : HashCache
    {

        public abstract CodeMemberMethod DumpMethodDefinition();

        public GetTokenBase() : base(GetUniqueString) { }

        static string GetUniqueString(HashCache cache)
        {
            GetTokenBase obj = cache as GetTokenBase;
            return obj.Dump();
        }

    }

    class GetPunctuation : GetTokenBase
    {

        public GetPunctuation(string value)
        {
            this.Value = value;
        }

        public override CodeMemberMethod DumpMethodDefinition()
        {
            // TODO:
            throw new NotImplementedException();
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write(this.Value);
        }

        public string Value { get; set; }

        public override int CompareTo(HashCache other)
        {
            GetPunctuation obj = other as GetPunctuation;
            if ((Object)obj == null) { return -1; }

            return -(this.Value.Length - obj.Value.Length);
        }

    }
    class GetIdentifier : GetTokenBase
    {


        public override void Dump(System.IO.TextWriter stream)
        {
            throw new NotImplementedException();
        }

        public override CodeMemberMethod DumpMethodDefinition()
        {
            return null;
        }

    }

    class GetNumber : GetTokenBase
    {

        public override CodeMemberMethod DumpMethodDefinition()
        {
            return null;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            throw new NotImplementedException();
        }

    }

    class GetConstString : GetTokenBase
    {

        public override CodeMemberMethod DumpMethodDefinition()
        {
            return null;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            throw new NotImplementedException();
        }

    }

    class GetChar : GetTokenBase
    {

        public override CodeMemberMethod DumpMethodDefinition()
        {
            return null;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            throw new NotImplementedException();
        }

    }

    class GetUnknown : GetTokenBase
    {

        public override CodeMemberMethod DumpMethodDefinition()
        {
            return null;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            throw new NotImplementedException();
        }

    }
}
