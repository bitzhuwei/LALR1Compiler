using LALR1Compiler;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    /// <summary>
    /// 产生处理某种字符的GetXXX();的定义的关键部分
    /// </summary>
    class CodeGetTokenCollection : OrderedCollection<CodeGetToken>
    {

        public CodeGetTokenCollection() : base(Environment.NewLine) { }

        internal virtual CodeStatement[] DumpReadToken()
        {
            List<CodeStatement> result = new List<CodeStatement>();

            for (int length = this[0].Value.Type.Length; length > 0; length--)
            {
                bool exists = false;
                // if (context.NextLetterIndex + {0} <= count)
                var ifStatement = new CodeConditionStatement(
                    new CodeSnippetExpression(string.Format(
                        "context.NextLetterIndex + {0} <= count", length)));
                {
                    // str = context.SourceCode.Substring(context.NextLetterIndex, {0});
                    var str = new CodeVariableDeclarationStatement(typeof(string), "str");
                    str.InitExpression = new CodeSnippetExpression(string.Format(
                        "context.SourceCode.Substring(context.NextLetterIndex, {0});", length));
                    ifStatement.TrueStatements.Add(str);
                }
                foreach (var item in this)
                {
                    if (item.Value.Content.Length != length) { continue; }

                    // if ("XXX" == str) { ... }
                    CodeStatement[] statements = item.DumpReadToken();
                    if (statements != null)
                    {
                        ifStatement.TrueStatements.AddRange(statements);
                        exists = true;
                    }
                }
                if (exists)
                {
                    result.Add(ifStatement);
                }
            }

            return result.ToArray();
        }
    }
}
