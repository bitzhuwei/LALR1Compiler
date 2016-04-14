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
    {


        /// <summary>
        /// 字符串常量 "XXX"
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetDoubleQuotation(Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            StringBuilder builder = new StringBuilder("\"");
            context.NextLetterIndex++;
            bool notMatched = true;
            char c;
            while ((context.NextLetterIndex < count) && notMatched)
            {
                c = context.SourceCode[context.NextLetterIndex];
                if (c == '"')
                {
                    builder.Append(c);
                    notMatched = false;
                    context.NextLetterIndex++;
                }
                else if (c == '\r' || c == '\n')
                {
                    break;
                }
                else
                {
                    builder.Append(c);
                    context.NextLetterIndex++;
                }
            }
            result.TokenType = new TokenType(
                "constString", builder.ToString(), "constString");
            return true;
        }

    }

}
