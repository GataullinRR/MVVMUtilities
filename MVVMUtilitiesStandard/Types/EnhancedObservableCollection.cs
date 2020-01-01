using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace MVVMUtilities.Types
{
    public class ItemPropertyChangedEventArgs<T> : EventArgs
    {
        public PropertyChangedEventArgs PropertyChangedEventArgs { get; }
        public T Item { get; }

        public ItemPropertyChangedEventArgs(PropertyChangedEventArgs propertyChangedEventArgs, T item)
        {
            PropertyChangedEventArgs = propertyChangedEventArgs;
            Item = item;
        }
    }

    public class CollectionOptions
    {
        public bool NotifyOnItemPropertyChanges { get; } = true;

        internal CollectionOptions()
        {

        }

        public CollectionOptions(bool notifyOnItemPropertyChanges)
        {
            NotifyOnItemPropertyChanges = notifyOnItemPropertyChanges;
        }
    }

    public interface ISlimEnhancedObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        event EventHandler<ItemPropertyChangedEventArgs<T>> ItemPropertyChanged;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDisposable EventSuppressingMode { get; }
        /// <summary>
        /// After holder is released the <see cref="ItemPropertyChanged"/> event will not be raised
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDisposable ItemChangesEventIgnoringModeHolder { get; }
        /// <summary>
        /// After holder is released the <see cref="ItemPropertyChanged"/> event will be raised for all changed items which are still exist in the collection
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDisposable ItemChangesEventSuppressingModeHolder { get; }
    }

    public interface IEnhancedObservableCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList, INotifyCollectionChanged, INotifyPropertyChanged, ISlimEnhancedObservableCollection<T>
    {

    }

    public class EnhancedObservableCollection<T> : ObservableCollection<T>, IEnhancedObservableCollection<T>
    {
        readonly ModeManager _suppressNotification = new ModeManager();
        readonly ModeManager _ignoreItemPropertyChangedNotifications = new ModeManager();
        readonly ModeManager _suppressItemPropertyChangedNotifications = new ModeManager();
        readonly List<ItemPropertyChangedEventArgs<T>> _awaitingItemPropertyChangedEvents = new List<ItemPropertyChangedEventArgs<T>>();

        public event EventHandler<ItemPropertyChangedEventArgs<T>> ItemPropertyChanged;

        readonly CollectionOptions _options;
        public bool IsReadOnly { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable EventSuppressingMode => _suppressNotification.Holder;
        /// <summary>
        /// After holder is released the <see cref="ItemPropertyChanged"/> event will not be raised
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable ItemChangesEventIgnoringModeHolder => _ignoreItemPropertyChangedNotifications.Holder;
        /// <summary>
        /// After holder is released the <see cref="ItemPropertyChanged"/> event will be raised for all changed items which are still exist in the collection
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable ItemChangesEventSuppressingModeHolder => _suppressItemPropertyChangedNotifications.Holder;

        public EnhancedObservableCollection()
            : this(new CollectionOptions())
        {

        }
        public EnhancedObservableCollection(CollectionOptions options)
            : this(new T[0], options)
        {

        }
        public EnhancedObservableCollection(IEnumerable<T> collection)
            : this(collection, new CollectionOptions())
        {

        }
        public EnhancedObservableCollection(IEnumerable<T> collection, CollectionOptions options)
            : base(collection)
        {
            _options = options;
            collection?.ForEach(subscribeToPropertyChanged);

            _suppressNotification.Deactivated +=
                () => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            _suppressItemPropertyChangedNotifications.Deactivated +=
                () =>
                {
                    foreach (var e in _awaitingItemPropertyChangedEvents)
                    {
                        if (this.Contains(e.Item))
                        {
                            ItemPropertyChanged?.Invoke(this, e);
                        }
                    }
                    _awaitingItemPropertyChangedEvents.Clear();
                };
        }

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var eventArgs = new ItemPropertyChangedEventArgs<T>(e, (T)sender);

            if (!_suppressItemPropertyChangedNotifications.IsActive && !_ignoreItemPropertyChangedNotifications.IsActive)
            {
                ItemPropertyChanged?.Invoke(this, eventArgs);
            }
            else if (_suppressItemPropertyChangedNotifications.IsActive && !_ignoreItemPropertyChangedNotifications.IsActive)
            {
                _awaitingItemPropertyChangedEvents.Add(eventArgs);
            }
            else if (_suppressItemPropertyChangedNotifications.IsActive && _ignoreItemPropertyChangedNotifications.IsActive)
            {
                throw new NotSupportedException("Setting ignoring and supressing mode simultaneously is not supported");
            }
            else
            {
                // Ignoring
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification.IsActive)
            {

                base.OnCollectionChanged(e);
            }
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            using (EventSuppressingMode)
            {
                foreach (T item in list)
                {
                    Add(item);
                }
            }
        }

        public void MakeReadOnly()
        {
            IsReadOnly = true;
        }

        protected override void SetItem(int index, T item)
        {
            throwIfReadOnly();

            unsubscribeFromPropertyChanged(index);

            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            throwIfReadOnly();

            this.ForEach(unsubscribeFromPropertyChanged);

            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            throwIfReadOnly();

            subscribeToPropertyChanged(item);

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            throwIfReadOnly();

            unsubscribeFromPropertyChanged(index);

            base.RemoveItem(index);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throwIfReadOnly();

            base.MoveItem(oldIndex, newIndex);
        }

        void throwIfReadOnly()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException();
            }
        }

        void unsubscribeFromPropertyChanged(int itemIndex)
        {
            if (_options.NotifyOnItemPropertyChanges)
            {
                if (itemIndex < Count)
                {
                    unsubscribeFromPropertyChanged(this[itemIndex]);
                }
            }
        }
        void unsubscribeFromPropertyChanged(T item)
        {
            if (_options.NotifyOnItemPropertyChanges)
            {
                if (item is INotifyPropertyChanged changeable)
                {
                    changeable.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }
        void subscribeToPropertyChanged(T item)
        {
            if (_options.NotifyOnItemPropertyChanges)
            {
                if (item is INotifyPropertyChanged changeable)
                {
                    changeable.PropertyChanged += Item_PropertyChanged;
                }
            }
        }
    }
}
