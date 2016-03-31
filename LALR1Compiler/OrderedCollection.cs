using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 对使用者，这是一个不能控制元素顺序的集合。
    /// 对开发者，这是一个只能添加元素的集合，其元素是有序的，是按二分法插入的。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderedCollection<T> : HashCache, IEnumerable<T>
        where T : HashCache
    {
        private List<T> list = new List<T>();
        private string seperator;

        public OrderedCollection(string separator)
            : base(GetUniqueString)
        {
            this.seperator = separator;
        }

        private static string GetUniqueString(HashCache cache)
        {
            OrderedCollection<T> obj = cache as OrderedCollection<T>;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < obj.list.Count; i++)
            {
                builder.Append(obj.list[i]);
                if (i + 1 < obj.list.Count)
                {
                    builder.Append(obj.seperator);
                }
            }

            return builder.ToString();
        }

        public bool TryBinaryInsert(T item)
        {
            return this.list.TryBinaryInsert(item);
        }

        public int IndexOf(T item)
        {
            return this.list.BinarySearch(item);
        }

        public bool Contains(T item)
        {
            int index = this.list.BinarySearch(item);
            return (0 <= index && index < this.list.Count);
        }

        public T this[int index] { get { return this.list[index]; } }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in this.list)
            {
                yield return item;
            }
        }

        public int Count { get { return this.list.Count; } }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override void Dump(System.IO.StreamWriter stream)
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                this.list[i].Dump(stream);
                if (i + 1 < this.list.Count)
                {
                    stream.Write(this.seperator);
                }
            }
        }
    }
}
