using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LL1语法分析器需要的分析表
    /// </summary>
    public class LRParsingMap : IEnumerable<KeyValuePair<string, List<LRParsingAction>>>
    {
        //TODO: rename "next"
        /// <summary>
        /// 设置给定语法类型、单词类型所对应的分析函数
        /// </summary>
        /// <param name="leftNode"></param>
        /// <param name="next"></param>
        /// <param name="function"></param>
        public void SetAction(int stateId, TreeNodeType next, LRParsingAction function)
        {
            string key = stateId.ToString() + "+" + next.Type.ToString();
            List<LRParsingAction> value = null;
            if (parserMap.TryGetValue(key, out value))
            {
                value.Add(function);
            }
            else
            {
                List<LRParsingAction> list = new List<LRParsingAction>();
                list.Add(function);
                parserMap.Add(key, list);
            }
        }

        // rename "next"
        /// <summary>
        /// 获取处理函数
        /// </summary>
        /// <param name="leftNode">当前结非终点类型</param>
        /// <param name="nodeType">要处理的结点类型</param>
        /// <returns></returns>
        public LRParsingAction GetAction(int stateId, TreeNodeType nodeType)
        {
            string key = stateId.ToString() + "+" + nodeType.Type.ToString();
            List<LRParsingAction> value = null;
            if (parserMap.TryGetValue(key, out value))
            {
                return value[0];
            }
            else
            {
                return null;// new LR1ParsingAction(); // TODO:将来这里可以放对语法错误进行分析的函数
            }
        }

        private Dictionary<string, List<LRParsingAction>> parserMap =
            new Dictionary<string, List<LRParsingAction>>();

        public override string ToString()
        {
            StringBuilder totalBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();
            StringBuilder conflictsBuilder = new StringBuilder();
            int conflictCount = 0;
            int count = 0;
            foreach (var item in this.parserMap)
            {
                int valueCount = item.Value.Count;
                if (valueCount > 1) { conflictCount++; }
                count += valueCount;

                foreach (var function in item.Value)
                {
                    string str = string.Format("[{0}]->[{1}]", item.Key, function);
                    builder.AppendLine(str);
                    if (valueCount > 1)
                    { conflictsBuilder.AppendLine(str); }
                }
            }

            builder.AppendLine("------------------------------------------------------------------------------------------");
            totalBuilder.AppendFormat("[{0} items]:", count);
            totalBuilder.AppendLine();
            totalBuilder.Append(builder.ToString());
            builder.AppendLine("------------------------------------------------------------------------------------------");
            totalBuilder.AppendFormat("[{0} conflicts]:", conflictCount);
            totalBuilder.AppendLine();
            totalBuilder.Append(conflictsBuilder.ToString());

            return totalBuilder.ToString();
        }

        public IEnumerator<KeyValuePair<string, List<LRParsingAction>>> GetEnumerator()
        {
            foreach (var item in this.parserMap)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dump(System.IO.StreamWriter stream)
        {
            int conflictCount = 0;
            int count = 0;
            {
                stream.WriteLine("------------------------------------------------------------------------------------------");
                foreach (var item in this.parserMap)
                {
                    int valueCount = item.Value.Count;
                    if (valueCount > 1) { conflictCount++; }
                    count += valueCount;
                }
                stream.WriteLine("[{0} items]", count);
                stream.WriteLine("[{0} conflicts]", conflictCount);
            }
            {
                stream.WriteLine("------------------------------------------------------------------------------------------");
                stream.WriteLine("[{0} items]:", count);
                foreach (var item in this.parserMap)
                {
                    foreach (var function in item.Value)
                    {
                        stream.WriteLine("[{0}]->[{1}]", item.Key, function);
                    }
                }
            }
            {
                stream.WriteLine("------------------------------------------------------------------------------------------");
                stream.WriteLine("[{0} conflicts]:", conflictCount);
                foreach (var item in this.parserMap)
                {
                    int valueCount = item.Value.Count;
                    foreach (var function in item.Value)
                    {
                        if (valueCount > 1)
                        { stream.WriteLine("[{0}]->[{1}]", item.Key, function); }
                    }
                }
            }
        }
    }
}
