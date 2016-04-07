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
        /// space tab \r \n
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetSpace(Token result, AnalyzingContext context)
        {
            char c = context.CurrentChar();
            context.NextLetterIndex++;
            if (c == '\n')// || c == '\r') //换行：Windows：\r\n Linux：\n
            {
                context.CurrentLine++;
                context.CurrentColumn = 1;
            }
            return false;
        }
    }
}

