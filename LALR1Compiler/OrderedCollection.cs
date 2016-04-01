using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LALR1Compiler
{
    /// <summary>
    /// 经过优化的列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
    /// 对于LALR(1)Compiler项目，只需支持“添加元素”的功能，所以我没有写修改和删除元素的功能。
    /// </summary>
    /// <typeparam name="T">元素也要支持快速比较。</typeparam>
    public class OrderedCollection<T> :
        HashCache // 快速比较两个OrderedCollection<T>是否相同。
        , IEnumerable<T> // 可枚举该集合的元素。
        where T : HashCache // 元素也要支持快速比较。
    {
        private List<T> list = new List<T>();
        private string seperator = Environment.NewLine;

        /// <summary>
        /// 这是一个只能添加元素的集合，其元素是有序的，是按二分法插入的。
        /// 但是使用者不能控制元素的顺序。
        /// </summary>
        /// <param name="separator">在Dump到流时用什么分隔符分隔各个元素？</param>
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

        public bool TryInsert(T item)
        {
            if (this.list.TryBinaryInsert(item))
            {
                this.SetDirty();
                return true;
            }
            else
            {
                return false;
            }
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
