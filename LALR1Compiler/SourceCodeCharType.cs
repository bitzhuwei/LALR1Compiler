﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public enum SourceCodeCharType
    {

        /// <summary>
        /// 未知字符
        /// </summary>
        Unknown,
        /// <summary>
        /// a-z A-Z
        /// </summary>
        Letter,
        ///// <summary>
        ///// 汉字
        ///// </summary>
        //ChineseLetter,
        /// <summary>
        /// 0 1 2 3 4 5 6 7 8 9
        /// </summary>
        Number,
        /// <summary>
        /// _
        /// </summary>
        UnderLine,
        /// <summary>
        /// .
        /// </summary>
        Dot,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// *
        /// </summary>
        Multiply,
        /// <summary>
        /// /
        /// </summary>
        Slash,
        /// <summary>
        /// %
        /// </summary>
        Percent,
        /// <summary>
        /// ^
        /// </summary>
        Xor,//10
        /// <summary>
        /// &amp;
        /// </summary>
        And,
        /// <summary>
        /// |
        /// </summary>
        Or,
        /// <summary>
        /// ~
        /// </summary>
        Reverse,
        /// <summary>
        /// $
        /// </summary>
        Dollar,
        /// <summary>
        /// &lt;
        /// </summary>
        LessThan,
        /// <summary>
        /// &gt;
        /// </summary>
        GreaterThan,
        /// <summary>
        /// (
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// )
        /// </summary>
        RightParentheses,
        /// <summary>
        /// [
        /// </summary>
        LeftBracket,
        /// <summary>
        /// ]
        /// </summary>
        RightBracket,
        /// <summary>
        /// {
        /// </summary>
        LeftBrace,
        /// <summary>
        /// }
        /// </summary>
        RightBrace,
        /// <summary>
        /// !
        /// </summary>
        Not,
        /// <summary>
        /// #
        /// </summary>
        Pound,
        /// <summary>
        /// \
        /// </summary>
        BackSlash,
        /// <summary>
        /// ?
        /// </summary>
        Question,
        /// <summary>
        /// '
        /// </summary>
        Quotation,
        /// <summary>
        /// "
        /// </summary>
        DoubleQuotation,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// ;
        /// </summary>
        Semicolon,
        /// <summary>
        /// =
        /// </summary>
        Equality,
        /// <summary>
        /// @
        /// </summary>
        At,
        /// <summary>
        /// space Tab \r\n
        /// </summary>
        Space,
    }
}
