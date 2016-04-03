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

        protected override bool TryGetToken(
            AnalyzingContext context, Token result, SourceCodeCharType charType)
        {
            bool gotToken = false;
            switch (charType)
            {
                case SourceCodeCharType.Letter:
                    gotToken = GetLetter(result, context);
                    break;
                case SourceCodeCharType.UnderLine:
                    gotToken = Getunderline(result, context);
                    break;
                case SourceCodeCharType.Or:
                    gotToken = GetOr(result, context);
                    break;
                case SourceCodeCharType.LessThan:
                    gotToken = GetLessThan(result, context);
                    break;
                case SourceCodeCharType.GreaterThan:
                    gotToken = GetGreaterThan(result, context);
                    break;
                case SourceCodeCharType.DoubleQuotation:
                    gotToken = GetDoubleQuotation(result, context);
                    break;
                case SourceCodeCharType.Semicolon:
                    gotToken = GetSemicolon(result, context);
                    break;
                case SourceCodeCharType.Colon:
                    gotToken = GetColon(result, context);
                    break;
                case SourceCodeCharType.Divide:
                    gotToken = GetDivide(result, context);
                    break;
                case SourceCodeCharType.Space:
                    gotToken = GetSpace(result, context);
                    break;
                default:
                    gotToken = GetUnknown(result, context);
                    break;
            }

            return gotToken;
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

        protected static readonly List<Keyword> keywords = new List<Keyword>();

        protected override IEnumerable<Keyword> GetKeywords()
        {
            return keywords;
        }

        static ContextfreeGrammarLexicalAnalyzer()
        {
            keywords.Add(new Keyword(ContextfreeGrammarTokenType.__identifier, ContextfreeGrammarTokenType.__identifier.Substring(2)));
            keywords.Add(new Keyword(ContextfreeGrammarTokenType.__null, ContextfreeGrammarTokenType.__null.Substring(2)));
            keywords.Add(new Keyword(ContextfreeGrammarTokenType.__constString, ContextfreeGrammarTokenType.__constString.Substring(2)));
            keywords.Add(new Keyword(ContextfreeGrammarTokenType.__number, ContextfreeGrammarTokenType.__number.Substring(2)));
        }

    }
}
