using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class LR0StateList : OrderedCollection<LR0State>
    {

        public LR0StateList(params LR0State[] states)
            : base(Environment.NewLine)
        {
            if (states != null)
            {
                foreach (var item in states)
                {
                    this.TryBinaryInsert(item);
                }
            }
        }

    }
}
