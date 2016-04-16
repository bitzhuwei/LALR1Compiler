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

        /// <summary>
        /// LR(0)项：Regulation + 圆点的位置
        /// </summary>
        /// <param name="regulation"></param>
        /// <param name="dotPosition"></param>
        public LR0Item(Regulation regulation, int dotPosition)
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

        public override void Dump(System.IO.TextWriter stream)
        {
            this.Regulation.Left.Dump(stream);
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
