using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    // http://blog.csdn.net/lanlicen/article/details/8913065
    /// <summary>
    /// 缓存一个对象的hash code。提高比较（==、!=、Equals、GetHashCode、Compare）的效率。
    /// </summary>
    public abstract class HashCache : IComparable<HashCache>, IDump2Stream
    {
        public static bool operator ==(HashCache left, HashCache right)
        {
            object leftObj = left, rightObj = right;
            if (leftObj == null)
            {
                if (rightObj == null) { return true; }
                else { return false; }
            }
            else
            {
                if (rightObj == null) { return false; }
            }

            return left.Equals(right);
        }

        public static bool operator !=(HashCache left, HashCache right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            HashCache p = obj as HashCache;
            if ((System.Object)p == null)
            {
                return false;
            }

            return this.HashCode == p.HashCode;
        }

        public override int GetHashCode()
        {
            return this.HashCode;
        }

        private Func<HashCache, string> getUniqueString;

        private bool dirty = true;

        /// <summary>
        /// 指明此cache需要更新才能用。
        /// </summary>
        public void SetDirty() { this.dirty = true; }

        private int hashCode;
        /// <summary>
        /// hash值。
        /// </summary>
        public int HashCode
        {
            get
            {
                if (this.dirty)
                {
                    this.uniqueString = getUniqueString(this);
                    this.hashCode = this.uniqueString.GetHashCode();
                    this.dirty = false;
                }

                return this.hashCode;
            }
        }

        // TODO: 功能稳定后应精简此字段的内容。
        /// <summary>
        /// 功能稳定后应精简此字段的内容。
        /// </summary>
        private string uniqueString = string.Empty;

        /// <summary>
        /// 可唯一标识该对象的字符串。
        /// 功能稳定后应精简此字段的内容。
        /// </summary>
        public string UniqueString
        {
            get
            {
                if (this.dirty)
                {
                    this.uniqueString = getUniqueString(this);
                    this.hashCode = this.uniqueString.GetHashCode();
                    this.dirty = false;
                }

                return this.uniqueString;
            }
        }

        /// <summary>
        /// 缓存一个对象的hash code。提高比较（==、!=、Equals、GetHashCode、Compare）的效率。
        /// </summary>
        /// <param name="getUniqueString">获取一个可唯一标识此对象的字符串。</param>
        public HashCache(Func<HashCache, string> getUniqueString)
        {
            if (getUniqueString == null) { throw new ArgumentNullException(); }

            this.getUniqueString = getUniqueString;
        }

        public override string ToString()
        {
            return this.UniqueString;
        }

        public int CompareTo(HashCache other)
        {
            if (other == null) { return 1; }

            return this.HashCode - other.HashCode;
        }

        /// <summary>
        /// 为了尽可能减少占用内存，提供此方法。
        /// 这样，内存中就不用同时存在太多太大的string。
        /// </summary>
        /// <param name="stream"></param>
        public abstract void Dump(System.IO.TextWriter stream);

    }
}
