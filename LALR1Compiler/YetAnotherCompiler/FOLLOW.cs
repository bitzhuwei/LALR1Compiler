using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 一个FOLLOW集
    /// </summary>
    public class FOLLOW : HashCache
    {

        /// <summary>
        /// 一个FOLLOW集
        /// </summary>
        /// <param name="target"></param>
        /// <param name="values"></param>
        public FOLLOW(TreeNodeType target, params TreeNodeType[] values)
            : base(GetUniqueString)
        {
            this.Target = target;
            if (values != null)
            {
                foreach (var item in values)
                {
                    this.values.TryInsert(item);
                }
            }
        }

        private static string GetUniqueString(HashCache cache)
        {
            FOLLOW obj = cache as FOLLOW;
            StringBuilder builder = new StringBuilder();
            builder.Append("FOLLOW(");
            builder.Append(obj.Target.Nickname);
            builder.Append(") = 【 ");
            int count = obj.values.Count;
            for (int i = 0; i < count; i++)
            {
                builder.Append(obj.values[i].Nickname);
                builder.Append(" ");
            }
            builder.Append("】");

            return builder.ToString();
        }

        public TreeNodeType Target { get; private set; }

        private OrderedCollection<TreeNodeType> values = new OrderedCollection<TreeNodeType>(" ");

        public IEnumerable<TreeNodeType> Values { get { return this.values; } }

        public bool TryInsert(TreeNodeType value)
        {
            if (this.values.TryInsert(value))
            {
                this.SetDirty();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Dump(System.IO.StreamWriter stream)
        {
            stream.Write("FOLLOW(");
            this.Target.Dump(stream);
            stream.Write(") = 【 ");
            for (int i = 0; i < this.values.Count; i++)
            {
                this.values[i].Dump(stream);
                stream.Write(" ");
            }
            stream.Write("】");

        }
    }
}
