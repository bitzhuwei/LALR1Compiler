using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class LR1EdgeList : OrderedCollection<LR1Edge>
    {

        public LR1EdgeList(params LR1Edge[] edges)
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
