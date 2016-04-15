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

        protected virtual bool GetLetter(Token result, AnalyzingContext context)
        {
            return GetIdentifier(result, context);
        }
        protected virtual bool Getunderline(Token result, AnalyzingContext context)
        {
            return GetIdentifier(result, context);
        }
        /// <summary>
        /// 获取标识符（函数名，变量名，等）
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetIdentifier(Token result, AnalyzingContext context)
        {
            StringBuilder builder = new StringBuilder();
            while (context.NextLetterIndex < context.SourceCode.Length)
            {
                char ch = context.CurrentChar();
                var ct = ch.GetCharType();
                if (ct == SourceCodeCharType.Letter
                    || ct == SourceCodeCharType.Number
                    || ct == SourceCodeCharType.UnderLine)
                {
                    builder.Append(ch);
                    context.NextLetterIndex++;
                }
                else
                { break; }
            }
            string content = builder.ToString();
            {
                // specify if this string is a keyword
                OrderedCollection<Keyword> keywords = this.GetKeywords();
                int index = keywords.IndexOf(new Keyword("__" + content, content));
                if (0 <= index)
                {
                    Keyword keyword = keywords[index];
                    result.TokenType = new TokenType(keyword.TokenType, content, content);
                    return true;
                }
            }
            {
                // 只对C语言这种需要提前声明的语言有用。
                // C#、Java这种语言不会需要这个东西。（是吗？）
                UserDefinedTypeCollection userDefinedTypeTable = this.userDefinedTypeTable;
                if (userDefinedTypeTable != null)
                {
                    UserDefinedType t = new UserDefinedType(content);
                    int index = userDefinedTypeTable.IndexOf(t);
                    if (index >= 0)
                    {
                        result.TokenType = new TokenType(
                            "__userDefinedType", content, content);
                        return true;
                    }
                }
            }
            {
                result.TokenType = new TokenType(
                    "identifier", content, content);
            }

            return true;
        }

    }

}
