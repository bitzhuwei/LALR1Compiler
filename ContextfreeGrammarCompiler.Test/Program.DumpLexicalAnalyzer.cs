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
            string grammarId, string directory)
        {
            var lexiType = new CodeTypeDeclaration(GetLexicalAnalyzerName(grammarId));
            lexiType.IsClass = true;
            lexiType.IsPartial = true;
            lexiType.BaseTypes.Add(typeof(LexicalAnalyzer));
            DumpLexicalAnalyzer_TryGetToken(grammar, lexiType);
            DumpLexicalAnalyzer_GetSymbol(grammar, lexiType);
            DumpLexicalAnalyzer_GetKeywordList(grammar, grammarId, lexiType);

            var lexiNamespace = new CodeNamespace(GetNamespace(grammarId));
            lexiNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Object).Namespace));
            lexiNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Collections.Generic.List<int>).Namespace));
            lexiNamespace.Imports.Add(new CodeNamespaceImport(typeof(LALR1Compiler.HashCache).Namespace));
            lexiNamespace.Types.Add(lexiType);

            //生成代码  
            string parserCodeFullname = Path.Combine(directory, GetLexicalAnalyzerName(grammarId) + ".cs");
            using (var sw = new StreamWriter(parserCodeFullname, false))
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CodeGeneratorOptions geneOptions = new CodeGeneratorOptions();//代码生成选项
                geneOptions.BlankLinesBetweenMembers = true;
                geneOptions.BracingStyle = "C";
                geneOptions.ElseOnClosing = false;
                geneOptions.IndentString = "    ";
                geneOptions.VerbatimOrder = true;

                codeProvider.GenerateCodeFromNamespace(lexiNamespace, sw, geneOptions);
            }
        }

        private static void DumpLexicalAnalyzer_GetSymbol(
            RegulationList grammar, CodeTypeDeclaration lexiType)
        {
            List<LexiState> lexiStateList = grammar.GetLexiStateList();
            DivideState divideState = null;// 为了处理注释，"/"符号要特殊对待。
            foreach (var state in lexiStateList)
            {
                if (state.CharTypeList.Contains(SourceCodeCharType.Divide))
                {
                    divideState = new DivideState(state);
                    continue;
                }

                CodeMemberMethod method = state.GetMethodDefinitionStatement();
                if (method != null)
                {
                    lexiType.Members.Add(method);
                }
            }
            {
                if (divideState == null)
                { divideState = new DivideState(new LexiState()); }
                CodeMemberMethod method = divideState.GetMethodDefinitionStatement();
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
                // private static readonly IEnumerable<Keyword> keywords;
                var field = new CodeMemberField("IEnumerable<Keyword>", "keywords");
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                lexiType.Members.Add(field);
            }
            {
                // static DemoLexicalAnalyzer()
                var method = new CodeTypeConstructor();
                method.Name = GetLexicalAnalyzerName(grammarId);
                method.Attributes = MemberAttributes.Static;
                {
                    // List<Keyword> keyword = new List<Keyword>();
                    var keyword = new CodeVariableDeclarationStatement("List<Keyword>", "keyword");
                    keyword.InitExpression = new CodeObjectCreateExpression("List<Keyword>");
                    method.Statements.Add(keyword);
                }
                var convertor = new TreeNodeType2TokenType();
                foreach (var node in grammar.GetAllTreeNodeLeaveTypes())
                {
                    // keywords.Add(new Keyword("__x", "x"));
                    if (node.IsIdentifier())
                    {
                        TokenType tokenType = convertor.GetTokenType(node);
                        var ctor = new CodeObjectCreateExpression("Keyword",
                            new CodePrimitiveExpression(tokenType.Type),
                            new CodePrimitiveExpression(tokenType.Content));
                        var add = new CodeMethodInvokeExpression(
                            new CodeVariableReferenceExpression("keywords"),
                            "Add",
                            ctor);
                        method.Statements.Add(add);
                    }
                }
                {
                    // DemoLexicalAnalyzer.keywords = keywords;
                    var assign = new CodeAssignStatement(
                        new CodeFieldReferenceExpression(
                            new CodeSnippetExpression(GetLexicalAnalyzerName(grammarId)), "keywords"),
                        new CodeVariableReferenceExpression("keywords"));
                    method.Statements.Add(assign);
                }
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
                foreach (var state in lexiStateList)
                {
                    var condition = new CodeConditionStatement(
                        state.GetCondition(),
                        state.GetMethodInvokeStatement());
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
