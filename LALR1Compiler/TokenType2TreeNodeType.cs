using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    /// <summary>
    /// 由单词类型获取对应的V类型
    /// </summary>
    public class TokenType2TreeNodeType
    {
        public virtual TreeNodeType GetNodeType(TokenType tokenType)
        {
            //TODO:“Leave__”后缀，这是个自定义规则
            string strTreeNodeType = tokenType.Type + "Leave__";
            string content = tokenType.Content;
            TreeNodeType result = new TreeNodeType(strTreeNodeType, content, tokenType.Nickname);

            return result;
        }
    }
}
