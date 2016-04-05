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
        /// 用LALR(1)分析法计算分析表
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        public static void GetLALR1ParsingMap(this RegulationList grammar,
            out LRParsingMap map,
            out LALR1StateCollection stateCollection,
            out LALR1EdgeCollection edgeCollection, TextWriter writer)
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
            var firstItem = new LR1Item(decoratedGrammar[0], 0, decoratedEnd);
            var firstState = new LALR1State(firstItem);
            Dictionary<TreeNodeType, bool> nullableDict;
            decoratedGrammar.GetNullableDict(out nullableDict);
            FIRSTCollection firstCollection;
            decoratedGrammar.GetFirstCollection(out firstCollection, nullableDict);
            firstState = decoratedGrammar.Closure(firstState, nullableDict, firstCollection);
            stateCollection = new LALR1StateCollection(firstState);
            edgeCollection = new LALR1EdgeCollection(stateCollection);
            var queue = new Queue<LALR1State>();
            queue.Enqueue(firstState);
            int lastOutputLength = 0;
            int stateListCount = 1;
            int queueCount = 1;
            while (queue.Count > 0)
            {
                LALR1State fromState = queue.Dequeue(); queueCount--;
                int itemIndex = 0;
                int itemCount = fromState.Count();
                foreach (var item in fromState)
                {
                    {
                        TextWriter currentWriter = Console.Out;
                        if (Console.Out != writer)
                        {
                            Console.SetOut(writer);
                        }
                        for (int i = 0; i < lastOutputLength; i++) { Console.Write('\u0008'); }
                        string output = string.Format("Calculating LALR(1) State List: {0} <-- {1}, working on {2}/{3} ...",
                            stateListCount, queueCount, 1 + itemIndex++, itemCount);
                        Console.Write(output);
                        lastOutputLength = output.Length;
                        Console.SetOut(currentWriter);
                    }
                    TreeNodeType x = item.GetNodeNext2Dot();
                    if (x == decoratedEnd || x == null) { continue; }

                    LALR1State toState = decoratedGrammar.Goto(fromState, x, nullableDict, firstCollection);
                    if (stateCollection.TryInsert(toState))//融入组织之中吧
                    {
                        int index = stateCollection.IndexOf(toState);
                        toState = stateCollection[index];
                        queue.Enqueue(toState);
                        stateListCount++;
                        queueCount++;
                        var edge = new LALR1Edge(fromState, x, toState);
                        edgeCollection.TryInsert(edge);
                    }
                    else
                    {
                        int index = stateCollection.IndexOf(toState);
                        toState = stateCollection[index];
                        var edge = new LALR1Edge(fromState, x, toState);
                        edgeCollection.TryInsert(edge);
                    }
                }
            }
            {
                TextWriter currentWriter = Console.Out;
                if (Console.Out != writer)
                {
                    Console.SetOut(writer);
                }
                Console.WriteLine();
                Console.SetOut(currentWriter);
            }

            map = new LRParsingMap();
            foreach (var edge in edgeCollection)
            {
                if (edge.X.IsLeave)
                {
                    int stateId = edge.From.ParsingMapIndex + 1;// stateCollection.IndexOf(edge.From) + 1;
                    int gotoId = edge.To.ParsingMapIndex + 1;//stateCollection.IndexOf(edge.To) + 1
                    map.SetAction(stateId, edge.X, new LR1ShiftInAction(gotoId));
                }
                else
                {
                    int stateId = edge.From.ParsingMapIndex + 1;// stateCollection.IndexOf(edge.From) + 1;
                    int gotoId = edge.To.ParsingMapIndex + 1;//stateCollection.IndexOf(edge.To) + 1
                    map.SetAction(stateId, edge.X, new LR1GotoAction(gotoId));
                }
            }
            // TODO: not implemented
            var endItem = new LR1Item(decoratedRegulation, 1, decoratedEnd);
            foreach (var state in stateCollection)
            {
                if (state.Contains(endItem))
                {
                    int stateId = state.ParsingMapIndex + 1;// stateCollection.IndexOf(state) + 1;
                    map.SetAction(stateId, decoratedEnd, new LR1AceptAction());
                }
                foreach (var LR1Item in state)
                {
                    if (LR1Item.GetNodeNext2Dot() == null)
                    {
                        int stateId = state.ParsingMapIndex + 1;// stateCollection.IndexOf(state) + 1;
                        map.SetAction(stateId, LR1Item.LookAheadNodeType,
                            new LR1ReducitonAction(decoratedGrammar.IndexOf(LR1Item.Regulation)));
                    }
                }
            }
        }

        /// <summary>
        /// LR(1)的Closure操作。
        /// 补全一个状态。
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        static LALR1State Closure(this RegulationList grammar, LALR1State state,
            Dictionary<TreeNodeType, bool> nullableDict = null, FIRSTCollection firstCollection = null)
        {
            if (nullableDict == null)
            { grammar.GetNullableDict(out nullableDict); }
            if (firstCollection == null)
            { grammar.GetFirstCollection(out firstCollection, nullableDict); }

            Queue<LR1Item> queue = new Queue<LR1Item>();
            foreach (var item in state)
            {
                queue.Enqueue(item);
            }
            while (queue.Count > 0)
            {
                LR1Item item = queue.Dequeue();
                TreeNodeType node = item.GetNodeNext2Dot();
                if (node == null || node.IsLeave) { continue; }

                List<TreeNodeType> betaZ = item.GetBetaZ();
                FIRST first = grammar.GetFirst(firstCollection, nullableDict, betaZ);
                List<Regulation> regulations = grammar.GetRegulations(node);
                foreach (var regulation in regulations)
                {
                    foreach (var value in first.Values)
                    {
                        LR1Item newItem = new LR1Item(regulation, 0, value);
                        if (state.TryInsert(newItem))
                        {
                            queue.Enqueue(newItem);
                        }
                    }
                }
            }

            return state;
        }

        /// <summary>
        /// LR(1)的Goto操作。
        /// 将圆点移到所有LR(1)项中的符号<paramref name="x"/>之后。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="state"></param>
        /// <param name="x">一个文法符号，终结点或非终结点。</param>
        /// <param name="firstList"></param>
        /// <returns></returns>
        static LALR1State Goto(this RegulationList list, LALR1State state, TreeNodeType x, Dictionary<TreeNodeType, bool> nullableDict, FIRSTCollection firstList = null)
        {
            var toState = new LALR1State();
            foreach (var item in state)
            {
                TreeNodeType nextNode = item.GetNodeNext2Dot();
                if (nextNode == x)
                {
                    var newItem = new LR1Item(item.Regulation, item.DotPosition + 1, item.LookAheadNodeType);
                    toState.TryInsert(newItem);
                }
            }

            return Closure(list, toState, nullableDict, firstList);
        }

    }

}
