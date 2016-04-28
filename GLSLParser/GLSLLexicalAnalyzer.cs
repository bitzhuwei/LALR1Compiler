namespace GLSLParser
{
    using System;
    using System.Collections.Generic;
    using LALR1Compiler;
    
    
    public partial class GLSLLexicalAnalyzer : LALR1Compiler.LexicalAnalyzer
    {
        
        protected override bool TryGetToken(AnalyzingContext context, LALR1Compiler.Token result, SourceCodeCharType charType)
        {
            bool gotToken = false;
            if (((charType == SourceCodeCharType.Letter) 
                        || (charType == SourceCodeCharType.UnderLine)))
            {
                gotToken = this.GetIdentifier(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.LeftBrace))
            {
                gotToken = this.GetLeftBrace(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.LeftParentheses))
            {
                gotToken = this.GetLeftParentheses(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.RightParentheses))
            {
                gotToken = this.GetRightParentheses(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.LeftBracket))
            {
                gotToken = this.GetLeftBracket(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.RightBracket))
            {
                gotToken = this.GetRightBracket(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Dot))
            {
                gotToken = this.GetDot(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Plus))
            {
                gotToken = this.GetPlus(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Minus))
            {
                gotToken = this.GetMinus(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Comma))
            {
                gotToken = this.GetComma(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Not))
            {
                gotToken = this.GetNot(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Reverse))
            {
                gotToken = this.GetReverse(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Multiply))
            {
                gotToken = this.GetMultiply(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Percent))
            {
                gotToken = this.GetPercent(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.LessThan))
            {
                gotToken = this.GetLessThan(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.GreaterThan))
            {
                gotToken = this.GetGreaterThan(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Equality))
            {
                gotToken = this.GetEquality(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.And))
            {
                gotToken = this.GetAnd(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Xor))
            {
                gotToken = this.GetXor(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Or))
            {
                gotToken = this.GetOr(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Question))
            {
                gotToken = this.GetQuestion(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Colon))
            {
                gotToken = this.GetColon(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Semicolon))
            {
                gotToken = this.GetSemicolon(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.RightBrace))
            {
                gotToken = this.GetRightBrace(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Slash))
            {
                gotToken = this.Getslash(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Space))
            {
                gotToken = this.GetSpace(result, context);
                return gotToken;
            }
            gotToken = this.GetUnknown(result, context);
            return gotToken;
        }
        
        protected virtual bool GetLeftBrace(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 8 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 8);;
                if (("{number}" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__numberLeave__, "{number}", "number");
                    context.NextLetterIndex = (context.NextLetterIndex + 8);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("{" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__left_braceLeave__, "{", "\"{\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetLeftParentheses(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("(" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__left_parenLeave__, "(", "\"(\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetRightParentheses(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if ((")" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__right_parenLeave__, ")", "\")\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetLeftBracket(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("[" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__left_bracketLeave__, "[", "\"[\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetRightBracket(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("]" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__right_bracketLeave__, "]", "\"]\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetDot(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("." == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__dotLeave__, ".", "\".\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetPlus(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("++" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__plusplusLeave__, "++", "\"++\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if (("+=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__plusequalLeave__, "+=", "\"+=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("+" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__plusLeave__, "+", "\"+\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetMinus(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("--" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__dashdashLeave__, "--", "\"--\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if (("-=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__dashequalLeave__, "-=", "\"-=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("-" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__dashLeave__, "-", "\"-\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetComma(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("," == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__commaLeave__, ",", "\",\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetNot(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("!=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__bangequalLeave__, "!=", "\"!=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("!" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__bangLeave__, "!", "\"!\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetReverse(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("~" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__tildeLeave__, "~", "\"~\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetMultiply(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("*=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__starequalLeave__, "*=", "\"*=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("*" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__starLeave__, "*", "\"*\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetPercent(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("%=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__percentequalLeave__, "%=", "\"%=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("%" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__percentLeave__, "%", "\"%\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetLessThan(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 3 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 3);;
                if (("<<=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__left_angleleft_angleequalLeave__, "<<=", "\"<<=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 3);
                    return true;
                }
            }
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("<<" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__left_angleleft_angleLeave__, "<<", "\"<<\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if (("<=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__left_angleequalLeave__, "<=", "\"<=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("<" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__left_angleLeave__, "<", "\"<\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetGreaterThan(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 3 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 3);;
                if ((">>=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__right_angleright_angleequalLeave__, ">>=", "\">>=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 3);
                    return true;
                }
            }
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if ((">>" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__right_angleright_angleLeave__, ">>", "\">>\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if ((">=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__right_angleequalLeave__, ">=", "\">=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if ((">" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__right_angleLeave__, ">", "\">\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetEquality(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("==" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__equalequalLeave__, "==", "\"==\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__equalLeave__, "=", "\"=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetAnd(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("&&" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__and_opand_opLeave__, "&&", "\"&&\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if (("&=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__and_opequalLeave__, "&=", "\"&=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("&" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__and_opLeave__, "&", "\"&\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetXor(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("^^" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__caretcaretLeave__, "^^", "\"^^\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if (("^=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__caretequalLeave__, "^=", "\"^=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("^" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__caretLeave__, "^", "\"^\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetOr(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("||" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__vertical_barvertical_barLeave__, "||", "\"||\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if (("|=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__vertical_barequalLeave__, "|=", "\"|=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("|" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__vertical_barLeave__, "|", "\"|\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetQuestion(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("?" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__questionLeave__, "?", "\"?\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetColon(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if ((":" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__colonLeave__, ":", "\":\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetSemicolon(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if ((";" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__semicolonLeave__, ";", "\";\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool GetRightBrace(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("}" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__right_braceLeave__, "}", "\"}\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        protected virtual bool Getslash(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 2 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 2);;
                if (("/=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__slashequalLeave__, "/=", "\"/=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 2);
                    return true;
                }
                if (("//" == str))
                {
                    this.SkipSingleLineNote(context);
                    return false;
                }
                if (("/*" == str))
                {
                    this.SkipMultilineNote(context);
                    return false;
                }
            }
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("/" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(GLSLTokenType.NODE__slashLeave__, "/", "\"/\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 1);
                    return true;
                }
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        private static LALR1Compiler.OrderedCollection<LALR1Compiler.Keyword> keywords = new LALR1Compiler.OrderedCollection<LALR1Compiler.Keyword>(", ");
        
        public override LALR1Compiler.OrderedCollection<LALR1Compiler.Keyword> GetKeywords()
        {
            return keywords;
        }
        
        static GLSLLexicalAnalyzer()
        {
            keywords.TryInsert(new LALR1Compiler.Keyword("__true", "true"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__false", "false"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__void", "void"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__precision", "precision"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__invariant", "invariant"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__smooth", "smooth"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__flat", "flat"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__noperspective", "noperspective"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__layout", "layout"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__precise", "precise"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__const", "const"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__inout", "inout"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__in", "in"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__out", "out"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__centroid", "centroid"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__patch", "patch"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sample", "sample"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uniform", "uniform"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__buffer", "buffer"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__shared", "shared"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__coherent", "coherent"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__volatile", "volatile"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__restrict", "restrict"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__readonly", "readonly"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__writeonly", "writeonly"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__subroutine", "subroutine"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__float", "float"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__double", "double"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__int", "int"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uint", "uint"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__bool", "bool"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__vec2", "vec2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__vec3", "vec3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__vec4", "vec4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dvec2", "dvec2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dvec3", "dvec3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dvec4", "dvec4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__bvec2", "bvec2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__bvec3", "bvec3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__bvec4", "bvec4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__ivec2", "ivec2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__ivec3", "ivec3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__ivec4", "ivec4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uvec2", "uvec2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uvec3", "uvec3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uvec4", "uvec4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat2", "mat2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat3", "mat3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat4", "mat4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat2x2", "mat2x2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat2x3", "mat2x3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat2x4", "mat2x4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat3x2", "mat3x2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat3x3", "mat3x3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat3x4", "mat3x4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat4x2", "mat4x2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat4x3", "mat4x3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__mat4x4", "mat4x4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat2", "dmat2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat3", "dmat3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat4", "dmat4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat2x2", "dmat2x2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat2x3", "dmat2x3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat2x4", "dmat2x4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat3x2", "dmat3x2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat3x3", "dmat3x3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat3x4", "dmat3x4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat4x2", "dmat4x2"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat4x3", "dmat4x3"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__dmat4x4", "dmat4x4"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__atomic_uint", "atomic_uint"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler1D", "sampler1D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2D", "sampler2D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler3D", "sampler3D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__samplerCube", "samplerCube"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler1DShadow", "sampler1DShadow"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2DShadow", "sampler2DShadow"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__samplerCubeShadow", "samplerCubeShadow"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler1DArray", "sampler1DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2DArray", "sampler2DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler1DArrayShadow", "sampler1DArrayShadow"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2DArrayShadow", "sampler2DArrayShadow"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__samplerCubeArray", "samplerCubeArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__samplerCubeArrayShadow", "samplerCubeArrayShadow"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler1D", "isampler1D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler2D", "isampler2D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler3D", "isampler3D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isamplerCube", "isamplerCube"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler1DArray", "isampler1DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler2DArray", "isampler2DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isamplerCubeArray", "isamplerCubeArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler1D", "usampler1D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler2D", "usampler2D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler3D", "usampler3D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usamplerCube", "usamplerCube"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler1DArray", "usampler1DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler2DArray", "usampler2DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usamplerCubeArray", "usamplerCubeArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2DRect", "sampler2DRect"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2DRectShadow", "sampler2DRectShadow"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler2DRect", "isampler2DRect"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler2DRect", "usampler2DRect"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__samplerBuffer", "samplerBuffer"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isamplerBuffer", "isamplerBuffer"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usamplerBuffer", "usamplerBuffer"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2DMS", "sampler2DMS"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler2DMS", "isampler2DMS"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler2DMS", "usampler2DMS"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__sampler2DMSArray", "sampler2DMSArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__isampler2DMSArray", "isampler2DMSArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__usampler2DMSArray", "usampler2DMSArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image1D", "image1D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage1D", "iimage1D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage1D", "uimage1D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image2D", "image2D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage2D", "iimage2D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage2D", "uimage2D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image3D", "image3D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage3D", "iimage3D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage3D", "uimage3D"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image2DRect", "image2DRect"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage2DRect", "iimage2DRect"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage2DRect", "uimage2DRect"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__imageCube", "imageCube"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimageCube", "iimageCube"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimageCube", "uimageCube"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__imageBuffer", "imageBuffer"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimageBuffer", "iimageBuffer"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimageBuffer", "uimageBuffer"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image1DArray", "image1DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage1DArray", "iimage1DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage1DArray", "uimage1DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image2DArray", "image2DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage2DArray", "iimage2DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage2DArray", "uimage2DArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__imageCubeArray", "imageCubeArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimageCubeArray", "iimageCubeArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimageCubeArray", "uimageCubeArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image2DMS", "image2DMS"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage2DMS", "iimage2DMS"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage2DMS", "uimage2DMS"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__image2DMSArray", "image2DMSArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__iimage2DMSArray", "iimage2DMSArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__uimage2DMSArray", "uimage2DMSArray"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__high_precision", "high_precision"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__medium_precision", "medium_precision"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__low_precision", "low_precision"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__struct", "struct"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__if", "if"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__else", "else"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__switch", "switch"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__case", "case"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__default", "default"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__while", "while"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__do", "do"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__for", "for"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__continue", "continue"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__break", "break"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__return", "return"));
            keywords.TryInsert(new LALR1Compiler.Keyword("__discard", "discard"));
        }
    }
}
