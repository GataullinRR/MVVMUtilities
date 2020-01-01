using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;
using Vectors;

namespace MVVMUtilities.Types
{
    public class DoubleValueVM : ValueVM<double>
    {
        public DoubleValueVM() : this(v => true)
        {

        }

        public DoubleValueVM(Func<double, bool> valueRangeValidator, Func<double, double> coerceBeforeShow)
            : base(((Func<string, double?>)ParsingEx.TryParseToDoubleInvariant).ToOldTryParse(), mv => mv.ToStringInvariant(), valueRangeValidator, coerceBeforeShow)
        {

        }

        public DoubleValueVM(Func<double, bool> valueRangeValidator)
            : base(((Func<string, double?>)ParsingEx.TryParseToDoubleInvariant).ToOldTryParse(), mv => mv.ToStringInvariant(), valueRangeValidator)
        {

        }
        public DoubleValueVM(Interval range, bool inclusive, int maxPrecession)
            : this(v => range.Contains(v, !inclusive) && v.Round(maxPrecession) == v)
        {

        }

        public DoubleValueVM(Interval range, bool inclusive, int maxPrecession, Func<double> get, Action<double> set, INotifyPropertyChanged valueChanged)
            : this(range, inclusive, maxPrecession)
        {
            PropertyChanged += (o, e) =>
            {
                set(ModelValue);
            };
            valueChanged.PropertyChanged += (o, e) =>
            {
                ModelValue = get();
            };
        }
    }
}
