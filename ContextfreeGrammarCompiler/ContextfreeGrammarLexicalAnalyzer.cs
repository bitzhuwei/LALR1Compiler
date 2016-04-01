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
        /// <summary>
        /// 从<code>context.NextLetterIndex</code>开始获取下一个<code>Token</code>
        /// </summary>
        /// <returns></returns>
        protected override Token NextToken(AnalyzingContext context)
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
            var ct = GetCharType(ch);
            switch (ct)
            {
                case ContextfreeGrammarCharType.Letter:
                    gotToken = GetIdentifier(result, context);
                    break;
                case ContextfreeGrammarCharType.UnderLine:
                    gotToken = GetIdentifier(result, context);
                    break;
                case ContextfreeGrammarCharType.Or:
                    gotToken = GetOr(result, context);
                    break;
                case ContextfreeGrammarCharType.LessThan:
                    gotToken = GetLessThan(result, context);
                    break;
                case ContextfreeGrammarCharType.GreaterThan:
                    gotToken = GetGreaterThan(result, context);
                    break;
                case ContextfreeGrammarCharType.DoubleQuotation:
                    gotToken = GetConstentString(result, context);
                    break;
                case ContextfreeGrammarCharType.Semicolon:
                    gotToken = GetSemicolon(result, context);
                    break;
                case ContextfreeGrammarCharType.Colon:
                    gotToken = GetColon(result, context);
                    break;
                case ContextfreeGrammarCharType.Divide:
                    gotToken = GetDivide(result, context);
                    break;
                case ContextfreeGrammarCharType.Space:
                    gotToken = GetSpace(result, context);
                    break;
                default:
                    gotToken = GetUnknown(result, context);
                    break;
            }
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

        protected virtual bool GetDivide(Token result, AnalyzingContext context)
        {
            var count = context.SourceCode.Length;
            //item.CharType: Colon
            //Mapped nodes:
            //    "//" "/*"
            if (context.NextLetterIndex + 2 <= count)
            {
                var str = context.SourceCode.Substring(context.NextLetterIndex, 2);
                if ("//" == str)
                {
                    SkipSingleLineNote(context);
                }
                else if ("/*" == str)
                {
                    SkipMultilineNote(context);
                }
            }

            return false;
        }

        /// <summary>
        /// ::=
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetColon(Token result, AnalyzingContext context)
        {
            var count = context.SourceCode.Length;
            //item.CharType: Colon
            //Mapped nodes:
            //    "::="
            if (context.NextLetterIndex + 3 <= count)
            {
                var str = context.SourceCode.Substring(context.NextLetterIndex, 3);
                if ("::=" == str)
                {
                    result.TokenType = new TokenType(
                        ContextfreeGrammarTokenType.__colon_colon_equal, str, "\"" + str + "\"");
                    context.NextLetterIndex += 3;
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// |
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetOr(Token result, AnalyzingContext context)
        {
            var count = context.SourceCode.Length;
            //item.CharType: Or
            //Mapped nodes:
            //    "|"
            if (context.NextLetterIndex + 1 <= count)
            {
                var str = context.SourceCode.Substring(context.NextLetterIndex, 1);
                if ("|" == str)
                {
                    result.TokenType = new TokenType(
                        ContextfreeGrammarTokenType.__vertical_bar, str, "\"" + str + "\"");
                    context.NextLetterIndex += 1;
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// &lt;
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetLessThan(Token result, AnalyzingContext context)
        {
            var count = context.SourceCode.Length;
            //item.CharType: lessThan
            //Mapped nodes:
            //    "<"
            if (context.NextLetterIndex + 1 <= count)
            {
                var str = context.SourceCode.Substring(context.NextLetterIndex, 1);
                if ("<" == str)
                {
                    result.TokenType = new TokenType(
                        ContextfreeGrammarTokenType.__left_angle, str, "\"" + str + "\"");
                    context.NextLetterIndex += 1;
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// &gt;
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetGreaterThan(Token result, AnalyzingContext context)
        {
            var count = context.SourceCode.Length;
            //item.CharType: greaterThan
            //Mapped nodes:
            //    ">"
            if (context.NextLetterIndex + 1 <= count)
            {
                var str = context.SourceCode.Substring(context.NextLetterIndex, 1);
                if (">" == str)
                {
                    result.TokenType = new TokenType(
                        ContextfreeGrammarTokenType.__right_angle, str, "\"" + str + "\"");
                    context.NextLetterIndex += 1;
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// ;
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool GetSemicolon(Token result, AnalyzingContext context)
        {
            var count = context.SourceCode.Length;
            //item.CharType: greaterThan
            //Mapped nodes:
            //    ""
            if (context.NextLetterIndex + 1 <= count)
            {
                var str = context.SourceCode.Substring(context.NextLetterIndex, 1);
                if (";" == str)
                {
                    result.TokenType = new TokenType(
                        ContextfreeGrammarTokenType.__semicolon, str, "\"" + str + "\"");
                    context.NextLetterIndex += 1;
                    return true;
                }
            }

            return false;
        }

        private static readonly IEnumerable<Keyword> keywords = new List<Keyword>()
        {
            new Keyword(ContextfreeGrammarTokenType.__identifier,ContextfreeGrammarTokenType.__identifier.Substring(2)),
            new Keyword(ContextfreeGrammarTokenType.__null,ContextfreeGrammarTokenType.__null.Substring(2)),
            new Keyword(ContextfreeGrammarTokenType.__constString,ContextfreeGrammarTokenType.__constString.Substring(2)),
            new Keyword(ContextfreeGrammarTokenType.__number,ContextfreeGrammarTokenType.__number.Substring(2)),
        };


    }
}
