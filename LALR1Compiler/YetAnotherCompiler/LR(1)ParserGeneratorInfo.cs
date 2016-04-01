using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class LR1ParserGeneratorInfo
    {

        public LR1ParserGeneratorInfo(RegulationList grammar, List<FIRST> firstList, LR1StateCollection stateList, LR1EdgeCollection edgeList, LRParsingMap parsingMap)
        {
            this.Grammar = grammar;
            this.FirstList = firstList;
            this.StateList = stateList;
            this.EdgeList = edgeList;
            this.ParsingMap = parsingMap;
        }

        public RegulationList Grammar { get; set; }

        public List<FIRST> FirstList { get; set; }

        public LR1StateCollection StateList { get; set; }

        public LR1EdgeCollection EdgeList { get; set; }

        public LRParsingMap ParsingMap { get; set; }

        //public void Dump(string fullname)
        //{
        //    using (StreamWriter sw = new StreamWriter(fullname, false))
        //    {
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("grammar:");
        //        sw.WriteLine(Grammar.ToString());
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("FIRST list:");
        //        foreach (var item in this.FirstList)
        //        {
        //            sw.WriteLine(item.ToString());
        //        }
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("state list:");
        //        StateList.Dump(sw);
        //        sw.WriteLine();
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("edge list:");
        //        EdgeList.Dump(sw);
        //        sw.WriteLine();
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("map for LR(1) test:");
        //        ParsingMap.Dump(sw);
        //    }
        //}
    }
}
