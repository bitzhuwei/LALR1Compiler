using LALR1Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextfreeGrammarCompiler.Test
{
    /// <summary>
    /// 文法中用""引起来的都是关键字（"int"等）或关键符号（"<<="等）
    /// </summary>
    abstract class Symbol : HashCache, IComparable<Symbol>
    {

        public Symbol() : base(GetUniqueString) { }

        public abstract int CompareTo(Symbol other);

        private static string GetUniqueString(HashCache cache)
        {
            Symbol obj = cache as Symbol;
            return obj.Dump();
        }
    }

    /// <summary>
    /// 由标点符号组成（"/=","//","/*"）
    /// </summary>
    class Punctuation : Symbol
    {
        /// <summary>
        /// 由标点符号组成（"/=","//","/*"）
        /// </summary>
        /// <param name="value"></param>
        public Punctuation(string value)
        {
            if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException(); }

            this.Value = value;
        }

        public string Value { get; set; }

        public override int CompareTo(Symbol other)
        {
            var p = other as Punctuation;
            if ((object)p == null) { return -1; }

            return -(this.Value.Length - p.Value.Length);
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write("Punctuation: {0}", this.Value);
        }
    }

    /// <summary>
    /// ContextfreeGrammar的关键字（identifier，number，constString）
    /// </summary>
    class ContextfreeGrammarKeyword : Symbol
    {
        public ContextfreeGrammarKeyword(EKeyword keyword)
        {
            if (keyword == EKeyword.KEY_null) { throw new ArgumentException(); }

            this.Keyword = keyword;
        }

        public EKeyword Keyword { get; set; }

        public enum EKeyword
        {
            KEY_null, KEY_identifier, KEY_number, KEY_constString, KEY_char,
        }

        public override int CompareTo(Symbol other)
        {
            var p = other as ContextfreeGrammarKeyword;
            if ((object)p == null) { return -1; }

            return -((int)(this.Keyword) - (int)(p.Keyword));
        }

        public override void Dump(System.IO.TextWriter stream)
        {
            stream.Write("Keyword: {0}", this.Keyword.ToString().Substring(4));
        }
    }


    class SymbolList : OrderedCollection<Symbol>
    {
        public SymbolList() : base(", ") { }
    }
}
