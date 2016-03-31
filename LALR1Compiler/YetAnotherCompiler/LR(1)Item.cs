using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LR(1)项（A->α.β,x）指出，序列α在栈顶，且输入中开头的是可以从βx导出的符号。
    /// </summary>
    public class LR1Item : HashCache
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
        /// 超前查看的符号
        /// </summary>
        public TreeNodeType LookAheadNodeType { get; private set; }

        /// <summary>
        /// LR(1)项（A->α.β,x）指出，序列α在栈顶，且输入中开头的是可以从βx导出的符号。
        /// </summary>
        /// <param name="regulation">A->αβ/param>
        /// <param name="dotPosition">圆点的位置</param>
        /// <param name="lookAheadNodeType">x代表的类型</param>
        public LR1Item(Regulation regulation, int dotPosition, TreeNodeType lookAheadNodeType)
            : base(GetUniqueString)
        {
            if (regulation == null || lookAheadNodeType == null) { throw new ArgumentNullException(); }
            if (dotPosition < 0 || regulation.RightPart.Count() < dotPosition)
            { throw new ArgumentOutOfRangeException(); }

            this.Regulation = regulation;
            this.DotPosition = dotPosition;
            this.LookAheadNodeType = lookAheadNodeType;
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

        public List<TreeNodeType> GetBetaZ()
        {
            List<TreeNodeType> result = new List<TreeNodeType>();
            int count = this.Regulation.RightPart.Count();
            for (int i = this.DotPosition + 1; i < count; i++)
            {
                result.Add(this.Regulation.RightNode(i));
            }

            result.Add(this.LookAheadNodeType);

            return result;
        }

        private static string GetUniqueString(HashCache cache)
        {
            LR1Item obj = cache as LR1Item;
            StringBuilder builder = new StringBuilder();
            builder.Append(obj.Regulation.Left.Nickname);
            builder.Append(" ::= ");

            int count = obj.Regulation.RightPart.Count();
            for (int i = 0; i < count; i++)
            {
                if (i == obj.DotPosition)
                { builder.Append('.'); builder.Append(' '); }

                TreeNodeType item = obj.Regulation.RightNode(i);
                //TODO:这是我定义的规则
                builder.Append(item.Nickname);
                
                builder.Append(' ');
            }

            if (obj.DotPosition == count)
            { builder.Append('.'); builder.Append(' '); }

            builder.Append(';');
            builder.Append(", ");
            builder.Append(obj.LookAheadNodeType.Nickname);

            return builder.ToString();
        }


        public override void Dump(System.IO.StreamWriter stream)
        {
            this.Regulation.Left.Dump(stream);
            stream.Write(" ::= ");

            int count = this.Regulation.RightPart.Count();
            for (int i = 0; i < count; i++)
            {
                if (i == this.DotPosition)
                { stream.Write('.'); stream.Write(' '); }

                TreeNodeType item = this.Regulation.RightNode(i);
                item.Dump(stream);

                stream.Write(' ');
            }

            if (this.DotPosition == count)
            { stream.Write('.'); stream.Write(' '); }

            stream.Write(';');
            stream.Write(", ");
            this.LookAheadNodeType.Dump(stream);
        }
    }
}
