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

        private static string GetNodeName(TreeNodeType node)
        {
            return string.Format("NODE{0}", node.Type);
        }

        private static string GetParserName(string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            return string.Format("{0}{1}SyntaxParser", grammarId, algorithm);
        }

        private static string GetNamespace(string grammarId)
        {
            return string.Format("{0}Compiler", grammarId);
        }

    }
}
