using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class LR0ParserGeneratorInfo
    {

        public LR0ParserGeneratorInfo(RegulationList grammar, LR0StateCollection stateList, LR0EdgeCollection edgeList, LRParsingMap parsingMap)
        {
            this.Grammar = grammar;
            this.StateList = stateList;
            this.EdgeList = edgeList;
            this.ParsingMap = parsingMap;
        }

        public RegulationList Grammar { get; set; }

        public LR0StateCollection StateList { get; set; }

        public LR0EdgeCollection EdgeList { get; set; }

        public LRParsingMap ParsingMap { get; set; }

        //public void Dump(string fullname)
        //{
        //    using (StreamWriter sw = new StreamWriter(fullname, false))
        //    {
        //        sw.AutoFlush = true;
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("grammar:");
        //        sw.WriteLine(Grammar.ToString());
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("state list:");
        //        StateList.Dump(sw);
        //        sw.WriteLine();
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("edge list:");
        //        EdgeList.Dump(sw);
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("map for LR(0) test:");
        //        ParsingMap.Dump(sw);
        //    }
        //}
    }
}
