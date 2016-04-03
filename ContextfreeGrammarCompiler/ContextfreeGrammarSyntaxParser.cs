using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LALR1Compiler;

namespace ContextfreeGrammarCompiler
{
    public class ContextfreeGrammarSyntaxParser : LRSyntaxParser
    {

        /// <summary>
        /// LR1分析表。
        /// </summary>
        protected static LRParsingMap parsingMap;

        /// <summary>
        /// 规则列表。即文法。
        /// </summary>
        protected static RegulationList grammar;

        // 非叶结点
        private static readonly TreeNodeType __Grammar = new TreeNodeType("__Grammar", "Grammar", "<Grammar>");
        private static readonly TreeNodeType __ProductionList = new TreeNodeType("__ProductionList", "ProductionList", "<ProductionList>");
        private static readonly TreeNodeType __Production = new TreeNodeType("__Production", "Production", "<Production>");
        private static readonly TreeNodeType __Canditate = new TreeNodeType("__Canditate", "Canditate", "<Canditate>");
        private static readonly TreeNodeType __VList = new TreeNodeType("__VList", "VList", "<VList>");
        private static readonly TreeNodeType __RightPartList = new TreeNodeType("__RightPartList", "RightPartList", "<RightPartList>");
        private static readonly TreeNodeType __V = new TreeNodeType("__V", "V", "<V>");
        private static readonly TreeNodeType __Vn = new TreeNodeType("__Vn", "Vn", "<Vn>");
        private static readonly TreeNodeType __Vt = new TreeNodeType("__Vt", "Vt", "<Vt>");
        // 叶结点
        private static readonly TreeNodeType __colon_colon_equalLeave__ = new TreeNodeType("__colon_colon_equalLeave__", "::=", "\"::=\"");
        private static readonly TreeNodeType __semicolonLeave__ = new TreeNodeType("__semicolonLeave__", ";", "\";\"");
        private static readonly TreeNodeType __vertical_barLeave__ = new TreeNodeType("__vertical_barLeave__", "|", "\"|\"");
        private static readonly TreeNodeType __left_angleLeave__ = new TreeNodeType("__left_angleLeave__", "<", "\"<\"");
        private static readonly TreeNodeType identifierLeave__ = new TreeNodeType("identifierLeave__", "{identifier}", "identifier");
        private static readonly TreeNodeType __right_angleLeave__ = new TreeNodeType("__right_angleLeave__", ">", "\">\"");
        private static readonly TreeNodeType __nullLeave__ = new TreeNodeType("__nullLeave__", "null", "\"null\"");
        private static readonly TreeNodeType __identifierLeave__ = new TreeNodeType("__identifierLeave__", "identifier", "\"identifier\"");
        private static readonly TreeNodeType __numberLeave__ = new TreeNodeType("__numberLeave__", "number", "\"number\"");
        private static readonly TreeNodeType __constStringLeave__ = new TreeNodeType("__constStringLeave__", "constString", "\"constString\"");
        private static readonly TreeNodeType constStringLeave__ = new TreeNodeType("constStringLeave__", "{constString}", "constString");
        //
        private static readonly TreeNodeType end_of_token_listLeave__ = TreeNodeType.endOfTokenListNode;

        protected override RegulationList GetGrammar()
        {
            if (grammar != null) { return grammar; }

            var list = new RegulationList();
            // <Grammar> ::= <Production> <ProductionList> ;
            list.Add(new Regulation(__Grammar, __Production, __ProductionList));
            // <ProductionList> ::= <Production> <RegulationList> | null ;
            list.Add(new Regulation(__ProductionList, __Production, __ProductionList));
            list.Add(new Regulation(__ProductionList));
            // <Production> ::= <Vn> "::=" <Canditate> <RightPartList> ";" ;
            list.Add(new Regulation(__Production, __Vn, __colon_colon_equalLeave__, __Canditate, __RightPartList, __semicolonLeave__));
            // <Canditate> ::= <V> <VList> ;
            list.Add(new Regulation(__Canditate, __V, __VList));
            // <VList> ::= <V> <VList> | null ;
            list.Add(new Regulation(__VList, __V, __VList));
            list.Add(new Regulation(__VList));
            // <RightPartList> ::= "|" <Canditate> <RightPartList> | null ;
            list.Add(new Regulation(__RightPartList, __vertical_barLeave__, __Canditate, __RightPartList));
            list.Add(new Regulation(__RightPartList));
            // <V> ::= <Vn> | <Vt> ;
            list.Add(new Regulation(__V, __Vn));
            list.Add(new Regulation(__V, __Vt));
            // <Vn> ::= "<" identifier ">" ;
            list.Add(new Regulation(__Vn, __left_angleLeave__, identifierLeave__, __right_angleLeave__));
            // <Vt> ::= "null" | "identifier" | "number" | "constString" | constString ;
            list.Add(new Regulation(__Vt, __nullLeave__));
            list.Add(new Regulation(__Vt, __identifierLeave__));
            list.Add(new Regulation(__Vt, __numberLeave__));
            list.Add(new Regulation(__Vt, __constStringLeave__));
            list.Add(new Regulation(__Vt, constStringLeave__));

            grammar = list;

            return grammar;
        }

        protected override LRParsingMap GetParsingMap()
        {
            if (parsingMap != null) { return parsingMap; }

            var map = new LRParsingMap();
            map.SetAction(1, __Grammar, new LR1GotoAction(2));
            map.SetAction(1, __Production, new LR1GotoAction(3));
            map.SetAction(1, __Vn, new LR1GotoAction(4));
            map.SetAction(1, __left_angleLeave__, new LR1ShiftInAction(5));
            map.SetAction(3, __ProductionList, new LR1GotoAction(6));
            map.SetAction(3, __Production, new LR1GotoAction(7));
            map.SetAction(3, __Vn, new LR1GotoAction(4));
            map.SetAction(3, __left_angleLeave__, new LR1ShiftInAction(5));
            map.SetAction(4, __colon_colon_equalLeave__, new LR1ShiftInAction(8));
            map.SetAction(5, identifierLeave__, new LR1ShiftInAction(9));
            map.SetAction(7, __ProductionList, new LR1GotoAction(10));
            map.SetAction(7, __Production, new LR1GotoAction(7));
            map.SetAction(7, __Vn, new LR1GotoAction(4));
            map.SetAction(7, __left_angleLeave__, new LR1ShiftInAction(5));
            map.SetAction(8, __Canditate, new LR1GotoAction(11));
            map.SetAction(8, __V, new LR1GotoAction(12));
            map.SetAction(8, __Vn, new LR1GotoAction(13));
            map.SetAction(8, __Vt, new LR1GotoAction(14));
            map.SetAction(8, __left_angleLeave__, new LR1ShiftInAction(5));
            map.SetAction(8, __nullLeave__, new LR1ShiftInAction(15));
            map.SetAction(8, __identifierLeave__, new LR1ShiftInAction(16));
            map.SetAction(8, __numberLeave__, new LR1ShiftInAction(17));
            map.SetAction(8, __constStringLeave__, new LR1ShiftInAction(18));
            map.SetAction(8, constStringLeave__, new LR1ShiftInAction(19));
            map.SetAction(9, __right_angleLeave__, new LR1ShiftInAction(20));
            map.SetAction(11, __RightPartList, new LR1GotoAction(21));
            map.SetAction(11, __vertical_barLeave__, new LR1ShiftInAction(22));
            map.SetAction(12, __VList, new LR1GotoAction(23));
            map.SetAction(12, __V, new LR1GotoAction(24));
            map.SetAction(12, __Vn, new LR1GotoAction(13));
            map.SetAction(12, __Vt, new LR1GotoAction(14));
            map.SetAction(12, __left_angleLeave__, new LR1ShiftInAction(5));
            map.SetAction(12, __nullLeave__, new LR1ShiftInAction(15));
            map.SetAction(12, __identifierLeave__, new LR1ShiftInAction(16));
            map.SetAction(12, __numberLeave__, new LR1ShiftInAction(17));
            map.SetAction(12, __constStringLeave__, new LR1ShiftInAction(18));
            map.SetAction(12, constStringLeave__, new LR1ShiftInAction(19));
            map.SetAction(21, __semicolonLeave__, new LR1ShiftInAction(25));
            map.SetAction(22, __Canditate, new LR1GotoAction(26));
            map.SetAction(22, __V, new LR1GotoAction(12));
            map.SetAction(22, __Vn, new LR1GotoAction(13));
            map.SetAction(22, __Vt, new LR1GotoAction(14));
            map.SetAction(22, __left_angleLeave__, new LR1ShiftInAction(5));
            map.SetAction(22, __nullLeave__, new LR1ShiftInAction(15));
            map.SetAction(22, __identifierLeave__, new LR1ShiftInAction(16));
            map.SetAction(22, __numberLeave__, new LR1ShiftInAction(17));
            map.SetAction(22, __constStringLeave__, new LR1ShiftInAction(18));
            map.SetAction(22, constStringLeave__, new LR1ShiftInAction(19));
            map.SetAction(24, __VList, new LR1GotoAction(27));
            map.SetAction(24, __V, new LR1GotoAction(24));
            map.SetAction(24, __Vn, new LR1GotoAction(13));
            map.SetAction(24, __Vt, new LR1GotoAction(14));
            map.SetAction(24, __left_angleLeave__, new LR1ShiftInAction(5));
            map.SetAction(24, __nullLeave__, new LR1ShiftInAction(15));
            map.SetAction(24, __identifierLeave__, new LR1ShiftInAction(16));
            map.SetAction(24, __numberLeave__, new LR1ShiftInAction(17));
            map.SetAction(24, __constStringLeave__, new LR1ShiftInAction(18));
            map.SetAction(24, constStringLeave__, new LR1ShiftInAction(19));
            map.SetAction(26, __RightPartList, new LR1GotoAction(28));
            map.SetAction(26, __vertical_barLeave__, new LR1ShiftInAction(22));
            map.SetAction(2, end_of_token_listLeave__, new LR1AceptAction());
            map.SetAction(3, end_of_token_listLeave__, new LR1ReducitonAction(3));
            map.SetAction(6, end_of_token_listLeave__, new LR1ReducitonAction(1));
            map.SetAction(7, end_of_token_listLeave__, new LR1ReducitonAction(3));
            map.SetAction(10, end_of_token_listLeave__, new LR1ReducitonAction(2));
            map.SetAction(11, __semicolonLeave__, new LR1ReducitonAction(9));
            map.SetAction(12, __vertical_barLeave__, new LR1ReducitonAction(7));
            map.SetAction(12, __semicolonLeave__, new LR1ReducitonAction(7));
            map.SetAction(13, __left_angleLeave__, new LR1ReducitonAction(10));
            map.SetAction(13, __nullLeave__, new LR1ReducitonAction(10));
            map.SetAction(13, __identifierLeave__, new LR1ReducitonAction(10));
            map.SetAction(13, __numberLeave__, new LR1ReducitonAction(10));
            map.SetAction(13, __constStringLeave__, new LR1ReducitonAction(10));
            map.SetAction(13, constStringLeave__, new LR1ReducitonAction(10));
            map.SetAction(13, __vertical_barLeave__, new LR1ReducitonAction(10));
            map.SetAction(13, __semicolonLeave__, new LR1ReducitonAction(10));
            map.SetAction(14, __left_angleLeave__, new LR1ReducitonAction(11));
            map.SetAction(14, __nullLeave__, new LR1ReducitonAction(11));
            map.SetAction(14, __identifierLeave__, new LR1ReducitonAction(11));
            map.SetAction(14, __numberLeave__, new LR1ReducitonAction(11));
            map.SetAction(14, __constStringLeave__, new LR1ReducitonAction(11));
            map.SetAction(14, constStringLeave__, new LR1ReducitonAction(11));
            map.SetAction(14, __vertical_barLeave__, new LR1ReducitonAction(11));
            map.SetAction(14, __semicolonLeave__, new LR1ReducitonAction(11));
            map.SetAction(15, __left_angleLeave__, new LR1ReducitonAction(13));
            map.SetAction(15, __nullLeave__, new LR1ReducitonAction(13));
            map.SetAction(15, __identifierLeave__, new LR1ReducitonAction(13));
            map.SetAction(15, __numberLeave__, new LR1ReducitonAction(13));
            map.SetAction(15, __constStringLeave__, new LR1ReducitonAction(13));
            map.SetAction(15, constStringLeave__, new LR1ReducitonAction(13));
            map.SetAction(15, __vertical_barLeave__, new LR1ReducitonAction(13));
            map.SetAction(15, __semicolonLeave__, new LR1ReducitonAction(13));
            map.SetAction(16, __left_angleLeave__, new LR1ReducitonAction(14));
            map.SetAction(16, __nullLeave__, new LR1ReducitonAction(14));
            map.SetAction(16, __identifierLeave__, new LR1ReducitonAction(14));
            map.SetAction(16, __numberLeave__, new LR1ReducitonAction(14));
            map.SetAction(16, __constStringLeave__, new LR1ReducitonAction(14));
            map.SetAction(16, constStringLeave__, new LR1ReducitonAction(14));
            map.SetAction(16, __vertical_barLeave__, new LR1ReducitonAction(14));
            map.SetAction(16, __semicolonLeave__, new LR1ReducitonAction(14));
            map.SetAction(17, __left_angleLeave__, new LR1ReducitonAction(15));
            map.SetAction(17, __nullLeave__, new LR1ReducitonAction(15));
            map.SetAction(17, __identifierLeave__, new LR1ReducitonAction(15));
            map.SetAction(17, __numberLeave__, new LR1ReducitonAction(15));
            map.SetAction(17, __constStringLeave__, new LR1ReducitonAction(15));
            map.SetAction(17, constStringLeave__, new LR1ReducitonAction(15));
            map.SetAction(17, __vertical_barLeave__, new LR1ReducitonAction(15));
            map.SetAction(17, __semicolonLeave__, new LR1ReducitonAction(15));
            map.SetAction(18, __left_angleLeave__, new LR1ReducitonAction(16));
            map.SetAction(18, __nullLeave__, new LR1ReducitonAction(16));
            map.SetAction(18, __identifierLeave__, new LR1ReducitonAction(16));
            map.SetAction(18, __numberLeave__, new LR1ReducitonAction(16));
            map.SetAction(18, __constStringLeave__, new LR1ReducitonAction(16));
            map.SetAction(18, constStringLeave__, new LR1ReducitonAction(16));
            map.SetAction(18, __vertical_barLeave__, new LR1ReducitonAction(16));
            map.SetAction(18, __semicolonLeave__, new LR1ReducitonAction(16));
            map.SetAction(19, __left_angleLeave__, new LR1ReducitonAction(17));
            map.SetAction(19, __nullLeave__, new LR1ReducitonAction(17));
            map.SetAction(19, __identifierLeave__, new LR1ReducitonAction(17));
            map.SetAction(19, __numberLeave__, new LR1ReducitonAction(17));
            map.SetAction(19, __constStringLeave__, new LR1ReducitonAction(17));
            map.SetAction(19, constStringLeave__, new LR1ReducitonAction(17));
            map.SetAction(19, __vertical_barLeave__, new LR1ReducitonAction(17));
            map.SetAction(19, __semicolonLeave__, new LR1ReducitonAction(17));
            map.SetAction(20, __colon_colon_equalLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, __left_angleLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, __nullLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, __identifierLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, __numberLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, __constStringLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, constStringLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, __vertical_barLeave__, new LR1ReducitonAction(12));
            map.SetAction(20, __semicolonLeave__, new LR1ReducitonAction(12));
            map.SetAction(23, __vertical_barLeave__, new LR1ReducitonAction(5));
            map.SetAction(23, __semicolonLeave__, new LR1ReducitonAction(5));
            map.SetAction(24, __vertical_barLeave__, new LR1ReducitonAction(7));
            map.SetAction(24, __semicolonLeave__, new LR1ReducitonAction(7));
            map.SetAction(25, __left_angleLeave__, new LR1ReducitonAction(4));
            map.SetAction(25, end_of_token_listLeave__, new LR1ReducitonAction(4));
            map.SetAction(26, __semicolonLeave__, new LR1ReducitonAction(9));
            map.SetAction(27, __vertical_barLeave__, new LR1ReducitonAction(6));
            map.SetAction(27, __semicolonLeave__, new LR1ReducitonAction(6));
            map.SetAction(28, __semicolonLeave__, new LR1ReducitonAction(8));

            parsingMap = map;
            return parsingMap;
        }
    }
}
