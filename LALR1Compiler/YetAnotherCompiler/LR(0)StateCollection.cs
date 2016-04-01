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

    }
}
