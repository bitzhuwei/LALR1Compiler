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
        public static string Dump(this HashCache cache)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    cache.Dump(sw);
                }

                ms.Position = 0;
                
                using (StreamReader sr = new StreamReader(ms))
                {
                    string str = sr.ReadToEnd();

                    return str;
                }
            }
        }
    }
}
