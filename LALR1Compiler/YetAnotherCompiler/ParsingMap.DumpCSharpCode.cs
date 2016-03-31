using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public static class LRParsingMapHelper
    {

        public static IEnumerable<string> ToCSharpCode(this LRParsingMap parsingMap)
        {
            // Demo output:
            // map.SetAction(23, __right_parenthesesLeave__, new LR1ReducitonAction(9));
            // map.SetAction(1, __S, new LR1GotoAction(2));

            foreach (var item in parsingMap)
            {
                string[] parts = item.Key.Split('+');
                string stateId = parts[0];
                string nodeType = parts[1];
                foreach (var value in item.Value)
                {
                    string result = string.Format("map.SetAction({0}, {1}, new {2}({3}));",
                        stateId, nodeType, value.GetType().Name, value.ActionParam());
                    yield return result;
                }
            }
        }
    }
}
