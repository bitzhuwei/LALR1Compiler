<Start> ::= <Vn> "::=" <VList> ";" <PList>;
<PList> ::= <Vn> "::=" <VList> ";" <PList> | null;
<VList> ::= <V> <VOpt>;
<V> ::= <Vn> | <Vt>;
<VOpt> ::= <V> <VOpt> | "|" <V> <VOpt> | null;
<Vn> ::= "<" identifier ">";
<Vt> ::= "null" | "identifier" | "number" | "constString" | constString;

这是文法的文法。用于制作LALR(1)编译器的自动生成工具。
