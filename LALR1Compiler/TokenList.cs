using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    /// <summary>
    /// 词法分析的结果。
    /// </summary>
    public class TokenList : List<Token>
    {
        /// <summary>
        /// 给出单词列表
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("TokenList[Count: {0}]", this.Count);
        }

        public void Dump(string tokenListFile)
        {
            using (StreamWriter sw = new StreamWriter(tokenListFile, false))
            {
                sw.WriteLine(string.Format(
                "TokenList[Count: {0}]", this.Count));
                foreach (var v in this)
                {
                    sw.WriteLine(v.ToString());
                }
            }
        }
    }
}