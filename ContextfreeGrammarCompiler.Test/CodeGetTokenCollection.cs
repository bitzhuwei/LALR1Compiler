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
    class CodeGetTokenCollection :
        //OrderedCollection<CodeGetToken>
        IEnumerable<CodeGetToken>
    {

        public int Count { get { return this.list.Count; } }

        public CodeGetToken this[int index]
        {
            get { return this.list[index]; }
        }

        private List<CodeGetToken> list = new List<CodeGetToken>();

        //public CodeGetTokenCollection() : base(Environment.NewLine) { }

        internal virtual CodeStatement[] DumpReadToken()
        {
            List<CodeStatement> result = new List<CodeStatement>();

            for (int length = this.list[0].Value.Type.Length; length > 0; length--)
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
                foreach (var item in this.list)
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

        public IEnumerator<CodeGetToken> GetEnumerator()
        {
            foreach (var item in this.list)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void TryInsert(CodeGetToken codeGetToken)
        {
            if (!this.list.Contains(codeGetToken))
            {
                this.list.Add(codeGetToken);
            }
        }
    }
}
