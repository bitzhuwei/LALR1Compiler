using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// LL1语法分析器基类
    /// </summary>
    public abstract partial class LRSyntaxParser
    {

        protected abstract LRParsingMap GetParsingMap();

        protected abstract RegulationList GetGrammar();

#if DEBUG
        static bool print = false;
#endif

        bool inParsingStep = false;
        ParsingStepContext parsingStepContext;

        internal void StartParsing(UserDefinedTypeCollection userDefinedTypeTable)
        {
            if (!inParsingStep)
            {
                LRParsingMap parsingMap = GetParsingMap();
                RegulationList grammar = GetGrammar();
                var tokenTypeConvertor = new TokenType2TreeNodeType();
                parsingStepContext = new ParsingStepContext(
                    grammar, parsingMap, tokenTypeConvertor, userDefinedTypeTable);
                inParsingStep = true;
            }
        }

        internal SyntaxTree StopParsing()
        {
            SyntaxTree result = null;
            if (inParsingStep)
            {
                result = ParseStep(Token.endOfTokenList);
                parsingStepContext.TokenList.RemoveAt(parsingStepContext.TokenList.Count - 1);
                parsingStepContext = null;
                inParsingStep = false;
            }

            return result;
        }

        // 在子类里实现
        //protected static Dictionary<LRParsingAction, Action<ParsingStepContext>> semanticsDict =
        //    new Dictionary<LRParsingAction, Action<ParsingStepContext>>();

        /// <summary>
        /// 获取归约动作对应的语义动作。
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual Action<ParsingStepContext> GetSemanticAction(LR1ReducitonAction action)
        {
            return null;
        }

        internal SyntaxTree ParseStep(Token token)
        {
            if (!inParsingStep) { throw new Exception("Must invoke this.StartParsing() first!"); }

            parsingStepContext.AddToken(token);

            // 语法分析
            LRParsingAction action = null;
            while (parsingStepContext.CurrentTokenIndex < parsingStepContext.TokenList.Count)
            {
                TreeNodeType nodeType = parsingStepContext.CurrentNodeType();
                int stateId = parsingStepContext.StateIdStack.Peek();
                action = parsingStepContext.ParsingMap.GetAction(stateId, nodeType);
                int currentTokenIndex = action.Execute(parsingStepContext);
                parsingStepContext.CurrentTokenIndex = currentTokenIndex;
            }

            // 语义分析
            var reduction = action as LR1ReducitonAction;
            if (reduction != null)
            {
                Action<ParsingStepContext> semanticAction = GetSemanticAction(reduction);
                if (semanticAction != null)
                {
                    semanticAction(parsingStepContext);
                }
            }

            if (parsingStepContext.TreeStack.Count > 0)
            {
                return parsingStepContext.TreeStack.Peek();
            }
            else
            {
                return new SyntaxTree();
            }
        }

        public SyntaxTree Parse(TokenList tokenList)
        {
            LRParsingMap parsingMap = GetParsingMap();
            RegulationList grammar = GetGrammar();

            //TODO:这个convertor以后是可以配置的。
            var tokenTypeConvertor = new TokenType2TreeNodeType();
            var context = new ParsingContext(tokenList, grammar, parsingMap, tokenTypeConvertor);
            while (context.CurrentTokenIndex < context.TokenList.Count + 1)
            {
                PrintParsingProgress(context);
                TreeNodeType nodeType = context.CurrentNodeType();
                int stateId = context.StateIdStack.Peek();
                LRParsingAction action = parsingMap.GetAction(stateId, nodeType);
                int currentTokenIndex = action.Execute(context);
                context.CurrentTokenIndex = currentTokenIndex;
            }

            PrintLastState(context);

            if (context.TreeStack.Count > 0)
            {
                return context.TreeStack.Peek();
            }
            else
            {
                return new SyntaxTree();
            }
        }

        private void PrintLastState(ParsingContext context)
        {
#if DEBUG
            if (!print) { return; } // 调试时快速取消print

            Debug.WriteLine("=======================");
            Debug.WriteLine("***********************");
            {
                Debug.WriteLine("Last Stack:");
                Debug.Write("    ");
                var stateIdArray = context.StateIdStack.ToArray();
                var treeArray = context.TreeStack.ToArray();
                for (int i = stateIdArray.Length - 1; i > 0; i--)
                {
                    Debug.Write(stateIdArray[i]);
                    Debug.Write('['); Debug.Write(treeArray[i - 1].NodeType.Nickname); Debug.Write(']');
                }
                Debug.Write(stateIdArray[0]); Debug.Write(' ');
                Debug.WriteLine("");
            }
            {
                Debug.WriteLine("Last token list:");
                Debug.Write("    ");
                for (int i = context.CurrentTokenIndex; i < context.TokenList.Count; i++)
                {
                    Debug.Write(context.TokenList[i].TokenType.Content); Debug.Write(' ');
                }
                Debug.WriteLine("");
            }
            {
                Debug.WriteLine("Last action:");
                TreeNodeType nodeType = context.CurrentNodeType();
                int stateId = context.StateIdStack.Peek();
                LRParsingAction action = context.ParsingMap.GetAction(stateId, nodeType);
                Debug.Write("    "); Debug.WriteLine(action);
            }
            {
                Debug.WriteLine("Last syntax tree:");
                Debug.WriteLine(context.TreeStack.Peek());
                foreach (var item in context.TreeStack)
                {
                    item.Parent = null;
                }
            }
            Debug.WriteLine("***********************");
            Debug.WriteLine("=======================");

#endif
        }

        private void PrintParsingProgress(ParsingContext context)
        {
#if DEBUG
            if (!print) { return; } // 调试时快速取消print

            Debug.WriteLine("=======================");
            {
                Debug.WriteLine("Current Stack:");
                Debug.Write("    ");
                var stateIdArray = context.StateIdStack.ToArray();
                var treeArray = context.TreeStack.ToArray();
                for (int i = stateIdArray.Length - 1; i > 0; i--)
                {
                    Debug.Write(stateIdArray[i]);
                    Debug.Write('['); Debug.Write(treeArray[i - 1].NodeType.Nickname); Debug.Write(']');
                }
                Debug.Write(stateIdArray[0]); Debug.Write(' ');
                Debug.WriteLine("");
            }
            {
                Debug.WriteLine("Current token list:");
                Debug.Write("    ");
                for (int i = context.CurrentTokenIndex; i < context.TokenList.Count; i++)
                {
                    Debug.Write(context.TokenList[i].TokenType.Content); Debug.Write(' ');
                }
                Debug.WriteLine("");
            }
            {
                Debug.WriteLine("Next action:");
                TreeNodeType nodeType = context.CurrentNodeType();
                int stateId = context.StateIdStack.Peek();
                LRParsingAction action = context.ParsingMap.GetAction(stateId, nodeType);
                Debug.Write("    "); Debug.WriteLine(action);
            }
            {
                Debug.WriteLine("Current syntax tree:");
                SyntaxTree virtualParent = new SyntaxTree();
                virtualParent.NodeType = new TreeNodeType("VirtualParent", "", "Virtual Parent");
                var treeArray = context.TreeStack.ToArray();
                for (int i = treeArray.Length - 1; i >= 0; i--)
                {
                    virtualParent.Children.Add(treeArray[i]);
                    treeArray[i].Parent = virtualParent;
                }
                Debug.WriteLine(virtualParent);
                foreach (var item in context.TreeStack)
                {
                    item.Parent = null;
                }
            }
            Debug.WriteLine("=======================");

#endif
        }

    }

    public class ParsingStepContext : ParsingContext
    {
        public UserDefinedTypeCollection UserDefinedTypeTable { get; private set; }

        public void AddToken(Token token)
        {
            this.TokenList.Add(token);
        }

        public ParsingStepContext(
            RegulationList grammar,
            LRParsingMap parsingMap,
            TokenType2TreeNodeType tokenTypeConvertor,
            UserDefinedTypeCollection userDefinedTypeTable)
            : base(new TokenList(), grammar, parsingMap, tokenTypeConvertor)
        {
            this.UserDefinedTypeTable = userDefinedTypeTable;
        }

    }

    public class ParsingContext
    {
        public int CurrentTokenIndex { get; set; }

        public Stack<int> StateIdStack { get; private set; }

        public Stack<SyntaxTree> TreeStack { get; private set; }

        public TokenList TokenList { get; private set; }

        /// <summary>
        /// 规则列表。等价于文法。
        /// </summary>
        public RegulationList Grammar { get; private set; }

        /// <summary>
        /// LR1分析表。
        /// </summary>
        public LRParsingMap ParsingMap { get; private set; }

        public TokenType2TreeNodeType TokenTypeConvertor { get; private set; }


        public TreeNodeType CurrentNodeType()
        {
            TokenType tokenType;
            if (this.CurrentTokenIndex >= this.TokenList.Count)
            {
                tokenType = TokenType.endOfTokenList;
            }
            else
            {
                tokenType = this.TokenList[this.CurrentTokenIndex].TokenType;
            }

            return TokenTypeConvertor.GetNodeType(tokenType);
        }

        public ParsingContext(TokenList tokenList,
            RegulationList grammar,
            LRParsingMap parsingMap,
            TokenType2TreeNodeType tokenTypeConvertor)
        {
            this.StateIdStack = new Stack<int>();
            this.StateIdStack.Push(1);
            this.TreeStack = new Stack<SyntaxTree>();

            this.TokenList = tokenList;
            this.Grammar = grammar;
            this.ParsingMap = parsingMap;
            this.TokenTypeConvertor = tokenTypeConvertor;
        }

    }

    /// <summary>
    /// 由单词类型获取对应的V类型
    /// </summary>
    public class TokenType2TreeNodeType
    {
        public virtual TreeNodeType GetNodeType(TokenType tokenType)
        {
            //TODO:“Leave__”后缀，这是个自定义规则
            string strTreeNodeType = tokenType.Type + "Leave__";
            string content = tokenType.Content;
            TreeNodeType result = new TreeNodeType(strTreeNodeType, content, tokenType.Nickname);

            return result;
        }
    }

    public class TreeNodeType2TokenType
    {
        public virtual TokenType GetTokenType(TreeNodeType treeNodeType)
        {
            //TODO:“Leave__”后缀，这是个自定义规则
            string strTokenType = treeNodeType.Type.Substring(0, treeNodeType.Type.Length - "Leave__".Length);
            string content = treeNodeType.Content;
            TokenType result = new TokenType(strTokenType, treeNodeType.Content, treeNodeType.Nickname);

            return result;
        }
    }
}
