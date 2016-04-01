using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LALR1Compiler;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace ContextfreeGrammarCompiler.Test
{
    partial class Program
    {
        
        private static void DumpSyntaxParserCode(RegulationList grammar, LRParsingMap map, string grammarId, string LR0Directory)
        {
            var parserType = new CodeTypeDeclaration(GetParserName(grammarId));
            parserType.IsClass = true;
            DumpSyntaxParserFields(grammar, parserType);
            DumpSyntaxParserMethod_GetGrammar(grammar, parserType);
            DumpSyntaxParserMethod_GetParsingMap(grammar, map, parserType);

            var parserNamespace = new CodeNamespace(GetNamespace(grammarId));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Object).Namespace));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Collections.Generic.List<int>).Namespace));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(LALR1Compiler.HashCache).Namespace));
            parserNamespace.Types.Add(parserType);

            //生成代码  
            string parserCodeFullname = Path.Combine(LR0Directory, GetParserName(grammarId) + ".cs");
            using (var sw = new StreamWriter(parserCodeFullname, false))
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CodeGeneratorOptions geneOptions = new CodeGeneratorOptions();//代码生成选项
                geneOptions.BlankLinesBetweenMembers = true;
                geneOptions.BracingStyle = "C";
                geneOptions.ElseOnClosing = false;
                geneOptions.IndentString = "    ";
                geneOptions.VerbatimOrder = true;

                codeProvider.GenerateCodeFromNamespace(parserNamespace, sw, geneOptions);
            }
        }

        private static void DumpSyntaxParserMethod_GetParsingMap(
            RegulationList grammar,
            LRParsingMap map,
            CodeTypeDeclaration parserType)
        {
            var method = new CodeMemberMethod();
            method.Name = "GetParsingMap";
            method.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            method.ReturnType = new CodeTypeReference(typeof(LRParsingMap));
            string varName = "map";
            var varDeclaration = new CodeVariableDeclarationStatement(typeof(LRParsingMap), varName);
            method.Statements.Add(varDeclaration);
            foreach (var action in map)
            {
                var parametes = new List<CodeExpression>();
                string[] parts = action.Key.Split('+');
                var stateId = new CodePrimitiveExpression(int.Parse(parts[0]));
                var nodeType = new CodeVariableReferenceExpression(
                    GetNodeName(new TreeNodeType(parts[1], parts[1], parts[1])));
                foreach (var value in action.Value)
                {
                    // action.Value超过1项，就说明有冲突。
                    Type t = value.GetType();
                    CodeObjectCreateExpression ctor = null;
                    string actionParam = value.ActionParam();
                    if (t == typeof(LR1AceptAction))
                    {
                        ctor = new CodeObjectCreateExpression(value.GetType());
                    }
                    else
                    {
                        ctor = new CodeObjectCreateExpression(t,
                            new CodePrimitiveExpression(int.Parse(actionParam)));
                    }

                    var SetActionMethod = new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression(varName), "SetAction");
                    var setAction = new CodeMethodInvokeExpression(
                        SetActionMethod, stateId, nodeType, ctor);
                    method.Statements.Add(setAction);
                }
            }
            {
                var returnMap = new CodeMethodReturnStatement(
                    new CodeVariableReferenceExpression(varName));
                method.Statements.Add(returnMap);
            }

            parserType.Members.Add(method);
        }

        private static void DumpSyntaxParserMethod_GetGrammar(RegulationList grammar, CodeTypeDeclaration parserType)
        {
            var method = new CodeMemberMethod();
            method.Name = "GetGrammar";
            method.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            method.ReturnType = new CodeTypeReference(typeof(RegulationList));
            string varName = "grammar";
            var varDeclaration = new CodeVariableDeclarationStatement(typeof(RegulationList), varName);
            method.Statements.Add(varDeclaration);
            foreach (var regulation in grammar)
            {
                //string str = regulation.Dump();
                //var comment = new CodeComment(string.Format("// {0}", str));
                var parametes = new List<CodeExpression>();
                parametes.Add(new CodeVariableReferenceExpression(GetNodeName(regulation.Left)));
                parametes.AddRange(
                    from item in regulation.RightPart
                    select new CodeVariableReferenceExpression(GetNodeName(item)));
                var ctor = new CodeObjectCreateExpression(typeof(Regulation), parametes.ToArray());
                var AddMethod = new CodeMethodReferenceExpression(
                    new CodeVariableReferenceExpression(varName),
                    "Add");
                    //,
                    //new CodeTypeReference(typeof(Regulation)));
                var addRegulation = new CodeMethodInvokeExpression(AddMethod, ctor);
                method.Statements.Add(addRegulation);
            }
            {
                var returnGrammar = new CodeMethodReturnStatement(
                    new CodeVariableReferenceExpression(varName));
                method.Statements.Add(returnGrammar);
            }

            parserType.Members.Add(method);
        }

        private static void DumpSyntaxParserFields(RegulationList grammar, CodeTypeDeclaration parserType)
        {
            foreach (var node in grammar.GetAllTreeNodeNonLeaveTypes())
            {
                CodeMemberField field = new CodeMemberField(typeof(TreeNodeType), GetNodeName(node));
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                var ctor = new CodeObjectCreateExpression(typeof(TreeNodeType),
                    new CodePrimitiveExpression(node.Type),
                    new CodePrimitiveExpression(node.Content),
                    new CodePrimitiveExpression(node.Nickname));
                field.InitExpression = ctor;
                parserType.Members.Add(field);
            }
            foreach (var node in grammar.GetAllTreeNodeLeaveTypes())
            {
                CodeMemberField field = new CodeMemberField(typeof(TreeNodeType), GetNodeName(node));
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                var ctor = new CodeObjectCreateExpression(typeof(TreeNodeType),
                    new CodePrimitiveExpression(node.Type),
                    new CodePrimitiveExpression(node.Content),
                    new CodePrimitiveExpression(node.Nickname));
                field.InitExpression = ctor;
                parserType.Members.Add(field);
            }
            {
                var node = TreeNodeType.endOfTokenListNode;
                CodeMemberField field = new CodeMemberField(typeof(TreeNodeType), GetNodeName(node));
                field.Attributes = MemberAttributes.Private | MemberAttributes.Static;
                var ctor = new CodeObjectCreateExpression(typeof(TreeNodeType),
                    new CodePrimitiveExpression(node.Type),
                    new CodePrimitiveExpression(node.Content),
                    new CodePrimitiveExpression(node.Nickname));
                field.InitExpression = ctor;
                parserType.Members.Add(field);
            }
        }

    }
}
