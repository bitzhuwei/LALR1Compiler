using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class LR0Edge : HashCache
    {

        public LR0Edge(LR0State from, TreeNodeType x, LR0State to)
            : base(GetToString)
        {
            this.From = from;
            this.x = x;
            this.To = to;
        }

        private static string GetToString(HashCache cache)
        {
            LR0Edge obj = cache as LR0Edge;
            StringBuilder builder = new StringBuilder();

            builder.Append(obj.From);
            builder.AppendLine();
            builder.Append("    └──("); builder.Append(obj.x); builder.Append(")──┒");
            builder.AppendLine();
            builder.Append(obj.To);

            return builder.ToString();
        }

        public LR0State From { get; set; }

        public TreeNodeType x { get; set; }

        public LR0State To { get; set; }


        public override void Dump(System.IO.StreamWriter stream)
        {
            this.From.Dump(stream);
            stream.WriteLine();
            stream.Write("    └──("); this.x.Dump(stream);stream.Write(")──┒");
            stream.WriteLine();
            this.To.Dump(stream);
        }
    }
}
