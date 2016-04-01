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
        /// <param name="edges"></param>
        public LR0EdgeCollection(params LR0Edge[] edges)
            :base(Environment.NewLine)
        {
            if (edges != null)
            {
                foreach (var item in edges)
                {
                    this.TryInsert(item);
                }
            }
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            for (int i = 0; i < this.Count; i++)
            {
                stream.WriteLine("Edge [{0}]:", i + 1);
                this[i].Dump(stream);
                if (i + 1 < this.Count)
                {
                    stream.WriteLine();
                }
            }
        }
    }
}
