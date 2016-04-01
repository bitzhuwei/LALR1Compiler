using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LALR1Compiler;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace ContextfreeGrammarCompiler.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] directories = Directory.GetDirectories(
                ".", "*.Grammar", SearchOption.AllDirectories)
                .OrderBy(x =>
                    Directory.GetFiles(x, "*.Grammar.txt", SearchOption.TopDirectoryOnly)[0].Length)
                    .ToArray();
            foreach (var directory in directories)
            {
                Run(directory);
            }
        }

        private static void Run(string directory)
        {
            string[] grammarFullnames = Directory.GetFiles(directory, "*.Grammar.txt", SearchOption.TopDirectoryOnly);
            foreach (var grammarFullname in grammarFullnames)
            {
                ProcessGrammar(grammarFullname);
            }
        }

        private static void ProcessGrammar(string grammarFullname)
        {
            Console.WriteLine("Processing {0}", grammarFullname);

            FileInfo fileInfo = new FileInfo(grammarFullname);
            string directory = fileInfo.DirectoryName;
            string grammarId = fileInfo.Name.Substring(fileInfo.Name.Length - (".Grammar.txt").Length);

            string sourceCode = File.ReadAllText(grammarFullname);
            RegulationList grammar = GetGrammar(sourceCode, directory, grammarId);

            DumpCode(grammar, directory, grammarId);
        }

        /// <summary>
        /// 生成给定文法的compiler。
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="directory"></param>
        /// <param name="grammarId"></param>
        private static void DumpCode(RegulationList grammar, string directory, string grammarId)
        {
            Dictionary<TreeNodeType, bool> nullableDict;
            grammar.GetNullableDict(out nullableDict);
            FIRSTCollection firstCollection;
            grammar.GetFirstCollection(out firstCollection, nullableDict);
            using (StreamWriter stream = new StreamWriter(Path.Combine(directory, grammarId + ".FIRST.txt")))
            { firstCollection.Dump(stream); }
            FOLLOWCollection followCollection;
            grammar.GetFollowCollection(out followCollection, nullableDict, firstCollection);
            using (StreamWriter stream = new StreamWriter(Path.Combine(directory, grammarId + ".FOLLOW.txt")))
            { followCollection.Dump(stream); }

            LRParsingMap LR0Map = grammar.GetLR0ParsingMap();
            DumpLR0Code(grammar, LR0Map, directory, grammarId);

            LRParsingMap SLRMap = grammar.GetSLRParsingMap();
            DumpSLRCode(grammar, SLRMap, directory, grammarId);

            LRParsingMap LR1Map = grammar.GetLR1ParsingMap();
            DumpLR1Code(grammar, LR1Map, directory, grammarId);
        }

        private static void DumpLR1Code(
            RegulationList grammar, 
            LRParsingMap LR1Map,
            string directory, string grammarId)
        {
            string LR0Directory = Path.Combine(directory, "LR(1)");

            DumpSyntaxParserCode(grammar, LR1Map, grammarId, LR0Directory);
        }

        private static void DumpSLRCode(
            RegulationList grammar,
            LRParsingMap SLRMap,
            string directory, string grammarId)
        {
            string LR0Directory = Path.Combine(directory, "SLR");

            DumpSyntaxParserCode(grammar, SLRMap, grammarId, LR0Directory);
        }


        private static void DumpLR0Code(
            RegulationList grammar,
            LRParsingMap LR0Map,
            string directory, string grammarId)
        {
            string LR0Directory = Path.Combine(directory, "LR(0)");

            DumpSyntaxParserCode(grammar, LR0Map, grammarId, LR0Directory);
        }

        private static void DumpSyntaxParserCode(RegulationList grammar, LRParsingMap map, string grammarId, string LR0Directory)
        {
            var parserType = new CodeTypeDeclaration(GetParserName(grammarId));
            parserType.IsClass = true;
            DumpSyntaxParserFields(grammar, parserType);
            DumpSyntaxParserMethod_GetGrammar(grammar, parserType);
            DumpSyntaxParserMethod_GetParsingMap(grammar, map, parserType);

            var parserNamespace = new CodeNamespace(GetNamespace(grammarId));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Object).Namespace));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Collections.Generic.List<int>).Namespace));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(LALR1Compiler.HashCache).Namespace));
            parserNamespace.Types.Add(parserType);

            //生成代码  
            string parserCodeFullname = Path.Combine(LR0Directory, GetParserName(grammarId) + ".cs");
            using (var sw = new StreamWriter(parserCodeFullname, false))
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CodeGeneratorOptions geneOptions = new CodeGeneratorOptions();//代码生成选项
                geneOptions.BlankLinesBetweenMembers = true;
                geneOptions.BracingStyle = "C";
                geneOptions.ElseOnClosing = false;
                geneOptions.IndentString = "    ";
                geneOptions.VerbatimOrder = true;

                codeProvider.GenerateCodeFromNamespace(parserNamespace, sw, geneOptions);
            }
        }

        private static void DumpSyntaxParserMethod_GetParsingMap(
            RegulationList grammar,
            LRParsingMap map,
            CodeTypeDeclaration parserType)
        {
            var method = new CodeMemberMethod();
            method.Name = "GetParsingMap";
            method.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            method.ReturnType = new CodeTypeReference(typeof(LRParsingMap));
            string varName = "map";
            var varDeclaration = new CodeVariableDeclarationStatement(typeof(LRParsingMap), varName);
            method.Statements.Add(varDeclaration);
            foreach (var action in map)
            {
                var parametes = new List<CodeExpression>();
                string[] parts = action.Key.Split('+');
                var stateId = new CodePrimitiveExpression(int.Parse(parts[0]));
                var nodeType = new CodeVariableReferenceExpression(parts[1]);
                foreach (var value in action.Value)
                {
                    // action.Value超过1项，就说明有冲突。
                    Type t = value.GetType();
                    CodeObjectCreateExpression ctor = null;
                    string actionParam = value.ActionParam();
                    if (t == typeof(LR1AceptAction))
                    {
                        ctor = new CodeObjectCreateExpression(value.GetType());
                    }
                    else
                    {
                        ctor = new CodeObjectCreateExpression(t,
                            new CodePrimitiveExpression(int.Parse(actionParam)));
                    }

                    var SetActionMethod = new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression(varName), "SetAction",
                        new CodeTypeReference(typeof(int)),
                        new CodeTypeReference(typeof(TreeNodeType)),
                        new CodeTypeReference(typeof(LRParsingAction)));
                    var setAction = new CodeMethodInvokeExpression(
                        SetActionMethod, stateId, nodeType, ctor);
                    method.Statements.Add(setAction);
                }
            }
            {
                var returnMap = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(varName));
                method.Statements.Add(returnMap);
            }

            parserType.Members.Add(method);
        }

        private static void DumpSyntaxParserMethod_GetGrammar(RegulationList grammar, CodeTypeDeclaration parserType)
        {
            var method = new CodeMemberMethod();
            method.Name = "GetGrammar";
            method.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            method.ReturnType = new CodeTypeReference(typeof(RegulationList));
            string varName = "grammar";
            var varDeclaration = new CodeVariableDeclarationStatement(typeof(RegulationList), varName);
            method.Statements.Add(varDeclaration);
            foreach (var regulation in grammar)
            {
                //string str = regulation.Dump();
                //var comment = new CodeComment(string.Format("// {0}", str));
                var parametes = new List<CodeExpression>();
                parametes.Add(new CodeVariableReferenceExpression(regulation.Left.Type));
                parametes.AddRange(from item in regulation.RightPart select new CodeVariableReferenceExpression(item.Type));
                var ctor = new CodeObjectCreateExpression(typeof(Regulation), parametes.ToArray());
                var AddMethod = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression(varName), "Add", new CodeTypeReference(typeof(Regulation)));
                var addRegulation = new CodeMethodInvokeExpression(AddMethod, ctor);
                method.Statements.Add(addRegulation);
            }
            {
                var returnGrammar = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(varName));
                method.Statements.Add(returnGrammar);
            }

            parserType.Members.Add(method);
        }

        private static void DumpSyntaxParserFields(RegulationList grammar, CodeTypeDeclaration parserType)
        {
            foreach (var node in grammar.GetAllTreeNodeNonLeaveTypes())
            {
                CodeMemberField field = new CodeMemberField(typeof(TreeNodeType), node.Type);
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                var ctor = new CodeObjectCreateExpression(typeof(TreeNodeType),
                    new CodePrimitiveExpression(node.Type),
                    new CodePrimitiveExpression(node.Content),
                    new CodePrimitiveExpression(node.Nickname));
                field.InitExpression = ctor;
                parserType.Members.Add(field);
            }
            foreach (var node in grammar.GetAllTreeNodeLeaveTypes())
            {
                CodeMemberField field = new CodeMemberField(typeof(TreeNodeType), node.Type);
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                var ctor = new CodeObjectCreateExpression(typeof(TreeNodeType),
                    new CodePrimitiveExpression(node.Type),
                    new CodePrimitiveExpression(node.Content),
                    new CodePrimitiveExpression(node.Nickname));
                field.InitExpression = ctor;
                parserType.Members.Add(field);
            }
            {
                var node = TreeNodeType.endOfTokenListNode;
                CodeMemberField field = new CodeMemberField(typeof(TreeNodeType), node.Type);
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                var ctor = new CodeObjectCreateExpression(typeof(TreeNodeType),
                    new CodePrimitiveExpression(node.Type),
                    new CodePrimitiveExpression(node.Content),
                    new CodePrimitiveExpression(node.Nickname));
                field.InitExpression = ctor;
                parserType.Members.Add(field);
            }
        }

        private static string GetParserName(string grammarId)
        {
            return string.Format("{0}SyntaxParser", grammarId);
        }

        private static string GetNamespace(string grammarId)
        {
            return string.Format("{0}Compiler", grammarId);
        }

        /// <summary>
        /// ContextfreeGrammar compiler的工作过程。
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="directory"></param>
        /// <param name="grammarId"></param>
        /// <returns></returns>
        private static RegulationList GetGrammar(string sourceCode, string directory, string grammarId)
        {
            var lexi = new ContextfreeGrammarLexicalAnalyzer();
            TokenList tokenList = lexi.Analyze(sourceCode);
            Console.WriteLine("Dump token list...");
            tokenList.Dump(Path.Combine(directory, grammarId + ".TokenList.txt"));
            var parser = new ContextfreeGrammarSyntaxParser();
            SyntaxTree tree = parser.Parse(tokenList);
            Console.WriteLine("Dump syntax tree...");
            tree.Dump(Path.Combine(directory, grammarId + ".Tree.txt"));
            RegulationList grammar = tree.DumpGrammar();
            Console.WriteLine("Dump formated grammar...");
            grammar.Dump(Path.Combine(directory, grammarId + ".FormatedGrammar.txt"));
            return grammar;
        }
    }
}
