using System.Collections;

namespace Common.Lib.Client.UI
{
    public class ItemsList<T> : IEnumerable<ListItem<T>>
    {
        public bool AllowMultipleSelect { get; set; } = false;

        public List<ListItem<T>> Items { get; set; } = new List<ListItem<T>>();

        public void AddItem(T item, string selectedClass = "", string unselectedClass = "", bool isSelected = false) 
        { 
            Items.Add(new ListItem<T>()
            {
                Index = Items.Count,
                SelectedClass = selectedClass, 
                UnselectedClass = unselectedClass,
                Object = item,
                OnItemSelected = UnselectAllButOne,
                IsSelected = isSelected
            }); 
        }


        public void UnselectAllButOne(ListItem<T> selectedItem)
        {
            if (AllowMultipleSelect)
                return;

            foreach (var item in Items)
            {
                if (item != selectedItem)
                    item.Unselect();
            }
        }

        public IEnumerator<ListItem<T>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }

    public class ListItem<T>
    {
        public int Index { get; set; }
        public string SelectedClass { get; set; } = string.Empty;

        public string UnselectedClass { get; set; } = string.Empty;

        public string CurrentClass
        {
            get
            {
                return IsSelected ? SelectedClass : UnselectedClass;
            }
        }

        public Action<ListItem<T>> OnItemSelected { get; set; }

        public T? Object { get; set; }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (_isSelected && OnItemSelected != null)
                    OnItemSelected(this);
            }
        }
        bool _isSelected;

        public void Unselect()
        {
            _isSelected = false;
        }
    }

    public class ValueItem
    {
        public object? Value { get; set; }

        public string Name
        {
            get
            {
                return Value.ToString();
            }
        }

        public ValueItem(object o)
        {
            Value = o;
        }
    }
}
