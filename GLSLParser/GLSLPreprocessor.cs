using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSLParser
{
    public static class GLSLPreprocessor
    {
        /// <summary>
        /// 去掉行尾可能出现的'\\'
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string ConnectLines(this string fullname)
        {
            var builder = new StringBuilder();
            using (StreamReader sr = new StreamReader(fullname))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.EndsWith("\\"))
                    {
                        int length = line.Length;
                        builder.Append(line.Substring(0, length - 1));
                    }
                    else
                    {
                        builder.AppendLine(line);
                    }
                }
            }

            return builder.ToString();
        }
        /// <summary>
        /// 预处理。去掉所有的#指令。
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        public static string DummyPreprocess(this string fullname)
        {
            StringBuilder builder = new StringBuilder();
            using (StreamReader sr = new StreamReader(fullname))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.Trim().StartsWith("#")) { continue; }

                    builder.AppendLine(line);
                }
            }

            return builder.ToString();
        }
    }
}
