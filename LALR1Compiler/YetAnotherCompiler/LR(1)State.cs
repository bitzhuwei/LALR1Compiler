using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LR(1)状态
    /// 经过优化的LR(1)Item列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
    /// </summary>
    public class LR1State : OrderedCollection<LR1Item>
    {
        // 分析表对第一个State是有要求的。必须是<S'> ::= . <S> "$" ;所在的state。
        /// <summary>
        /// 由外部（<see cref="LR1StateCollection"/>）指定的索引。
        /// 分析表对第一个State是有要求的。必须是&lt;S'&gt; ::= . &lt;S&gt; "$" ;所在的state。
        /// </summary>
        public int ParsingMapIndex { get; set; }

        /// <summary>
        /// LR(1)状态
        /// 经过优化的LR(1)Item列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
        /// </summary>
        /// <param name="items"></param>
        public LR1State(params LR1Item[] items)
            : base(Environment.NewLine)
        {
            ParsingMapIndex = -1;// not ready

            if (items != null)
            {
                foreach (var item in items)
                {
                    this.TryInsert(item);
                }
            }
        }

    }

    /// <summary>
    /// 占用内存更少的LR1State
    /// </summary>
    public class SmallerLR1State : HashCache, IEnumerable<LR1Item>
    {
        class LR1ItemGroup : HashCache
        {
            public LR0Item Item { get; private set; }
            public OrderedCollection<TreeNodeType> LookAheadNodeList { get; private set; }

            public LR1ItemGroup(Regulation regulation, int dotPosition, params TreeNodeType[] lookAheadNodes)
                : base(GetUniqueString)
            {
                this.Item = new LR0Item(regulation, dotPosition);
                this.LookAheadNodeList = new OrderedCollection<TreeNodeType>(", ");
                foreach (var node in lookAheadNodes)
                {
                    this.LookAheadNodeList.TryInsert(node);
                }
            }
            private static string GetUniqueString(HashCache cache)
            {
                LR1ItemGroup obj = cache as LR1ItemGroup;
                return obj.Dump();
            }

            public override void Dump(System.IO.TextWriter stream)
            {
                this.Item.Dump(stream);
                stream.Write(", ");
                this.LookAheadNodeList.Dump(stream);
            }

            public override int CompareTo(HashCache other)
            {
                LR1ItemGroup obj = other as LR1ItemGroup;
                if ((Object)obj == null) { return -1; }
                return this.Item.CompareTo(obj.Item);
            }
        }
        private OrderedCollection<LR1ItemGroup> list = new OrderedCollection<LR1ItemGroup>(Environment.NewLine);

        /// <summary>
        /// 这是一个只能添加元素的集合，其元素是有序的，是按二分法插入的。
        /// 但是使用者不能控制元素的顺序。
        /// </summary>
        /// <param name="separator">在Dump到流时用什么分隔符分隔各个元素？</param>
        public SmallerLR1State()
            : base(GetUniqueString)
        {
        }

        public override int CompareTo(HashCache other)
        {
            return base.CompareTo(other);
        }

        private static string GetUniqueString(HashCache cache)
        {
            SmallerLR1State obj = cache as SmallerLR1State;
            return obj.Dump();
            //StringBuilder builder = new StringBuilder();
            //for (int i = 0; i < obj.list.Count; i++)
            //{
            //    builder.Append(obj.list[i]);
            //    if (i + 1 < obj.list.Count)
            //    {
            //        builder.Append(obj.seperator);
            //    }
            //}

            //return builder.ToString();
        }

        public bool TryInsert(LR1Item item)
        {
            var fakeGroup = new LR1ItemGroup(item.Regulation, item.DotPosition);
            if (this.list.TryInsert(fakeGroup))
            {
                this.SetDirty();
                return true;
            }
            else
            {
                int index = this.list.IndexOf(fakeGroup);
                LR1ItemGroup group = this.list[index];
                if (group.LookAheadNodeList.TryInsert(item.LookAheadNodeType))
                {
                    this.SetDirty();
                    return true;
                }
                else
                { return false; }
            }
        }

        public int IndexOf(LR1Item item)
        {
            int index = 0;
            foreach (var group in this.list)
            {
                if (group.Item.Regulation == item.Regulation
                    && group.Item.DotPosition == item.DotPosition)
                {
                    foreach (var node in group.LookAheadNodeList)
                    {
                        if (node == item.LookAheadNodeType)
                        {
                            return index;
                        }

                        index++;
                    }
                }

                index += group.LookAheadNodeList.Count;
            }

            return -1;
        }

        public bool Contains(LR1Item item)
        {
            var fakeGroup = new LR1ItemGroup(item.Regulation, item.DotPosition);
            int index = this.list.IndexOf(fakeGroup);
            return (0 <= index && index < this.list.Count);
        }

        public LR1Item this[int index]
        {
            get
            {
                if (index < 0) { throw new ArgumentOutOfRangeException(); }

                int i = 0;
                foreach (var group in this.list)
                {
                    foreach (var node in group.LookAheadNodeList)
                    {
                        if (i == index)
                        {
                            return new LR1Item(
                                group.Item.Regulation, group.Item.DotPosition, node);
                        }
                        else
                        { i++; }
                    }
                }

                throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerator<LR1Item> GetEnumerator()
        {
            foreach (var group in this.list)
            {
                foreach (var node in group.LookAheadNodeList)
                {
                    yield return new LR1Item(
                        group.Item.Regulation, group.Item.DotPosition, node);
                }
            }
        }

        public int GroupCount { get { return this.list.Count; } }

        public int Count { get { return this.list.Sum(x => x.LookAheadNodeList.Count); } }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                this.list[i].Dump(stream);
                if (i + 1 < this.list.Count)
                {
                    stream.Write(Environment.NewLine);
                }
            }
        }
    }
}
