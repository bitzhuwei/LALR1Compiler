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
            DumpLexicalAnalyzer_GetKeywordList(grammar, lexiType);

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

        private static void DumpLexicalAnalyzer_GetKeywordList(
            RegulationList grammar, CodeTypeDeclaration lexiType)
        {
            // TODO:
            //throw new NotImplementedException();
        }

        private static void DumpLexicalAnalyzer_GetSymbol(
            RegulationList grammar, CodeTypeDeclaration lexiType)
        {
            // TODO:
            //throw new NotImplementedException();

        }

        private static void DumpLexicalAnalyzer_TryGetToken(
            RegulationList grammar, CodeTypeDeclaration lexiType)
        {
            var method = new CodeMemberMethod();
            method.Name = "TryGetToken";
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(AnalyzingContext), "context"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Token), "result"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(SourceCodeCharType), "charType"));
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
