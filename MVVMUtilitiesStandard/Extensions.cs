using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace MVVMUtilities
{
    public static class Extensions
    {
        public static void RedirectAnyChangesTo(
            this INotifyPropertyChanged source,
            object sender,
            Func<PropertyChangedEventHandler> target,
            params string[] properties)
        {
            RedirectAnyChangesTo(source, p => target()?.Invoke(sender, new PropertyChangedEventArgs(p)), properties);
        }
        public static void RedirectAnyChangesTo(
            this INotifyPropertyChanged source,
            object sender,
            Action<object, PropertyChangedEventArgs> onPropertyChanged,
            params string[] properties)
        {
            RedirectAnyChangesTo(source, p => onPropertyChanged(sender, new PropertyChangedEventArgs(p)), properties);
        }
        public static void RedirectAnyChangesTo(
            this INotifyPropertyChanged source,
            Action<string> onPropertyChanged,
            params string[] properties)
        {
            source.PropertyChanged += (o, e) =>
            {
                foreach (var property in properties)
                {
                    onPropertyChanged.Invoke(property);
                }
            };
        }

        public static void WhenChanged(
            this PropertyChangedEventHandler source,
            Action<string> onPropertyChanged,
            params string[] properties)
        {
            source += (o, e) =>
            {
                var property = properties.FindOrDefault(p => p == e.PropertyName);
                if (property != null)
                {
                    onPropertyChanged.Invoke(property);
                }
            };
        }

        public static void BroadcastInvoke(this PropertyChangedEventHandler handler, object sender)
        {
            handler.Invoke(sender, null);
        }
    }
}
