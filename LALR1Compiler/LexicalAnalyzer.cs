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
        protected abstract bool TryGetToken(AnalyzingContext context, Token result, SourceCodeCharType charType);
    }

}
