using LALR1Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLParser
{
    public static class GLSLParserSyntaxTree_DumpShaderDeclarations
    {
        public static List<Declaration> DumpDeclarations(this SyntaxTree syntaxTree)
        {
            if (syntaxTree.NodeType.Type != GLSLTreeNodeType.NODE__translation_unit)
            { throw new ArgumentException(); }

            List<Declaration> result = new List<Declaration>();
            if (syntaxTree.Children.Count == 1)
            {
                // <translation_unit> ::= <external_declaration> ;
                ExternalDeclaration(syntaxTree.Children[0], result);
            }
            else if (syntaxTree.Children.Count == 2)
            {
                // <translation_unit> ::= <translation_unit> <external_declaration> ;
                TranslationUnit(syntaxTree.Children[0], result);
                ExternalDeclaration(syntaxTree.Children[1], result);
            }

            return result;
        }

        private static void TranslationUnit(SyntaxTree syntaxTree, List<Declaration> result)
        {
            throw new NotImplementedException();
        }

        private static void ExternalDeclaration(SyntaxTree syntaxTree, List<Declaration> result)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Declaration
    {
        public string Name { get; set; }
    }


}
