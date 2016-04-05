using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    /// <summary>
    /// 产生可处理注释的GetXXX()的关键部分。
    /// </summary>
    class CommentCodeGetTokenCollection : CodeGetTokenCollection
    {
        public CommentCodeGetTokenCollection(CodeGetTokenCollection collection)
        {
            foreach (var item in collection)
            {
                this.TryInsert(item);
            }
        }

        internal override CodeStatement[] DumpReadToken(string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            List<CodeStatement> result = new List<CodeStatement>();
            if (this.Count > 0)// 有除了"//"和"/*"之外的项，例如可能是"/", "/="
            {
                OtherItemsAndNote(result, grammarId, algorithm);
            }
            else
            {
                OnlyNote(result);
            }
            return result.ToArray();
        }

        private static void OnlyNote(List<CodeStatement> result)
        {
            int length = 2;
            // if (context.NextLetterIndex + {0} <= count)
            var ifStatement = new CodeConditionStatement(
                new CodeSnippetExpression(string.Format(
                    "context.NextLetterIndex + {0} <= count", length)));
            {
                // str = context.SourceCode.SubString(context.NextLetterIndex, {0});
                var str = new CodeVariableDeclarationStatement(typeof(string), "str");
                str.InitExpression = new CodeSnippetExpression(string.Format(
                    "context.SourceCode.Substring(context.NextLetterIndex, {0});", length));
                ifStatement.TrueStatements.Add(str);
            }
            SingleLine(ifStatement);
            MultiLine(ifStatement);

            result.Add(ifStatement);
        }

        private void OtherItemsAndNote(List<CodeStatement> result, string grammarId, SyntaxParserMapAlgorithm algorithm)
        {
            for (int length = this[0].Value.Type.Length; length > 0; length--)
            {
                bool exists = false;
                // if (context.NextLetterIndex + {0} <= count)
                var ifStatement = new CodeConditionStatement(
                    new CodeSnippetExpression(string.Format(
                        "context.NextLetterIndex + {0} <= count", length)));
                {
                    // str = context.SourceCode.SubString(context.NextLetterIndex, {0});
                    var str = new CodeVariableDeclarationStatement(typeof(string), "str");
                    str.InitExpression = new CodeSnippetExpression(string.Format(
                        "context.SourceCode.Substring(context.NextLetterIndex, {0});", length));
                    ifStatement.TrueStatements.Add(str);
                }
                foreach (var item in this)
                {
                    // if ("xxx" == str) { ... }
                    if (item.Value.Content.Length != length) { continue; }

                    CodeStatement[] statements = item.DumpReadToken(grammarId, algorithm);
                    if (statements != null)
                    {
                        ifStatement.TrueStatements.AddRange(statements);
                        exists = true;
                    }
                }

                if (length == 2)// 轮到添加对注释的处理了
                {
                    SingleLine(ifStatement);
                    MultiLine(ifStatement);

                    exists = true;
                }

                if (exists)
                {
                    result.Add(ifStatement);
                }
            }
        }

        private static void MultiLine(CodeConditionStatement ifStatement)
        {
            // if ("xxx" == str)
            var multiLine = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodePrimitiveExpression("/*"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeVariableReferenceExpression("str")));
            // this.SkipMultilineNote(context);
            multiLine.TrueStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "SkipMultilineNote",
                    new CodeVariableReferenceExpression("context")));
            ifStatement.TrueStatements.Add(multiLine);
        }

        private static void SingleLine(CodeConditionStatement ifStatement)
        {
            // if ("xxx" == str)
            var singleLine = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodePrimitiveExpression("//"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeVariableReferenceExpression("str")));
            // this.SkipSingleLineNote(context);
            singleLine.TrueStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "SkipSingleLineNote",
                    new CodeVariableReferenceExpression("context")));
            ifStatement.TrueStatements.Add(singleLine);
        }
    }
}
