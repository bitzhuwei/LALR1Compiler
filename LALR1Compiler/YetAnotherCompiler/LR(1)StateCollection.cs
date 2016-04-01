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
    }
}
