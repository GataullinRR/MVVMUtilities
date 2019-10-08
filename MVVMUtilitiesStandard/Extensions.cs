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
            source.PropertyChanged += (o, e) =>
            {
                var t = target();
                foreach (var property in properties.ToEmptyIfTrue(t == null))
                {
                    t.Invoke(sender, new PropertyChangedEventArgs(property));
                }
            };
        }

        public static void BroadcastInvoke(this PropertyChangedEventHandler handler, object sender)
        {
            handler.Invoke(sender, null);
        }
    }
}
