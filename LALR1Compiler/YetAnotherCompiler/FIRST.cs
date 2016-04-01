using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 一个FIRST集
    /// </summary>
    public class FIRST : HashCache
    {

        /// <summary>
        /// 一个FIRST集
        /// </summary>
        /// <param name="target"></param>
        /// <param name="values"></param>
        public FIRST(TreeNodeType target, params TreeNodeType[] values)
            : base(GetUniqueString)
        {
            this.target.Add(target);
            if (values != null)
            {
                foreach (var item in values)
                {
                    this.values.TryInsert(item);
                }
            }
        }

        public FIRST(IEnumerable<TreeNodeType> target, params TreeNodeType[] values)
            : base(GetUniqueString)
        {
            this.target.AddRange(target);
            if (values != null)
            {
                foreach (var item in values)
                {
                    this.values.TryInsert(item);
                }
            }
        }

        public FIRST(IEnumerable<TreeNodeType> target)
            : base(GetUniqueString)
        {
            this.target.AddRange(target);
        }

        private static string GetUniqueString(HashCache cache)
        {
            FIRST obj = cache as FIRST;
            StringBuilder builder = new StringBuilder();
            {
                builder.Append("FIRST( ");

                int count = obj.target.Count;
                for (int i = 0; i < count; i++)
                {
                    builder.Append(obj.target[i].Nickname);
                    if (i + 1 < count)
                    { builder.Append(" "); }
                }
                if (count == 0)
                { builder.Append("ε "); }

                builder.Append(" ) = ");
            }
            {
                builder.Append("【 ");
                int count = obj.values.Count;
                for (int i = 0; i < count; i++)
                {
                    builder.Append(obj.values[i].Nickname);
                    if (i + 1 < count)
                    { builder.Append(" "); }
                }
                builder.Append(" 】");
            }
            return builder.ToString();
        }

        private List<TreeNodeType> target = new List<TreeNodeType>();
        public IEnumerable<TreeNodeType> Target { get { return this.target; } }

        public TreeNodeType GetNode(int index)
        {
            return this.target[index];
        }

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
            {
                stream.Write("FIRST( ");

                int count = this.target.Count;
                for (int i = 0; i < count; i++)
                {
                    this.target[i].Dump(stream);
                    stream.Write(" ");
                }
                if (count == 0)
                { stream.Write("ε "); }

                stream.Write(") = ");
            }
            {
                stream.Write("【 ");
                int count = this.values.Count;
                for (int i = 0; i < count; i++)
                {
                    this.values[i].Dump(stream);
                    stream.Write(" ");
                }
                stream.Write("】");
            }
        }
    }
}
