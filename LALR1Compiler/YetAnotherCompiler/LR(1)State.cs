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

        /// <summary>
        /// LR(1)状态
        /// 经过优化的LR(1)Item列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
        /// </summary>
        /// <param name="items"></param>
        public LR1State(params LR1Item[] items)
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
