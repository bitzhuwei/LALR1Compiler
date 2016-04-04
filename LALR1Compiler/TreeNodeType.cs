using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    /// <summary>
    /// 语法树结点的类型信息。
    /// </summary>
    public class TreeNodeType : HashCache
    {
        /// <summary>
        /// token流结束的标识。
        /// </summary>
        public static readonly TreeNodeType endOfTokenListNode = new TreeNodeType("end_of_token_listLeave__", "$", "\"$\"");

        /// <summary>
        /// 空结点。代表"null"（即厄普西隆ε）
        /// </summary>
        public static readonly TreeNodeType NullNode = new TreeNodeType("nullLeave__", "ε", "null");

        /// <summary>
        /// 语法树结点的类型。
        /// </summary>
        /// <param name="type">例如：“__E”or “numberLeave__”
        /// <para>除ContextfreeGrammar的关键字"null", "identifier", "number", "constString"外，都要以"__"开头。</para>
        /// <para>叶结点必须以"Leave__"结尾。</para>
        /// <para>非叶结点禁止以"Leave__"结尾。</para></param>
        /// <para>必须是以"_"或字母开头的C#标识符。</para>
        /// <param name="content">例如：“E”or “9”</param>
        /// <param name="nickName">例如：“&lt;E&gt;”or “number”
        /// <para>对于identifier和number，此值用于保存"identifier"和"number"这样的字符串。</para></param>
        public TreeNodeType(string type, string content, string nickName)
            : base(GetUniqueString)
        {
            if (string.IsNullOrEmpty(type)) { throw new ArgumentNullException(); }

            this.Type = type;
            this.Content = content;
            this.Nickname = nickName;

            this.IsLeave = this.Type.EndsWith("Leave__");
        }

        private static string GetUniqueString(HashCache cache)
        {
            TreeNodeType obj = cache as TreeNodeType;
            return obj.Dump();
            //return string.Format("({0})[{1}][{2}]", obj.Type, obj.Content, obj.Nickname);
        }

        // “__E”or “numberLeave__”
        /// <summary>
        /// 例如：“__E”or “numberLeave__”
        /// </summary>
        public string Type { get; private set; }

        // “E”or “9”
        /// <summary>
        /// 例如：“E”or “9”
        /// </summary>
        public string Content { get; private set; }

        // “<E>”or “9”
        /// <summary>
        /// 例如：“&lt;E&gt;”or “number”
        /// 对于identifier和number，此值用于保存"identifier"和"number"这样的字符串。
        /// </summary>
        public string Nickname { get; private set; }

        /// <summary>
        /// 此结点是叶结点？
        /// </summary>
        public bool IsLeave { get; private set; }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write(this.Nickname);
        }
    }
}
