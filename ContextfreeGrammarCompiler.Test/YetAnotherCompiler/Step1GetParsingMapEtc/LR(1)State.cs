﻿using System;
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

        /// <summary>
        /// 返回regulations+DotPosition（即LR0Item）都相同的一组LR1Item（即LALR1Item）。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<LR0Item, OrderedCollection<TreeNodeType>>> GetGroups()
        {
            int count = this.regulationDotList.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new Tuple<LR0Item, OrderedCollection<TreeNodeType>>(
                    this.regulationDotList[i], this.lookAheadCollectionList[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lr0Item">group标识</param>
        /// <param name="lookAheadNodes">此group下包含的lookAhead结点列表</param>
        /// <returns></returns>
        public bool TryInsert(LR0Item lr0Item, IEnumerable<TreeNodeType> lookAheadNodes)
        {
            if (this.regulationDotList.TryInsert(lr0Item))
            {
                int index = this.regulationDotList.IndexOf(lr0Item);
                OrderedCollection<TreeNodeType> list = new OrderedCollection<TreeNodeType>(" ");
                foreach (var node in lookAheadNodes)
                {
                    list.TryInsert(node);
                }
                this.lookAheadCollectionList.Insert(index, list);

                this.SetDirty();
                return true;
            }
            else
            {
                int index = this.regulationDotList.IndexOf(lr0Item);
                bool result = false;
                OrderedCollection<TreeNodeType> list = this.lookAheadCollectionList[index];
                foreach (var item in lookAheadNodes)
                {
                    result = list.TryInsert(item) || result;
                }

                //this.SetDirty();// 无论是否插入新的lookAheadNode，都不影响state的hashcode
                return result;
            }
        }

        // 分析表对第一个State是有要求的。必须是<S'> ::= . <S> "$" ;所在的state。
        /// <summary>
        /// 由外部（<see cref="LR1StateCollection"/>）指定的索引。
        /// 分析表对第一个State是有要求的。必须是&lt;S'&gt; ::= . &lt;S&gt; "$" ;所在的state。
        /// </summary>
        public int ParsingMapIndex { get; set; }

        private OrderedCollection<LR0Item> regulationDotList = new OrderedCollection<LR0Item>("this should not occur");
        private List<OrderedCollection<TreeNodeType>> lookAheadCollectionList = new List<OrderedCollection<TreeNodeType>>();

        /// <summary>
        /// 这是一个只能添加元素的集合，其元素是有序的，是按二分法插入的。
        /// 但是使用者不能控制元素的顺序。
        /// </summary>
        /// <param name="separator">在Dump到流时用什么分隔符分隔各个元素？</param>
        public SmallerLR1State(params LR1Item[] items)
        {
            foreach (var item in items)
            {
                this.TryInsert(item);
            }
        }

        public bool TryInsert(LR1Item item)
        {
            LR0Item lr0Item = new LR0Item(item.Regulation, item.DotPosition);
            if (this.regulationDotList.TryInsert(lr0Item))
            {
                int index = this.regulationDotList.IndexOf(lr0Item);
                var collection = new OrderedCollection<TreeNodeType>(", ");
                collection.TryInsert(item.LookAheadNodeType);
                this.lookAheadCollectionList.Insert(index, collection);

                this.SetDirty();
                return true;
            }
            else
            {
                int index = this.regulationDotList.IndexOf(lr0Item);
                if (this.lookAheadCollectionList[index].TryInsert(item.LookAheadNodeType))
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
            LR0Item lr0Item = new LR0Item(item.Regulation, item.DotPosition);
            int groupIndex = this.regulationDotList.IndexOf(lr0Item);
            if (0 <= groupIndex && groupIndex < this.regulationDotList.Count)
            {
                for (int i = 0; i < groupIndex; i++)
                {
                    index += this.lookAheadCollectionList[i].Count;
                }
                index += this.lookAheadCollectionList[groupIndex].IndexOf(item.LookAheadNodeType);
            }
            else
            { index = -1; }

            return index;
        }

        public bool Contains(LR1Item item)
        {
            LR0Item lr0Item = new LR0Item(item.Regulation, item.DotPosition);
            int groupIndex = this.regulationDotList.IndexOf(lr0Item);
            if (0 <= groupIndex && groupIndex < this.regulationDotList.Count)
            {
                int index = this.lookAheadCollectionList[groupIndex].IndexOf(item.LookAheadNodeType);
                if (0 <= index && index < this.lookAheadCollectionList[groupIndex].Count)
                { return true; }
                else
                { return false; }
            }
            else
            { return false; }
        }

        public LR1Item this[int index]
        {
            get
            {
                if (index < 0) { throw new ArgumentOutOfRangeException(); }

                int current = 0;
                for (int i = 0; i < this.regulationDotList.Count; i++)
                {
                    if (index <= current + this.regulationDotList.Count)
                    {
                        TreeNodeType node = this.lookAheadCollectionList[i][index - current];
                        LR0Item item = this.regulationDotList[i];
                        return new LR1Item(item.Regulation, item.DotPosition, node);
                    }
                    else
                    { current += this.regulationDotList.Count; }
                }

                throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerator<LR1Item> GetEnumerator()
        {
            for (int i = 0; i < this.regulationDotList.Count; i++)
            {
                LR0Item item = this.regulationDotList[i];
                foreach (var lookAheadNode in this.lookAheadCollectionList[i])
                {
                    yield return new LR1Item(item.Regulation, item.DotPosition, lookAheadNode);
                }
            }
        }

        public int GroupCount { get { return this.regulationDotList.Count; } }

        public int Count { get { return this.lookAheadCollectionList.Sum(x => x.Count); } }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            for (int i = 0; i < this.regulationDotList.Count; i++)
            {
                this.regulationDotList[i].Dump(stream);
                stream.Write(", ");
                foreach (var lookAheadNode in this.lookAheadCollectionList[i])
                {
                    lookAheadNode.Dump(stream);
                }
                if (i + 1 < this.regulationDotList.Count)
                {
                    stream.WriteLine();
                }
            }
        }
    }
}
