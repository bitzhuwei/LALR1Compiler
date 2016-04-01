using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    /// <summary>
    /// 一系列的语法树结点。
    /// </summary>
    public class TreeNodeTypeList : List<TreeNodeType>, IDump2Stream
    {

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                TreeNodeType nodeType = this[i];
                builder.Append(nodeType.Nickname);

                if (i + 1 < this.Count)
                {
                    builder.Append(' ');
                }
            }
            if (this.Count == 0)
            {
                builder.Append("null");// ε
            }

            return builder.ToString();
        }

        public void Dump(System.IO.TextWriter stream)
        {
            for (int i = 0; i < this.Count; i++)
            {
                TreeNodeType nodeType = this[i];
                nodeType.Dump(stream);

                if (i + 1 < this.Count)
                {
                    stream.Write(' ');
                }
            }
            if (this.Count == 0)
            {
                stream.Write("null");// ε
            }
        }
    }
}
