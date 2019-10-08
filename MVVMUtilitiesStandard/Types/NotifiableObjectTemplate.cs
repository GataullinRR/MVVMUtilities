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
    [Serializable]
    public abstract class NotifiableObjectTemplate : INotifyPropertyChanged
    {
        protected readonly NotifiablePropertyHolder _propertyHolder;

        public event PropertyChangedEventHandler PropertyChanged;

        public NotifiableObjectTemplate()
        {
            _propertyHolder = new NotifiablePropertyHolder(() => PropertyChanged, this);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, propertyName);
        }
    }
}
