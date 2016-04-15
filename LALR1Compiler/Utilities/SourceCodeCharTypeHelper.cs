using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public static class SourceCodeCharTypeHelper
    {

        /// <summary>
        /// 获取字符类型
        /// </summary>
        /// <param name="c">要归类的字符</param>
        /// <returns></returns>
        public static SourceCodeCharType GetCharType(this char c)
        {
            if (('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z')) return SourceCodeCharType.Letter;
            if ('0' <= c && c <= '9') return SourceCodeCharType.Number;
            if (c == '_') return SourceCodeCharType.UnderLine;
            if (c == '.') return SourceCodeCharType.Dot;
            if (c == ',') return SourceCodeCharType.Comma;
            if (c == '+') return SourceCodeCharType.Plus;
            if (c == '-') return SourceCodeCharType.Minus;
            if (c == '*') return SourceCodeCharType.Multiply;
            if (c == '/') return SourceCodeCharType.Slash;
            if (c == '%') return SourceCodeCharType.Percent;
            if (c == '^') return SourceCodeCharType.Xor;
            if (c == '&') return SourceCodeCharType.And;
            if (c == '|') return SourceCodeCharType.Or;
            if (c == '~') return SourceCodeCharType.Reverse;
            if (c == '$') return SourceCodeCharType.Dollar;
            if (c == '<') return SourceCodeCharType.LessThan;
            if (c == '>') return SourceCodeCharType.GreaterThan;
            if (c == '(') return SourceCodeCharType.LeftParentheses;
            if (c == ')') return SourceCodeCharType.RightParentheses;
            if (c == '[') return SourceCodeCharType.LeftBracket;
            if (c == ']') return SourceCodeCharType.RightBracket;
            if (c == '{') return SourceCodeCharType.LeftBrace;
            if (c == '}') return SourceCodeCharType.RightBrace;
            if (c == '!') return SourceCodeCharType.Not;
            if (c == '#') return SourceCodeCharType.Pound;
            if (c == '\\') return SourceCodeCharType.BackSlash;
            if (c == '?') return SourceCodeCharType.Question;
            if (c == '\'') return SourceCodeCharType.Quotation;
            if (c == '"') return SourceCodeCharType.DoubleQuotation;
            if (c == ':') return SourceCodeCharType.Colon;
            if (c == ';') return SourceCodeCharType.Semicolon;
            if (c == '=') return SourceCodeCharType.Equality;
            if (c == '@') return SourceCodeCharType.At;
            //if (regChineseLetter.IsMatch(Convert.ToString(c))) return SourceCodeCharType.ChineseLetter;
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n') return SourceCodeCharType.Space;
            return SourceCodeCharType.Unknown;
        }
        ///// <summary>
        ///// 汉字 new Regex("^[^\x00-\xFF]")
        ///// </summary>
        //private static readonly Regex regChineseLetter = new Regex("^[^\x00-\xFF]");
    }
}
