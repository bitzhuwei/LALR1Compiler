using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class LR1Edge : HashCache
    {

        public LR1Edge(LR1State from, TreeNodeType x, LR1State to)
            : base(GetUniqueString)
        {
            this.From = from;
            this.X = x;
            this.To = to;
        }

        private static string GetUniqueString(HashCache cache)
        {
            LR1Edge obj = cache as LR1Edge;
            StringBuilder builder = new StringBuilder();

            builder.Append(obj.From);
            builder.AppendLine();
            builder.Append("    └──("); builder.Append(obj.X); builder.Append(")──┒");
            builder.AppendLine();
            builder.Append(obj.To);

            return builder.ToString();
        }

        public LR1State From { get; set; }

        public TreeNodeType X { get; set; }

        public LR1State To { get; set; }


        public override void Dump(System.IO.StreamWriter stream)
        {
            this.From.Dump(stream);
            stream.WriteLine();
            stream.Write("    └──("); this.X.Dump(stream); stream.Write(")──┒");
            stream.WriteLine();
            this.To.Dump(stream);
        }
    }
}
