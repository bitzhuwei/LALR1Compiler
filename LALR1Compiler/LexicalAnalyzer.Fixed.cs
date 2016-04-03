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

        /// <summary>
        /// 在子类的静态构造函数中填充此列表。
        /// </summary>
        protected static readonly List<Keyword> keywords = new List<Keyword>();
    }

    /// <summary>
    /// 文法中指出的关键字（用""引起来的字符串）
    /// </summary>
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

    /// <summary>
    /// 词法分析的上下文
    /// </summary>
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
