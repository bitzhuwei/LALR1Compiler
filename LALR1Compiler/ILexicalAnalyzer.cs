using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 词法分析器接口
    /// </summary>
    public interface ILexicalAnalyzer
    {
        /// <summary>
        /// 分析源代码获得Token序列
        /// </summary>
        /// <returns></returns>
        TokenList Analyze(string sourceCode);
    }
}
