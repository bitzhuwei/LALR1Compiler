using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class LR0EdgeList : OrderedCollection<LR0Edge>
    {

        public LR0EdgeList(params LR0Edge[] edges)
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

    }
}
