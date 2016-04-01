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
        static void Main(string[] args)
        {
            string[] directories = Directory.GetDirectories(
                ".", "*.Grammar", SearchOption.AllDirectories)
                .OrderBy(x =>
                    (new FileInfo(Directory.GetFiles(
                        x, "*.Grammar.txt", SearchOption.TopDirectoryOnly)[0]).Length))
                    .ToArray();
            foreach (var directory in directories)
            {
                Run(directory);
            }
        }

        private static void Run(string directory)
        {
            string[] grammarFullnames = Directory.GetFiles(
                directory, "*.Grammar.txt", SearchOption.TopDirectoryOnly);
            foreach (var grammarFullname in grammarFullnames)
            {
                ProcessGrammar(grammarFullname);
            }
        }

        private static void ProcessGrammar(string grammarFullname)
        {
            Console.WriteLine("=====> Processing {0}", grammarFullname);

            FileInfo fileInfo = new FileInfo(grammarFullname);
            string directory = fileInfo.DirectoryName;
            string grammarId = fileInfo.Name.Substring(0,
                fileInfo.Name.Length - (".Grammar.txt").Length);

            string sourceCode = File.ReadAllText(grammarFullname);
            RegulationList grammar = GetGrammar(sourceCode, directory, grammarId);

            DumpCode(grammar, directory, grammarId);
        }

        /// <summary>
        /// 生成给定文法的compiler。
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="directory"></param>
        /// <param name="grammarId"></param>
        private static void DumpCode(RegulationList grammar, string directory, string grammarId)
        {
            {
                Dictionary<TreeNodeType, bool> nullableDict;
                grammar.GetNullableDict(out nullableDict);
                FIRSTCollection firstCollection;
                grammar.GetFirstCollection(out firstCollection, nullableDict);
                Console.WriteLine("    Dump {0}", grammarId + ".FIRST.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(directory, grammarId + ".FIRST.txt")))
                { firstCollection.Dump(stream); }
                FOLLOWCollection followCollection;
                grammar.GetFollowCollection(out followCollection, nullableDict, firstCollection);
                Console.WriteLine("    Dump {0}", grammarId + ".FOLLOW.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(directory, grammarId + ".FOLLOW.txt")))
                { followCollection.Dump(stream); }
            }
            {
                LR0StateCollection stateCollection;
                LR0EdgeCollection edgeCollection;
                LRParsingMap LR0Map;
                grammar.GetLR0ParsingMap(out LR0Map, out stateCollection, out edgeCollection);

                string LR0Directory = Path.Combine(directory, "LR(0)");
                if (!Directory.Exists(LR0Directory)) { Directory.CreateDirectory(LR0Directory); }

                Console.WriteLine("    Dump {0}", grammarId + ".State.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR0Directory, grammarId + ".State.txt")))
                { stateCollection.Dump(stream); }
                Console.WriteLine("    Dump {0}", grammarId + ".Edge.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR0Directory, grammarId + ".Edge.txt")))
                { edgeCollection.Dump(stream); }
                Console.WriteLine("    Dump LR(0) source code...");
                DumpSyntaxParserCode(grammar, LR0Map, grammarId, LR0Directory, SyntaxParserMapAlgorithm.LR0);
                DumpLexicalAnalyzerCode(grammar, grammarId, LR0Directory);
            }

            {
                LRParsingMap SLRMap;
                LR0StateCollection stateCollection;
                LR0EdgeCollection edgeCollection;
                grammar.GetSLRParsingMap(out SLRMap, out stateCollection, out edgeCollection);

                string SLRDirectory = Path.Combine(directory, "SLR");
                if (!Directory.Exists(SLRDirectory)) { Directory.CreateDirectory(SLRDirectory); }

                Console.WriteLine("    Dump {0}", grammarId + ".State.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(SLRDirectory, grammarId + ".State.txt")))
                { stateCollection.Dump(stream); }
                Console.WriteLine("    Dump {0}", grammarId + ".Edge.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(SLRDirectory, grammarId + ".Edge.txt")))
                { edgeCollection.Dump(stream); }
                Console.WriteLine("    Dump SLR source code...");
                DumpSyntaxParserCode(grammar, SLRMap, grammarId, SLRDirectory, SyntaxParserMapAlgorithm.SLR);
                DumpLexicalAnalyzerCode(grammar, grammarId, SLRDirectory);
            }

            {
                LRParsingMap LR1Map;
                LR1StateCollection stateCollection;
                LR1EdgeCollection edgeCollection;
                grammar.GetLR1ParsingMap(out LR1Map, out stateCollection, out edgeCollection);

                string LR1Directory = Path.Combine(directory, "LR(1)");
                if (!Directory.Exists(LR1Directory)) { Directory.CreateDirectory(LR1Directory); }

                Console.WriteLine("    Dump {0}", grammarId + ".State.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR1Directory, grammarId + ".State.txt")))
                { stateCollection.Dump(stream); }
                Console.WriteLine("    Dump {0}", grammarId + ".Edge.txt");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR1Directory, grammarId + ".Edge.txt")))
                { edgeCollection.Dump(stream); }
                Console.WriteLine("    Dump LR(1) source code...");
                DumpSyntaxParserCode(grammar, LR1Map, grammarId, LR1Directory, SyntaxParserMapAlgorithm.LR1);
                DumpLexicalAnalyzerCode(grammar, grammarId, LR1Directory);
            }
        }


    }

}
