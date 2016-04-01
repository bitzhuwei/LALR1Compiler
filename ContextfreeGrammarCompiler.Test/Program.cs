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
            Console.WriteLine("Processing {0}", grammarFullname);

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
            Dictionary<TreeNodeType, bool> nullableDict;
            grammar.GetNullableDict(out nullableDict);
            FIRSTCollection firstCollection;
            grammar.GetFirstCollection(out firstCollection, nullableDict);
            using (StreamWriter stream = new StreamWriter(
                Path.Combine(directory, grammarId + ".FIRST.txt")))
            { firstCollection.Dump(stream); }
            FOLLOWCollection followCollection;
            grammar.GetFollowCollection(out followCollection, nullableDict, firstCollection);
            using (StreamWriter stream = new StreamWriter(
                Path.Combine(directory, grammarId + ".FOLLOW.txt")))
            { followCollection.Dump(stream); }

            LRParsingMap LR0Map = grammar.GetLR0ParsingMap();
            DumpLR0Code(grammar, LR0Map, directory, grammarId);

            LRParsingMap SLRMap = grammar.GetSLRParsingMap();
            DumpSLRCode(grammar, SLRMap, directory, grammarId);

            LRParsingMap LR1Map = grammar.GetLR1ParsingMap();
            DumpLR1Code(grammar, LR1Map, directory, grammarId);
        }

        private static void DumpLR1Code(
            RegulationList grammar,
            LRParsingMap LR1Map,
            string directory, string grammarId)
        {
            string LR1Directory = Path.Combine(directory, "LR(1)");
            if (!Directory.Exists(LR1Directory)) { Directory.CreateDirectory(LR1Directory); }

            DumpSyntaxParserCode(grammar, LR1Map, grammarId, LR1Directory);
        }

        private static void DumpSLRCode(
            RegulationList grammar,
            LRParsingMap SLRMap,
            string directory, string grammarId)
        {
            string SLRDirectory = Path.Combine(directory, "SLR");
            if (!Directory.Exists(SLRDirectory)) { Directory.CreateDirectory(SLRDirectory); }

            DumpSyntaxParserCode(grammar, SLRMap, grammarId, SLRDirectory);
        }


        private static void DumpLR0Code(
            RegulationList grammar,
            LRParsingMap LR0Map,
            string directory, string grammarId)
        {
            string LR0Directory = Path.Combine(directory, "LR(0)");
            if (!Directory.Exists(LR0Directory)) { Directory.CreateDirectory(LR0Directory); }

            DumpSyntaxParserCode(grammar, LR0Map, grammarId, LR0Directory);
        }

    }
}
