using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LALR1Compiler;
using System.Text.RegularExpressions;

namespace ContextfreeGrammarCompiler
{
    public partial class ContextfreeGrammarLexicalAnalyzer : LexicalAnalyzer
    {
        #region 获取某类型的单词

        /// <summary>
        /// 字符串常量 "XXX"
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetConstentString(Token result, AnalyzingContext context)
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
                ContextfreeGrammarTokenType.constString, builder.ToString(), "constString");
            return true;
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
                var ct = GetCharType(ch);
                if (ct == ContextfreeGrammarCharType.Letter
                    || ct == ContextfreeGrammarCharType.Number
                    || ct == ContextfreeGrammarCharType.UnderLine
                    || ct == ContextfreeGrammarCharType.ChineseLetter)
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
            foreach (var item in ContextfreeGrammarLexicalAnalyzer.keywords)
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
                    ContextfreeGrammarTokenType.identifier, content, "identifier");
            }

            return true;
        }

        class Keyword
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

        /// <summary>
        /// 未知符号
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetUnknown(Token result, AnalyzingContext context)
        {
            string content = context.CurrentChar().ToString();
            result.TokenType = new TokenType(
                ContextfreeGrammarTokenType.__unknown, content, "unknown");
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

        /// <summary>
        /// 获取字符类型
        /// </summary>
        /// <param name="c">要归类的字符</param>
        /// <returns></returns>
        protected virtual ContextfreeGrammarCharType GetCharType(char c)
        {
            if (('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z')) return ContextfreeGrammarCharType.Letter;
            if ('0' <= c && c <= '9') return ContextfreeGrammarCharType.Number;
            if (c == '_') return ContextfreeGrammarCharType.UnderLine;
            if (c == '.') return ContextfreeGrammarCharType.Dot;
            if (c == ',') return ContextfreeGrammarCharType.Comma;
            if (c == '+') return ContextfreeGrammarCharType.Plus;
            if (c == '-') return ContextfreeGrammarCharType.Minus;
            if (c == '*') return ContextfreeGrammarCharType.Multiply;
            if (c == '/') return ContextfreeGrammarCharType.Divide;
            if (c == '%') return ContextfreeGrammarCharType.Percent;
            if (c == '^') return ContextfreeGrammarCharType.Xor;
            if (c == '&') return ContextfreeGrammarCharType.And;
            if (c == '|') return ContextfreeGrammarCharType.Or;
            if (c == '~') return ContextfreeGrammarCharType.Reverse;
            if (c == '$') return ContextfreeGrammarCharType.Dollar;
            if (c == '<') return ContextfreeGrammarCharType.LessThan;
            if (c == '>') return ContextfreeGrammarCharType.GreaterThan;
            if (c == '(') return ContextfreeGrammarCharType.LeftParentheses;
            if (c == ')') return ContextfreeGrammarCharType.RightParentheses;
            if (c == '[') return ContextfreeGrammarCharType.LeftBracket;
            if (c == ']') return ContextfreeGrammarCharType.RightBracket;
            if (c == '{') return ContextfreeGrammarCharType.LeftBrace;
            if (c == '}') return ContextfreeGrammarCharType.RightBrace;
            if (c == '!') return ContextfreeGrammarCharType.Not;
            if (c == '#') return ContextfreeGrammarCharType.Pound;
            if (c == '\\') return ContextfreeGrammarCharType.Slash;
            if (c == '?') return ContextfreeGrammarCharType.Question;
            if (c == '\'') return ContextfreeGrammarCharType.Quotation;
            if (c == '"') return ContextfreeGrammarCharType.DoubleQuotation;
            if (c == ':') return ContextfreeGrammarCharType.Colon;
            if (c == ';') return ContextfreeGrammarCharType.Semicolon;
            if (c == '=') return ContextfreeGrammarCharType.Equality;
            if (c == '@') return ContextfreeGrammarCharType.At;
            if (regChineseLetter.IsMatch(Convert.ToString(c))) return ContextfreeGrammarCharType.ChineseLetter;
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n') return ContextfreeGrammarCharType.Space;
            return ContextfreeGrammarCharType.Unknown;
        }
        /// <summary>
        /// 汉字 new Regex("^[^\x00-\xFF]")
        /// </summary>
        private static readonly Regex regChineseLetter = new Regex("^[^\x00-\xFF]");
    }
}
