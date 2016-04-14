using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 经过优化的FOLLOW列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
    /// </summary>
    public class FOLLOWCollection : OrderedCollection<FOLLOW>
    {
        public FOLLOWCollection(params FOLLOW[] items)
            : base(Environment.NewLine)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.TryInsert(item);
                }
            }
        }
    }
}
