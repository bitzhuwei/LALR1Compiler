using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LR(0)项：Regulation + 圆点的位置
    /// </summary>
    public class LR0Item : HashCache
    {

        /// <summary>
        /// 规则。
        /// </summary>
        public Regulation Regulation { get; private set; }

        /// <summary>
        /// 圆点的位置（0 - Regulation.Right.Count）
        /// </summary>
        public int DotPosition { get; private set; }

        public LR0Item(Regulation regulation, int dotPosition)
            : base(GetUniqueString)
        {
            if (regulation == null) { throw new ArgumentNullException(); }
            if (dotPosition < 0 || regulation.RightPart.Count() < dotPosition)
            { throw new ArgumentOutOfRangeException(); }

            this.Regulation = regulation;
            this.DotPosition = dotPosition;
        }

        /// <summary>
        /// 获取圆点后面的第一个结点。
        /// </summary>
        /// <returns></returns>
        public TreeNodeType GetNodeNext2Dot()
        {
            if (this.DotPosition < this.Regulation.RightPart.Count())
            {
                return this.Regulation.RightNode(DotPosition);
            }
            else
            {
                return null;
            }
        }

        private static string GetUniqueString(HashCache cache)
        {
            LR0Item obj = cache as LR0Item;
            StringBuilder builder = new StringBuilder();
            builder.Append(obj.Regulation.Left.Nickname);
            builder.Append(" ::= ");

            int count = obj.Regulation.RightPart.Count();
            for (int i = 0; i < count; i++)
            {
                if (i == obj.DotPosition)
                { builder.Append('.'); builder.Append(' '); }

                var item = obj.Regulation.RightNode(i);
                builder.Append(item.Nickname);
                builder.Append(' ');
            }

            if (obj.DotPosition == count)
            { builder.Append('.'); builder.Append(' '); }

            builder.Append(';');

            return builder.ToString();
        }


        public override void Dump(System.IO.StreamWriter stream)
        {
            stream.Write(this.Regulation.Left.Nickname);
            stream.Write(" ::= ");

            int count = this.Regulation.RightPart.Count();
            for (int i = 0; i < count; i++)
            {
                if (i == this.DotPosition)
                { stream.Write('.'); stream.Write(' '); }

                var item = this.Regulation.RightNode(i);
                stream.Write(item.Nickname);
                stream.Write(' ');
            }

            if (this.DotPosition == count)
            { stream.Write('.'); stream.Write(' '); }

            stream.Write(';');
        }
    }
}
