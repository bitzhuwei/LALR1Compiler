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
            if (syntaxTree.NodeType.Type != GLSLTreeNodeType.NODE__declaration)
            { throw new ArgumentException(); }

            if (syntaxTree.Children.Count == 1)
            {
                if (syntaxTree.Children[0].NodeType.Type == GLSLTreeNodeType.NODE__function_prototype)
                {
                    // <declaration> ::= <function_prototype> ";" ;
                    function_prototype(syntaxTree.Children[0], result);
                }
                else if (syntaxTree.Children[0].NodeType.Type == GLSLTreeNodeType.NODE__init_declarator_list)
                {
                    // <declaration> ::= <init_declarator_list> ";" ;
                    init_declarator_list(syntaxTree.Children[0], result);
                }
                else
                { throw new Exception(); }
            }
            else if (syntaxTree.Children.Count == 2)
            {
                // <declaration> ::= <type_qualifier> ";" ;
                TypeQualifier qualifier = type_qualifier(syntaxTree.Children[0]);
                result.Add(qualifier);
            }
            else if (syntaxTree.Children.Count == 3)
            {
                // <declaration> ::= <type_qualifier> identifier ";" ;
                TypeQualifier qualifier = type_qualifier(syntaxTree.Children[0]);
                qualifier.Identifier = identifier(syntaxTree.Children[1]);
                result.Add(qualifier);
            }
            else if (syntaxTree.Children.Count == 4)
            {
                if (syntaxTree.Children[0].NodeType.Type == GLSLTreeNodeType.NODE__precisionLeave__)
                {
                    // <declaration> ::= "precision" <precision_qualifier> <type_specifier> ";" ;
                    PrecisionDeclaration precisionDeclaration = new PrecisionDeclaration();
                    precisionDeclaration.Qualifier = precision_qualifier(syntaxTree.Children[1]);
                    precisionDeclaration.TypeName = type_specifier(syntaxTree.Children[2]);
                    result.Add(precisionDeclaration);
                }
                else if (syntaxTree.Children[0].NodeType.Type == GLSLTreeNodeType.NODE__type_qualifier)
                {
                    // <declaration> ::= <type_qualifier> identifier <identifier_list> ";" ;
                    TypeQualifier qualifier = type_qualifier(syntaxTree.Children[0]);
                    qualifier.Identifier = identifier(syntaxTree.Children[1]);
                    result.Add(qualifier);
                    List<string> identifierList = identifier_list(syntaxTree.Children[2]);
                    foreach (var id in identifierList)
                    {
                        qualifier = qualifier.Clone() as TypeQualifier;
                        qualifier.Identifier = id;
                        result.Add(qualifier);
                    }
                }
                else
                { throw new ArgumentException(); }
            }
            else if (syntaxTree.Children.Count == 6)
            {
                // <declaration> ::= <type_qualifier> identifier "{" <struct_declaration_list> "}" ";" ;

            }
            else if (syntaxTree.Children.Count == 7)
            {
                // <declaration> ::= <type_qualifier> identifier "{" <struct_declaration_list> "}" identifier ";" ;

            }
            else if (syntaxTree.Children.Count == 8)
            {
                // <declaration> ::= <type_qualifier> identifier "{" <struct_declaration_list> "}" identifier <array_specifier> ";" ;

            }
            else
            { throw new Exception(); }
        }

        private static List<string> identifier_list(SyntaxTree syntaxTree)
        {
            throw new NotImplementedException();
        }

        private static string type_specifier(SyntaxTree syntaxTree)
        {
            throw new NotImplementedException();
        }

        private static PrecisionQualifierType precision_qualifier(SyntaxTree syntaxTree)
        {
            throw new NotImplementedException();
        }


        private static string identifier(SyntaxTree syntaxTree)
        {
            throw new NotImplementedException();
        }

        private static TypeQualifier type_qualifier(SyntaxTree syntaxTree)
        {
            throw new NotImplementedException();
        }

        private static void init_declarator_list(SyntaxTree syntaxTree, List<Declaration> result)
        {
            throw new NotImplementedException();
        }

        private static void function_prototype(SyntaxTree syntaxTree, List<Declaration> result)
        {
            throw new NotImplementedException();
        }

        private static void function_definition(SyntaxTree syntaxTree, List<Declaration> result)
        {
            throw new NotImplementedException();
        }
    }



}
