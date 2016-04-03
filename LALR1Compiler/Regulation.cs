using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    // 一个<L> ::= <L> "," <E> ;就是一个规则
    /// <summary>
    /// 规则。从左部产生某一种右部的规则。
    /// </summary>
    public class Regulation : HashCache
    {
        public Regulation(TreeNodeType left, params TreeNodeType[] rights)
            : base(getUniqueString)
        {
            if (left == null)
            { throw new ArgumentNullException(); }

            this.Left = left;
            this.rightPart.AddRange(rights);
        }

        private static string getUniqueString(HashCache cache)
        {
            Regulation obj = cache as Regulation;
            return obj.Dump();
            //StringBuilder builder = new StringBuilder();
            //builder.Append(obj.Left.Nickname);
            //builder.Append(" ::= ");
            //builder.Append(obj.RightPart);
            //builder.Append(" ;");

            //return builder.ToString();
        }

        public TreeNodeType Left { get; private set; }

        TreeNodeTypeList rightPart = new TreeNodeTypeList();

        //public IEnumerable<TreeNodeType> RightPart { get { return this.rightPart; } }
        public IReadOnlyList<TreeNodeType> RightPart { get { return this.rightPart; } }

        public TreeNodeType RightNode(int index)
        {
            return rightPart[index];
        }


        public override void Dump(System.IO.TextWriter stream)
        {
            this.Left.Dump(stream);
            stream.Write(" ::= ");
            this.rightPart.Dump(stream);
            stream.Write(" ;");
        }
    }
}
