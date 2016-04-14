using LALR1Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler2
{
    public static class ContextfreeGrammarSLRSyntaxTreeHelper
    {
        /// <summary>
        /// 遍历语法树，导出其中描述的文法。
        /// </summary>
        /// <returns></returns>
        public static RegulationList DumpGrammar(this SyntaxTree syntaxTree)
        {
            RegulationList grammar = new RegulationList();

            // <Grammar> ::= <ProductionList> <Production> ;
            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__Grammar)
            { throw new ArgumentException(); }

            {
                List<Regulation> regulationList = GetProductionList(syntaxTree.Children[0]);
                grammar.AddRange(regulationList);
                List<Regulation> production = GetProduction(syntaxTree.Children[1]);
                grammar.AddRange(production);
            }

            return grammar;
        }

        private static List<Regulation> GetProductionList(SyntaxTree syntaxTree)
        {
            List<Regulation> result = null;

            // <ProductionList> ::= <ProductionList> <Production> | null ;
            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__ProductionList)
            { throw new ArgumentException(); }

            if (syntaxTree.Children.Count == 2)
            {
                // <ProductionList> ::= <ProductionList> <Production> ;
                List<Regulation> regulationList = GetProductionList(syntaxTree.Children[0]);
                List<Regulation> production = GetProduction(syntaxTree.Children[1]);
                result = new List<Regulation>();
                result.AddRange(regulationList);
                result.AddRange(production);
            }
            else if (syntaxTree.Children.Count == 0)
            {
                // <ProductionList> ::= null ;
                result = new List<Regulation>();
            }

            return result;
        }

        private static List<Regulation> GetProduction(SyntaxTree syntaxTree)
        {
            // <Production> ::= <Vn> "::=" <Canditate> <RightPartList> ";" ;
            List<Regulation> result = new List<Regulation>();

            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__Production)
            { throw new ArgumentException(); }

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
            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__RightPartList)
            { throw new ArgumentException(); }

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

            return result;
        }

        class RightPartList : List<Candidate>
        {
        }

        private static Candidate GetCandidate(SyntaxTree syntaxTree)
        {
            // <Canditate> ::= <VList> <V> ;
            Candidate candidate = null;
            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__Canditate)
            { throw new ArgumentException(); }

            {
                VList vlist = GetVList(syntaxTree.Children[0]);
                V v = GetV(syntaxTree.Children[1]);
                candidate = new Candidate(vlist, v);
            }

            return candidate;
        }

        private static VList GetVList(SyntaxTree syntaxTree)
        {
            VList result = null;

            // <VList> ::= <VList> <V> | null ;
            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__VList)
            { throw new ArgumentException(); }

            if (syntaxTree.Children.Count == 2)
            {
                // <VList> ::= <VList> <V> ;
                VList vlist = GetVList(syntaxTree.Children[0]);
                V v = GetV(syntaxTree.Children[1]);
                result = new VList(vlist, v);
            }
            else if (syntaxTree.Children.Count == 0)
            {
                // <VList> ::= null ;
                result = new VList();
            }

            return result;
        }

        private static V GetV(SyntaxTree syntaxTree)
        {
            V v = null;
            // <V> ::= <Vn> | <Vt> ;
            if (syntaxTree.NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__V)
            {
                if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__Vn)
                {
                    // <V> ::= <Vn> ;
                    v = GetVn(syntaxTree.Children[0]);
                }
                else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__Vt)
                {
                    // <V> ::= <Vt> ;
                    v = GetVt(syntaxTree.Children[0]);
                }
            }

            return v;
        }

        private static Vt GetVt(SyntaxTree syntaxTree)
        {
            // <Vt> ::= "null" | "identifier" | "number" | "constString"  | "userDefinedType"| constString ;
            Vt vt = null;
            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__Vt)
            { throw new ArgumentException(); }

            if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__nullLeave__)
            {
                // <Vt> ::= "null" ;
                //vt = new KeywordNull();
                vt = null;
            }
            else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__identifierLeave__)
            {
                // <Vt> ::= "identifier" ;
                vt = new KeywordIdentifier();
            }
            else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__EnumberLeave__)
            {
                // <Vt> ::= "number" ;
                vt = new KeywordNumber();
            }
            else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__constStringLeave__)
            {
                // <Vt> ::= "constString" ;
                vt = new KeywordConstString();
            }
            else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODE__userDefinedTypeLeave__)
            {
                // <Vt> ::= "userDefinedType" ;
                vt = new KeywordUserDefinedType();
            }
            else if (syntaxTree.Children[0].NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODEconstStringLeave__)
            {
                // <Vt> ::= constString ;
                vt = GetConstString(syntaxTree.Children[0]);
            }
            else
            {
                throw new NotImplementedException();
            }

            return vt;
        }

        private static Vt GetConstString(SyntaxTree syntaxTree)
        {
            ConstString result = null;

            if (syntaxTree.NodeType.Type == ContextfreeGrammarSLRTreeNodeType.NODEconstStringLeave__)
            {
                string originalName = syntaxTree.NodeType.Content;
                originalName = originalName.Substring(1, originalName.Length - 2);
                string identifieredName = ConstString2IdentifierHelper.ConstString2Identifier(originalName);
                result = new ConstString(originalName, identifieredName);
            }

            return result;
        }

        class KeywordUserDefinedType : Vt
        {
            public override string ToString()
            {
                return GetTreeNodeType().ToString();
            }
            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__userDefinedTypeLeave__, "{userDefinedType}", "userDefinedType");
            }
        }

        class KeywordConstString : Vt
        {
            public override string ToString()
            {
                return GetTreeNodeType().ToString();
            }
            public override TreeNodeType GetTreeNodeType()
            {
                return new TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODEconstStringLeave__, "{constString}", "constString");
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
                return new TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__EnumberLeave__, "{number}", "number");
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
                return new TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODEidentifierLeave__, "{identifier}", "identifier");
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
                return new TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__nullLeave__, "null", "null");
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
                //return new TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODEconstStringLeave__, Content, Content);
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
            if (syntaxTree.NodeType.Type != ContextfreeGrammarSLRTreeNodeType.NODE__Vn)
            { throw new ArgumentException(); }

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

            public Candidate(VList vlist, V v)
            {
                this.array.AddRange(vlist);

                if (v != null)
                {
                    this.array.Add(v);
                }
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

            public VList(VList vlist, V v)
            {
                array.AddRange(vlist);

                if (v != null)
                {
                    array.Add(v);
                }
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
