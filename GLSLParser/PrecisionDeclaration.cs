using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLSLParser
{
    class PrecisionDeclaration : Declaration
    {
        public PrecisionQualifierType Qualifier { get; set; }

        public string TypeName { get; set; }
    }

    enum PrecisionQualifierType
    {
        high_precision, medium_precision, low_precision,
    }
}
