using LALR1Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    public static partial class RegulationListHelper
    {
        /// <summary>
        /// 检测文法是否有什么直接要修改的问题。
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        public static bool SemanticCheck(this RegulationList grammar, out string errorInfo)
        {
            bool error = false;

            StringBuilder builder = new StringBuilder();
            {
                bool innerError = false;
                // 重复的推导式
                string reduplicative = Getduplication(grammar, out innerError);
                if (innerError)
                {
                    builder.AppendLine(reduplicative);
                    error = true;
                }
            }

            {
                bool innerError = false;
                // 某些非叶结点没有定义其产生式
                string lack = GetLack(grammar, out innerError);
                if (innerError)
                {
                    builder.AppendLine(lack);
                    error = true;
                }
            }

            {
                bool innerError = false;
                // 某些非产生式没有被用到
                string lack = GetUnused(grammar, out innerError);
                if (innerError)
                {
                    builder.AppendLine(lack);
                    error = true;
                }
            }
            {
                bool innerError = false;
                // constString里的内容也不是随便什么都可以
                string lack = CheckConstStrings(grammar, out innerError);
                if (innerError)
                {
                    builder.AppendLine(lack);
                    error = true;
                }
            }

            errorInfo = builder.ToString();

            return !error;
        }

        static List<string> contextfreeGrammarKeywordList = new List<string>();
        static RegulationListHelper()
        {
            var lexi = new ContextfreeGrammarLexicalAnalyzer();
            foreach (var item in lexi.GetKeywords())
            {
                contextfreeGrammarKeywordList.Add(item.NickName);
            }
        }

        private static string CheckConstStrings(RegulationList grammar, out bool error)
        {
            error = false;
            StringBuilder builder = new StringBuilder();
            foreach (var node in grammar.GetAllTreeNodeLeaveTypes())
            {
                if (contextfreeGrammarKeywordList.Contains(node.Nickname)) { continue; }

                bool innderError = false;
                string lack = CheckNode(node, out innderError);
                builder.Append(innderError);
                if (innderError) { error = true; }
            }

            return builder.ToString();
        }

        private static string CheckNode(TreeNodeType node, out bool innderError)
        {
            string content = node.Content;
            {
                // 长度为0（即""）
                if (string.IsNullOrEmpty(content))
                {
                    innderError = true;
                    return string.Format("Empty node is not allowed in [{0}]", node);
                }
            }
            {
                // 有空格的
                int count = content.Count(x => x.GetCharType() == SourceCodeCharType.Space);
                if (count > 0)
                {
                    innderError = true;
                    return string.Format("Space is not allowed in [{0}]", node.Content);
                }
            }
            {
                // 数字开头的
                if ('0' <= content[0] && content[0] <= '9')
                {
                    innderError = true;
                    return string.Format("Starting with number is not allowed in [{0}]", node.Content);
                }
            }
            SourceCodeCharType charType = content[0].GetCharType();
            {
                if (charType == SourceCodeCharType.Letter || charType == SourceCodeCharType.UnderLine)
                {
                    // 标识符内混入符号
                    foreach (var item in content)
                    {
                        SourceCodeCharType type = item.GetCharType();
                        if (type != SourceCodeCharType.Letter
                            && type != SourceCodeCharType.UnderLine
                            && type != SourceCodeCharType.Number)
                        {
                            innderError = true;
                            return string.Format("Only letter, _ and number are allowed in an identifier for [{0}]", node.Content);
                        }
                    }
                }
                else
                {
                    // 符号内混入标识符
                    foreach (var item in content)
                    {
                        SourceCodeCharType type = item.GetCharType();
                        if (type == SourceCodeCharType.Letter
                            || type == SourceCodeCharType.UnderLine
                            || type == SourceCodeCharType.Number)
                        {
                            innderError = true;
                            return string.Format("No letter, _ or number is allowed in an operator for [{0}]", node.Content);
                        }
                    }
                }
            }

            // 检查到此，没有问题
            innderError = false;
            return string.Empty;
        }

        private static string GetUnused(RegulationList grammar, out bool error)
        {
            StringBuilder builder = new StringBuilder();
            List<TreeNodeType> usedList = new List<TreeNodeType>();
            usedList.Add(grammar[0].Left);
            bool changed = false;
            int index = 0;
            do
            {
                changed = false;
                int count = usedList.Count;
                for (; index < count; index++)
                {
                    TreeNodeType node = usedList[index];
                    foreach (var regulation in grammar)
                    {
                        if (regulation.Left == node)
                        {
                            foreach (var item in regulation.RightPart)
                            {
                                if ((!item.IsLeave) && (!usedList.Contains(item)))
                                {
                                    usedList.Add(item);
                                    changed = true;
                                }
                            }
                        }
                    }
                }
            } while (changed);

            List<TreeNodeType> unusedList = new List<TreeNodeType>();
            List<TreeNodeType> allNonLeaveNodeList = grammar.GetAllTreeNodeNonLeaveTypes();
            foreach (var node in allNonLeaveNodeList)
            {
                if (!usedList.Contains(node))
                {
                    unusedList.Add(node);
                }
            }
            builder.AppendLine("====================================================================");
            builder.AppendLine(string.Format("{0} unused nodes:", unusedList.Count));
            foreach (var item in unusedList)
            {
                builder.AppendLine(item.ToString());
            }

            error = unusedList.Count > 0;

            return builder.ToString();
        }

        private static string GetLack(RegulationList grammar, out bool error)
        {
            StringBuilder builder = new StringBuilder();
            List<TreeNodeType> lackList = new List<TreeNodeType>();
            var nonLeaveNodeList = grammar.GetAllTreeNodeNonLeaveTypes();

            foreach (var regulation in grammar)
            {
                foreach (var node in regulation.RightPart)
                {
                    if ((!node.IsLeave)
                        && (!nonLeaveNodeList.Contains(node))
                        && (!lackList.Contains(node)))
                    {
                        lackList.Add(node);
                    }
                }
            }

            builder.AppendLine("====================================================================");
            builder.AppendLine(string.Format("Lack of [{0}] regulation's definitions:", lackList.Count));
            foreach (var item in lackList)
            {
                builder.AppendLine(string.Format("{0}", item));
            }

            error = lackList.Count > 0;

            return builder.ToString();
        }

        private static string Getduplication(RegulationList grammar, out bool error)
        {
            StringBuilder builder = new StringBuilder();
            var duplications = (from a in grammar
                                join b in grammar on a equals b
                                where grammar.IndexOf(a) != grammar.IndexOf(b)
                                select new { a, b }).ToList();

            builder.AppendLine("====================================================================");
            builder.AppendLine(string.Format("[{0}] duplicated regulation couples:", duplications.Count));
            foreach (var item in duplications)
            {
                builder.AppendLine(string.Format("{0}: duplicated at index {1} and {2}",
                    item.a, grammar.IndexOf(item.a), grammar.IndexOf(item.b)));
            }

            error = duplications.Count > 0;

            return builder.ToString();
        }
    }
}
