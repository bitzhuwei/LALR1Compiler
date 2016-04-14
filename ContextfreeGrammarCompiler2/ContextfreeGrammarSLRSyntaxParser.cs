namespace ContextfreeGrammarCompiler2
{
    using System;
    using System.Collections.Generic;
    using LALR1Compiler;
    
    
    public class ContextfreeGrammarSLRSyntaxParser : LALR1Compiler.LRSyntaxParser
    {
        
        private static LALR1Compiler.LRParsingMap parsingMap;
        
        private static LALR1Compiler.RegulationList grammar;
        
        private static LALR1Compiler.TreeNodeType NODE__Grammar = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__Grammar, "Grammar", "<Grammar>");
        
        private static LALR1Compiler.TreeNodeType NODE__ProductionList = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__ProductionList, "ProductionList", "<ProductionList>");
        
        private static LALR1Compiler.TreeNodeType NODE__Production = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__Production, "Production", "<Production>");
        
        private static LALR1Compiler.TreeNodeType NODE__Vn = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__Vn, "Vn", "<Vn>");
        
        private static LALR1Compiler.TreeNodeType NODE__coloncolonequalLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__coloncolonequalLeave__, "::=", "\"::=\"");
        
        private static LALR1Compiler.TreeNodeType NODE__Canditate = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__Canditate, "Canditate", "<Canditate>");
        
        private static LALR1Compiler.TreeNodeType NODE__RightPartList = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__RightPartList, "RightPartList", "<RightPartList>");
        
        private static LALR1Compiler.TreeNodeType NODE__semicolonLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__semicolonLeave__, ";", "\";\"");
        
        private static LALR1Compiler.TreeNodeType NODE__VList = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__VList, "VList", "<VList>");
        
        private static LALR1Compiler.TreeNodeType NODE__V = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__V, "V", "<V>");
        
        private static LALR1Compiler.TreeNodeType NODE__vertical_barLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__vertical_barLeave__, "|", "\"|\"");
        
        private static LALR1Compiler.TreeNodeType NODE__Vt = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__Vt, "Vt", "<Vt>");
        
        private static LALR1Compiler.TreeNodeType NODE__left_angleLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__left_angleLeave__, "<", "\"<\"");
        
        private static LALR1Compiler.TreeNodeType NODEidentifierLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODEidentifierLeave__, "{identifier}", "identifier");
        
        private static LALR1Compiler.TreeNodeType NODE__right_angleLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__right_angleLeave__, ">", "\">\"");
        
        private static LALR1Compiler.TreeNodeType NODE__nullLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__nullLeave__, "null", "\"null\"");
        
        private static LALR1Compiler.TreeNodeType NODE__identifierLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__identifierLeave__, "identifier", "\"identifier\"");
        
        private static LALR1Compiler.TreeNodeType NODE__numberLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__EnumberLeave__, "number", "\"number\"");
        
        private static LALR1Compiler.TreeNodeType NODE__constStringLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__constStringLeave__, "constString", "\"constString\"");
        
        private static LALR1Compiler.TreeNodeType NODE__userDefinedTypeLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__userDefinedTypeLeave__, "{userDefinedType}", "\"userDefinedType\"");
        
        private static LALR1Compiler.TreeNodeType NODEconstStringLeave__ = new LALR1Compiler.TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODEconstStringLeave__, "{constString}", "constString");
        
        private static LALR1Compiler.TreeNodeType NODEend_of_token_listLeave__ = LALR1Compiler.TreeNodeType.endOfTokenListNode;
        
        protected override LALR1Compiler.RegulationList GetGrammar()
        {
            if ((grammar != null))
            {
                return grammar;
            }
            LALR1Compiler.RegulationList list = new LALR1Compiler.RegulationList();
            // 1: <Grammar> ::= <ProductionList> <Production> ;
            list.Add(new LALR1Compiler.Regulation(NODE__Grammar, NODE__ProductionList, NODE__Production));
            // 2: <ProductionList> ::= <ProductionList> <Production> ;
            list.Add(new LALR1Compiler.Regulation(NODE__ProductionList, NODE__ProductionList, NODE__Production));
            // 3: <ProductionList> ::= null ;
            list.Add(new LALR1Compiler.Regulation(NODE__ProductionList));
            // 4: <Production> ::= <Vn> "::=" <Canditate> <RightPartList> ";" ;
            list.Add(new LALR1Compiler.Regulation(NODE__Production, NODE__Vn, NODE__coloncolonequalLeave__, NODE__Canditate, NODE__RightPartList, NODE__semicolonLeave__));
            // 5: <Canditate> ::= <VList> <V> ;
            list.Add(new LALR1Compiler.Regulation(NODE__Canditate, NODE__VList, NODE__V));
            // 6: <VList> ::= <VList> <V> ;
            list.Add(new LALR1Compiler.Regulation(NODE__VList, NODE__VList, NODE__V));
            // 7: <VList> ::= null ;
            list.Add(new LALR1Compiler.Regulation(NODE__VList));
            // 8: <RightPartList> ::= "|" <Canditate> <RightPartList> ;
            list.Add(new LALR1Compiler.Regulation(NODE__RightPartList, NODE__vertical_barLeave__, NODE__Canditate, NODE__RightPartList));
            // 9: <RightPartList> ::= null ;
            list.Add(new LALR1Compiler.Regulation(NODE__RightPartList));
            // 10: <V> ::= <Vn> ;
            list.Add(new LALR1Compiler.Regulation(NODE__V, NODE__Vn));
            // 11: <V> ::= <Vt> ;
            list.Add(new LALR1Compiler.Regulation(NODE__V, NODE__Vt));
            // 12: <Vn> ::= "<" identifier ">" ;
            list.Add(new LALR1Compiler.Regulation(NODE__Vn, NODE__left_angleLeave__, NODEidentifierLeave__, NODE__right_angleLeave__));
            // 13: <Vt> ::= "null" ;
            list.Add(new LALR1Compiler.Regulation(NODE__Vt, NODE__nullLeave__));
            // 14: <Vt> ::= "identifier" ;
            list.Add(new LALR1Compiler.Regulation(NODE__Vt, NODE__identifierLeave__));
            // 15: <Vt> ::= "number" ;
            list.Add(new LALR1Compiler.Regulation(NODE__Vt, NODE__numberLeave__));
            // 16: <Vt> ::= "constString" ;
            list.Add(new LALR1Compiler.Regulation(NODE__Vt, NODE__constStringLeave__));
            // 17: <Vt> ::= "userDefinedType" ;
            list.Add(new LALR1Compiler.Regulation(NODE__Vt, NODE__userDefinedTypeLeave__));
            // 18: <Vt> ::= constString ;
            list.Add(new LALR1Compiler.Regulation(NODE__Vt, NODEconstStringLeave__));
            grammar = list;
            return grammar;
        }
        
        // 141 SetAction() items
        protected override LALR1Compiler.LRParsingMap GetParsingMap()
        {
            if ((parsingMap != null))
            {
                return parsingMap;
            }
            LALR1Compiler.LRParsingMap map = new LALR1Compiler.LRParsingMap();
            map.SetAction(2, NODE__Production, new LALR1Compiler.LR1GotoAction(4));
            map.SetAction(11, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ShiftInAction(20));
            map.SetAction(6, NODE__coloncolonequalLeave__, new LALR1Compiler.LR1ShiftInAction(8));
            map.SetAction(11, NODE__identifierLeave__, new LALR1Compiler.LR1ShiftInAction(19));
            map.SetAction(11, NODE__V, new LALR1Compiler.LR1GotoAction(15));
            map.SetAction(1, NODE__Grammar, new LALR1Compiler.LR1GotoAction(3));
            map.SetAction(23, NODE__RightPartList, new LALR1Compiler.LR1GotoAction(25));
            map.SetAction(8, NODE__VList, new LALR1Compiler.LR1GotoAction(11));
            map.SetAction(11, NODE__Vn, new LALR1Compiler.LR1GotoAction(18));
            map.SetAction(10, NODE__vertical_barLeave__, new LALR1Compiler.LR1ShiftInAction(12));
            map.SetAction(7, NODE__right_angleLeave__, new LALR1Compiler.LR1ShiftInAction(9));
            map.SetAction(11, NODE__numberLeave__, new LALR1Compiler.LR1ShiftInAction(16));
            map.SetAction(13, NODE__semicolonLeave__, new LALR1Compiler.LR1ShiftInAction(24));
            map.SetAction(10, NODE__RightPartList, new LALR1Compiler.LR1GotoAction(13));
            map.SetAction(12, NODE__VList, new LALR1Compiler.LR1GotoAction(11));
            map.SetAction(2, NODE__Vn, new LALR1Compiler.LR1GotoAction(6));
            map.SetAction(11, NODE__Vt, new LALR1Compiler.LR1GotoAction(17));
            map.SetAction(2, NODE__left_angleLeave__, new LALR1Compiler.LR1ShiftInAction(5));
            map.SetAction(11, NODEconstStringLeave__, new LALR1Compiler.LR1ShiftInAction(14));
            map.SetAction(11, NODE__left_angleLeave__, new LALR1Compiler.LR1ShiftInAction(5));
            map.SetAction(11, NODE__nullLeave__, new LALR1Compiler.LR1ShiftInAction(22));
            map.SetAction(1, NODE__ProductionList, new LALR1Compiler.LR1GotoAction(2));
            map.SetAction(5, NODEidentifierLeave__, new LALR1Compiler.LR1ShiftInAction(7));
            map.SetAction(23, NODE__vertical_barLeave__, new LALR1Compiler.LR1ShiftInAction(12));
            map.SetAction(11, NODE__constStringLeave__, new LALR1Compiler.LR1ShiftInAction(21));
            map.SetAction(8, NODE__Canditate, new LALR1Compiler.LR1GotoAction(10));
            map.SetAction(12, NODE__Canditate, new LALR1Compiler.LR1GotoAction(23));
            map.SetAction(16, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(16, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(15));
            map.SetAction(10, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(9));
            map.SetAction(24, NODEend_of_token_listLeave__, new LALR1Compiler.LR1ReducitonAction(4));
            map.SetAction(24, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(4));
            map.SetAction(8, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(8, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(8, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(8, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(8, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(8, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(8, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(23, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(9));
            map.SetAction(17, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(17, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(11));
            map.SetAction(1, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(3));
            map.SetAction(15, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(5));
            map.SetAction(15, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(5));
            map.SetAction(15, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(6));
            map.SetAction(15, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(6));
            map.SetAction(15, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(6));
            map.SetAction(15, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(6));
            map.SetAction(15, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(6));
            map.SetAction(15, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(6));
            map.SetAction(15, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(6));
            map.SetAction(19, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(19, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(14));
            map.SetAction(12, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(12, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(12, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(12, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(12, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(12, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(12, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(7));
            map.SetAction(18, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(18, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(10));
            map.SetAction(22, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(22, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(13));
            map.SetAction(21, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(21, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(16));
            map.SetAction(20, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(20, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(17));
            map.SetAction(14, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(14, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(18));
            map.SetAction(9, NODE__coloncolonequalLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__nullLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODEconstStringLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__userDefinedTypeLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__numberLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__constStringLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__identifierLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(9, NODE__vertical_barLeave__, new LALR1Compiler.LR1ReducitonAction(12));
            map.SetAction(4, NODE__left_angleLeave__, new LALR1Compiler.LR1ReducitonAction(2));
            map.SetAction(4, NODEend_of_token_listLeave__, new LALR1Compiler.LR1ReducitonAction(1));
            map.SetAction(25, NODE__semicolonLeave__, new LALR1Compiler.LR1ReducitonAction(8));
            map.SetAction(3, NODEend_of_token_listLeave__, new LALR1Compiler.LR1AceptAction());
            parsingMap = map;
            return parsingMap;
        }
    }
}
