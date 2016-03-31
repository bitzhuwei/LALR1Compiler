using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public static partial class RegulationListHelper
    {
        /// <summary>
        /// 检测文法是否有什么直接要修改的问题。
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        public static string CheckCompleteness(this RegulationList grammar, out bool error)
        {
            error = false;

            StringBuilder builder = new StringBuilder();
            {
                bool innerError = false;
                // 重复的推导式
                string reduplicative = Getduplication(grammar, out innerError);
                builder.AppendLine(reduplicative);
                if (innerError) { error = true; }
            }

            {
                bool innerError = false;
                // 某些非叶结点没有定义其产生式
                string lack = GetLack(grammar, out innerError);
                builder.AppendLine(lack);
                if (innerError) { error = true; }
            }

            {
                bool innerError = false;
                // 某些非产生式没有被用到
                string lack = GetUnused(grammar, out innerError);
                builder.AppendLine(lack);
                if (innerError) { error = true; }
            }
            return builder.ToString();
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
