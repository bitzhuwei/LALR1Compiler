using LALR1Compiler;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    partial class Program
    {
        private static void DumpLexicalAnalyzerCode(RegulationList grammar,
            string grammarId, SyntaxParserMapAlgorithm algorithm, string directory)
        {
            var lexiType = new CodeTypeDeclaration(Utilities.GetLexicalAnalyzerName(grammarId));
            lexiType.IsClass = true;
            lexiType.IsPartial = true;
            lexiType.BaseTypes.Add(typeof(LexicalAnalyzer));
            DumpLexicalAnalyzer_TryGetToken(grammar, lexiType);
            DumpLexicalAnalyzer_GetSymbol(grammar, lexiType,grammarId, algorithm);
            DumpLexicalAnalyzer_GetKeywordList(grammar, grammarId, lexiType);

            var lexiNamespace = new CodeNamespace(Utilities.GetNamespace(grammarId));
            lexiNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Object).Namespace));
            lexiNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Collections.Generic.List<int>).Namespace));
            lexiNamespace.Imports.Add(new CodeNamespaceImport(typeof(LALR1Compiler.HashCache).Namespace));
            lexiNamespace.Types.Add(lexiType);

            //生成代码  
            string parserCodeFullname = Path.Combine(directory, Utilities.GetLexicalAnalyzerName(grammarId) + ".cs");
            using (var stream = new StreamWriter(parserCodeFullname, false))
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CodeGeneratorOptions geneOptions = new CodeGeneratorOptions();//代码生成选项
                geneOptions.BlankLinesBetweenMembers = true;
                geneOptions.BracingStyle = "C";
                geneOptions.ElseOnClosing = false;
                geneOptions.IndentString = "    ";
                geneOptions.VerbatimOrder = true;

                codeProvider.GenerateCodeFromNamespace(lexiNamespace, stream, geneOptions);
            }
        }

        private static void DumpLexicalAnalyzer_GetSymbol(
            RegulationList grammar, CodeTypeDeclaration lexiType,
            string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            List<LexiState> lexiStateList = grammar.GetLexiStateList();
            DivideState commentState = null;// 为了处理注释，"/"符号要特殊对待。
            foreach (var state in lexiStateList)
            {
                if (state.CharTypeList.Contains(SourceCodeCharType.Divide))
                {
                    commentState = new DivideState(state);
                    continue;
                }

                CodeMemberMethod method = state.GetMethodDefinitionStatement(grammarId, algorithm);
                if (method != null)
                {
                    lexiType.Members.Add(method);
                }
            }
            {
                if (commentState == null)
                { commentState = LexiState.GetCommentState(); }
                CodeMemberMethod method = commentState.GetMethodDefinitionStatement(grammarId, algorithm);
                if (method != null)
                {
                    lexiType.Members.Add(method);
                }
            }
        }

        private static void DumpLexicalAnalyzer_GetKeywordList(
            RegulationList grammar, string grammarId, CodeTypeDeclaration lexiType)
        {
            {
                // private static readonly List<Keyword> keywords = new List<Keyword>();
                var field = new CodeMemberField(typeof(List<Keyword>), "keywords");
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                field.InitExpression = new CodeObjectCreateExpression(typeof(List<Keyword>));
                lexiType.Members.Add(field);
            }
            {
                // protected override IEnumerable<Keyword> GetKeywords()
                var method = new CodeMemberMethod();
                method.Name = "GetKeywords";
                method.Attributes = MemberAttributes.Public | MemberAttributes.Override;
                method.ReturnType = new CodeTypeReference(typeof(IEnumerable<Keyword>));
                var returnKeywords = new CodeMethodReturnStatement(
                    new CodeVariableReferenceExpression("keywords"));
                method.Statements.Add(returnKeywords);
                lexiType.Members.Add(method);
            }
            {
                // static DemoLexicalAnalyzer()
                var method = new CodeTypeConstructor();
                method.Name = Utilities.GetLexicalAnalyzerName(grammarId);
                method.Attributes = MemberAttributes.Static;
                //{
                //    // List<Keyword> keyword = new List<Keyword>();
                //    var keyword = new CodeVariableDeclarationStatement("List<Keyword>", "keyword");
                //    keyword.InitExpression = new CodeObjectCreateExpression("List<Keyword>");
                //    method.Statements.Add(keyword);
                //}
                var convertor = new TreeNodeType2TokenType();
                foreach (var node in grammar.GetAllTreeNodeLeaveTypes())
                {
                    // keywords.Add(new Keyword("__x", "x"));
                    if (node.IsIdentifier())
                    {
                        TokenType tokenType = convertor.GetTokenType(node);
                        var ctor = new CodeObjectCreateExpression(typeof(Keyword),
                            new CodePrimitiveExpression(tokenType.Type),
                            new CodePrimitiveExpression(tokenType.Content));
                        var add = new CodeMethodInvokeExpression(
                            new CodeVariableReferenceExpression("keywords"),
                            "Add",
                            ctor);
                        method.Statements.Add(add);
                    }
                }
                //{
                //    // DemoLexicalAnalyzer.keywords = keywords;
                //    var assign = new CodeAssignStatement(
                //        new CodeFieldReferenceExpression(
                //            new CodeSnippetExpression(GetLexicalAnalyzerName(grammarId)), "keywords"),
                //        new CodeVariableReferenceExpression("keywords"));
                //    method.Statements.Add(assign);
                //}
                lexiType.Members.Add(method);
            }
        }

        private static void DumpLexicalAnalyzer_TryGetToken(
            RegulationList grammar, CodeTypeDeclaration lexiType)
        {
            var method = new CodeMemberMethod();
            method.Name = "TryGetToken";
            method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Parameters.Add(new CodeParameterDeclarationExpression("AnalyzingContext", "context"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Token), "result"));
            method.Parameters.Add(new CodeParameterDeclarationExpression("SourceCodeCharType", "charType"));
            {
                // bool gotToken = false;
                string vargotToken = "gotToken"; // :(
                var varDeclaration = new CodeVariableDeclarationStatement(typeof(bool), vargotToken);
                varDeclaration.InitExpression = new CodePrimitiveExpression(false);
                method.Statements.Add(varDeclaration);
            }
            {
                List<LexiState> lexiStateList = grammar.GetLexiStateList();
                DivideState commentState = null;// 为了处理注释，"/"符号要特殊对待。
                foreach (var state in lexiStateList)
                {
                    if (state.CharTypeList.Contains(SourceCodeCharType.Divide))
                    {
                        commentState = new DivideState(state);
                        continue;
                    }

                    var condition = new CodeConditionStatement(
                        state.GetCondition(),
                        state.GetMethodInvokeStatement());
                    method.Statements.Add(condition);
                }
                {
                    if (commentState == null)
                    {
                        commentState = LexiState.GetCommentState();
                    }
                    var condition = new CodeConditionStatement(
                        commentState.GetCondition(),
                        commentState.GetMethodInvokeStatement());
                    method.Statements.Add(condition);
                }
                {
                    var getSpaceState = LexiState.GetSpaceState();
                    var condition = new CodeConditionStatement(
                        getSpaceState.GetCondition(),
                        getSpaceState.GetMethodInvokeStatement());
                    method.Statements.Add(condition);
                }
                {
                    var getUnknownState = LexiState.GetUnknownState();
                    var lastCondition = getUnknownState.GetMethodInvokeStatement();
                    method.Statements.AddRange(lastCondition);
                }
            }

            lexiType.Members.Add(method);
        }
    }
}
