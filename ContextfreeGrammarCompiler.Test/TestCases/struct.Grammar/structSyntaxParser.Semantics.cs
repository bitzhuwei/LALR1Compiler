namespace structCompiler
{
    using System;
    using System.Collections.Generic;
    using LALR1Compiler;


    public partial class structSyntaxParser
    {
        static Dictionary<LRParsingAction, Action<ParsingContext>> dict =
            new Dictionary<LRParsingAction, Action<ParsingContext>>();

        protected override Action<ParsingContext> GetSemanticAction(LRParsingAction parsingAction)
        {
            Action<ParsingContext> semanticAction = null;
            if (dict.TryGetValue(parsingAction, out semanticAction))
            {
                return semanticAction;
            }
            else
            {
                return null;
            }
        }

        static structSyntaxParser()
        {
            dict.Add(new LR1ShiftInAction(9), StructDefinition);
        }

        static void StructDefinition(ParsingContext context)
        {
            SyntaxTree tree = context.TreeStack.Peek();
            string name = tree.NodeType.Content;
            context.UserDefinedTypeTable.TryInsert(new UserDefinedType(name));
        }
    }
}
