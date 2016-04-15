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
    static partial class Utilities 
    {
        public static bool IsIdentifier(this TreeNodeType node)
        {
            if (!node.IsLeave) { throw new ArgumentException(); }

            {
                char item = node.Content[0];
                SourceCodeCharType charType = item.GetCharType();

                if (!(
                    charType == SourceCodeCharType.UnderLine
                    || charType == SourceCodeCharType.Letter))
                {
                    return false;
                }
            }
            for (int i = 1; i < node.Content.Length; i++)
            {
                char item = node.Content[i];
                SourceCodeCharType charType = item.GetCharType();

                if (!(
                    charType == SourceCodeCharType.UnderLine
                    || charType == SourceCodeCharType.Number
                    || charType == SourceCodeCharType.Letter))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetLexicalAnalyzerName(string grammarId)
        {
            SourceCodeCharType charType = grammarId[0].GetCharType();
            if (charType == SourceCodeCharType.Letter || charType == SourceCodeCharType.UnderLine)
            {
                return string.Format("{0}LexicalAnalyzer",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId));
            }
            else
            {
                return string.Format("_{0}LexicalAnalyzer",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId));
            }
        }

        public static string GetNodeNameInParser(TreeNodeType node)
        {
            return string.Format("NODE{0}", node.Type);
        }

        public static string GetTokenConstTypeName(string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            SourceCodeCharType charType = grammarId[0].GetCharType();
            if (charType == SourceCodeCharType.Letter || charType == SourceCodeCharType.UnderLine)
            {
                return string.Format("{0}{1}TokenType",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId),
                    "");//algorithm);
                    //algorithm);
            }
            else
            {
                return string.Format("_{0}{1}TokenType",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId),
                    "");//algorithm);
                    //algorithm);
            }
        }

        public static string GetTreeNodeConstTypeName(string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            SourceCodeCharType charType = grammarId[0].GetCharType();
            if (charType == SourceCodeCharType.Letter || charType == SourceCodeCharType.UnderLine)
            {
                return string.Format("{0}{1}TreeNodeType",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId),
                    "");//algorithm);
                    //algorithm);
            }
            else
            {
                return string.Format("_{0}{1}TreeNodeType",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId),
                    "");//algorithm);
                    //algorithm);
            }
        }

        public static string GetParserName(string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            SourceCodeCharType charType = grammarId[0].GetCharType();
            if (charType == SourceCodeCharType.Letter || charType == SourceCodeCharType.UnderLine)
            {
                return string.Format("{0}{1}SyntaxParser",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId),
                    "");//algorithm);
            }
            else
            {
                return string.Format("_{0}{1}SyntaxParser",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId), 
                    "");//algorithm);
            }
        }

        public static string GetNamespace(string grammarId)
        {
            SourceCodeCharType charType = grammarId[0].GetCharType();
            if (charType == SourceCodeCharType.Letter || charType == SourceCodeCharType.UnderLine)
            {
                return string.Format("{0}Compiler",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId));
            }
            else
            {
                return string.Format("_{0}Compiler",
                    ConstString2IdentifierHelper.ConstString2Identifier(grammarId));
            }
        }

    }
}
