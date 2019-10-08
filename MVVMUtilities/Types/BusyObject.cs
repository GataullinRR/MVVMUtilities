using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.Threading;
using System.Diagnostics;

namespace MVVMUtilities.Types
{
    public class BusyObject : NotifiableObjectTemplate
    {
        public static implicit operator bool(BusyObject busy)
        {
            return busy.IsBusy;
        }

        readonly SemaphoreSlim _notifier = new SemaphoreSlim(1);
        int _numOfActiveHolders = 0;

        public bool IsBusy
        {
            get => _propertyHolder.Get(() => false);
            private set => _propertyHolder.SetOnlyIfDifferent(value);
        }
        public bool IsNotBusy => !IsBusy;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable BusyHolder
        {
            get
            {
                IsBusy = true;
                _numOfActiveHolders++;
                if (_notifier.CurrentCount == 0)
                {
                    _notifier.Acquire();
                }

                return new DisposingAction(deactivate);

                void deactivate()
                {
                    _numOfActiveHolders = (_numOfActiveHolders - 1).NegativeToZero();
                    _notifier.Release(1);
                    if (_numOfActiveHolders == 0)
                    {
                        IsBusy = false;
                    }
                }
            }
        }

        public BusyObject()
        {
            _propertyHolder.CreateDependency(nameof(IsNotBusy), nameof(IsBusy));
        }

        public async Task WaitAsync() => (await _notifier.AcquireAsync()).Dispose();
    }
}
