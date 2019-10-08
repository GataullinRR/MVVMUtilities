using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Types;

namespace MVVMUtilities.Types
{
    //public class Range
    //{
    //    public enum LocationMode
    //    {
    //        FROM_END
    //    }

    //    public LocationMode Mode { get; }
    //    public int Count { get; }

    //    private Range(LocationMode mode, int count)
    //    {
    //        Mode = mode;
    //        Count = count;
    //    }

    //    public static Range FromEnd(int count)
    //    {
    //        return new Range(LocationMode.FROM_END, count);
    //    }
    //}

    //public class WindowedObservableCollectionFilter<T> : ListBase<T, ObservableCollection<T>>, INotifyCollectionChanged
    //{
    //    public event NotifyCollectionChangedEventHandler CollectionChanged;

    //    /// <summary>
    //    /// Max amount of items
    //    /// </summary>
    //    public Range Window { get; }

    //    public WindowedObservableCollectionFilter(Range window)
    //        : base(new ObservableCollection<T>(new List<T>(window.Count)))
    //    {
    //        Window = window;
    //        _baseCollection.CollectionChanged += _baseCollection_CollectionChanged;
    //    }
    //    void _baseCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        CollectionChanged?.Invoke(this, e);
    //    }

    //    public override void Add(T item)
    //    {
    //        throw new NotSupportedException();
    //    }

    //    public override void Insert(int index, T item)
    //    {
    //        throw new NotSupportedException();
    //    }

    //    public DisplaceCollection<T> Fill(T value)
    //    {
    //        throw new NotSupportedException();
    //    }
    //}
}
