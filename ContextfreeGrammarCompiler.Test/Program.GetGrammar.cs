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

        /// <summary>
        /// ContextfreeGrammar compiler的工作过程。
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="directory"></param>
        /// <param name="grammarId"></param>
        /// <returns></returns>
        private static bool GetGrammar(string sourceCode,
            string directory, string grammarId, out RegulationList grammar, out string errorInfo)
        {
            grammar = null;
            errorInfo = string.Empty;
            Console.WriteLine("    Get grammar from source code...");
            var lexicalAnalyzer = new ContextfreeGrammarLexicalAnalyzer();
            var syntaxParser = new ContextfreeGrammarSLRSyntaxParser();
            FrontEndParser parser = new FrontEndParser(lexicalAnalyzer, syntaxParser);
            TokenList tokenList;
            SyntaxTree tree = parser.Parse(sourceCode, out tokenList);
            if (!tokenList.Check(out errorInfo))
            {
                return false;
            }
            Console.WriteLine("        Dump {0}", grammarId + ".TokenList.log");
            tokenList.Dump(Path.Combine(directory, grammarId + ".TokenList.log"));
            Console.WriteLine("        Dump {0}", grammarId + ".Tree.log");
            tree.Dump(Path.Combine(directory, grammarId + ".Tree.log"));
            grammar = tree.DumpGrammar();
            Console.WriteLine("        Dump {0}", grammarId + ".FormatedGrammar.log");
            grammar.Dump(Path.Combine(directory, grammarId + ".FormatedGrammar.log"));

            return true;
        }
    }
}
