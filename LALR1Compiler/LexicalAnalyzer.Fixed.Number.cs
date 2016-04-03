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
        
        protected virtual bool GetNumber(Token result, AnalyzingContext context)
        {
            return GetConstentNumber(result, context);
        }

        #region GetConstentNumber
        /// <summary>
        /// 数值
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetConstentNumber(Token result, AnalyzingContext context)
        {
            char ch = context.CurrentChar();
            if (ch == '0')//可能是八进制或十六进制数
            {
                if (context.NextLetterIndex + 1 < context.SourceCode.Length)
                {
                    char c = context.SourceCode[context.NextLetterIndex + 1];
                    if (c == 'x' || c == 'X')
                    {//十六进制数
                        return GetConstentHexadecimalNumber(result, context);
                    }
                    else if (ch.GetCharType() == SourceCodeCharType.Number)
                    {//八进制数
                        return GetConstentOctonaryNumber(result, context);
                    }
                    else//十进制数
                    {
                        return GetConstentDecimalNumber(result, context);
                    }
                }
                else
                {//源代码最后一个字符 0
                    result.TokenType = new TokenType("number", "0", "number");
                    return true;
                }
            }
            else//十进制数
            {
                return GetConstentDecimalNumber(result, context);
            }
        }
        /// <summary>
        /// 十进制数
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetConstentDecimalNumber(Token result, AnalyzingContext context)
        {
            char ch = context.CurrentChar();
            StringBuilder tag = new StringBuilder();
            string numberSerial1, numberSerial2, numberSerial3;
            numberSerial1 = GetNumberSerial(context, 10);
            tag.Append(numberSerial1);
            result.LexicalError = string.IsNullOrEmpty(numberSerial1);
            if (context.NextLetterIndex < context.SourceCode.Length)
            {
                //ch = context.CurrentChar();
                if (ch == 'l' || ch == 'L')
                {
                    tag.Append(ch);
                    context.NextLetterIndex++;
                }
                if (ch == '.')
                {
                    tag.Append(ch);
                    context.NextLetterIndex++;
                    numberSerial2 = GetNumberSerial(context, 10);
                    tag.Append(numberSerial2);
                    result.LexicalError = result.LexicalError || string.IsNullOrEmpty(numberSerial2);
                    if (context.NextLetterIndex < context.SourceCode.Length)
                    {
                        ch = context.CurrentChar();
                    }
                }
                if (ch == 'e' || ch == 'E')
                {
                    tag.Append(ch);
                    context.NextLetterIndex++;
                    if (context.NextLetterIndex < context.SourceCode.Length)
                    {
                        ch = context.CurrentChar();
                        if (ch == '+' || ch == '-')
                        {
                            tag.Append(ch);
                            context.NextLetterIndex++;
                        }
                    }
                    numberSerial3 = GetNumberSerial(context, 10);
                    tag.Append(numberSerial3);
                    result.LexicalError = result.LexicalError || string.IsNullOrEmpty(numberSerial3);
                }
            }
            if (result.LexicalError)
            {
                result.TokenType = new TokenType("number",
                    string.Format("十进制数[{0}]格式错误，无法解析。", tag.ToString()),
                    "number");
            }
            else
            {
                result.TokenType = new TokenType("number", tag.ToString(), "number");
            }
            return true;
        }
        /// <summary>
        /// 八进制数
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetConstentOctonaryNumber(Token result, AnalyzingContext context)
        {
            char c;
            StringBuilder tag = new StringBuilder(context.SourceCode.Substring(context.NextLetterIndex, 1));
            context.NextLetterIndex++;
            string numberSerial = GetNumberSerial(context, 8);
            tag.Append(numberSerial);
            if (context.NextLetterIndex < context.SourceCode.Length)
            {
                c = context.CurrentChar();
                if (c == 'l' || c == 'L')
                {
                    tag.Append(c);
                    context.NextLetterIndex++;
                }
            }
            if (string.IsNullOrEmpty(numberSerial))
            {
                result.LexicalError = true;
                result.TokenType = new TokenType("number",
                    string.Format("八进制数[{0}]格式错误，无法解析。", tag.ToString()),
                    "number");
            }
            else
            {
                result.TokenType = new TokenType("number", tag.ToString(), "number");
            }
            return true;
        }
        /// <summary>
        /// 十六进制数
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetConstentHexadecimalNumber(Token result, AnalyzingContext context)
        {
            char c;
            StringBuilder tag = new StringBuilder(context.SourceCode.Substring(context.NextLetterIndex, 2));
            context.NextLetterIndex += 2;
            string numberSerial = GetNumberSerial(context, 16);
            tag.Append(numberSerial);
            if (context.NextLetterIndex < context.SourceCode.Length)
            {
                c = context.CurrentChar();
                if (c == 'l' || c == 'L')
                {
                    tag.Append(c);
                    context.NextLetterIndex++;
                }
            }
            if (string.IsNullOrEmpty(numberSerial))
            {
                result.LexicalError = true;
                result.TokenType = new TokenType("number",
                    string.Format("十六进制数[{0}]格式错误。", tag.ToString()),
                    "number");
            }
            else
            {
                result.TokenType = new TokenType("number", tag.ToString(), "number");
            }
            return true;
        }
        /// <summary>
        /// 数字序列
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="scale">进制</param>
        /// <returns></returns>
        protected virtual string GetNumberSerial(AnalyzingContext context, int scale)
        {
            if (scale == 10)
            {
                return GetNumberSerialDecimal(context);
            }
            if (scale == 16)
            {
                return GetNumberSerialHexadecimal(context);
            }
            if (scale == 8)
            {
                return GetNumberSerialOctonary(context);
            }
            return string.Empty;
        }
        /// <summary>
        /// 十进制数序列
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        protected virtual string GetNumberSerialDecimal(AnalyzingContext context)
        {
            StringBuilder result = new StringBuilder(String.Empty);
            char c;
            while (context.NextLetterIndex < context.SourceCode.Length)
            {
                c = context.CurrentChar();
                if ('0' <= c && c <= '9')
                {
                    result.Append(c);
                    context.NextLetterIndex++;
                }
                else
                    break;
            }
            return result.ToString();
        }
        /// <summary>
        /// 八进制数序列
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        protected virtual string GetNumberSerialOctonary(AnalyzingContext context)
        {
            StringBuilder result = new StringBuilder(String.Empty);
            char c;
            while (context.NextLetterIndex < context.SourceCode.Length)
            {
                c = context.CurrentChar();
                if ('0' <= c && c <= '7')
                {
                    result.Append(c);
                    context.NextLetterIndex++;
                }
                else
                    break;
            }
            return result.ToString();
        }
        /// <summary>
        /// 十六进制数序列（不包括0x前缀）
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        protected virtual string GetNumberSerialHexadecimal(AnalyzingContext context)
        {
            StringBuilder result = new StringBuilder(String.Empty);
            char c;
            while (context.NextLetterIndex < context.SourceCode.Length)
            {
                c = context.CurrentChar();
                if (('0' <= c && c <= '9')
                || ('a' <= c && c <= 'f')
                || ('A' <= c && c <= 'F'))
                {
                    result.Append(c);
                    context.NextLetterIndex++;
                }
                else
                    break;
            }
            return result.ToString();
        }
        #endregion GetConstentNumber
    }
}
