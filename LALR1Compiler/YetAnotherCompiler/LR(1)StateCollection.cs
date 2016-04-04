using System;
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
    public class LR1StateCollection : OrderedCollection<LR1State>
    {
        private int nextStateIndex = 0;

        public override bool TryInsert(LR1State item)
        {
            if(base.TryInsert(item))
            {
                item.ParsingMapIndex = nextStateIndex;
                nextStateIndex++;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// LR(1)状态的列表
        /// 经过优化的LR(1)State列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
        /// </summary>
        /// <param name="states"></param>
        public LR1StateCollection(params LR1State[] states)
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
                this[i].Dump(stream);
                if (i + 1 < this.Count)
                {
                    stream.WriteLine();
                }
            }
        }
    }
}
