using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public static class ConstString2IdentifierHelper
    {

        static Dictionary<char, string> punctuationDict = new Dictionary<char, string>();

        static ConstString2IdentifierHelper()
        {
            punctuationDict.Add('~', "tilde"); // 波浪字符
            punctuationDict.Add('`', "separation"); // 间隔号
            punctuationDict.Add('!', "bang");
            punctuationDict.Add('@', "at");
            punctuationDict.Add('#', "pound");
            punctuationDict.Add('$', "dollar");
            punctuationDict.Add('%', "percent");
            punctuationDict.Add('^', "caret");
            punctuationDict.Add('&', "and_op");
            punctuationDict.Add('*', "star");
            punctuationDict.Add('(', "left_paren");
            punctuationDict.Add(')', "right_paren");
            //punctuationDict.Add('_', "underline");
            punctuationDict.Add('-', "dash");
            punctuationDict.Add('+', "plus");
            punctuationDict.Add('=', "equal");
            punctuationDict.Add('{', "left_brace");
            punctuationDict.Add('[', "left_bracket");
            punctuationDict.Add('}', "right_brace");
            punctuationDict.Add(']', "right_bracket");
            punctuationDict.Add('|', "vertical_bar");
            punctuationDict.Add('\\', "backslash");
            punctuationDict.Add(':', "colon");
            punctuationDict.Add(';', "semicolon");
            punctuationDict.Add('"', "double_quote");
            punctuationDict.Add('\'', "quote");
            punctuationDict.Add('<', "left_angle");
            punctuationDict.Add(',', "comma");
            punctuationDict.Add('>', "right_angle");
            punctuationDict.Add('.', "dot");
            punctuationDict.Add('?', "question");
            punctuationDict.Add('/', "slash");
        }

        public static string ConstString2Identifier(string content)
        {
            if (string.IsNullOrEmpty(content)) { throw new ArgumentNullException(); }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
            {
                char ch = content[i];
                string part = null;
                if (!punctuationDict.TryGetValue(ch, out part))
                {
                    part = ch.ToString();
                }

                builder.Append(part);
            }
            string result = builder.ToString();
            {
                char firstChar = result[0];
                bool needPrefix = true;
                if ('a' <= firstChar && firstChar <= 'z') { needPrefix = false; }
                if ('A' <= firstChar && firstChar <= 'Z') { needPrefix = false; }
                if (firstChar == '_') { needPrefix = false; }

                if (needPrefix)
                { result = "_" + result; }
            }

            return result;
        }

    }
}
