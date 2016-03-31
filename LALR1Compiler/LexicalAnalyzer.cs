using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 词法分析器的抽象基类。对一个字符串进行词法分析
    /// </summary>
    public abstract partial class LexicalAnalyzer
        : ILexicalAnalyzer
    {

        /// <summary>
        /// 分析源代码获得Token序列
        /// </summary>
        /// <returns></returns>
        public TokenList Analyze(string sourceCode)
        {
            var tokens = new TokenList();
            if (string.IsNullOrEmpty(sourceCode)) { return tokens; }

            int count = sourceCode.Length;
            var context = new AnalyzingContext(sourceCode);

            while (context.NextLetterIndex < count)
            {
                var tk = NextToken(context);
                if (tk != null)
                {
                    tokens.Add(tk);
                }
            }

            return tokens;
        }

        /// <summary>
        /// 从ptNextLetter开始获取下一个Token
        /// </summary>
        /// <returns></returns>
        protected abstract Token NextToken(AnalyzingContext context);

    }

    public class AnalyzingContext
    {
        public string SourceCode { get; set; }

        public AnalyzingContext(string sourceCode)
        {
            this.SourceCode = sourceCode;
        }

        /// <summary>
        /// 将要分析的字符索引（从0开始）
        /// </summary>
        public int NextLetterIndex
        {
            get { return nextLetterIndex; }
            set
            {
                CurrentColumn += value - nextLetterIndex;
                nextLetterIndex = value;
            }
        }

        /// <summary>
        /// 将要分析的字符索引（从0开始）
        /// </summary>
        private int nextLetterIndex { get; set; }

        /// <summary>
        /// ptNextLetter所在行（从0开始）
        /// </summary>
        public int CurrentLine { get; set; }
        /// <summary>
        /// ptNextLetter所在列（从0开始）
        /// </summary>
        public int CurrentColumn { get; set; }


        public char CurrentChar()
        {
            return this.SourceCode[this.NextLetterIndex];
        }
    }
}
