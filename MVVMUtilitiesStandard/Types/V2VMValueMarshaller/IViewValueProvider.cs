using System.ComponentModel;

namespace MVVMUtilities.Types
{
    public interface IViewValueProvider
    {
        object Value { get; set; }
        /// <summary>
        /// It's view's representation of the current view model's value. It can differ from <seealso cref="Value"/> 
        /// if validation fails.
        /// </summary>
        object AcceptedValue { get; }
        double SortKey { get; }
    }

    public interface IViewValueProvider<T> : IViewValueProvider, INotifyPropertyChanged
    {
        new T Value { get; set; }
        /// <summary>
        /// It's view's representation of the current view model's value. It can differ from <seealso cref="Value"/> 
        /// if validation fails.
        /// </summary>
        new T AcceptedValue { get; }
    }
}
