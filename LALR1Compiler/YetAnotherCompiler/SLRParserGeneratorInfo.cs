using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public class SLRParserGeneratorInfo
    {

        public SLRParserGeneratorInfo(RegulationList grammar, List<FOLLOW> followList, LR0StateList stateList, LR0EdgeList edgeList, LRParsingMap parsingMap)
        {
            this.Grammar = grammar;
            this.FollowList = followList;
            this.StateList = stateList;
            this.EdgeList = edgeList;
            this.ParsingMap = parsingMap;
        }

        public RegulationList Grammar { get; set; }

        public List<FOLLOW> FollowList { get; set; }

        public LR0StateList StateList { get; set; }

        public LR0EdgeList EdgeList { get; set; }

        public LRParsingMap ParsingMap { get; set; }

        //public void Dump(string fullname)
        //{
        //    using (StreamWriter sw = new StreamWriter(fullname, false))
        //    {
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("grammar:");
        //        sw.WriteLine(Grammar.ToString());
        //        sw.WriteLine("==========================================================================================");
        //        sw.WriteLine("Follow list:");
        //        foreach (var item in this.FollowList)
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
        //        sw.WriteLine("map for SLR test:");
        //        ParsingMap.Dump(sw);
        //        sw.WriteLine();
        //    }
        //}
    }
}
