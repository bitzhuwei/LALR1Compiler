using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler
{
    // TODO:所有单词类型（除CGCompiler的关键字外）都以“__”前缀开头。这是为了避免与CGCompiler的关键字冲突。
    // 所以，CGCompiler的关键字就不能以“__”前缀开头了。
    /// <summary>
    /// 单词类型
    /// </summary>
    static class ContextfreeGrammarTokenType
    {
        
        // 固定项
        /// <summary>
        /// 未知的单词，这意味着源代码或者编译器有错误。
        /// </summary>
        public const string __unknown = "__unknown";

        /// <summary>
        /// ::=
        /// </summary>
        public const string __colon_colon_equal = "__colon_colon_equal";

        // ;
        /// <summary>
        /// ;
        /// </summary>
        public const string __semicolon = "__semicolon";

        /// <summary>
        /// |
        /// </summary>
        public const string __vertical_bar = "__vertical_bar";
        /// <summary>
        /// &lt;
        /// </summary>
        public const string __left_angle = "__left_angle";

        /// <summary>
        /// &gt;
        /// </summary>
        public const string __right_angle = "__right_angle";
        /// <summary>
        /// null
        /// </summary>
        public const string __null = "__null";
        /// <summary>
        /// identifier
        /// </summary>
        public const string __identifier = "__identifier";
        /// <summary>
        /// number
        /// </summary>
        public const string __number = "__number";
        /// <summary>
        /// constString
        /// </summary>
        public const string __constString = "__constString";

        // Grammar的关键字作为结点
        /// <summary>
        /// identifier
        /// </summary>
        public const string identifier = "identifier";
        /// <summary>
        /// constString
        /// </summary>
        public const string constString = "constString";
        ///// <summary>
        ///// null
        ///// </summary>
        //public const string _null = "null";
    }
}
