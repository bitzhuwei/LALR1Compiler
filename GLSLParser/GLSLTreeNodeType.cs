namespace GLSLParser
{
    using System;
    
    
    public class GLSLTreeNodeType
    {
        
        public const string NODE__translation_unit = "__translation_unit";
        
        public const string NODE__external_declaration = "__external_declaration";
        
        public const string NODE__function_definition = "__function_definition";
        
        public const string NODE__declaration = "__declaration";
        
        public const string NODE__function_prototype = "__function_prototype";
        
        public const string NODE__compound_statement_no_new_scope = "__compound_statement_no_new_scope";
        
        public const string NODE__variable_identifier = "__variable_identifier";
        
        public const string NODEidentifierLeave__ = "identifierLeave__";
        
        public const string NODE__primary_expression = "__primary_expression";
        
        public const string NODE__numberLeave__ = "__numberLeave__";
        
        public const string NODE__BOOLCONSTANT = "__BOOLCONSTANT";
        
        public const string NODE__left_parenLeave__ = "__left_parenLeave__";
        
        public const string NODE__expression = "__expression";
        
        public const string NODE__right_parenLeave__ = "__right_parenLeave__";
        
        public const string NODE__trueLeave__ = "__trueLeave__";
        
        public const string NODE__falseLeave__ = "__falseLeave__";
        
        public const string NODE__postfix_expression = "__postfix_expression";
        
        public const string NODE__left_bracketLeave__ = "__left_bracketLeave__";
        
        public const string NODE__integer_expression = "__integer_expression";
        
        public const string NODE__right_bracketLeave__ = "__right_bracketLeave__";
        
        public const string NODE__function_call = "__function_call";
        
        public const string NODE__dotLeave__ = "__dotLeave__";
        
        public const string NODE__FIELD_SELECTION = "__FIELD_SELECTION";
        
        public const string NODE__plusplusLeave__ = "__plusplusLeave__";
        
        public const string NODE__dashdashLeave__ = "__dashdashLeave__";
        
        public const string NODE__function_call_or_method = "__function_call_or_method";
        
        public const string NODE__function_call_generic = "__function_call_generic";
        
        public const string NODE__function_call_header_with_parameters = "__function_call_header_with_parameters";
        
        public const string NODE__function_call_header_no_parameters = "__function_call_header_no_parameters";
        
        public const string NODE__function_call_header = "__function_call_header";
        
        public const string NODE__voidLeave__ = "__voidLeave__";
        
        public const string NODE__assignment_expression = "__assignment_expression";
        
        public const string NODE__commaLeave__ = "__commaLeave__";
        
        public const string NODE__function_identifier = "__function_identifier";
        
        public const string NODE__type_specifier = "__type_specifier";
        
        public const string NODE__unary_expression = "__unary_expression";
        
        public const string NODE__unary_operator = "__unary_operator";
        
        public const string NODE__plusLeave__ = "__plusLeave__";
        
        public const string NODE__dashLeave__ = "__dashLeave__";
        
        public const string NODE__bangLeave__ = "__bangLeave__";
        
        public const string NODE__tildeLeave__ = "__tildeLeave__";
        
        public const string NODE__multiplicative_expression = "__multiplicative_expression";
        
        public const string NODE__starLeave__ = "__starLeave__";
        
        public const string NODE__slashLeave__ = "__slashLeave__";
        
        public const string NODE__percentLeave__ = "__percentLeave__";
        
        public const string NODE__additive_expression = "__additive_expression";
        
        public const string NODE__shift_expression = "__shift_expression";
        
        public const string NODE__left_angleleft_angleLeave__ = "__left_angleleft_angleLeave__";
        
        public const string NODE__right_angleright_angleLeave__ = "__right_angleright_angleLeave__";
        
        public const string NODE__relational_expression = "__relational_expression";
        
        public const string NODE__left_angleLeave__ = "__left_angleLeave__";
        
        public const string NODE__right_angleLeave__ = "__right_angleLeave__";
        
        public const string NODE__left_angleequalLeave__ = "__left_angleequalLeave__";
        
        public const string NODE__right_angleequalLeave__ = "__right_angleequalLeave__";
        
        public const string NODE__equality_expression = "__equality_expression";
        
        public const string NODE__equalequalLeave__ = "__equalequalLeave__";
        
        public const string NODE__bangequalLeave__ = "__bangequalLeave__";
        
        public const string NODE__and_expression = "__and_expression";
        
        public const string NODE__and_opLeave__ = "__and_opLeave__";
        
        public const string NODE__exclusive_or_expression = "__exclusive_or_expression";
        
        public const string NODE__caretLeave__ = "__caretLeave__";
        
        public const string NODE__inclusive_or_expression = "__inclusive_or_expression";
        
        public const string NODE__vertical_barLeave__ = "__vertical_barLeave__";
        
        public const string NODE__logical_and_expression = "__logical_and_expression";
        
        public const string NODE__and_opand_opLeave__ = "__and_opand_opLeave__";
        
        public const string NODE__logical_xor_expression = "__logical_xor_expression";
        
        public const string NODE__caretcaretLeave__ = "__caretcaretLeave__";
        
        public const string NODE__logical_or_expression = "__logical_or_expression";
        
        public const string NODE__vertical_barvertical_barLeave__ = "__vertical_barvertical_barLeave__";
        
        public const string NODE__conditional_expression = "__conditional_expression";
        
        public const string NODE__questionLeave__ = "__questionLeave__";
        
        public const string NODE__colonLeave__ = "__colonLeave__";
        
        public const string NODE__assignment_operator = "__assignment_operator";
        
        public const string NODE__equalLeave__ = "__equalLeave__";
        
        public const string NODE__starequalLeave__ = "__starequalLeave__";
        
        public const string NODE__slashequalLeave__ = "__slashequalLeave__";
        
        public const string NODE__percentequalLeave__ = "__percentequalLeave__";
        
        public const string NODE__plusequalLeave__ = "__plusequalLeave__";
        
        public const string NODE__dashequalLeave__ = "__dashequalLeave__";
        
        public const string NODE__left_angleleft_angleequalLeave__ = "__left_angleleft_angleequalLeave__";
        
        public const string NODE__right_angleright_angleequalLeave__ = "__right_angleright_angleequalLeave__";
        
        public const string NODE__and_opequalLeave__ = "__and_opequalLeave__";
        
        public const string NODE__caretequalLeave__ = "__caretequalLeave__";
        
        public const string NODE__vertical_barequalLeave__ = "__vertical_barequalLeave__";
        
        public const string NODE__constant_expression = "__constant_expression";
        
        public const string NODE__semicolonLeave__ = "__semicolonLeave__";
        
        public const string NODE__init_declarator_list = "__init_declarator_list";
        
        public const string NODE__precisionLeave__ = "__precisionLeave__";
        
        public const string NODE__precision_qualifier = "__precision_qualifier";
        
        public const string NODE__type_qualifier = "__type_qualifier";
        
        public const string NODE__left_braceLeave__ = "__left_braceLeave__";
        
        public const string NODE__struct_declaration_list = "__struct_declaration_list";
        
        public const string NODE__right_braceLeave__ = "__right_braceLeave__";
        
        public const string NODE__array_specifier = "__array_specifier";
        
        public const string NODE__identifier_list = "__identifier_list";
        
        public const string NODE__function_declarator = "__function_declarator";
        
        public const string NODE__function_header = "__function_header";
        
        public const string NODE__function_header_with_parameters = "__function_header_with_parameters";
        
        public const string NODE__parameter_declaration = "__parameter_declaration";
        
        public const string NODE__fully_specified_type = "__fully_specified_type";
        
        public const string NODE__parameter_declarator = "__parameter_declarator";
        
        public const string NODE__parameter_type_specifier = "__parameter_type_specifier";
        
        public const string NODE__single_declaration = "__single_declaration";
        
        public const string NODE__initializer = "__initializer";
        
        public const string NODE__invariant_qualifier = "__invariant_qualifier";
        
        public const string NODE__invariantLeave__ = "__invariantLeave__";
        
        public const string NODE__interpolation_qualifier = "__interpolation_qualifier";
        
        public const string NODE__smoothLeave__ = "__smoothLeave__";
        
        public const string NODE__flatLeave__ = "__flatLeave__";
        
        public const string NODE__noperspectiveLeave__ = "__noperspectiveLeave__";
        
        public const string NODE__layout_qualifier = "__layout_qualifier";
        
        public const string NODE__layoutLeave__ = "__layoutLeave__";
        
        public const string NODE__layout_qualifier_id_list = "__layout_qualifier_id_list";
        
        public const string NODE__layout_qualifier_id = "__layout_qualifier_id";
        
        public const string NODE__precise_qualifier = "__precise_qualifier";
        
        public const string NODE__preciseLeave__ = "__preciseLeave__";
        
        public const string NODE__single_type_qualifier = "__single_type_qualifier";
        
        public const string NODE__storage_qualifier = "__storage_qualifier";
        
        public const string NODE__constLeave__ = "__constLeave__";
        
        public const string NODE__inoutLeave__ = "__inoutLeave__";
        
        public const string NODE__inLeave__ = "__inLeave__";
        
        public const string NODE__outLeave__ = "__outLeave__";
        
        public const string NODE__centroidLeave__ = "__centroidLeave__";
        
        public const string NODE__patchLeave__ = "__patchLeave__";
        
        public const string NODE__sampleLeave__ = "__sampleLeave__";
        
        public const string NODE__uniformLeave__ = "__uniformLeave__";
        
        public const string NODE__bufferLeave__ = "__bufferLeave__";
        
        public const string NODE__sharedLeave__ = "__sharedLeave__";
        
        public const string NODE__coherentLeave__ = "__coherentLeave__";
        
        public const string NODE__volatileLeave__ = "__volatileLeave__";
        
        public const string NODE__restrictLeave__ = "__restrictLeave__";
        
        public const string NODE__readonlyLeave__ = "__readonlyLeave__";
        
        public const string NODE__writeonlyLeave__ = "__writeonlyLeave__";
        
        public const string NODE__subroutineLeave__ = "__subroutineLeave__";
        
        public const string NODE__type_name_list = "__type_name_list";
        
        public const string NODE__userDefinedTypeLeave__ = "__userDefinedTypeLeave__";
        
        public const string NODE__type_specifier_nonarray = "__type_specifier_nonarray";
        
        public const string NODE__floatLeave__ = "__floatLeave__";
        
        public const string NODE__doubleLeave__ = "__doubleLeave__";
        
        public const string NODE__intLeave__ = "__intLeave__";
        
        public const string NODE__uintLeave__ = "__uintLeave__";
        
        public const string NODE__boolLeave__ = "__boolLeave__";
        
        public const string NODE__vec2Leave__ = "__vec2Leave__";
        
        public const string NODE__vec3Leave__ = "__vec3Leave__";
        
        public const string NODE__vec4Leave__ = "__vec4Leave__";
        
        public const string NODE__dvec2Leave__ = "__dvec2Leave__";
        
        public const string NODE__dvec3Leave__ = "__dvec3Leave__";
        
        public const string NODE__dvec4Leave__ = "__dvec4Leave__";
        
        public const string NODE__bvec2Leave__ = "__bvec2Leave__";
        
        public const string NODE__bvec3Leave__ = "__bvec3Leave__";
        
        public const string NODE__bvec4Leave__ = "__bvec4Leave__";
        
        public const string NODE__ivec2Leave__ = "__ivec2Leave__";
        
        public const string NODE__ivec3Leave__ = "__ivec3Leave__";
        
        public const string NODE__ivec4Leave__ = "__ivec4Leave__";
        
        public const string NODE__uvec2Leave__ = "__uvec2Leave__";
        
        public const string NODE__uvec3Leave__ = "__uvec3Leave__";
        
        public const string NODE__uvec4Leave__ = "__uvec4Leave__";
        
        public const string NODE__mat2Leave__ = "__mat2Leave__";
        
        public const string NODE__mat3Leave__ = "__mat3Leave__";
        
        public const string NODE__mat4Leave__ = "__mat4Leave__";
        
        public const string NODE__mat2x2Leave__ = "__mat2x2Leave__";
        
        public const string NODE__mat2x3Leave__ = "__mat2x3Leave__";
        
        public const string NODE__mat2x4Leave__ = "__mat2x4Leave__";
        
        public const string NODE__mat3x2Leave__ = "__mat3x2Leave__";
        
        public const string NODE__mat3x3Leave__ = "__mat3x3Leave__";
        
        public const string NODE__mat3x4Leave__ = "__mat3x4Leave__";
        
        public const string NODE__mat4x2Leave__ = "__mat4x2Leave__";
        
        public const string NODE__mat4x3Leave__ = "__mat4x3Leave__";
        
        public const string NODE__mat4x4Leave__ = "__mat4x4Leave__";
        
        public const string NODE__dmat2Leave__ = "__dmat2Leave__";
        
        public const string NODE__dmat3Leave__ = "__dmat3Leave__";
        
        public const string NODE__dmat4Leave__ = "__dmat4Leave__";
        
        public const string NODE__dmat2x2Leave__ = "__dmat2x2Leave__";
        
        public const string NODE__dmat2x3Leave__ = "__dmat2x3Leave__";
        
        public const string NODE__dmat2x4Leave__ = "__dmat2x4Leave__";
        
        public const string NODE__dmat3x2Leave__ = "__dmat3x2Leave__";
        
        public const string NODE__dmat3x3Leave__ = "__dmat3x3Leave__";
        
        public const string NODE__dmat3x4Leave__ = "__dmat3x4Leave__";
        
        public const string NODE__dmat4x2Leave__ = "__dmat4x2Leave__";
        
        public const string NODE__dmat4x3Leave__ = "__dmat4x3Leave__";
        
        public const string NODE__dmat4x4Leave__ = "__dmat4x4Leave__";
        
        public const string NODE__atomic_uintLeave__ = "__atomic_uintLeave__";
        
        public const string NODE__sampler1DLeave__ = "__sampler1DLeave__";
        
        public const string NODE__sampler2DLeave__ = "__sampler2DLeave__";
        
        public const string NODE__sampler3DLeave__ = "__sampler3DLeave__";
        
        public const string NODE__samplerCubeLeave__ = "__samplerCubeLeave__";
        
        public const string NODE__sampler1DShadowLeave__ = "__sampler1DShadowLeave__";
        
        public const string NODE__sampler2DShadowLeave__ = "__sampler2DShadowLeave__";
        
        public const string NODE__samplerCubeShadowLeave__ = "__samplerCubeShadowLeave__";
        
        public const string NODE__sampler1DArrayLeave__ = "__sampler1DArrayLeave__";
        
        public const string NODE__sampler2DArrayLeave__ = "__sampler2DArrayLeave__";
        
        public const string NODE__sampler1DArrayShadowLeave__ = "__sampler1DArrayShadowLeave__";
        
        public const string NODE__sampler2DArrayShadowLeave__ = "__sampler2DArrayShadowLeave__";
        
        public const string NODE__samplerCubeArrayLeave__ = "__samplerCubeArrayLeave__";
        
        public const string NODE__samplerCubeArrayShadowLeave__ = "__samplerCubeArrayShadowLeave__";
        
        public const string NODE__isampler1DLeave__ = "__isampler1DLeave__";
        
        public const string NODE__isampler2DLeave__ = "__isampler2DLeave__";
        
        public const string NODE__isampler3DLeave__ = "__isampler3DLeave__";
        
        public const string NODE__isamplerCubeLeave__ = "__isamplerCubeLeave__";
        
        public const string NODE__isampler1DArrayLeave__ = "__isampler1DArrayLeave__";
        
        public const string NODE__isampler2DArrayLeave__ = "__isampler2DArrayLeave__";
        
        public const string NODE__isamplerCubeArrayLeave__ = "__isamplerCubeArrayLeave__";
        
        public const string NODE__usampler1DLeave__ = "__usampler1DLeave__";
        
        public const string NODE__usampler2DLeave__ = "__usampler2DLeave__";
        
        public const string NODE__usampler3DLeave__ = "__usampler3DLeave__";
        
        public const string NODE__usamplerCubeLeave__ = "__usamplerCubeLeave__";
        
        public const string NODE__usampler1DArrayLeave__ = "__usampler1DArrayLeave__";
        
        public const string NODE__usampler2DArrayLeave__ = "__usampler2DArrayLeave__";
        
        public const string NODE__usamplerCubeArrayLeave__ = "__usamplerCubeArrayLeave__";
        
        public const string NODE__sampler2DRectLeave__ = "__sampler2DRectLeave__";
        
        public const string NODE__sampler2DRectShadowLeave__ = "__sampler2DRectShadowLeave__";
        
        public const string NODE__isampler2DRectLeave__ = "__isampler2DRectLeave__";
        
        public const string NODE__usampler2DRectLeave__ = "__usampler2DRectLeave__";
        
        public const string NODE__samplerBufferLeave__ = "__samplerBufferLeave__";
        
        public const string NODE__isamplerBufferLeave__ = "__isamplerBufferLeave__";
        
        public const string NODE__usamplerBufferLeave__ = "__usamplerBufferLeave__";
        
        public const string NODE__sampler2DMSLeave__ = "__sampler2DMSLeave__";
        
        public const string NODE__isampler2DMSLeave__ = "__isampler2DMSLeave__";
        
        public const string NODE__usampler2DMSLeave__ = "__usampler2DMSLeave__";
        
        public const string NODE__sampler2DMSArrayLeave__ = "__sampler2DMSArrayLeave__";
        
        public const string NODE__isampler2DMSArrayLeave__ = "__isampler2DMSArrayLeave__";
        
        public const string NODE__usampler2DMSArrayLeave__ = "__usampler2DMSArrayLeave__";
        
        public const string NODE__image1DLeave__ = "__image1DLeave__";
        
        public const string NODE__iimage1DLeave__ = "__iimage1DLeave__";
        
        public const string NODE__uimage1DLeave__ = "__uimage1DLeave__";
        
        public const string NODE__image2DLeave__ = "__image2DLeave__";
        
        public const string NODE__iimage2DLeave__ = "__iimage2DLeave__";
        
        public const string NODE__uimage2DLeave__ = "__uimage2DLeave__";
        
        public const string NODE__image3DLeave__ = "__image3DLeave__";
        
        public const string NODE__iimage3DLeave__ = "__iimage3DLeave__";
        
        public const string NODE__uimage3DLeave__ = "__uimage3DLeave__";
        
        public const string NODE__image2DRectLeave__ = "__image2DRectLeave__";
        
        public const string NODE__iimage2DRectLeave__ = "__iimage2DRectLeave__";
        
        public const string NODE__uimage2DRectLeave__ = "__uimage2DRectLeave__";
        
        public const string NODE__imageCubeLeave__ = "__imageCubeLeave__";
        
        public const string NODE__iimageCubeLeave__ = "__iimageCubeLeave__";
        
        public const string NODE__uimageCubeLeave__ = "__uimageCubeLeave__";
        
        public const string NODE__imageBufferLeave__ = "__imageBufferLeave__";
        
        public const string NODE__iimageBufferLeave__ = "__iimageBufferLeave__";
        
        public const string NODE__uimageBufferLeave__ = "__uimageBufferLeave__";
        
        public const string NODE__image1DArrayLeave__ = "__image1DArrayLeave__";
        
        public const string NODE__iimage1DArrayLeave__ = "__iimage1DArrayLeave__";
        
        public const string NODE__uimage1DArrayLeave__ = "__uimage1DArrayLeave__";
        
        public const string NODE__image2DArrayLeave__ = "__image2DArrayLeave__";
        
        public const string NODE__iimage2DArrayLeave__ = "__iimage2DArrayLeave__";
        
        public const string NODE__uimage2DArrayLeave__ = "__uimage2DArrayLeave__";
        
        public const string NODE__imageCubeArrayLeave__ = "__imageCubeArrayLeave__";
        
        public const string NODE__iimageCubeArrayLeave__ = "__iimageCubeArrayLeave__";
        
        public const string NODE__uimageCubeArrayLeave__ = "__uimageCubeArrayLeave__";
        
        public const string NODE__image2DMSLeave__ = "__image2DMSLeave__";
        
        public const string NODE__iimage2DMSLeave__ = "__iimage2DMSLeave__";
        
        public const string NODE__uimage2DMSLeave__ = "__uimage2DMSLeave__";
        
        public const string NODE__image2DMSArrayLeave__ = "__image2DMSArrayLeave__";
        
        public const string NODE__iimage2DMSArrayLeave__ = "__iimage2DMSArrayLeave__";
        
        public const string NODE__uimage2DMSArrayLeave__ = "__uimage2DMSArrayLeave__";
        
        public const string NODE__struct_specifier = "__struct_specifier";
        
        public const string NODE__high_precisionLeave__ = "__high_precisionLeave__";
        
        public const string NODE__medium_precisionLeave__ = "__medium_precisionLeave__";
        
        public const string NODE__low_precisionLeave__ = "__low_precisionLeave__";
        
        public const string NODE__structLeave__ = "__structLeave__";
        
        public const string NODE__type_name = "__type_name";
        
        public const string NODE__struct_declaration = "__struct_declaration";
        
        public const string NODE__struct_declarator_list = "__struct_declarator_list";
        
        public const string NODE__struct_declarator = "__struct_declarator";
        
        public const string NODE__initializer_list = "__initializer_list";
        
        public const string NODE__declaration_statement = "__declaration_statement";
        
        public const string NODE__statement = "__statement";
        
        public const string NODE__compound_statement = "__compound_statement";
        
        public const string NODE__simple_statement = "__simple_statement";
        
        public const string NODE__expression_statement = "__expression_statement";
        
        public const string NODE__selection_statement = "__selection_statement";
        
        public const string NODE__switch_statement = "__switch_statement";
        
        public const string NODE__case_label = "__case_label";
        
        public const string NODE__iteration_statement = "__iteration_statement";
        
        public const string NODE__jump_statement = "__jump_statement";
        
        public const string NODE__statement_list = "__statement_list";
        
        public const string NODE__statement_no_new_scope = "__statement_no_new_scope";
        
        public const string NODE__ifLeave__ = "__ifLeave__";
        
        public const string NODE__selection_rest_statement = "__selection_rest_statement";
        
        public const string NODE__elseLeave__ = "__elseLeave__";
        
        public const string NODE__condition = "__condition";
        
        public const string NODE__switchLeave__ = "__switchLeave__";
        
        public const string NODE__switch_statement_list = "__switch_statement_list";
        
        public const string NODE__caseLeave__ = "__caseLeave__";
        
        public const string NODE__defaultLeave__ = "__defaultLeave__";
        
        public const string NODE__whileLeave__ = "__whileLeave__";
        
        public const string NODE__doLeave__ = "__doLeave__";
        
        public const string NODE__forLeave__ = "__forLeave__";
        
        public const string NODE__for_init_statement = "__for_init_statement";
        
        public const string NODE__for_rest_statement = "__for_rest_statement";
        
        public const string NODE__conditionopt = "__conditionopt";
        
        public const string NODE__continueLeave__ = "__continueLeave__";
        
        public const string NODE__breakLeave__ = "__breakLeave__";
        
        public const string NODE__returnLeave__ = "__returnLeave__";
        
        public const string NODE__discardLeave__ = "__discardLeave__";
    }
}
