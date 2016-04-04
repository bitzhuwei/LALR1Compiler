using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LR(0)状态的列表
    /// 经过优化的LR(0)State列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
    /// </summary>
    public class LR0StateCollection : OrderedCollection<LR0State>
    {
        private int nextStateIndex = 0;

        public override bool TryInsert(LR0State item)
        {
            if (base.TryInsert(item))
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
        /// LR(0)状态的列表
        /// 经过优化的LR(0)State列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
        /// </summary>
        /// <param name="states"></param>
        public LR0StateCollection(params LR0State[] states)
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
