using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    public class SyntaxTree : ICloneable, IDump2Stream
    {
        /// <summary>
        /// 有缩进的树状信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} with {1} children.", this.NodeType, this.Children.Count);
        }

        #region 字段和属性

        /// <summary>
        /// 对应的第一个单词
        /// </summary>
        public int MappedTokenStartIndex { get; set; }

        /// <summary>
        /// 对应的单词数目
        /// </summary>
        public int MappedTokenLength { get; set; }

        /// <summary>
        /// 对应的单词列表
        /// </summary>
        public TokenList GetMappedTokenList()
        {
            var result = new TokenList();
            if (MappedTotalTokenList != null)
            {
                for (int i = 0, j = MappedTokenStartIndex;
                    i < MappedTokenLength && j < MappedTotalTokenList.Count; i++, j++)
                {
                    result.Add(MappedTotalTokenList[j]);
                }
            }

            return result;
        }
        /// <summary>
        /// 整个语法树对应的单词列表
        /// </summary>
        public TokenList MappedTotalTokenList { get; set; }

        /// <summary>
        /// 标记，若发生语法错误，应在此说明
        /// </summary>
        public string ErrorInfo { get; set; }

        /// <summary>
        /// 是否有语法错误
        /// </summary>
        private bool m_SyntaxError = false;
        /// <summary>
        /// 是否有语法错误
        /// </summary>
        public bool SyntaxError
        {
            get
            {
                if (m_SyntaxError) return true;
                else
                {
                    foreach (var item in m_Children)
                    {
                        if (item.m_SyntaxError)
                            return true;
                    }
                    return false;
                }
            }
            set { m_SyntaxError = value; }
        }
        /// <summary>
        /// 获取此树及其子树中有语法错误的结点的列表
        /// </summary>
        /// <returns></returns>
        public SyntaxTreeList GetErrorTreeNodes()
        {
            var result = new SyntaxTreeList();
            this._GetErrorTreeNodes(result);
            return result;
        }

        private void _GetErrorTreeNodes(SyntaxTreeList result)
        {
            if (m_SyntaxError) result.Add(this);
            foreach (var item in m_Children)
            {
                item._GetErrorTreeNodes(result);
            }
        }

        // TODO: 这东西也许以后就没用了
        /// <summary>
        /// 此结点的值
        /// </summary>
        public TreeNodeType NodeType { get; set; }

        private SyntaxTreeList m_Children = new SyntaxTreeList();
        /// <summary>
        /// 子结点
        /// </summary>
        public SyntaxTreeList Children
        {
            get { return m_Children; }
            private set { m_Children = value; }
        }

        /// <summary>
        /// 父结点
        /// </summary>
        public SyntaxTree Parent { get; set; }

        #endregion 字段和属性

        /// <summary>
        /// 创建本语法树的深复制对象。
        /// <para>除Tag属性外，其他属性完全相同</para>
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var result = new SyntaxTree();
            result.MappedTokenLength = this.MappedTokenLength;
            result.MappedTokenStartIndex = this.MappedTokenStartIndex;
            result.MappedTotalTokenList = this.MappedTotalTokenList;
            result.NodeType = new TreeNodeType(this.NodeType.Type, this.NodeType.Content, this.NodeType.Nickname);
            result.SyntaxError = this.SyntaxError;
            result.ErrorInfo = this.ErrorInfo;
            foreach (var item in this.Children)
            {
                var c = item.Clone() as SyntaxTree;
                result.Children.Add(c);
                c.Parent = result;
            }
            return result;
        }

        private void Dump(StreamWriter stream, SyntaxTree syntaxTree)
        {
            DumpPremark(stream, syntaxTree);
            stream.WriteLine(syntaxTree.NodeType);
            syntaxTree.NodeType.Dump(stream);

            foreach (var item in syntaxTree.Children)
            {
                Dump(stream, item);
            }
        }

        private void DumpPremark(StreamWriter stream, SyntaxTree syntaxTree)
        {
            var parent = syntaxTree.Parent;
            if (parent == null) { return; }

            List<bool> lstline = new List<bool>();
            while (parent != null)
            {
                var pp = parent.Parent;
                if (pp != null)
                {
                    lstline.Add(pp.Children.IndexOf(parent) < pp.Children.Count - 1);
                }
                parent = pp;
            }
            for (int i = lstline.Count - 1; i >= 0; i--)
            {
                if (lstline[i])
                { stream.Write(" │ "); }
                else
                { stream.Write("    "); }
            }
            parent = syntaxTree.Parent;
            if (parent.Children.IndexOf(syntaxTree) < parent.Children.Count - 1)
            { stream.Write(" ├─"); }
            else
            { stream.Write(" └─"); }
        }

        public void Dump(StreamWriter stream)
        {
            Dump(stream, this);
        }
    }

}
