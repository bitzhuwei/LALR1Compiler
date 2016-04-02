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
        /// 用LR(0)分析法计算分析表
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        public static void GetLR0ParsingMap(this RegulationList grammar,
            out LRParsingMap map,
            out LR0StateCollection stateCollection,
            out LR0EdgeCollection edgeCollection)
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
            stateCollection = new LR0StateCollection(firstState);
            edgeCollection = new LR0EdgeCollection(stateCollection);
            Queue<LR0State> queue = new Queue<LR0State>();
            queue.Enqueue(firstState);
            //int lastOutputLength = 0;
            int stateListCount = 1;
            int queueCount = 1;
            while (queue.Count > 0)
            {
                LR0State fromState = queue.Dequeue(); queueCount--;
                //int itemIndex = 0;
                int itemCount = fromState.Count();
                foreach (var item in fromState)
                {
                    //for (int i = 0; i < lastOutputLength; i++) { Console.Write('\u0008'); }
                    //string output = string.Format("Calculating LR(0) State List: {0} <-- {1}, working on {2}/{3} ...",
                    //    stateListCount, queueCount, 1 + itemIndex++, itemCount);
                    //Console.Write(output);
                    //lastOutputLength = output.Length;
                    TreeNodeType x = item.GetNodeNext2Dot();
                    if (x == null || x == decoratedEnd) { continue; }

                    LR0State toState = decoratedGrammar.Goto(fromState, x);
                    if (stateCollection.TryInsert(toState))
                    {
                        queue.Enqueue(toState);
                        stateListCount++;
                        queueCount++;
                    }
                    LR0Edge edge = new LR0Edge(fromState, x, toState);
                    edgeCollection.TryInsert(edge);
                }
            }
            Console.WriteLine();

            map = new LRParsingMap();
            foreach (var edge in edgeCollection)
            {
                if (edge.X.IsLeave)
                {
                    map.SetAction(stateCollection.IndexOf(edge.From) + 1, edge.X,
                        new LR1ShiftInAction(stateCollection.IndexOf(edge.To) + 1));
                }
                else
                {
                    map.SetAction(stateCollection.IndexOf(edge.From) + 1, edge.X,
                        new LR1GotoAction(stateCollection.IndexOf(edge.To) + 1));
                }
            }
            var endItem = new LR0Item(decoratedRegulation, 1);
            foreach (var state in stateCollection)
            {
                if (state.Contains(endItem))
                {
                    map.SetAction(stateCollection.IndexOf(state) + 1, decoratedEnd,
                        new LR1AceptAction());
                }
                foreach (var item in state)
                {
                    if (item.GetNodeNext2Dot() == null)
                    {
                        List<TreeNodeType> allTreeNodeTypes = decoratedGrammar.GetAllTreeNodeLeaveTypes();
                        foreach (var treeNodeType in allTreeNodeTypes)
                        {
                            map.SetAction(stateCollection.IndexOf(state) + 1, treeNodeType,
                                new LR1ReducitonAction(decoratedGrammar.IndexOf(item.Regulation)));
                        }
                        map.SetAction(stateCollection.IndexOf(state) + 1, decoratedEnd,
                            new LR1ReducitonAction(decoratedGrammar.IndexOf(item.Regulation)));
                    }
                }
            }
        }

        /// <summary>
        /// LR(0)的Closure操作。
        /// 补全一个状态。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        static LR0State Closure(this RegulationList list, LR0State state)
        {
            Queue<LR0Item> queue = new Queue<LR0Item>();
            foreach (var item in state)
            {
                queue.Enqueue(item);
            }
            while (queue.Count > 0)
            {
                LR0Item item = queue.Dequeue();
                TreeNodeType node = item.GetNodeNext2Dot();
                if (node == null) { continue; }

                foreach (var regulation in list)
                {
                    if (regulation.Left == node)
                    {
                        var newItem = new LR0Item(regulation, 0);
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
        /// LR(0)的Goto操作。
        /// 将圆点移到所有LR(0)项中的符号<paramref name="x"/>之后。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="state"></param>
        /// <param name="x">一个文法符号，终结点或非终结点。</param>
        /// <returns></returns>
        static LR0State Goto(this RegulationList list, LR0State state, TreeNodeType x)
        {
            LR0State toState = new LR0State();
            foreach (var item in state)
            {
                TreeNodeType nextNode = item.GetNodeNext2Dot();
                if (nextNode == x)
                {
                    var newItem = new LR0Item(item.Regulation, item.DotPosition + 1);
                    toState.TryInsert(newItem);
                }
            }

            return Closure(list, toState);
        }

    }

}
