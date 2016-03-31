using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 将此对象的信息写入流中。
    /// </summary>
    public interface IDump2Stream
    {
        /// <summary>
        /// 将此对象的信息写入流中。
        /// </summary>
        /// <param name="stream"></param>
        void Dump(StreamWriter stream);
    }
}
