using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MVVMUtilities.Types
{
    public class TransformingObservableCollection<TSource, T> : ObservableCollection<T>
    {
        readonly IEnumerable<TSource> _source;
        readonly Func<TSource, T> _transformFunc;
        bool _suppressChangedEvent;

        public TransformingObservableCollection(ObservableCollection<TSource> source, Func<TSource, T> transformFunc)
            : this(source, source, transformFunc)
        {

        }
        public TransformingObservableCollection
            (IEnumerable<TSource> source, INotifyCollectionChanged sourceChanged, Func<TSource, T> transformFunc)
        {
            _source = source;
            _transformFunc = transformFunc;

            sourceChanged.CollectionChanged += _source_CollectionChanged;
            repopulate();
        }

        void _source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            repopulate();

            OnCollectionChanged(e);
        }

        void repopulate()
        {
            _suppressChangedEvent = true;
            using (new DisposingAction(() => _suppressChangedEvent = false))
            {
                while (Count != 0)
                {
                    RemoveAt(0);
                }
                foreach (var item in _source)
                {
                    Add(_transformFunc(item));
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressChangedEvent)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
