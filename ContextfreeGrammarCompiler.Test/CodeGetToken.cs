using LALR1Compiler;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    /// <summary>
    /// 产生处理某种字符的GetXXX();的定义的关键部分中的最小的一条语句
    /// </summary>
    class CodeGetToken : HashCache
    {

        public CodeGetToken(TreeNodeType value)
            : base(GetUniqueString)
        {
            this.Value = value;
        }

        public TreeNodeType Value { get; set; }

        public override int CompareTo(HashCache other)
        {
            CodeGetToken obj = other as CodeGetToken;
            if ((Object)obj == null) { return -1; }
            if (obj.Value == null) { return -1; }

            int thisLength = this.Value.Type.Length;
            int otherLength = obj.Value.Type.Length;
            if (thisLength > otherLength)
            { return -1; }
            else if (thisLength == otherLength)
            { return 0; }
            else
            { return 1; }
        }

        public virtual CodeStatement[] DumpReadToken()
        {
            List<CodeStatement> list = new List<CodeStatement>();

            {
                /// if ("XXX" == str)
                var ifStatement = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodePrimitiveExpression(this.Value.Content),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodeVariableReferenceExpression("str")));
                {
                    // result.TokenType = new TokenType("..", "..", "..");
                    var newTokenType = new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodeVariableReferenceExpression("result"),
                            "TokenType"),
                        new CodeObjectCreateExpression(typeof(TokenType),
                            new CodePrimitiveExpression(
                                "__" + ConstString2IdentifierHelper.ConstString2Identifier(
                                    this.Value.Content)),
                            new CodePrimitiveExpression(this.Value.Content),
                            new CodePrimitiveExpression(this.Value.Nickname)));
                    ifStatement.TrueStatements.Add(newTokenType);
                    // context.NextLetterIndex = context.NextLetterIndex + {0}
                    var pointer = new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("context"), "NextLetterIndex");
                    var incresePointer = new CodeAssignStatement(
                        pointer, new CodeBinaryOperatorExpression(
                            pointer, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(this.Value.Content.Length)));
                    ifStatement.TrueStatements.Add(incresePointer);
                    // return true;
                    var returnTrue = new CodeMethodReturnStatement(new CodePrimitiveExpression(true));
                    ifStatement.TrueStatements.Add(returnTrue);
                }
                list.Add(ifStatement);
            }

            return list.ToArray();
        }

        static string GetUniqueString(HashCache cache)
        {
            CodeGetToken obj = cache as CodeGetToken;
            return obj.Dump();
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write(this.Value);
        }

    }

    class CodeGetIdentifier : CodeGetToken
    {

        public CodeGetIdentifier() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }
    }

    class CodeGetNumber : CodeGetToken
    {
        public CodeGetNumber() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }

    class CodeGetConstString : CodeGetToken
    {
        public CodeGetConstString() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }

    class CodeGetChar : CodeGetToken
    {
        public CodeGetChar() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }

    class CodeGetUnknown : CodeGetToken
    {
        public CodeGetUnknown() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }

    class CodeGetSpace : CodeGetToken
    {
        public CodeGetSpace() : base(null) { }

        public override CodeStatement[] DumpReadToken()
        {
            return null;
        }

    }
}
