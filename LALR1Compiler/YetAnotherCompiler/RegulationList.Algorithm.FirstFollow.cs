using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public static partial class RegulationListHelper
    {
        /// <summary>
        /// 计算文法的FIRST和FOLLOW集
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        public static FirstListAndFollowList GetFirstListAndFollowList(this RegulationList grammar)
        {
            Dictionary<TreeNodeType, bool> nullableDict = new Dictionary<TreeNodeType, bool>();
            GetNullableDict(grammar, nullableDict);
            List<FIRST> firstList4Node = GetFirstList4Node(grammar, nullableDict);
            List<FIRST> firstList4Regulation = GetFirstList4Regulation(grammar, nullableDict, firstList4Node);
            List<FOLLOW> followList = GetFollowList(grammar, nullableDict, firstList4Node);

            List<FIRST> firstList = new List<FIRST>();
            firstList.AddRange(firstList4Node);
            firstList.AddRange(firstList4Regulation);
            return new FirstListAndFollowList(firstList, followList);
        }

        public static List<FOLLOW> GetFollowList(this RegulationList grammar, Dictionary<TreeNodeType, bool> nullableDict = null)
        {
            if (nullableDict == null)
            { nullableDict = new Dictionary<TreeNodeType, bool>(); }
            GetNullableDict(grammar, nullableDict);
            List<FIRST> firstList4Node = GetFirstList4Node(grammar, nullableDict);
            List<FIRST> firstList4Regulation = GetFirstList4Regulation(grammar, nullableDict, firstList4Node);
            List<FOLLOW> followList = GetFollowList(grammar, nullableDict, firstList4Node);

            return followList;
        }

        public static List<FIRST> GetFirstList(this RegulationList grammar, Dictionary<TreeNodeType, bool> nullableDict = null)
        {
            if (nullableDict == null)
            { nullableDict = new Dictionary<TreeNodeType, bool>(); }
            GetNullableDict(grammar, nullableDict);
            List<FIRST> firstList4Node = GetFirstList4Node(grammar, nullableDict);
            List<FIRST> firstList4Regulation = GetFirstList4Regulation(grammar, nullableDict, firstList4Node);

            var list = new List<FIRST>();
            list.AddRange(firstList4Node);
            list.AddRange(firstList4Regulation);

            return list;
        }

        /// <summary>
        /// 计算文法的FOLLOW集
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="nullableDict"></param>
        /// <param name="firstList4Node"></param>
        /// <returns></returns>
        private static List<FOLLOW> GetFollowList(RegulationList grammar, Dictionary<TreeNodeType, bool> nullableDict, List<FIRST> firstList4Node)
        {
            // 初始化Follow list
            List<FOLLOW> result = new List<FOLLOW>();
            foreach (var item in grammar.GetAllTreeNodeNonLeaveTypes())
            {
                result.Add(new FOLLOW(item));
            }

            // 迭代到不动点
            bool changed = false;
            do
            {
                changed = false;
                foreach (var regulation in grammar)
                {
                    int count = regulation.RightPart.Count();
                    for (int index = 0; index < count; index++)
                    {
                        // 准备为target添加follow元素
                        TreeNodeType target = regulation.RightNode(index);
                        if (target.IsLeave) { continue; } // 叶结点没有follow
                        FOLLOW follow = FindFollow(result, target); // 找到follow对象
                        for (int checkCount = 0; checkCount < count - (index + 1); checkCount++)
                        {
                            if (Nullable(regulation.RightPart, index + 1, checkCount, nullableDict))
                            {
                                // nullable之后的FIRST是target的follow的一部分
                                FIRST first = FindFirst(
                                    firstList4Node, regulation.RightNode(index + 1 + checkCount));
                                foreach (var value in first.Values)
                                {
                                    if (value != TreeNodeType.NullNode)
                                    {
                                        changed = follow.TryInsert(value) || changed;
                                    }
                                }
                            }
                        }
                        {
                            // 如果target之后的全部结点都是nullable，说明此regulation.Left的folow也是target的一部分。
                            if (Nullable(regulation.RightPart, index + 1, count - (index + 1), nullableDict))
                            {
                                // 找到此regulation.Left的folow
                                FOLLOW refFollow = FindFollow(result, regulation.Left);
                                foreach (var item in refFollow.Values)
                                {
                                    changed = follow.TryInsert(item) || changed;
                                }
                            }
                        }
                    }
                }
            } while (changed);

            return result;
        }

        private static FOLLOW FindFollow(List<FOLLOW> followList, TreeNodeType target)
        {
            foreach (var item in followList)
            {
                if (item.Target == target)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 计算文法的各个RightPart的FIRST集
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="nullableDict"></param>
        /// <param name="firstList4Node"></param>
        /// <returns></returns>
        private static List<FIRST> GetFirstList4Regulation(RegulationList grammar, Dictionary<TreeNodeType, bool> nullableDict, List<FIRST> firstList4Node)
        {
            // 初始化FIRST集
            List<FIRST> result = new List<FIRST>();
            foreach (var regulation in grammar)
            {
                result.Add(new FIRST(regulation.RightPart));
            }

            bool changed = false;
            do
            {
                changed = false;
                foreach (var first in result)
                {
                    int count = first.Target.Count();
                    for (int i = 0; i < count; i++)
                    {
                        // 如果前i个结点都可为null，就说明下一个结点（first.Target[i]）的FIRST是此RightPart的FIRST的一部分
                        if (Nullable(first.Target, 0, i, nullableDict))
                        {
                            // 找到下一个结点的FIRST
                            FIRST refFirst = FindFirst(firstList4Node, first.GetNode(i));
                            foreach (var value in refFirst.Values)
                            {
                                if ((value != TreeNodeType.NullNode)
                                    && (!first.Values.Contains(value)))
                                {
                                    first.TryBinaryInsert(value);
                                    changed = true;
                                }
                            }
                        }
                    }
                    {
                        // 如果RightPart的全部结点都可为null，就说明此RightPart的FIRST包含"null"结点。
                        if (Nullable(first.Target, 0, first.Target.Count(), nullableDict))
                        {
                            if (!first.Values.Contains(TreeNodeType.NullNode))
                            {
                                first.TryBinaryInsert(TreeNodeType.NullNode);
                                changed = true;
                            }
                        }
                    }
                }
            } while (changed);

            return result;
        }

        /// <summary>
        /// 计算文法的所有单个的结点的FIRST
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="nullableDict"></param>
        /// <returns></returns>
        private static List<FIRST> GetFirstList4Node(RegulationList grammar, Dictionary<TreeNodeType, bool> nullableDict)
        {
            // 初始化FIRST
            List<FIRST> result = new List<FIRST>();
            // 初始化非叶结点的FIRST
            foreach (var item in grammar.GetAllTreeNodeNonLeaveTypes())
            {
                if (nullableDict[item])
                {
                    result.Add(new FIRST(item, TreeNodeType.NullNode));
                }
                else
                {
                    result.Add(new FIRST(item));
                }
            }
            // 初始化叶结点的FIRST（叶结点的FIRST实际上已经完工）
            foreach (var item in grammar.GetAllTreeNodeLeaveTypes())
            {
                result.Add(new FIRST(item, item));
            }

            bool changed = false;
            do
            {
                changed = false;
                foreach (var regulation in grammar)
                {
                    FIRST first = FindFirst(result, regulation.Left);
                    int rightPartCount = regulation.RightPart.Count();
                    for (int checkCount = 0; checkCount < rightPartCount; checkCount++)
                    {
                        // 如果前面checkCount个结点都可为null，
                        // 就说明RightPart[checkCount]的FIRST是此regulation.Left的FIRST的一部分。
                        if (Nullable(regulation.RightPart, 0, checkCount, nullableDict))
                        {
                            FIRST refFirst = FindFirst(result, regulation.RightNode(checkCount));
                            if (refFirst == null) { throw new Exception("algorithm error!"); }
                            foreach (var value in refFirst.Values)
                            {
                                if ((value != TreeNodeType.NullNode)
                                    && (!first.Values.Contains(value)))
                                {
                                    first.TryBinaryInsert(value);
                                    changed = true;
                                }
                            }
                        }
                    }
                }
            } while (changed);

            return result;
        }

        private static FIRST FindFirst(List<FIRST> firstList, TreeNodeType target)
        {
            foreach (var item in firstList)
            {
                if (item.Target.Count() == 1
                    && item.Target.First() == target)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 计算所有可能推导出null的结点。
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        private static Dictionary<TreeNodeType, bool> GetNullableDict(this RegulationList grammar, Dictionary<TreeNodeType, bool> nullableDict)
        {
            // 初始化nullable
            List<TreeNodeType> allNodeTypes = grammar.GetAllTreeNodeTypes();
            foreach (var item in allNodeTypes)
            {
                nullableDict.Add(item, false);
            }

            // 迭代到不动点
            bool changed = false;
            do
            {
                changed = false;
                foreach (var regulation in grammar)
                {
                    // 如果RightPart的结点全部可为null，就说明此regulation.Left是可推导出"null"的。
                    if (Nullable(regulation.RightPart, 0, regulation.RightPart.Count(), nullableDict))
                    {
                        if (!nullableDict[regulation.Left])
                        {
                            nullableDict[regulation.Left] = true;
                            changed = true;
                        }
                    }
                }
            } while (changed);

            return nullableDict;
        }

        /// <summary>
        /// list中指定的某一段结点是否都能产生null？
        /// </summary>
        /// <param name="list"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="nullableDict"></param>
        /// <returns></returns>
        static bool Nullable(IEnumerable<TreeNodeType> list, int startIndex, int count, Dictionary<TreeNodeType, bool> nullableDict)
        {
            bool result = true;
            int i = 0;
            foreach (var node in list)
            {
                if (i < startIndex) { continue; }

                if (i - startIndex >= count) { break; }

                if (!nullableDict[node])
                {
                    result = false;
                    break;
                }

                i++;
            }

            return result;
        }
    }

    public class FirstListAndFollowList
    {
        public FirstListAndFollowList(List<FIRST> firstList, List<FOLLOW> followList)
        {
            this.FirstList = firstList;
            this.FollowList = followList;
        }

        public List<FIRST> FirstList { get; set; }

        public List<FOLLOW> FollowList { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("FIRST list:");
            foreach (var item in this.FirstList)
            {
                builder.AppendLine(item.ToString());
            }
            builder.AppendLine("FOLLOW list:");
            foreach (var item in this.FollowList)
            {
                builder.AppendLine(item.ToString());
            }

            return builder.ToString();
        }

        public void Dump(string fullname)
        {
            using (StreamWriter sw = new StreamWriter(fullname, false))
            {
                sw.WriteLine("FIRST list:");
                foreach (var item in this.FirstList)
                {
                    sw.WriteLine(item.ToString());
                }
                sw.WriteLine("FOLLOW list:");
                foreach (var item in this.FollowList)
                {
                    sw.WriteLine(item.ToString());
                }
            }
        }
    }

    

    
}
