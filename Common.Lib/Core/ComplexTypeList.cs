using System.Collections;

namespace Common.Lib.Core
{
    public class ComplexTypeList<T> : ComplexType, ICollection<T> where T : ComplexType
    {

        protected List<T> Items = new List<T>();

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public bool IsReadOnly => false;

        public virtual void Add(T item)
        {
            if (!Contains(item))
            {
                item.Setter = Setter;
                Items.Add(item);
            }
        }

        public virtual void Clear()
        {
            Items.Clear();
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }
        
        public virtual bool Remove(T item)
        {
            if (Contains(item))
            {
                var output = Items.Remove(item);
                return output;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
