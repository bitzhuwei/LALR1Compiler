﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LR(1)状态的列表
    /// 经过优化的LR(1)State列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
    /// </summary>
    public class LALR1StateCollection : OrderedCollection<LALR1State>
    {
        private int nextStateIndex = 0;

        public override bool TryInsert(LALR1State item)
        {
            if (base.TryInsert(item))// 插入新state
            {
                item.ParsingMapIndex = nextStateIndex;
                nextStateIndex++;
                return true;
            }
            else
            {
                // 将item中的LookAheadNodeList并入现有state中
                int index = this.IndexOf(item);
                LALR1State state = this[index];
                bool dirty = false;
                foreach (var group in item.GetGroups())
                {
                    if(state.TryInsert(group.Item1, group.Item2))
                    { dirty = true; }
                }
                return dirty;
            }
        }
        /// <summary>
        /// LR(1)状态的列表
        /// 经过优化的LR(1)State列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
        /// </summary>
        /// <param name="states"></param>
        public LALR1StateCollection(params LALR1State[] states)
            : base(Environment.NewLine)
        {
            if (states != null)
            {
                foreach (var item in states)
                {
                    this.TryInsert(item);
                }
            }
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            for (int i = 0; i < this.Count; i++)
            {
                stream.WriteLine("State [{0}]:", this[i].ParsingMapIndex + 1);
                this[i].Dump(stream);
                if (i + 1 < this.Count)
                {
                    stream.WriteLine();
                }
            }
        }
    }
}
