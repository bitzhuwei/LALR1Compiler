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
            : base(GetUniqueString)
        {
            this.From = from;
            this.X = x;
            this.To = to;
        }

        private static string GetUniqueString(HashCache cache)
        {
            LR0Edge obj = cache as LR0Edge;
            StringBuilder builder = new StringBuilder();

            builder.Append(obj.From);
            builder.AppendLine();
            builder.Append("    └──("); builder.Append(obj.X); builder.Append(")──┒");
            builder.AppendLine();
            builder.Append(obj.To);

            return builder.ToString();
        }

        public LR0State From { get; set; }

        public TreeNodeType X { get; set; }

        public LR0State To { get; set; }


        public override void Dump(System.IO.StreamWriter stream)
        {
            this.From.Dump(stream);
            stream.WriteLine();
            stream.Write("    └──("); this.X.Dump(stream);stream.Write(")──┒");
            stream.WriteLine();
            this.To.Dump(stream);
        }
    }
}
