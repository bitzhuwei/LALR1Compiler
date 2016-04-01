using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LALR1Compiler;

namespace ContextfreeGrammarCompiler.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] directories = Directory.GetDirectories(".", "*.Grammar", SearchOption.AllDirectories);
            foreach (var directory in directories)
            {
                Run(directory);
            }
        }

        private static void Run(string directory)
        {
            string[] grammarFullnames = Directory.GetFiles(directory, "*.Grammar.txt", SearchOption.TopDirectoryOnly);
            foreach (var grammarFullname in grammarFullnames)
            {
                ProcessGrammar(grammarFullname);
            }
        }

        private static void ProcessGrammar(string grammarFullname)
        {
            Console.WriteLine("Processing {0}", grammarFullname);

            FileInfo fileInfo = new FileInfo(grammarFullname);
            string directory = fileInfo.DirectoryName;
            string grammarId = fileInfo.Name.Substring(fileInfo.Name.Length - (".Grammar.txt").Length);
          
            string sourceCode = File.ReadAllText(grammarFullname);
            RegulationList grammar = GetGrammar(sourceCode, directory, grammarId);

            DumpCSharpCode(grammar, directory, grammarId);
        }

        /// <summary>
        /// 生成给定文法的compiler。
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="directory"></param>
        /// <param name="grammarId"></param>
        private static void DumpCSharpCode(RegulationList grammar, string directory, string grammarId)
        {
            
            throw new NotImplementedException();
        }

        /// <summary>
        /// ContextfreeGrammar compiler的工作过程。
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="directory"></param>
        /// <param name="grammarId"></param>
        /// <returns></returns>
        private static RegulationList GetGrammar(string sourceCode, string directory, string grammarId)
        {
            var lexi = new ContextfreeGrammarLexicalAnalyzer();
            TokenList tokenList = lexi.Analyze(sourceCode);
            Console.WriteLine("Dump token list...");
            tokenList.Dump(Path.Combine(directory, grammarId + ".TokenList.txt"));
            var parser = new ContextfreeGrammarSyntaxParser();
            SyntaxTree tree = parser.Parse(tokenList);
            Console.WriteLine("Dump syntax tree...");
            tree.Dump(Path.Combine(directory, grammarId + ".Tree.txt"));
            RegulationList grammar = tree.DumpGrammar();
            Console.WriteLine("Dump formated grammar...");
            grammar.Dump(Path.Combine(directory, grammarId + ".FormatedGrammar.txt"));
            return grammar;
        }
    }
}
