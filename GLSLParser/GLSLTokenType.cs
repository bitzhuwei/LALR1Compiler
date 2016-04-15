namespace GLSLParser
{
    using System;
    
    
    public class GLSLTokenType
    {
        
        public const string NODEidentifierLeave__ = "identifier";
        
        public const string NODE__numberLeave__ = "__number";
        
        public const string NODE__left_parenLeave__ = "__left_paren";
        
        public const string NODE__right_parenLeave__ = "__right_paren";
        
        public const string NODE__trueLeave__ = "__true";
        
        public const string NODE__falseLeave__ = "__false";
        
        public const string NODE__left_bracketLeave__ = "__left_bracket";
        
        public const string NODE__right_bracketLeave__ = "__right_bracket";
        
        public const string NODE__dotLeave__ = "__dot";
        
        public const string NODE__plusplusLeave__ = "__plusplus";
        
        public const string NODE__dashdashLeave__ = "__dashdash";
        
        public const string NODE__voidLeave__ = "__void";
        
        public const string NODE__commaLeave__ = "__comma";
        
        public const string NODE__plusLeave__ = "__plus";
        
        public const string NODE__dashLeave__ = "__dash";
        
        public const string NODE__bangLeave__ = "__bang";
        
        public const string NODE__tildeLeave__ = "__tilde";
        
        public const string NODE__starLeave__ = "__star";
        
        public const string NODE__slashLeave__ = "__slash";
        
        public const string NODE__percentLeave__ = "__percent";
        
        public const string NODE__left_angleleft_angleLeave__ = "__left_angleleft_angle";
        
        public const string NODE__right_angleright_angleLeave__ = "__right_angleright_angle";
        
        public const string NODE__left_angleLeave__ = "__left_angle";
        
        public const string NODE__right_angleLeave__ = "__right_angle";
        
        public const string NODE__left_angleequalLeave__ = "__left_angleequal";
        
        public const string NODE__right_angleequalLeave__ = "__right_angleequal";
        
        public const string NODE__equalequalLeave__ = "__equalequal";
        
        public const string NODE__bangequalLeave__ = "__bangequal";
        
        public const string NODE__and_opLeave__ = "__and_op";
        
        public const string NODE__caretLeave__ = "__caret";
        
        public const string NODE__vertical_barLeave__ = "__vertical_bar";
        
        public const string NODE__and_opand_opLeave__ = "__and_opand_op";
        
        public const string NODE__caretcaretLeave__ = "__caretcaret";
        
        public const string NODE__vertical_barvertical_barLeave__ = "__vertical_barvertical_bar";
        
        public const string NODE__questionLeave__ = "__question";
        
        public const string NODE__colonLeave__ = "__colon";
        
        public const string NODE__equalLeave__ = "__equal";
        
        public const string NODE__starequalLeave__ = "__starequal";
        
        public const string NODE__slashequalLeave__ = "__slashequal";
        
        public const string NODE__percentequalLeave__ = "__percentequal";
        
        public const string NODE__plusequalLeave__ = "__plusequal";
        
        public const string NODE__dashequalLeave__ = "__dashequal";
        
        public const string NODE__left_angleleft_angleequalLeave__ = "__left_angleleft_angleequal";
        
        public const string NODE__right_angleright_angleequalLeave__ = "__right_angleright_angleequal";
        
        public const string NODE__and_opequalLeave__ = "__and_opequal";
        
        public const string NODE__caretequalLeave__ = "__caretequal";
        
        public const string NODE__vertical_barequalLeave__ = "__vertical_barequal";
        
        public const string NODE__semicolonLeave__ = "__semicolon";
        
        public const string NODE__precisionLeave__ = "__precision";
        
        public const string NODE__left_braceLeave__ = "__left_brace";
        
        public const string NODE__right_braceLeave__ = "__right_brace";
        
        public const string NODE__invariantLeave__ = "__invariant";
        
        public const string NODE__smoothLeave__ = "__smooth";
        
        public const string NODE__flatLeave__ = "__flat";
        
        public const string NODE__noperspectiveLeave__ = "__noperspective";
        
        public const string NODE__layoutLeave__ = "__layout";
        
        public const string NODE__preciseLeave__ = "__precise";
        
        public const string NODE__constLeave__ = "__const";
        
        public const string NODE__inoutLeave__ = "__inout";
        
        public const string NODE__inLeave__ = "__in";
        
        public const string NODE__outLeave__ = "__out";
        
        public const string NODE__centroidLeave__ = "__centroid";
        
        public const string NODE__patchLeave__ = "__patch";
        
        public const string NODE__sampleLeave__ = "__sample";
        
        public const string NODE__uniformLeave__ = "__uniform";
        
        public const string NODE__bufferLeave__ = "__buffer";
        
        public const string NODE__sharedLeave__ = "__shared";
        
        public const string NODE__coherentLeave__ = "__coherent";
        
        public const string NODE__volatileLeave__ = "__volatile";
        
        public const string NODE__restrictLeave__ = "__restrict";
        
        public const string NODE__readonlyLeave__ = "__readonly";
        
        public const string NODE__writeonlyLeave__ = "__writeonly";
        
        public const string NODE__subroutineLeave__ = "__subroutine";
        
        public const string NODE__userDefinedTypeLeave__ = "__userDefinedType";
        
        public const string NODE__floatLeave__ = "__float";
        
        public const string NODE__doubleLeave__ = "__double";
        
        public const string NODE__intLeave__ = "__int";
        
        public const string NODE__uintLeave__ = "__uint";
        
        public const string NODE__boolLeave__ = "__bool";
        
        public const string NODE__vec2Leave__ = "__vec2";
        
        public const string NODE__vec3Leave__ = "__vec3";
        
        public const string NODE__vec4Leave__ = "__vec4";
        
        public const string NODE__dvec2Leave__ = "__dvec2";
        
        public const string NODE__dvec3Leave__ = "__dvec3";
        
        public const string NODE__dvec4Leave__ = "__dvec4";
        
        public const string NODE__bvec2Leave__ = "__bvec2";
        
        public const string NODE__bvec3Leave__ = "__bvec3";
        
        public const string NODE__bvec4Leave__ = "__bvec4";
        
        public const string NODE__ivec2Leave__ = "__ivec2";
        
        public const string NODE__ivec3Leave__ = "__ivec3";
        
        public const string NODE__ivec4Leave__ = "__ivec4";
        
        public const string NODE__uvec2Leave__ = "__uvec2";
        
        public const string NODE__uvec3Leave__ = "__uvec3";
        
        public const string NODE__uvec4Leave__ = "__uvec4";
        
        public const string NODE__mat2Leave__ = "__mat2";
        
        public const string NODE__mat3Leave__ = "__mat3";
        
        public const string NODE__mat4Leave__ = "__mat4";
        
        public const string NODE__mat2x2Leave__ = "__mat2x2";
        
        public const string NODE__mat2x3Leave__ = "__mat2x3";
        
        public const string NODE__mat2x4Leave__ = "__mat2x4";
        
        public const string NODE__mat3x2Leave__ = "__mat3x2";
        
        public const string NODE__mat3x3Leave__ = "__mat3x3";
        
        public const string NODE__mat3x4Leave__ = "__mat3x4";
        
        public const string NODE__mat4x2Leave__ = "__mat4x2";
        
        public const string NODE__mat4x3Leave__ = "__mat4x3";
        
        public const string NODE__mat4x4Leave__ = "__mat4x4";
        
        public const string NODE__dmat2Leave__ = "__dmat2";
        
        public const string NODE__dmat3Leave__ = "__dmat3";
        
        public const string NODE__dmat4Leave__ = "__dmat4";
        
        public const string NODE__dmat2x2Leave__ = "__dmat2x2";
        
        public const string NODE__dmat2x3Leave__ = "__dmat2x3";
        
        public const string NODE__dmat2x4Leave__ = "__dmat2x4";
        
        public const string NODE__dmat3x2Leave__ = "__dmat3x2";
        
        public const string NODE__dmat3x3Leave__ = "__dmat3x3";
        
        public const string NODE__dmat3x4Leave__ = "__dmat3x4";
        
        public const string NODE__dmat4x2Leave__ = "__dmat4x2";
        
        public const string NODE__dmat4x3Leave__ = "__dmat4x3";
        
        public const string NODE__dmat4x4Leave__ = "__dmat4x4";
        
        public const string NODE__atomic_uintLeave__ = "__atomic_uint";
        
        public const string NODE__sampler1DLeave__ = "__sampler1D";
        
        public const string NODE__sampler2DLeave__ = "__sampler2D";
        
        public const string NODE__sampler3DLeave__ = "__sampler3D";
        
        public const string NODE__samplerCubeLeave__ = "__samplerCube";
        
        public const string NODE__sampler1DShadowLeave__ = "__sampler1DShadow";
        
        public const string NODE__sampler2DShadowLeave__ = "__sampler2DShadow";
        
        public const string NODE__samplerCubeShadowLeave__ = "__samplerCubeShadow";
        
        public const string NODE__sampler1DArrayLeave__ = "__sampler1DArray";
        
        public const string NODE__sampler2DArrayLeave__ = "__sampler2DArray";
        
        public const string NODE__sampler1DArrayShadowLeave__ = "__sampler1DArrayShadow";
        
        public const string NODE__sampler2DArrayShadowLeave__ = "__sampler2DArrayShadow";
        
        public const string NODE__samplerCubeArrayLeave__ = "__samplerCubeArray";
        
        public const string NODE__samplerCubeArrayShadowLeave__ = "__samplerCubeArrayShadow";
        
        public const string NODE__isampler1DLeave__ = "__isampler1D";
        
        public const string NODE__isampler2DLeave__ = "__isampler2D";
        
        public const string NODE__isampler3DLeave__ = "__isampler3D";
        
        public const string NODE__isamplerCubeLeave__ = "__isamplerCube";
        
        public const string NODE__isampler1DArrayLeave__ = "__isampler1DArray";
        
        public const string NODE__isampler2DArrayLeave__ = "__isampler2DArray";
        
        public const string NODE__isamplerCubeArrayLeave__ = "__isamplerCubeArray";
        
        public const string NODE__usampler1DLeave__ = "__usampler1D";
        
        public const string NODE__usampler2DLeave__ = "__usampler2D";
        
        public const string NODE__usampler3DLeave__ = "__usampler3D";
        
        public const string NODE__usamplerCubeLeave__ = "__usamplerCube";
        
        public const string NODE__usampler1DArrayLeave__ = "__usampler1DArray";
        
        public const string NODE__usampler2DArrayLeave__ = "__usampler2DArray";
        
        public const string NODE__usamplerCubeArrayLeave__ = "__usamplerCubeArray";
        
        public const string NODE__sampler2DRectLeave__ = "__sampler2DRect";
        
        public const string NODE__sampler2DRectShadowLeave__ = "__sampler2DRectShadow";
        
        public const string NODE__isampler2DRectLeave__ = "__isampler2DRect";
        
        public const string NODE__usampler2DRectLeave__ = "__usampler2DRect";
        
        public const string NODE__samplerBufferLeave__ = "__samplerBuffer";
        
        public const string NODE__isamplerBufferLeave__ = "__isamplerBuffer";
        
        public const string NODE__usamplerBufferLeave__ = "__usamplerBuffer";
        
        public const string NODE__sampler2DMSLeave__ = "__sampler2DMS";
        
        public const string NODE__isampler2DMSLeave__ = "__isampler2DMS";
        
        public const string NODE__usampler2DMSLeave__ = "__usampler2DMS";
        
        public const string NODE__sampler2DMSArrayLeave__ = "__sampler2DMSArray";
        
        public const string NODE__isampler2DMSArrayLeave__ = "__isampler2DMSArray";
        
        public const string NODE__usampler2DMSArrayLeave__ = "__usampler2DMSArray";
        
        public const string NODE__image1DLeave__ = "__image1D";
        
        public const string NODE__iimage1DLeave__ = "__iimage1D";
        
        public const string NODE__uimage1DLeave__ = "__uimage1D";
        
        public const string NODE__image2DLeave__ = "__image2D";
        
        public const string NODE__iimage2DLeave__ = "__iimage2D";
        
        public const string NODE__uimage2DLeave__ = "__uimage2D";
        
        public const string NODE__image3DLeave__ = "__image3D";
        
        public const string NODE__iimage3DLeave__ = "__iimage3D";
        
        public const string NODE__uimage3DLeave__ = "__uimage3D";
        
        public const string NODE__image2DRectLeave__ = "__image2DRect";
        
        public const string NODE__iimage2DRectLeave__ = "__iimage2DRect";
        
        public const string NODE__uimage2DRectLeave__ = "__uimage2DRect";
        
        public const string NODE__imageCubeLeave__ = "__imageCube";
        
        public const string NODE__iimageCubeLeave__ = "__iimageCube";
        
        public const string NODE__uimageCubeLeave__ = "__uimageCube";
        
        public const string NODE__imageBufferLeave__ = "__imageBuffer";
        
        public const string NODE__iimageBufferLeave__ = "__iimageBuffer";
        
        public const string NODE__uimageBufferLeave__ = "__uimageBuffer";
        
        public const string NODE__image1DArrayLeave__ = "__image1DArray";
        
        public const string NODE__iimage1DArrayLeave__ = "__iimage1DArray";
        
        public const string NODE__uimage1DArrayLeave__ = "__uimage1DArray";
        
        public const string NODE__image2DArrayLeave__ = "__image2DArray";
        
        public const string NODE__iimage2DArrayLeave__ = "__iimage2DArray";
        
        public const string NODE__uimage2DArrayLeave__ = "__uimage2DArray";
        
        public const string NODE__imageCubeArrayLeave__ = "__imageCubeArray";
        
        public const string NODE__iimageCubeArrayLeave__ = "__iimageCubeArray";
        
        public const string NODE__uimageCubeArrayLeave__ = "__uimageCubeArray";
        
        public const string NODE__image2DMSLeave__ = "__image2DMS";
        
        public const string NODE__iimage2DMSLeave__ = "__iimage2DMS";
        
        public const string NODE__uimage2DMSLeave__ = "__uimage2DMS";
        
        public const string NODE__image2DMSArrayLeave__ = "__image2DMSArray";
        
        public const string NODE__iimage2DMSArrayLeave__ = "__iimage2DMSArray";
        
        public const string NODE__uimage2DMSArrayLeave__ = "__uimage2DMSArray";
        
        public const string NODE__high_precisionLeave__ = "__high_precision";
        
        public const string NODE__medium_precisionLeave__ = "__medium_precision";
        
        public const string NODE__low_precisionLeave__ = "__low_precision";
        
        public const string NODE__structLeave__ = "__struct";
        
        public const string NODE__ifLeave__ = "__if";
        
        public const string NODE__elseLeave__ = "__else";
        
        public const string NODE__switchLeave__ = "__switch";
        
        public const string NODE__caseLeave__ = "__case";
        
        public const string NODE__defaultLeave__ = "__default";
        
        public const string NODE__whileLeave__ = "__while";
        
        public const string NODE__doLeave__ = "__do";
        
        public const string NODE__forLeave__ = "__for";
        
        public const string NODE__continueLeave__ = "__continue";
        
        public const string NODE__breakLeave__ = "__break";
        
        public const string NODE__returnLeave__ = "__return";
        
        public const string NODE__discardLeave__ = "__discard";
    }
}
