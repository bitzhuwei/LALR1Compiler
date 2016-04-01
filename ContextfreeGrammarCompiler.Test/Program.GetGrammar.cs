﻿using System;
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
        private static RegulationList GetGrammar(string sourceCode, string directory, string grammarId)
        {
            Console.WriteLine("    Get grammar from source code...");
            var lexi = new ContextfreeGrammarLexicalAnalyzer();
            TokenList tokenList = lexi.Analyze(sourceCode);
            Console.WriteLine("        Dump {0}", grammarId + ".TokenList.txt");
            tokenList.Dump(Path.Combine(directory, grammarId + ".TokenList.txt"));
            var parser = new ContextfreeGrammarSyntaxParser();
            SyntaxTree tree = parser.Parse(tokenList);
            Console.WriteLine("        Dump {0}", grammarId + ".Tree.txt");
            tree.Dump(Path.Combine(directory, grammarId + ".Tree.txt"));
            RegulationList grammar = tree.DumpGrammar();
            Console.WriteLine("        Dump {0}", grammarId + ".FormatedGrammar.txt");
            grammar.Dump(Path.Combine(directory, grammarId + ".FormatedGrammar.txt"));
            return grammar;
        }
    }
}
