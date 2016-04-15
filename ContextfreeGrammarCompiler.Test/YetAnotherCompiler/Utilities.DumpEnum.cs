using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    static partial class Utilities
    {
        /// <summary>
        /// 根据Enum的类型获取其Code。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static CodeTypeDeclaration CreateEnumCodeDom(Type type)
        {
            var enumTypeDeclaration = new CodeTypeDeclaration(type.Name);
            enumTypeDeclaration.IsEnum = true;
            enumTypeDeclaration.Attributes = MemberAttributes.Public;

            FieldInfo[] FieldInfoList = type.GetFields();
            int count = 0;
            foreach (FieldInfo field in FieldInfoList)
            {
                var newField = new CodeMemberField(typeof(int), field.Name);
                if (count != 0)
                {
                    newField.InitExpression = new CodeSnippetExpression((count - 1).ToString());
                    enumTypeDeclaration.Members.Add(newField);
                }
                count++;
            }

            return enumTypeDeclaration;
        }
    }
}
