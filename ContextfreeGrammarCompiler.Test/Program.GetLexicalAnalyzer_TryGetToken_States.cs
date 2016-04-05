using LALR1Compiler;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    static partial class Program
    {
        /// <summary>
        /// 为生成词法分析器，获取该文法的所有词法状态。
        /// </summary>
        /// <param name="grammar"></param>
        /// <returns></returns>
        static List<LexiState> GetLexiStateList(this RegulationList grammar)
        {
            var result = new List<LexiState>();
            List<TreeNodeType> nodeList = grammar.GetAllTreeNodeLeaveTypes();
            foreach (var node in nodeList)
            {
                if (node.Type == "identifierLeave__")// ContextfreeGrammar的关键字
                {
                    TryInsertGetIdentifier(result);
                }
                else if (node.Type == "numberLeave__")// ContextfreeGrammar的关键字
                {
                    TryInsertGetNumber(result);
                }
                else if (node.Type == "constStringLeave__")// ContextfreeGrammar的关键字
                {
                    TryInserteGetConstString(result);
                }
                else if (node.Type == "charLeave__")// ContextfreeGrammar的关键字（暂未支持）
                {
                    TryInserteGetChar(result);
                }
                else
                {
                    CodeGetRegularToken(result, node);
                }
            }

            return result;
        }

        private static void CodeGetRegularToken(List<LexiState> result, TreeNodeType node)
        {
            string content = node.Content;
            SourceCodeCharType charType = content[0].GetCharType();
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(charType))
                {
                    state.GetTokenList.TryInsert(new CodeGetToken(node));
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(charType);
                state.GetTokenList.TryInsert(new CodeGetToken(node));
                result.Add(state);
            }
        }

        private static void TryInserteGetChar(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.Quotation))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.Quotation);
                state.GetTokenList.TryInsert(new CodeGetChar());
                result.Add(state);
            }
        }

        private static void TryInserteGetConstString(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.DoubleQuotation))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.DoubleQuotation);
                state.GetTokenList.TryInsert(new CodeGetConstString());
                result.Add(state);
            }
        }

        private static void TryInsertGetNumber(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.Number))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.Number);
                state.GetTokenList.TryInsert(new CodeGetNumber());
                result.Add(state);
            }
        }

        private static void TryInsertGetIdentifier(List<LexiState> result)
        {
            bool exists = false;
            foreach (var state in result)
            {
                if (state.Contains(SourceCodeCharType.Letter))
                {
                    exists = true; break;
                }
            }
            if (!exists)
            {
                var state = new LexiState();
                state.CharTypeList.Add(SourceCodeCharType.Letter);
                state.CharTypeList.Add(SourceCodeCharType.UnderLine);
                state.GetTokenList.TryInsert(new CodeGetIdentifier());
                result.Add(state);
            }
        }
    }



}
