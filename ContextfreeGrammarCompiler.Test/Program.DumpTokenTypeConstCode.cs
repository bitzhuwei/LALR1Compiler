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
    partial class Program
    {


        private static void DumpTokenTypeConstCode(RegulationList grammar, LRParsingMap parsingMap,
            string grammarId, string directory, SyntaxParserMapAlgorithm algorithm)
        {
            var tokenTypeConstType = new CodeTypeDeclaration(GetTokenConstTypeName(grammarId, algorithm));
            tokenTypeConstType.IsClass = true;
            DumpTokenTypeConstFields(grammar, tokenTypeConstType);

            var parserNamespace = new CodeNamespace(GetNamespace(grammarId));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Object).Namespace));
            parserNamespace.Types.Add(tokenTypeConstType);

            //生成代码  
            string fullname = Path.Combine(directory, GetTokenConstTypeName(grammarId, algorithm) + ".cs");
            using (var sw = new StreamWriter(fullname, false))
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

        private static void DumpTokenTypeConstFields(RegulationList grammar, CodeTypeDeclaration tokenTypeConstType)
        {
            // public const string __colon_colon_equal = "__colon_colon_equal";
            var convertor = new TreeNodeType2TokenType();
            foreach (var node in grammar.GetAllTreeNodeLeaveTypes())
            {
                var field = new CodeMemberField(typeof(string), GetNodeNameInParser(node));
                field.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                TokenType tokenType = convertor.GetTokenType(node);
                field.InitExpression = new CodePrimitiveExpression(tokenType.Type);
                tokenTypeConstType.Members.Add(field);
            }
        }



    }
}
