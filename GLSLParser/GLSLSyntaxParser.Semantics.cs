namespace GLSLParser
{
    using System;
    using System.Collections.Generic;
    using LALR1Compiler;


    /// <summary>
    /// LALR1 Symtax Parser
    /// </summary>
    public partial class GLSLSyntaxParser
    {
       static Dictionary<LRParsingAction, Action<ParsingStepContext>> dict =
            new Dictionary<LRParsingAction, Action<ParsingStepContext>>();

        protected override Action<ParsingStepContext> GetSemanticAction(LRParsingAction parsingAction)
        {
            Action<ParsingStepContext> semanticAction = null;
            if (dict.TryGetValue(parsingAction, out semanticAction))
            {
                return semanticAction;
            }
            else
            {
                return null;
            }
        }

        static GLSLSyntaxParser()
        {
            dict.Add(new LR1ShiftInAction(172), state172_struct_specifier);
        }

		// State [172]:
		// <struct_specifier> ::= "struct" identifier . "{" <struct_declaration_list> "}" ;, identifier "," ")" "(" ";" "["
		/// <summary>
        /// State [172]
		/// </summary>
		/// <param name="context"></param>
        static void state172_struct_specifier(ParsingStepContext context)
        {
            SyntaxTree tree = context.TreeStack.Peek();
            string name = tree.NodeType.Content;
            context.UserDefinedTypeTable.TryInsert(new UserDefinedType(name));
        }
    }
}
