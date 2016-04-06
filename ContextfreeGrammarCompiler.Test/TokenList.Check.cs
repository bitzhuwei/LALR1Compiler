using LALR1Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    static class TokenListHelper
    {
        public static bool Check(this TokenList tokenList, out string errorInfo)
        {
            bool error = false;
            errorInfo = string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Error tokens:");
            foreach (var token in tokenList)
            {
                if (token.LexicalError)
                {
                    builder.AppendLine(token.ToString());
                    error = true;
                }
            }

            if (error)
            { errorInfo = builder.ToString(); }

            return !error;
        }
    }
}
