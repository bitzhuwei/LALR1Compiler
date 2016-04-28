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
        static readonly Dictionary<LR1ReducitonAction, Action<ParsingContext>> dict =
            new Dictionary<LR1ReducitonAction, Action<ParsingContext>>();

        protected override Action<ParsingContext> GetSemanticAction(LR1ReducitonAction parsingAction)
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
            dict.Add(new LR1ReducitonAction(292), state174_type_name_identifier_DOT);
        }

        // State [174]:
        // <type_name> ::= identifier . ;, "{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        static void state174_type_name_identifier_DOT(ParsingContext context)
        {
            SyntaxTree tree = context.TreeStack.Peek();
            string name = tree.Children[0].NodeType.Content;
            context.UserDefinedTypeTable.TryInsert(new UserDefinedType(name));
        }
    }
}
