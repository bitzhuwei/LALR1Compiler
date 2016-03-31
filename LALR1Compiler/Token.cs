using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    /// <summary>
    /// 单词，即词法分析器输出列表的元素
    /// </summary>
    public class Token
    {

        /// <summary>
        /// 单词类型
        /// </summary>
        public TokenType TokenType { get; set; }

        /// <summary>
        /// 异常信息，一般在LexicalError为true的时候不为空
        /// </summary>
        public string ErrorInfo { get; set; }

        ///// <summary>
        ///// 附属到此单词对象上的某对象。
        ///// </summary>
        //public object Tag { get; set; }

        private bool m_LexicalError = false;
        /// <summary>
        /// 标识是否是正确的单词
        /// </summary>
        public bool LexicalError
        {
            get { return m_LexicalError; }
            set { m_LexicalError = value; }
        }

        private int m_Line = -1;
        /// <summary>
        /// 所在行（从0开始）
        /// </summary>
        public int Line
        {
            get { return m_Line; }
            set { m_Line = value; }
        }

        private int m_Column = -1;
        /// <summary>
        /// 所在列（从0开始）
        /// </summary>
        public int Column
        {
            get { return m_Column; }
            set { m_Column = value; }
        }

        private int m_IndexOfSourceCode = -1;
        /// <summary>
        /// 第一个字符在源代码字符串中的索引
        /// </summary>
        public int IndexOfSourceCode
        {
            get { return m_IndexOfSourceCode; }
            set { m_IndexOfSourceCode = value; }
        }

        /// <summary>
        /// 单词长度（字符个数）
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 显示[具体信息]$[单词类型]$[行数]$[列数]$[是否是正确的单词]$[备注说明]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}]$[Ln:{1}, Col:{2}]{3}",
                TokenType, Line, Column,
                LexicalError ? "[" + ErrorInfo + "]" : "");
        }

    }
}
