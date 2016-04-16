using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 从一个LR(0)状态跳转到另一个LR(0)状态
    /// </summary>
    public class LR0Edge : HashCache
    {

        /// <summary>
        /// 从一个LR(0)状态跳转到另一个LR(0)状态
        /// </summary>
        /// <param name="from"></param>
        /// <param name="x"></param>
        /// <param name="to"></param>
        public LR0Edge(LR0State from, TreeNodeType x, LR0State to)
        {
            this.From = from;
            this.X = x;
            this.To = to;
        }

        public LR0State From { get; set; }

        public TreeNodeType X { get; set; }

        public LR0State To { get; set; }


        public override void Dump(System.IO.TextWriter stream)
        {
            this.From.Dump(stream);
            stream.WriteLine();
            stream.Write("    └──("); this.X.Dump(stream);stream.Write(")──┒");
            stream.WriteLine();
            this.To.Dump(stream);
        }
    }
}
