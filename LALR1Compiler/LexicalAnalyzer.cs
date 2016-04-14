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
        protected UserDefinedTypeCollection userDefinedTypeTable;
        private bool inAnalyzingStep = false;

        internal void StartAnalyzing(UserDefinedTypeCollection userDefinedTypeTable)
        {
            if (!inAnalyzingStep)
            {
                this.userDefinedTypeTable = userDefinedTypeTable;
                inAnalyzingStep = true;
            }
        }

        internal void StopAnalyzing()
        {
            if (inAnalyzingStep)
            {
                this.userDefinedTypeTable = null;
                inAnalyzingStep = false;
            }
        }

        /// <summary>
        /// 每次分析都返回一个<see cref="Token"/>。
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        internal IEnumerable<Token> AnalyzeStep(string sourceCode)
        {
            if (!inAnalyzingStep) { throw new Exception("Must invoke this.StartAnalyzing() first!"); }

            if (!string.IsNullOrEmpty(sourceCode))
            {
                var context = new AnalyzingContext(sourceCode);
                int count = sourceCode.Length;

                while (context.NextLetterIndex < count)
                {
                    Token token = NextToken(context);
                    if (token != null)
                    {
                        yield return token;
                    }
                }
            }

            this.StopAnalyzing();
        }
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
                Token token = NextToken(context);
                if (token != null)
                {
                    tokens.Add(token);
                }
            }

            return tokens;
        }

        /// <summary>
        /// 从ptNextLetter开始获取下一个Token
        /// </summary>
        /// <returns></returns>
        protected abstract bool TryGetToken(AnalyzingContext context, Token result, SourceCodeCharType charType);

    }

}
