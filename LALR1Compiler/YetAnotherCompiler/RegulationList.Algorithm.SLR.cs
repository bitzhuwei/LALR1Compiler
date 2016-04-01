using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    public static partial class RegulationListHelper
    {

        /// <summary>
        /// 用SLR分析法计算分析表
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        public static LRParsingMap GetSLRParsingMap(this RegulationList grammar)
        {
            // 给文法添加一个辅助的开始产生式 S' -> S $
            // 如何添加一个外来的结点类型？用Enum是无法做到的。
            var decoratedS = new TreeNodeType("__S2", "S'", "<S'>");
            var decoratedEnd = TreeNodeType.endOfTokenListNode;
            var decoratedRegulation = new Regulation(
                decoratedS, grammar[0].Left, decoratedEnd);
            var decoratedGrammar = new RegulationList(decoratedRegulation);
            decoratedGrammar.AddRange(grammar);
            // 初始化T为{ Closure(S' -> S $) }
            var firstItem = new LR0Item(decoratedGrammar[0], 0);
            var firstState = new LR0State(firstItem);
            firstState = decoratedGrammar.Closure(firstState);
            var stateList = new LR0StateCollection(firstState);
            var edgeList = new LR0EdgeCollection();
            Queue<LR0State> queue = new Queue<LR0State>();
            queue.Enqueue(firstState);
            int lastOutputLength = 0;
            int stateListCount = 1;
            int queueCount = 1;
            while (queue.Count > 0)
            {
                LR0State fromState = queue.Dequeue(); queueCount--;
                int itemIndex = 0;
                int itemCount = fromState.Count();
                foreach (var item in fromState)
                {
                    for (int i = 0; i < lastOutputLength; i++) { Console.Write('\u0008'); }
                    string output = string.Format("Calculating SLR State List: {0} <-- {1}, working on {2}/{3} ...",
                        stateListCount, queueCount, 1 + itemIndex++, itemCount);
                    Console.Write(output);
                    lastOutputLength = output.Length;
                    TreeNodeType x = item.GetNodeNext2Dot();
                    if (x == null || x == decoratedEnd) { continue; }

                    LR0State toState = decoratedGrammar.Goto(fromState, x);
                    if (stateList.TryInsert(toState))
                    {
                        queue.Enqueue(toState);
                        stateListCount++;
                        queueCount++;
                    }
                    LR0Edge edge = new LR0Edge(fromState, x, toState);
                    edgeList.TryInsert(edge);
                }
            }
            Console.WriteLine();

            LRParsingMap parsingMap = new LRParsingMap();
            foreach (var edge in edgeList)
            {
                if (edge.X.IsLeave)
                {
                    parsingMap.SetAction(stateList.IndexOf(edge.From) + 1, edge.X,
                        new LR1ShiftInAction(stateList.IndexOf(edge.To) + 1));
                }
                else
                {
                    parsingMap.SetAction(stateList.IndexOf(edge.From) + 1, edge.X,
                        new LR1GotoAction(stateList.IndexOf(edge.To) + 1));
                }
            }
            var endItem = new LR0Item(decoratedRegulation, 1);
            FOLLOWCollection followCollection;
            decoratedGrammar.GetFollowCollection(out followCollection);
            foreach (var state in stateList)
            {
                if (state.Contains(endItem))
                {
                    parsingMap.SetAction(stateList.IndexOf(state) + 1, decoratedEnd,
                        new LR1AceptAction());
                }
                foreach (var lr0Item in state)
                {
                    if (lr0Item.GetNodeNext2Dot() == null)
                    {
                        FOLLOW follow = FindFollow(followCollection, lr0Item.Regulation.Left);
                        foreach (var value in follow.Values)
                        {
                            parsingMap.SetAction(stateList.IndexOf(state) + 1, value,
                                new LR1ReducitonAction(decoratedGrammar.IndexOf(lr0Item.Regulation)));
                        }
                    }
                }
            }

            return  parsingMap;
        }

    }
}
