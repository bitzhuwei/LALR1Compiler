using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 经过优化的LR(0)Edge列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
    /// </summary>
    public class LR0EdgeCollection : OrderedCollection<LR0Edge>
    {

        /// <summary>
        /// 经过优化的LR(0)Edge列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
        /// </summary>
        /// <param name="stateCollection"></param>
        /// <param name="edges"></param>
        public LR0EdgeCollection(LR0StateCollection stateCollection, params LR0Edge[] edges)
            : base(Environment.NewLine)
        {
            this.StateCollection = stateCollection;
            if (edges != null)
            {
                foreach (var item in edges)
                {
                    this.TryInsert(item);
                }
            }
        }

        public LR0StateCollection StateCollection { get; private set; }

        public override void Dump(System.IO.TextWriter stream)
        {
            for (int i = 0; i < this.Count; i++)
            {
                stream.WriteLine("Edge [{0}]:", i + 1);
                int fromId = this.StateCollection.IndexOf(this[i].From) + 1;
                int toId = this.StateCollection.IndexOf(this[i].To) + 1;
                stream.WriteLine("State[{0}] ==[{1}]==> State[{2}]", fromId, this[i].X.Nickname, toId);
                this[i].Dump(stream);
                if (i + 1 < this.Count)
                {
                    stream.WriteLine();
                }
            }
        }
    }
}
