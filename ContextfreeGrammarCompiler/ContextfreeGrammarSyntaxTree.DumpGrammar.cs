using LALR1Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler
{
    public static class ContextfreeGrammarSyntaxTreeHelper
    {
        /// <summary>
        /// 遍历语法树，导出其中描述的文法。
        /// </summary>
        /// <returns></returns>
        public static RegulationList DumpGrammar(this SyntaxTree syntaxTree)
        {
            RegulationList grammar = new RegulationList();

            // <Grammar> ::= <Production> <ProductionList> ;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__Grammar)
            {
                List<Regulation> production = GetProduction(syntaxTree.Children[0]);
                grammar.AddRange(production);
                RegulationList regulationList = GetProductionList(syntaxTree.Children[1]);
                grammar.AddRange(regulationList);
            }

            return grammar;
        }

        private static RegulationList GetProductionList(SyntaxTree syntaxTree)
        {
            RegulationList result = null;

            // <ProductionList> ::= <Production> <RegulationList> | null ;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__ProductionList)
            {
                if (syntaxTree.Children.Count == 2)
                {
                    // <ProductionList> ::= <Production> <RegulationList> ;
                    List<Regulation> production = GetProduction(syntaxTree.Children[0]);
                    RegulationList regulationList = GetProductionList(syntaxTree.Children[1]);
                    result = new RegulationList();
                    result.AddRange(production);
                    result.AddRange(regulationList);
                }
                else if (syntaxTree.Children.Count == 0)
                {
                    // <ProductionList> ::= null ;
                    result = new RegulationList();
                }
            }

            return result;
        }

        private static List<Regulation> GetProduction(SyntaxTree syntaxTree)
        {
            // <Production> ::= <Vn> "::=" <Canditate> <RightPartList> ;
            List<Regulation> result = new List<Regulation>();

            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__Production)
            {
                Vn left = GetVn(syntaxTree.Children[0]);
                TreeNodeType leftNode = left.GetTreeNodeType();
                {
                    Candidate candidate = GetCandidate(syntaxTree.Children[2]);
                    var regulation = new Regulation(leftNode,
                        (from item in candidate
                         select item.GetTreeNodeType()).ToArray());
                    result.Add(regulation);
                }
                RightPartList rightPartList = GetRightPartList(syntaxTree.Children[3]);
                foreach (var candidate in rightPartList)
                {
                    var regulation = new Regulation(leftNode,
                        (from item in candidate
                         select item.GetTreeNodeType()).ToArray());
                    result.Add(regulation);
                }
            }

            return result;
        }

        private static RightPartList GetRightPartList(SyntaxTree syntaxTree)
        {
            RightPartList result = null;

            // <RightPartList> ::= "|" <Canditate> <RightPartList> | null ;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__RightPartList)
            {
                if (syntaxTree.Children.Count == 3)
                {
                    // <RightPartList> ::= "|" <Canditate> <RightPartList> ;
                    Candidate candidate = GetCandidate(syntaxTree.Children[1]);
                    result = new RightPartList();
                    result.Add(candidate);
                    RightPartList list = GetRightPartList(syntaxTree.Children[2]);
                    result.AddRange(list);
                }
                else if (syntaxTree.Children.Count == 0)
                {
                    // <RightPartList> ::= null ;
                    result = new RightPartList();
                }
            }

            return result;
        }

        class RightPartList : List<Candidate>
        {
        }

        private static Candidate GetCandidate(SyntaxTree syntaxTree)
        {
            // <Canditate> ::= <V> <VList> ;
            Candidate candidate = null;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__Canditate)
            {
                V v = GetV(syntaxTree.Children[0]);
                VList vlist = GetVList(syntaxTree.Children[1]);
                candidate = new Candidate(v, vlist);
            }

            return candidate;
        }

        private static VList GetVList(SyntaxTree syntaxTree)
        {
            VList result = null;

            // <VList> ::= <V> <VList> | null ;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__VList)
            {
                // <VList> ::= <V> <VList> ;
                if (syntaxTree.Children.Count == 2)
                {
                    V v = GetV(syntaxTree.Children[0]);
                    VList vlist = GetVList(syntaxTree.Children[1]);
                    result = new VList(v, vlist);
                }
                else if (syntaxTree.Children.Count == 0)
                {
                    // <VList> ::= null ;
                    result = new VList();
                }
            }

            return result;
        }

        private static V GetV(SyntaxTree syntaxTree)
        {
            V v = null;
            // <V> ::= <Vn> | <Vt> ;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__V)
            {
                if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarTreeNodeType.__Vn)
                {
                    // <V> ::= <Vn> ;
                    v = GetVn(syntaxTree.Children[0]);
                }
                else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarTreeNodeType.__Vt)
                {
                    // <V> ::= <Vt> ;
                    v = GetVt(syntaxTree.Children[0]);
                }
            }

            return v;
        }

        private static Vt GetVt(SyntaxTree syntaxTree)
        {
            // <Vt> ::= "null" | "identifier" | "number" | "constString" | constString ;
            Vt vt = null;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__Vt)
            {
                if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarTreeNodeType.__nullLeave__)
                {
                    //vt = new KeywordNull();
                    vt = null;
                }
                else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarTreeNodeType.__identifierLeave__)
                {
                    vt = new KeywordIdentifier();
                }
                else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarTreeNodeType.__numberLeave__)
                {
                    vt = new KeywordNumber();
                }
                else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarTreeNodeType.__constStringLeave__)
                {
                    vt = new KeywordConstString();
                }
                else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarTreeNodeType.constStringLeave__)
                {
                    vt = GetConstString(syntaxTree.Children[0]);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return vt;
        }

        private static Vt GetConstString(SyntaxTree syntaxTree)
        {
            ConstString result = null;

            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.constStringLeave__)
            {
                string originalName = syntaxTree.NodeType.Content;
                originalName = originalName.Substring(1, originalName.Length - 2);
                string identifieredName = ConstString2Identifier(originalName);
                result = new ConstString(originalName, identifieredName);
            }

            return result;
        }

        static Dictionary<char, string> punctuationDict = new Dictionary<char, string>();
        static ContextfreeGrammarSyntaxTreeHelper()
        {
            punctuationDict.Add('~', "tilde"); // 波浪字符
            punctuationDict.Add('`', "separation"); // 间隔号
            punctuationDict.Add('!', "bang");
            punctuationDict.Add('@', "at");
            punctuationDict.Add('#', "pound");
            punctuationDict.Add('$', "dollar");
            punctuationDict.Add('%', "percent");
            punctuationDict.Add('^', "caret");
            punctuationDict.Add('&', "and_op");
            punctuationDict.Add('*', "star");
            punctuationDict.Add('(', "left_paren");
            punctuationDict.Add(')', "right_paren");
            punctuationDict.Add('_', "underline");
            punctuationDict.Add('-', "dash");
            punctuationDict.Add('+', "plus");
            punctuationDict.Add('=', "equal");
            punctuationDict.Add('{', "left_brace");
            punctuationDict.Add('[', "left_bracket");
            punctuationDict.Add('}', "right_brace");
            punctuationDict.Add(']', "right_bracket");
            punctuationDict.Add('|', "vertical_bar");
            punctuationDict.Add('\\', "backslash");
            punctuationDict.Add(':', "colon");
            punctuationDict.Add(';', "semicolon");
            punctuationDict.Add('"', "double_quote");
            punctuationDict.Add('\'', "quote");
            punctuationDict.Add('<', "left_angle");
            punctuationDict.Add(',', "comma");
            punctuationDict.Add('>', "right_angle");
            punctuationDict.Add('.', "dot");
            punctuationDict.Add('?', "question");
            punctuationDict.Add('/', "slash");
        }

        private static string ConstString2Identifier(string content)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
            {
                char ch = content[i];
                string part = null;
                if (!punctuationDict.TryGetValue(ch, out part))
                {
                    part = ch.ToString();
                }

                builder.Append(part);
                if (i + 1 < content.Length)
                {
                    bool need = true;
                    if ('a' <= ch && ch <= 'z') { need = false; }
                    if ('A' <= ch && ch <= 'Z') { need = false; }
                    if ('0' <= ch && ch <= '9') { need = false; }

                    if (need)
                    { builder.Append('_'); }
                }
            }

            return builder.ToString();
        }

        class KeywordConstString : Vt
        {
            public override string ToString()
            {
                return GetTreeNodeType().ToString();
            }
            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType(ContextfreeGrammarTreeNodeType.constStringLeave__, "{constString}", "constString");
            }
        }

        class KeywordNumber : Vt
        {
            public override string ToString()
            {
                return GetTreeNodeType().ToString();
            }
            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType(ContextfreeGrammarTreeNodeType.numberLeave__, "{number}", "number");
            }
        }

        class KeywordIdentifier : Vt
        {
            public override string ToString()
            {
                return GetTreeNodeType().ToString();
            }
            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType(ContextfreeGrammarTreeNodeType.identifierLeave__, "{identifier}", "identifier");
            }
        }

        class KeywordNull : Vt
        {
            public override string ToString()
            {
                return GetTreeNodeType().ToString();
            }
            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType(ContextfreeGrammarTreeNodeType.__nullLeave__, "null", "null");
            }
        }

        class ConstString : Vt
        {

            public string OriginalContent { get; set; }

            public string IdentifieredName { get; set; }

            public ConstString(string originalConetnt, string identifieredName)
            {
                this.OriginalContent = originalConetnt;
                this.IdentifieredName = identifieredName;
            }
            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType(
                    "__" + IdentifieredName + "Leave__",
                    OriginalContent, "\"" + OriginalContent + "\"");
                //return new TreeNodeType(ContextfreeGrammarTreeNodeType.constStringLeave__, Content, Content);
            }
            public override string ToString()
            {
                return GetTreeNodeType().ToString();
            }

        }

        private static Vn GetVn(SyntaxTree syntaxTree)
        {
            // <Vn> ::= "<" identifier ">" ;
            Vn vn = null;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarTreeNodeType.__Vn)
            {
                string identifierContent = syntaxTree.Children[1].NodeType.Content;
                vn = new Vn(identifierContent);
            }

            return vn;
        }

        abstract class Vt : V
        {

        }

        class Vn : V
        {
            private string identifierContent;

            public string IdentifierContent
            {
                get { return identifierContent; }
                set { identifierContent = value; }
            }

            public Vn(string identifierContent)
            {
                this.identifierContent = identifierContent;
            }

            public override string ToString()
            {
                return string.Format("Vn: <{0}>", IdentifierContent);
            }

            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType("__" + IdentifierContent, IdentifierContent, "<" + IdentifierContent + ">");
            }
        }

        class Candidate : IEnumerable<V>
        {
            List<V> array = new List<V>();

            public Candidate(V v, VList vlist)
            {
                if (v != null)
                {
                    this.array.Add(v);
                }
                this.array.AddRange(vlist);
            }

            public IEnumerator<V> GetEnumerator()
            {
                foreach (var item in array)
                {
                    yield return item;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        abstract class V
        {
            public abstract TreeNodeType GetTreeNodeType();
        }

        class VList : IEnumerable<V>
        {
            List<V> array = new List<V>();

            public VList(V v, VList vlist)
            {
                if (v != null)
                {
                    array.Add(v);
                }
                array.AddRange(vlist);
            }

            public VList(params V[] array)
            {
                this.array.AddRange(array);
            }

            public IEnumerator<V> GetEnumerator()
            {
                foreach (var item in array)
                {
                    yield return item;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
