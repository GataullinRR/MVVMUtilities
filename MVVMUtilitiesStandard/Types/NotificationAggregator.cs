using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace MVVMUtilities.Types
{
    public class NotificationAggregator : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public NotificationAggregator(params INotifyPropertyChanged[] sources)
        {
            foreach (var source in sources)
            {
                source.PropertyChanged += (o, e) =>
                {
                    PropertyChanged?.Invoke(o, e);
                };
            }
        }
    }
}
