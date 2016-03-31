using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 语法分析器接口
    /// </summary>
    public interface ISyntaxParser
    {
        /// <summary>
        /// 分析TokenListSource获得语法树
        /// </summary>
        /// <returns></returns>
        SyntaxTree Parse(TokenList tokenList);
    }
}
