using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLSLParser
{
    public class TypeQualifier : Declaration, ICloneable
    {
        public string Identifier { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
