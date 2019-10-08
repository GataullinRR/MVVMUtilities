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
    public class NotificationDispatcher : INotifyPropertyChanged
    {
        class Association
        {
            public INotifyPropertyChanged Source { get; set; }
            public string SourcePropName { get; set; }
            public string DestinationPropName { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        List<Association> _associations = new List<Association>();

        public void AddAggeration(INotifyPropertyChanged source, string sourcePropName)
        {
            AddAggeration(source, sourcePropName, sourcePropName);
        }
        public void AddAggeration(INotifyPropertyChanged source, string sourcePropName, string destinationName)
        {
            _associations.Add(
                new Association()
                {
                    Source = source,
                    SourcePropName = sourcePropName,
                    DestinationPropName = destinationName
                });
            source.PropertyChanged += (o, e) => Source_PropertyChanged(o, source, e.PropertyName);
        }

        void Source_PropertyChanged(object sender, INotifyPropertyChanged source, string sourcePropName)
        {
            foreach (var association in _associations)
            {
                if (association.Source == source)
                {
                    if (association.SourcePropName == sourcePropName)
                    {
                        PropertyChanged.Invoke(sender, association.DestinationPropName);
                    }
                }
            }
        }
    }
}
