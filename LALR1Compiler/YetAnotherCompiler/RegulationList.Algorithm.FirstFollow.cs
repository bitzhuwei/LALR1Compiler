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
            GetNullableDict(grammar, out nullableDict);
            FIRSTCollection firstCollection4Node;
            grammar.GetFirstCollection4Node(out firstCollection4Node, nullableDict);
            FIRSTCollection firstCollection4Regulation;
            grammar.GetFirstCollection4Regulation(out firstCollection4Regulation, nullableDict, firstCollection4Node);
            FOLLOWCollection followCollection;
            grammar.GetFollowCollection(out followCollection, nullableDict, firstCollection4Node);

            var firstCollection = new FIRSTCollection();
            foreach (var item in firstCollection4Node)
            {
                firstCollection.TryInsert(item);
            }
            foreach (var item in firstCollection4Regulation)
            {
                firstCollection.TryInsert(item);
            }
            return new FirstListAndFollowList(firstCollection, followCollection);
        }

        public static void GetFollowCollection(
            this RegulationList grammar,
            out FOLLOWCollection followCollection,
            Dictionary<TreeNodeType, bool> nullableDict = null,
            FIRSTCollection firstCollection = null)
        {
            if (nullableDict == null)
            { grammar.GetNullableDict(out nullableDict); }
            if (firstCollection == null)
            { grammar.GetFirstCollection(out firstCollection, nullableDict); }

            FIRSTCollection firstList4Node;
            grammar.GetFirstCollection4Node(out firstList4Node, nullableDict);
            FIRSTCollection firstList4Regulation;
            grammar.GetFirstCollection4Regulation(out firstList4Regulation, nullableDict, firstList4Node);
            grammar.DoGetFollowList(out followCollection, nullableDict, firstList4Node);
        }

        public static void GetFirstCollection(
            this RegulationList grammar,
            out FIRSTCollection firstCollection, Dictionary<TreeNodeType, bool> nullableDict = null)
        {
            if (nullableDict == null)
            { GetNullableDict(grammar, out nullableDict); }

            FIRSTCollection firstList4Node;
            grammar.GetFirstCollection4Node(out firstList4Node, nullableDict);
            FIRSTCollection firstList4Regulation;
            grammar.GetFirstCollection4Regulation(out firstList4Regulation,
                nullableDict, firstList4Node);

            firstCollection = new FIRSTCollection();
            foreach (var item in firstList4Node)
            {
                firstCollection.TryInsert(item);
            }
            foreach (var item in firstList4Regulation)
            {
                firstCollection.TryInsert(item);
            }
        }

        /// <summary>
        /// 计算文法的FOLLOW集
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="nullableDict"></param>
        /// <param name="firstList4Node"></param>
        /// <returns></returns>
        private static void DoGetFollowList(
            this RegulationList grammar,
            out FOLLOWCollection followCollection,
            Dictionary<TreeNodeType, bool> nullableDict, FIRSTCollection firstList4Node)
        {
            // 初始化Follow list
            followCollection = new FOLLOWCollection();
            foreach (var item in grammar.GetAllTreeNodeNonLeaveTypes())
            {
                followCollection.TryInsert(new FOLLOW(item));
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
                        FOLLOW follow = FindFollow(followCollection, target); // 找到follow对象
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
                                FOLLOW refFollow = FindFollow(followCollection, regulation.Left);
                                if (refFollow != follow)
                                {
                                    foreach (var item in refFollow.Values)
                                    {
                                        changed = follow.TryInsert(item) || changed;
                                    }
                                }
                            }
                        }
                    }
                }
            } while (changed);
        }

        private static FOLLOW FindFollow(FOLLOWCollection followCollection, TreeNodeType target)
        {
            foreach (var item in followCollection)
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
        private static void GetFirstCollection4Regulation(
            this RegulationList grammar,
            out FIRSTCollection firstCollection,
            Dictionary<TreeNodeType, bool> nullableDict = null, FIRSTCollection firstList4Node = null)
        {
            // 初始化FIRST集
            firstCollection = new FIRSTCollection();
            foreach (var regulation in grammar)
            {
                firstCollection.TryInsert(new FIRST(regulation.RightPart));
            }

            bool changed = false;
            do
            {
                changed = false;
                foreach (var first in firstCollection)
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
                                if (value != TreeNodeType.NullNode)
                                {
                                    changed = first.TryInsert(value) || changed;
                                }
                            }
                        }
                    }
                    {
                        // 如果RightPart的全部结点都可为null，就说明此RightPart的FIRST包含"null"结点。
                        if (Nullable(first.Target, 0, first.Target.Count(), nullableDict))
                        {
                            changed = first.TryInsert(TreeNodeType.NullNode) || changed;
                        }
                    }
                }
            } while (changed);
        }

        /// <summary>
        /// 计算文法的所有单个的结点的FIRST
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="nullableDict"></param>
        /// <returns></returns>
        private static void GetFirstCollection4Node(
            this RegulationList grammar,
            out FIRSTCollection firstCollection,
            Dictionary<TreeNodeType, bool> nullableDict = null)
        {
            // 初始化FIRST
            firstCollection = new FIRSTCollection();
            // 初始化非叶结点的FIRST
            foreach (var item in grammar.GetAllTreeNodeNonLeaveTypes())
            {
                if (nullableDict[item])
                {
                    firstCollection.TryInsert(new FIRST(item, TreeNodeType.NullNode));
                }
                else
                {
                    firstCollection.TryInsert(new FIRST(item));
                }
            }
            // 初始化叶结点的FIRST（叶结点的FIRST实际上已经完工）
            foreach (var item in grammar.GetAllTreeNodeLeaveTypes())
            {
                firstCollection.TryInsert(new FIRST(item, item));
            }

            bool changed = false;
            do
            {
                changed = false;
                foreach (var regulation in grammar)
                {
                    FIRST first = FindFirst(firstCollection, regulation.Left);
                    int rightPartCount = regulation.RightPart.Count();
                    for (int checkCount = 0; checkCount < rightPartCount; checkCount++)
                    {
                        // 如果前面checkCount个结点都可为null，
                        // 就说明RightPart[checkCount]的FIRST是此regulation.Left的FIRST的一部分。
                        if (Nullable(regulation.RightPart, 0, checkCount, nullableDict))
                        {
                            FIRST refFirst = FindFirst(firstCollection, regulation.RightNode(checkCount));
                            if (refFirst == null) { throw new Exception("algorithm error!"); }
                            if (refFirst != first)
                            {
                                foreach (var value in refFirst.Values)
                                {
                                    if (value != TreeNodeType.NullNode)
                                    {
                                        changed = first.TryInsert(value) || changed;
                                    }
                                }
                            }
                        }
                    }
                }
            } while (changed);
        }

        private static FIRST FindFirst(FIRSTCollection firstCollection, TreeNodeType target)
        {
            foreach (var item in firstCollection)
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
        public static void GetNullableDict(
            this RegulationList grammar, out Dictionary<TreeNodeType, bool> nullableDict)
        {
            nullableDict = new Dictionary<TreeNodeType, bool>();

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
        }

        /// <summary>
        /// list中指定的某一段结点是否都能产生null？
        /// </summary>
        /// <param name="list"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="nullableDict"></param>
        /// <returns></returns>
        static bool Nullable(IReadOnlyList<TreeNodeType> list, int startIndex, int count, Dictionary<TreeNodeType, bool> nullableDict)
        {
            bool result = true;
            for (int i = 0; i < count; i++)
            {
                TreeNodeType node = list[i + startIndex];
                if (!nullableDict[node])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }

    public class FirstListAndFollowList
    {
        public FirstListAndFollowList(FIRSTCollection firstList, FOLLOWCollection followList)
        {
            this.FirstList = firstList;
            this.FollowList = followList;
        }

        public FIRSTCollection FirstList { get; set; }

        public FOLLOWCollection FollowList { get; set; }

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
