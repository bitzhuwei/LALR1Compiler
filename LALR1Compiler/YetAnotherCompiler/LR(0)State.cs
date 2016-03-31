using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LR(0)状态
    /// </summary>
    public class LR0State : OrderedCollection<LR0Item>
    {

        public LR0State(params LR0Item[] items)
            : base(Environment.NewLine)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.TryBinaryInsert(item);
                }
            }
        }

    }
}
