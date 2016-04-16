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

        public override void Dump(System.IO.TextWriter stream)
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
