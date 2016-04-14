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
        /// 未知符号
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetUnknown(Token result, AnalyzingContext context)
        {
            string content = context.CurrentChar().ToString();
            result.TokenType = new TokenType(
                "__unknown", content, "unknown");
            result.LexicalError = true;
            //result.Tag = string.Format("发现未知字符[{0}]。", result.Detail);
            context.NextLetterIndex++;
            return true;
        }

    }

}
