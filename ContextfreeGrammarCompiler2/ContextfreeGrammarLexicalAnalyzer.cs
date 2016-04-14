namespace ContextfreeGrammarCompiler2
{
    using System;
    using System.Collections.Generic;
    using LALR1Compiler;
    
    
    public partial class ContextfreeGrammarLexicalAnalyzer : LALR1Compiler.LexicalAnalyzer
    {
        
        protected override bool TryGetToken(AnalyzingContext context, LALR1Compiler.Token result, SourceCodeCharType charType)
        {
            bool gotToken = false;
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
            if ((charType == SourceCodeCharType.Or))
            {
                gotToken = this.GetOr(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.LessThan))
            {
                gotToken = this.GetLessThan(result, context);
                return gotToken;
            }
            if (((charType == SourceCodeCharType.Letter) 
                        || (charType == SourceCodeCharType.UnderLine)))
            {
                gotToken = this.GetIdentifier(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.GreaterThan))
            {
                gotToken = this.GetGreaterThan(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.DoubleQuotation))
            {
                gotToken = this.GetDoubleQuotation(result, context);
                return gotToken;
            }
            if ((charType == SourceCodeCharType.Divide))
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
        
        protected virtual bool GetColon(LALR1Compiler.Token result, AnalyzingContext context)
        {
            int count = context.SourceCode.Length;
            if (context.NextLetterIndex + 3 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 3);;
                if (("::=" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(ContextfreeGrammarSLRTokenType.NODE__coloncolonequalLeave__, "::=", "\"::=\"");
                    context.NextLetterIndex = (context.NextLetterIndex + 3);
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
                    result.TokenType = new LALR1Compiler.TokenType(ContextfreeGrammarSLRTokenType.NODE__semicolonLeave__, ";", "\";\"");
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
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("|" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(ContextfreeGrammarSLRTokenType.NODE__vertical_barLeave__, "|", "\"|\"");
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
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if (("<" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(ContextfreeGrammarSLRTokenType.NODE__left_angleLeave__, "<", "\"<\"");
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
            if (context.NextLetterIndex + 1 <= count)
            {
                string str = context.SourceCode.Substring(context.NextLetterIndex, 1);;
                if ((">" == str))
                {
                    result.TokenType = new LALR1Compiler.TokenType(ContextfreeGrammarSLRTokenType.NODE__right_angleLeave__, ">", "\">\"");
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
                if (("//" == str))
                {
                    this.SkipSingleLineNote(context);
                }
                return false;
                if (("/*" == str))
                {
                    this.SkipMultilineNote(context);
                }
                return false;
            }
            string current = context.CurrentChar().ToString();
            result.TokenType = new LALR1Compiler.TokenType("__error", current, current);
            result.LexicalError = true;
            context.NextLetterIndex = (context.NextLetterIndex + 1);
            return false;
        }
        
        private static System.Collections.Generic.List<LALR1Compiler.Keyword> keywords = new System.Collections.Generic.List<LALR1Compiler.Keyword>();
        
        public override System.Collections.Generic.IEnumerable<LALR1Compiler.Keyword> GetKeywords()
        {
            return keywords;
        }
        
        static ContextfreeGrammarLexicalAnalyzer()
        {
            keywords.Add(new LALR1Compiler.Keyword("__null", "null"));
            keywords.Add(new LALR1Compiler.Keyword("__identifier", "identifier"));
            keywords.Add(new LALR1Compiler.Keyword("__number", "number"));
            keywords.Add(new LALR1Compiler.Keyword("__constString", "constString"));
            keywords.Add(new LALR1Compiler.Keyword("__userDefinedType", "userDefinedType"));
        }
    }
}
