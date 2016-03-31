using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    public class SyntaxTreeList : List<SyntaxTree>
    {
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public override string ToString()
        //{
        //    StringBuilder result = new StringBuilder();
        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        result.Append(this[i].NodeValue);
        //        if (i + 1 < this.Count)
        //        {
        //            result.Append(", ");
        //        }
        //    }

        //    return result.ToString();
        //}
    }
}
