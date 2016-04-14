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

        private static void DumpTreeNodeTypeConstCode(
            RegulationList grammar, LRParsingMap map, string grammarId, string LR0Directory,
            SyntaxParserMapAlgorithm algorithm)
        {
            var treeNodeConstType = new CodeTypeDeclaration(Utilities.GetTreeNodeConstTypeName(grammarId, algorithm));
            treeNodeConstType.IsClass = true;
            DumpTreeNodeConstFields(grammar, treeNodeConstType);

            var parserNamespace = new CodeNamespace(Utilities.GetNamespace(grammarId));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Object).Namespace));
            parserNamespace.Types.Add(treeNodeConstType);

            //生成代码  
            string fullname = Path.Combine(LR0Directory, Utilities.GetTreeNodeConstTypeName(grammarId, algorithm) + ".cs");
            using (var stream = new StreamWriter(fullname, false))
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CodeGeneratorOptions geneOptions = new CodeGeneratorOptions();//代码生成选项
                geneOptions.BlankLinesBetweenMembers = true;
                geneOptions.BracingStyle = "C";
                geneOptions.ElseOnClosing = false;
                geneOptions.IndentString = "    ";
                geneOptions.VerbatimOrder = true;

                codeProvider.GenerateCodeFromNamespace(parserNamespace, stream, geneOptions);
            }
        }

        private static void DumpTreeNodeConstFields(RegulationList grammar, CodeTypeDeclaration treeNodeConstType)
        {
            // public const string __Grammar = "__Grammar";
            // public const string __colon_colon_equalLeave__ = "__colon_colon_equalLeave__";
            foreach (var node in grammar.GetAllTreeNodeTypes())
            {
                var field = new CodeMemberField(typeof(string), Utilities.GetNodeNameInParser(node));
                field.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                field.InitExpression = new CodePrimitiveExpression(node.Type);
                treeNodeConstType.Members.Add(field);
            }
        }



    }
}
