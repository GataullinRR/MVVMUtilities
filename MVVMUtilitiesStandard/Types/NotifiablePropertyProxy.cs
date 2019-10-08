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
    public class NotifiablePropertyGetAccessor: INotifyPropertyChanged
    {
        object _sender;
        string _propertyName;
        Func<dynamic> _get;

        public event PropertyChangedEventHandler PropertyChanged;

        internal NotifiablePropertyGetAccessor(Func<dynamic> getter, object sender, string propertyName)
        {
            _get = getter;
            _sender = sender;
            _propertyName = propertyName;
        }

        internal void NotifyChanged()
        {
            PropertyChanged?.Invoke(_sender, _propertyName);
        }

        public dynamic Get()
        {
            return _get();
        }
    }
}
