using LALR1Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLParser.Test
{
    class Program
    {
        static TextWriter consoleOriginalOut;

        static void Main(string[] args)
        {
            consoleOriginalOut = Console.Out;

            DeleteFiles();
            // 从最小的shader开始。
            var filenames =
                from item in
                    (from item in Directory.GetFiles(".", "*.vert", SearchOption.AllDirectories) select item).Union(
                        from item in Directory.GetFiles(".", "*.tesc", SearchOption.AllDirectories) select item).Union(
                        from item in Directory.GetFiles(".", "*.tese", SearchOption.AllDirectories) select item).Union(
                        from item in Directory.GetFiles(".", "*.geom", SearchOption.AllDirectories) select item).Union(
                        from item in Directory.GetFiles(".", "*.frag", SearchOption.AllDirectories) select item).Union(
                        from item in Directory.GetFiles(".", "*.comp", SearchOption.AllDirectories) select item)
                orderby (File.ReadAllText(item).Length)
                select item;
            foreach (var filename in filenames)
            {
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine("Testing {0}", filename);
                using (var fs = new FileStream("GLSLParser.Test.log", FileMode.Append, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.AutoFlush = true;
                        Console.SetOut(writer);
                        TestShader(filename);
                    }
                }

                Console.SetOut(consoleOriginalOut);
            }

            Console.WriteLine("All done!");
        }

        private static void TestShader(string filename)
        {
            Console.WriteLine("=================> Testing {0}", filename);
            FileInfo fileInfo = new FileInfo(filename);
            string directory = fileInfo.DirectoryName;
            string shaderName = fileInfo.Name;
            try
            {
                var parser = new FrontEndParser(new GLSLLexicalAnalyzer(), new GLSLSyntaxParser());
                Console.WriteLine("    Reading source code...");
                string sourceCode = File.ReadAllText(filename);
                TokenList tokenList;
                SyntaxTree tree = parser.Parse(sourceCode, out tokenList);
                string tokenListFullname = Path.Combine(directory, shaderName + ".tokenlist.log");
                Console.WriteLine("    Dump token list...");
                tokenList.Dump(tokenListFullname);
                string syntaxTreeFullname = Path.Combine(directory, shaderName + ".tree.log");
                Console.WriteLine("    Dump syntax tree...");
                tree.Dump(syntaxTreeFullname);
                Console.WriteLine("    Done");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
        }
    }
}
