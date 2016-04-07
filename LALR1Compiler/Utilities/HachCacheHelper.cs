using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public static class HachCacheHelper
    {
        /// <summary>
        /// 在确保不会产生大量大型字符串的情况下，可以使用此方式获取该对象Dump到流的内容。
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static string Dump(this IDump2Stream cache)
        {
            StringBuilder builder = new StringBuilder();
            using (StringWriter writer = new StringWriter(builder))
            {
                cache.Dump(writer);
            }

            string str = builder.ToString();
            return str;
        }
    }
}
