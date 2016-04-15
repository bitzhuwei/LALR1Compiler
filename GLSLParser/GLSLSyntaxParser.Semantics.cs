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
       static readonly Dictionary<LRParsingAction, Action<ParsingContext>> dict =
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

        static GLSLSyntaxParser()
        {
            // 将语义动作绑定的到state上。
            dict.Add(new LR1ShiftInAction(172), state172_struct_specifier);
        }

		// State [172]:
		// <struct_specifier> ::= "struct" identifier . "{" <struct_declaration_list> "}" ;, identifier "," ")" "(" ";" "["
		/// <summary>
        /// State [172]
		/// </summary>
		/// <param name="context"></param>
        static void state172_struct_specifier(ParsingContext context)
        {
            SyntaxTree tree = context.TreeStack.Peek();
            string name = tree.NodeType.Content;
            context.UserDefinedTypeTable.TryInsert(new UserDefinedType(name));
        }
    }
}
