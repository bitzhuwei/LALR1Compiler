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
        /// 跳过多行注释
        /// </summary>
        /// <returns></returns>
        protected virtual void SkipMultilineNote(AnalyzingContext context)
        {
            context.NextLetterIndex += 2;
            int count = context.SourceCode.Length;
            while (context.NextLetterIndex < count)
            {
                if (context.CurrentChar() == '*')
                {
                    context.NextLetterIndex++;
                    if (context.NextLetterIndex < count)
                    {
                        if (context.CurrentChar() == '/')
                        {
                            context.NextLetterIndex++;
                            break;
                        }
                        else
                            context.NextLetterIndex++;
                    }
                }
                else
                    context.NextLetterIndex++;
            }
        }
        /// <summary>
        /// 跳过单行注释
        /// </summary>
        /// <returns></returns>
        protected virtual void SkipSingleLineNote(AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            char cNext;
            while (context.NextLetterIndex < count)
            {
                cNext = context.CurrentChar();
                if (cNext == '\r' || cNext == '\n')
                {
                    break;
                }
                context.NextLetterIndex++;
            }
        }

    }
}
