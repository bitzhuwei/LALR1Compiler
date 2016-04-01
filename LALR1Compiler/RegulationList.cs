using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 规则列表。一个完整的规则列表就是一个文法。
    /// </summary>
    public class RegulationList : List<Regulation>, IDump2Stream
    {
        public RegulationList(params Regulation[] regulations)
        {
            if (regulations != null && regulations.Length > 0)
            {
                this.AddRange(regulations);
            }
        }

        public List<TreeNodeType> GetAllTreeNodeTypes()
        {
            var result = new List<TreeNodeType>();
            foreach (var item in this)
            {
                {
                    TreeNodeType node = item.Left;
                    if (!result.Contains(node))
                    { result.Add(node); }
                }
                foreach (var node in item.RightPart)
                {
                    if (!result.Contains(node))
                    { result.Add(node); }
                }
            }

            return result;
        }


        /// <summary>
        /// 获取所有叶结点。
        /// </summary>
        /// <returns></returns>
        public List<TreeNodeType> GetAllTreeNodeLeaveTypes()
        {
            var result = new List<TreeNodeType>();
            foreach (var item in this)
            {
                foreach (var node in item.RightPart)
                {
                    if (node.IsLeave)
                    {
                        if (!result.Contains(node))
                        { result.Add(node); }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取所有非叶结点。
        /// </summary>
        /// <returns></returns>
        public List<TreeNodeType> GetAllTreeNodeNonLeaveTypes()
        {
            var result = new List<TreeNodeType>();
            foreach (var item in this)
            {
                TreeNodeType node = item.Left;
                if (!result.Contains(node))
                { result.Add(node); }
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                Regulation item = this[i];
                builder.Append(item);
                if (i + 1 < this.Count)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取以指定结点为左部的规则。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<Regulation> GetRegulations(TreeNodeType node)
        {
            List<Regulation> list = new List<Regulation>();
            foreach (var item in this)
            {
                if (item.Left == node)
                {
                    list.Add(item);
                }
            }

            return list;
        }


        public void Dump(System.IO.TextWriter stream)
        {
            foreach (var item in this)
            {
                item.Dump(stream);
            }
        }

        public void Dump(string fullname)
        {
            using (StreamWriter stream = new StreamWriter(fullname, false))
            {
                Dump(stream);
            }
        }

    }
}
