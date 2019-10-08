using System;
using System.Diagnostics;
using Utilities.Extensions;
using Utilities.Types;

namespace MVVMUtilities.Types
{
    public class HoldersManager
    {
        int _activeHoldersCount = 0;
        public event Action Unholded;
        public bool IsHolding { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable Holder
        {
            get
            {
                IsHolding = true;
                _activeHoldersCount++;

                return new DisposingAction(restore);

                void restore()
                {
                    _activeHoldersCount--;
                    if (_activeHoldersCount <= 0)
                    {
                        IsHolding = false;
                        Unholded?.Invoke();
                    }
                }
            }
        }
    }
}
