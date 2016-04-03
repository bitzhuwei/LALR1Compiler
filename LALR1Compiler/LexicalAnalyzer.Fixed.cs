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
        /// 从<code>context.NextLetterIndex</code>开始获取下一个<code>Token</code>
        /// </summary>
        /// <returns></returns>
        protected Token NextToken(AnalyzingContext context)
        {
            var result = new Token();
            result.Line = context.CurrentLine;
            result.Column = context.CurrentColumn;
            result.IndexOfSourceCode = context.NextLetterIndex;
            var count = context.SourceCode.Length;
            if (context.NextLetterIndex < 0 || context.NextLetterIndex >= count)
            { return result; }
            var gotToken = false;
            char ch = context.CurrentChar();
            SourceCodeCharType charType = ch.GetCharType();
            gotToken = TryGetToken(context, result, charType);
            if (gotToken)
            {
                result.Length = context.NextLetterIndex - result.IndexOfSourceCode;
                return result;
            }
            else
            {
                return null;
            }
        }

        #region 获取某类型的单词

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

        protected virtual bool Getunderline(Token result, AnalyzingContext context)
        {
            return GetLetter(result, context);
        }
        /// <summary>
        /// 获取标识符（函数名，变量名，等）
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetLetter(Token result, AnalyzingContext context)
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
            // specify if this string is a keyword
            bool isKeyword = false;
            foreach (var item in LexicalAnalyzer.keywords)
            {
                if (item.NickName == content)
                {
                    result.TokenType = new TokenType(item.TokenType, content, content);
                    isKeyword = true;
                    break;
                }
            }
            if (!isKeyword)
            {
                result.TokenType = new TokenType(
                    "identifier", content, "identifier");
            }

            return true;
        }

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
                context.CurrentColumn = 0;
            }
            return false;
        }
        /// <summary>
        /// 跳过多行注释
        /// </summary>
        /// <returns></returns>
        protected virtual void SkipMultilineNote(AnalyzingContext context)
        {
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
        #endregion 获取某类型的单词


        protected static readonly List<Keyword> keywords = new List<Keyword>();
    }


    public class Keyword
    {
        public string TokenType { get; set; }
        public string NickName { get; set; }

        public Keyword(string tokenType, string nickName)
        {
            // TODO: Complete member initialization
            this.TokenType = tokenType;
            this.NickName = nickName;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", TokenType, NickName);
        }
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
