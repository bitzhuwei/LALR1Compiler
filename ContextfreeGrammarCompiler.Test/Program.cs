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
using System.Reflection;

namespace ContextfreeGrammarCompiler.Test
{
    partial class Program
    {
        static TextWriter consoleOriginalOut;

        static void Main(string[] args)
        {
            consoleOriginalOut = Console.Out;

            DeleteFiles();
            // 从最小的文法开始。
            var filenames = from item in Directory.GetFiles(".", "*.Grammar", SearchOption.AllDirectories)
                            orderby (File.ReadAllText(item).Length)
                            select item;
            foreach (var filename in filenames)
            {
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine("Testing {0}", filename);
                using (var fs = new FileStream("ContextfreeGrammarCompiler.Test.log", FileMode.Append, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.AutoFlush = true;
                        Console.SetOut(writer);
                        ProcessGrammar(filename);
                    }
                }
                Console.SetOut(consoleOriginalOut);
            }

            Console.WriteLine("All done!");
        }

        private static void DeleteFiles()
        {
            {
                string[] files = Directory.GetFiles(".", "*.log", SearchOption.AllDirectories);
                foreach (var item in files)
                {
                    File.Delete(item);
                }
            }
            {
                string[] files = Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories);
                foreach (var item in files)
                {
                    if (!item.ToLower().EndsWith("semantics.cs"))
                    {
                        File.Delete(item);
                    }
                }
            }
        }

        private static void ProcessGrammar(string grammarFullname)
        {
            Console.WriteLine("=====> Processing {0}", grammarFullname);

            FileInfo fileInfo = new FileInfo(grammarFullname);
            string directory = fileInfo.DirectoryName;
            string grammarId = fileInfo.Name.Substring(0,
                fileInfo.Name.Length - (".Grammar").Length);

            string sourceCode = File.ReadAllText(grammarFullname);
            string errorInfo;
            RegulationList grammar;
            if (!GetGrammar(sourceCode, directory, grammarId, out grammar, out errorInfo))
            {
                Console.WriteLine("        Error in token list of {0}", grammarId);
                Console.WriteLine(errorInfo);
                return;
            }

            if (!grammar.SemanticCheck(out errorInfo))
            {
                Console.WriteLine("        Error in semantics of {0}", grammarId);
                Console.WriteLine(errorInfo);
                return;
            }

            DumpCode(grammar, directory, grammarId);

            CompileAndTestCode(directory, grammarId);
        }

        private static void CompileAndTestCode(string directory, string grammarId)
        {
            {
                Console.WriteLine("    Compiling {0} of LR(0) version", grammarId);
                string LR0Directory = Path.Combine(directory, "LR(0)");
                Assembly asm = CompileCode(directory, LR0Directory, grammarId, SyntaxParserMapAlgorithm.LR0);
                Console.WriteLine("    Test Code {0} of LR(0) version", grammarId);
                TestCode(asm, directory, LR0Directory, grammarId, SyntaxParserMapAlgorithm.LR0);
            }
            {
                Console.WriteLine("    Compiling {0} of SLR version", grammarId);
                string SLRDirectory = Path.Combine(directory, "SLR");
                Assembly asm = CompileCode(directory, SLRDirectory, grammarId, SyntaxParserMapAlgorithm.SLR);
                Console.WriteLine("    Test Code {0} of SLR version", grammarId);
                TestCode(asm, directory, SLRDirectory, grammarId, SyntaxParserMapAlgorithm.SLR);
            }
            {
                Console.WriteLine("    Compiling {0} of LALR(1) version", grammarId);
                string LALR1Directory = Path.Combine(directory, "LALR(1)");
                Assembly asm = CompileCode(directory, LALR1Directory, grammarId, SyntaxParserMapAlgorithm.LALR1);
                Console.WriteLine("    Test Code {0} of LALR(1) version", grammarId);
                TestCode(asm, directory, LALR1Directory, grammarId, SyntaxParserMapAlgorithm.LALR1);
            }
            {
                Console.WriteLine("    Compiling {0} of LR(1) version", grammarId);
                string LR1Directory = Path.Combine(directory, "LR(1)");
                Assembly asm = CompileCode(directory, LR1Directory, grammarId, SyntaxParserMapAlgorithm.LR1);
                Console.WriteLine("    Test Code {0} of LR(1) version", grammarId);
                TestCode(asm, directory, LR1Directory, grammarId, SyntaxParserMapAlgorithm.LR1);
            }
        }

        private static void TestCode(Assembly asm, string directory, string compilerDir, string grammarId, SyntaxParserMapAlgorithm syntaxParserMapAlgorithm)
        {
            if (asm == null)
            {
                Console.WriteLine("        Get Compiled Compiler Failed...");
                return;
            }
            {
                string conflicts = File.ReadAllText(Path.Combine(compilerDir, "Conflicts.log"));
                if (int.Parse(conflicts) > 0)
                {
                    Console.WriteLine("        No need to Test Code with conflicts in SyntaxParser");
                    return;
                }
            }
            try
            {
                LexicalAnalyzer lexi = asm.CreateInstance(
                    Utilities.GetNamespace(grammarId) + "." + Utilities.GetLexicalAnalyzerName(grammarId)) as LexicalAnalyzer;
                LRSyntaxParser parser = asm.CreateInstance(
                    Utilities.GetNamespace(grammarId) + "." + Utilities.GetParserName(grammarId, syntaxParserMapAlgorithm)) as LRSyntaxParser;
                FrontEndParser frontEndParser = new FrontEndParser(lexi, parser);
                string[] sourceCodeFullnames = Directory.GetFiles(
                    directory, "*.Code", SearchOption.TopDirectoryOnly);
                foreach (var fullname in sourceCodeFullnames)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(fullname);
                        string sourceCode = File.ReadAllText(fullname);
                        TokenList tokenList;
                        SyntaxTree tree = frontEndParser.Parse(sourceCode, out tokenList);
                        tokenList.Dump(Path.Combine(compilerDir, fileInfo.Name + ".TokenList.log"));
                        tree.Dump(Path.Combine(compilerDir, fileInfo.Name + ".Tree.log"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Testing Error for {0}:", fullname);
                        Console.WriteLine(e);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Running Errors:");
                Console.Write("    ");
                Console.WriteLine(ex);
            }
        }

        private static Assembly CompileCode(string directory, string subDirectory, string grammarId, SyntaxParserMapAlgorithm syntaxParserMapAlgorithm)
        {
            // 自动生成的代码（词法分析+语法分析）
            string[] subFiles = Directory.GetFiles(subDirectory, "*.cs", SearchOption.TopDirectoryOnly);
            // 手工编写的代码（语义分析）
            string[] files = Directory.GetFiles(directory, "*.cs", SearchOption.TopDirectoryOnly);
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            string[] allFiles = files.Concat(subFiles).ToArray();

            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add(
                typeof(LALR1Compiler.LRSyntaxParser).Assembly.Location);
            objCompilerParameters.ReferencedAssemblies.Add(
                typeof(List<>).Assembly.Location);
            objCompilerParameters.ReferencedAssemblies.Add(
                typeof(Stack<>).Assembly.Location);
            objCompilerParameters.ReferencedAssemblies.Add(
                            typeof(Object).Assembly.Location);
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;
            objCompilerParameters.IncludeDebugInformation = true;
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromFile(
                objCompilerParameters, allFiles);
            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("Compiling Errors:");
                foreach (var item in cr.Errors)
                {
                    Console.Write("    ");
                    Console.WriteLine(item);
                }

                return null;
            }
            else
            {
                return cr.CompiledAssembly;
            }
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
                Console.WriteLine("    Dump {0}", grammarId + ".FIRST.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(directory, grammarId + ".FIRST.log")))
                { firstCollection.Dump(stream); }
                FOLLOWCollection followCollection;
                grammar.GetFollowCollection(out followCollection, nullableDict, firstCollection);
                Console.WriteLine("    Dump {0}", grammarId + ".FOLLOW.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(directory, grammarId + ".FOLLOW.log")))
                { followCollection.Dump(stream); }
            }
            {
                Console.WriteLine("    LR(0) parsing...");
                LR0StateCollection stateCollection;
                LR0EdgeCollection edgeCollection;
                LRParsingMap parsingMap;
                grammar.GetLR0ParsingMap(
                    out parsingMap, out stateCollection, out edgeCollection,
                    Program.consoleOriginalOut);

                string LR0Directory = Path.Combine(directory, "LR(0)");
                if (!Directory.Exists(LR0Directory)) { Directory.CreateDirectory(LR0Directory); }

                Console.WriteLine("        Dump {0}", grammarId + ".State.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR0Directory, grammarId + ".State.log")))
                { stateCollection.Dump(stream); }
                Console.WriteLine("        Dump {0}", grammarId + ".Edge.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR0Directory, grammarId + ".Edge.log")))
                { edgeCollection.Dump(stream); }
                Console.WriteLine("        Dump LR(0) Compiler's source code...");
                DumpTreeNodeTypeConstCode(grammar, parsingMap, grammarId, LR0Directory, SyntaxParserMapAlgorithm.LR0);
                DumpTokenTypeConstCode(grammar, parsingMap, grammarId, LR0Directory, SyntaxParserMapAlgorithm.LR0);
                DumpSyntaxParserCode(grammar, parsingMap, grammarId, LR0Directory, SyntaxParserMapAlgorithm.LR0);
                DumpLexicalAnalyzerCode(grammar, grammarId, SyntaxParserMapAlgorithm.LR0, LR0Directory);
                TestParsingMap(LR0Directory, parsingMap);
            }
            {
                Console.WriteLine("    SLR parsing...");
                LRParsingMap parsingMap;
                LR0StateCollection stateCollection;
                LR0EdgeCollection edgeCollection;
                grammar.GetSLRParsingMap(
                    out parsingMap, out stateCollection, out edgeCollection,
                    Program.consoleOriginalOut);
                string SLRDirectory = Path.Combine(directory, "SLR");
                if (!Directory.Exists(SLRDirectory)) { Directory.CreateDirectory(SLRDirectory); }

                Console.WriteLine("        Dump {0}", grammarId + ".State.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(SLRDirectory, grammarId + ".State.log")))
                { stateCollection.Dump(stream); }
                Console.WriteLine("        Dump {0}", grammarId + ".Edge.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(SLRDirectory, grammarId + ".Edge.log")))
                { edgeCollection.Dump(stream); }
                Console.WriteLine("        Dump SLR Compiler's source code...");
                DumpTreeNodeTypeConstCode(grammar, parsingMap, grammarId, SLRDirectory, SyntaxParserMapAlgorithm.SLR);
                DumpTokenTypeConstCode(grammar, parsingMap, grammarId, SLRDirectory, SyntaxParserMapAlgorithm.SLR);
                DumpSyntaxParserCode(grammar, parsingMap, grammarId, SLRDirectory, SyntaxParserMapAlgorithm.SLR);
                DumpLexicalAnalyzerCode(grammar, grammarId, SyntaxParserMapAlgorithm.SLR, SLRDirectory);
                TestParsingMap(SLRDirectory, parsingMap);
            }
            {
                Console.WriteLine("    LALR(1) parsing...");
                LRParsingMap parsingMap;
                LALR1StateCollection stateCollection;
                LALR1EdgeCollection edgeCollection;
                grammar.GetLALR1ParsingMap(
                    out parsingMap, out stateCollection, out edgeCollection,
                    Program.consoleOriginalOut);

                string LALR1Directory = Path.Combine(directory, "LALR(1)");
                if (!Directory.Exists(LALR1Directory)) { Directory.CreateDirectory(LALR1Directory); }

                Console.WriteLine("        Dump {0}", grammarId + ".State.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LALR1Directory, grammarId + ".State.log")))
                { stateCollection.Dump(stream); }
                Console.WriteLine("        Dump {0}", grammarId + ".Edge.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LALR1Directory, grammarId + ".Edge.log")))
                { edgeCollection.Dump(stream); }
                Console.WriteLine("        Dump LALR(1) Compiler's source code...");
                DumpTreeNodeTypeConstCode(grammar, parsingMap, grammarId, LALR1Directory, SyntaxParserMapAlgorithm.LALR1);
                DumpTokenTypeConstCode(grammar, parsingMap, grammarId, LALR1Directory, SyntaxParserMapAlgorithm.LALR1);
                DumpSyntaxParserCode(grammar, parsingMap, grammarId, LALR1Directory, SyntaxParserMapAlgorithm.LALR1);
                DumpLexicalAnalyzerCode(grammar, grammarId, SyntaxParserMapAlgorithm.LALR1, LALR1Directory);
                TestParsingMap(LALR1Directory, parsingMap);
            }
            {
                Console.WriteLine("    LR(1) parsing...");
                LRParsingMap parsingMap;
                LR1StateCollection stateCollection;
                LR1EdgeCollection edgeCollection;
                grammar.GetLR1ParsingMap(
                    out parsingMap, out stateCollection, out edgeCollection,
                    Program.consoleOriginalOut);

                string LR1Directory = Path.Combine(directory, "LR(1)");
                if (!Directory.Exists(LR1Directory)) { Directory.CreateDirectory(LR1Directory); }

                Console.WriteLine("        Dump {0}", grammarId + ".State.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR1Directory, grammarId + ".State.log")))
                { stateCollection.Dump(stream); }
                Console.WriteLine("        Dump {0}", grammarId + ".Edge.log");
                using (StreamWriter stream = new StreamWriter(
                    Path.Combine(LR1Directory, grammarId + ".Edge.log")))
                { edgeCollection.Dump(stream); }
                Console.WriteLine("        Dump LR(1) Compiler's source code...");
                DumpTreeNodeTypeConstCode(grammar, parsingMap, grammarId, LR1Directory, SyntaxParserMapAlgorithm.LR1);
                DumpTokenTypeConstCode(grammar, parsingMap, grammarId, LR1Directory, SyntaxParserMapAlgorithm.LR1);
                DumpSyntaxParserCode(grammar, parsingMap, grammarId, LR1Directory, SyntaxParserMapAlgorithm.LR1);
                DumpLexicalAnalyzerCode(grammar, grammarId, SyntaxParserMapAlgorithm.LR1, LR1Directory);
                TestParsingMap(LR1Directory, parsingMap);
            }
        }

        private static bool TestParsingMap(string directory, LRParsingMap parsingMap)
        {
            int conflicts = 0;
            foreach (var item in parsingMap)
            {
                if (item.Value.Count > 1)
                {
                    conflicts += item.Value.Count - 1;
                }
            }

            if (conflicts > 0)
            {
                Console.WriteLine("        【Exists {0} Conflicts in Parsingmap】", conflicts);
            }

            File.WriteAllText(Path.Combine(directory, "Conflicts.log"), conflicts.ToString());

            return conflicts > 0;
        }

    }

}
