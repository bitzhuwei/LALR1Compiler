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
                external_declaration(syntaxTree.Children[0], result);
            }
            else if (syntaxTree.Children.Count == 2)
            {
                // <translation_unit> ::= <translation_unit> <external_declaration> ;
                translation_unit(syntaxTree.Children[0], result);
                external_declaration(syntaxTree.Children[1], result);
            }
            else
            { throw new Exception(); }

            return result;
        }

        private static void translation_unit(SyntaxTree syntaxTree, List<Declaration> result)
        {
            if (syntaxTree.NodeType.Type != GLSLTreeNodeType.NODE__translation_unit)
            { throw new ArgumentException(); }

            if (syntaxTree.Children.Count == 1)
            {
                // <translation_unit> ::= <external_declaration> ;
                translation_unit(syntaxTree.Children[0], result);
            }
            else if (syntaxTree.Children.Count == 2)
            {
                // <translation_unit> ::= <translation_unit> <external_declaration> ;
                translation_unit(syntaxTree.Children[0], result);
                external_declaration(syntaxTree.Children[1], result);
            }
            else
            { throw new Exception(); }
        }

        private static void external_declaration(SyntaxTree syntaxTree, List<Declaration> result)
        {
            if (syntaxTree.NodeType.Type != GLSLTreeNodeType.NODE__external_declaration)
            { throw new ArgumentException(); }

            if (syntaxTree.Children[0].NodeType.Type == GLSLTreeNodeType.NODE__function_definition)
            {
                // <external_declaration> ::= <function_definition> ;
                function_definition(syntaxTree.Children[0], result);
            }
            else if (syntaxTree.Children[0].NodeType.Type == GLSLTreeNodeType.NODE__declaration)
            {
                // <external_declaration> ::= <declaration> ;
                declaration(syntaxTree.Children[0], result);
            }
            else
            { throw new Exception(); }
        }

        private static void declaration(SyntaxTree syntaxTree, List<Declaration> result)
        {
            throw new NotImplementedException();
        }

        private static void function_definition(SyntaxTree syntaxTree, List<Declaration> result)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Declaration
    {
        public string Name { get; set; }
    }


}
