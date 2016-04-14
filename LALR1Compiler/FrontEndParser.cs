using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 前端分析器。
    /// 词法分析、语法分析、语义动作同步进行。
    /// </summary>
    public class FrontEndParser
    {
        private LexicalAnalyzer lexicalAnalyzer;
        private LRSyntaxParser syntaxParser;

        /// <summary>
        /// 前端分析器。
        /// 词法分析、语法分析、语义动作同步进行。
        /// </summary>
        /// <param name="lexicalAnalyzer"></param>
        /// <param name="syntaxParser"></param>
        public FrontEndParser(LexicalAnalyzer lexicalAnalyzer, LRSyntaxParser syntaxParser)
        {
            this.lexicalAnalyzer = lexicalAnalyzer;
            this.syntaxParser = syntaxParser;
        }

        /// <summary>
        /// 词法分析、语法分析、语义动作同步进行。
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public SyntaxTree Parse(string sourceCode, out TokenList tokenList)
        {
            tokenList = new TokenList();
            UserDefinedTypeCollection userDefinedTypeTable = new UserDefinedTypeCollection();
            this.lexicalAnalyzer.StartAnalyzing(userDefinedTypeTable);
            this.syntaxParser.StartParsing(userDefinedTypeTable);
            foreach (var token in this.lexicalAnalyzer.AnalyzeStep(sourceCode))
            {
                tokenList.Add(token);
                this.syntaxParser.ParseStep(token);
            }

            SyntaxTree result = this.syntaxParser.StopParsing();
            return result;
        }
    }

    public class UserDefinedTypeCollection : OrderedCollection<UserDefinedType>
    {
        public UserDefinedTypeCollection()
            : base(", ")
        { }

        private static string GetUniqueString(HashCache cache)
        {
            return cache.Dump();
        }
    }

    public class UserDefinedType : HashCache
    {
        public UserDefinedType()
            : base(GetUniqueString)
        { }

        private static string GetUniqueString(HashCache cache)
        {
            return cache.Dump();
        }

        public string TypeName { get; set; }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write(TypeName);
        }
    }
}
