using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LALR1Compiler
{
    /// <summary>
    /// LR1分析动作的基类
    /// </summary>
    public abstract class LRParsingAction : HashCache
    {
        public LRParsingAction() { }

        /// <summary>
        /// 执行分析动作。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract int Execute(ParsingContext context);

        /// <summary>
        /// 为自动生成分析表而添加的抽象方法。代表(s1, g1, ...)中的数字。
        /// </summary>
        /// <returns></returns>
        public abstract string ActionParam();

    }

    /// <summary>
    /// 移进
    /// </summary>
    public class LR1ShiftInAction : LRParsingAction
    {

        public override string ActionParam()
        {
            return NextStateId.ToString();
        }

        public int NextStateId { get; private set; }

        public LR1ShiftInAction(int nextStateId)
        {
            this.NextStateId = nextStateId;
        }

        public override int Execute(ParsingContext context)
        {
            var newNode = new SyntaxTree();
            newNode.MappedTokenStartIndex = context.CurrentTokenIndex;
            newNode.MappedTokenLength = 1;
            newNode.MappedTotalTokenList = context.TokenList;
            newNode.NodeType = context.CurrentNodeType();
            context.TreeStack.Push(newNode);
            context.StateIdStack.Push(this.NextStateId);

            return context.CurrentTokenIndex + 1;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write("Shift in: next state id: [{0}]", NextStateId);
        }

        public override string ToString()
        {
            return string.Format("Shift in: next state id: [{0}]", NextStateId);
        }
    }

    /// <summary>
    /// 归约
    /// </summary>
    public class LR1ReducitonAction : LRParsingAction
    {

        public override string ActionParam()
        {
            return RegulationId.ToString();
        }

        //TODO: 我定义的规则：Id = index + 1
        /// <summary>
        /// 根据哪个规则进行归约？
        /// </summary>
        public int RegulationId { get; set; }

        public LR1ReducitonAction(int regulationId)
        {
            this.RegulationId = regulationId;
        }

        public override int Execute(ParsingContext context)
        {
            Regulation regulation = context.Grammar[RegulationId - 1];
            var newNode = new SyntaxTree();
            int tokenCount = 0;
            int count = regulation.RightPart.Count();
            for (int i = 0; i < count; i++)
            {
                var item = context.TreeStack.Pop();
                context.StateIdStack.Pop();//只弹出，不再使用。
                newNode.Children.Insert(0, item);
                item.Parent = newNode;
                tokenCount += item.MappedTokenLength;
                if (i == 0)
                { newNode.MappedTokenStartIndex = item.MappedTokenStartIndex; }
            }
            newNode.MappedTokenLength = tokenCount;
            newNode.MappedTotalTokenList = context.TokenList;
            newNode.NodeType = regulation.Left;
            context.TreeStack.Push(newNode);
            int stateId = context.StateIdStack.Peek();
            LRParsingAction gotoAction = context.ParsingMap.GetAction(stateId, regulation.Left);
            gotoAction.Execute(context);

            return context.CurrentTokenIndex;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write("Reduction regulation Id: {0}", RegulationId);
        }

        public override string ToString()
        {
            return string.Format("Reduction regulation Id: {0}", RegulationId);
        }
    }

    /// <summary>
    /// 转到
    /// </summary>
    public class LR1GotoAction : LRParsingAction
    {

        public override string ActionParam()
        {
            return GoToStateId.ToString();
        }

        public int GoToStateId { get; set; }

        public LR1GotoAction(int gotoStateId)
        {
            this.GoToStateId = gotoStateId;
        }
        public override int Execute(ParsingContext context)
        {
            context.StateIdStack.Push(GoToStateId);

            return context.CurrentTokenIndex;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write("Goto state id: [{0}]", GoToStateId);
        }

        public override string ToString()
        {
            return string.Format("Goto state id: [{0}]", GoToStateId);
        }
    }

    /// <summary>
    /// 接受（分析完毕）
    /// </summary>
    public class LR1AceptAction : LRParsingAction
    {
        public override string ActionParam()
        {
            return string.Empty;
        }

        public override int Execute(ParsingContext context)
        {
            return context.CurrentTokenIndex + 1;
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write("Accept");
        }

        public override string ToString()
        {
            return string.Format("Accept");
        }
    }

    /// <summary>
    /// 错误（语法错误）
    /// </summary>
    public class LR1DefaultErrorAction
        : LRParsingAction
    {

        public override string ActionParam()
        {
            return string.Empty;
        }

        public override int Execute(ParsingContext context)
        {
            throw new NotImplementedException();
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write("Error");
        }

        public override string ToString()
        {
            return string.Format("Error");
        }
    }
}
