using System;
using Utilities.Extensions;

namespace MVVMUtilities.Types
{
    /// <summary>
    /// Immutable
    /// </summary>
    public class DoubleMarshallingInfo : View2ViewModelMarshallingInfo<string, double>
    {
        public DoubleMarshallingInfo(
            Func<double, double> forwardTransformer,
            Func<double, double> backwardTransformer)
            : this(v => true, forwardTransformer, backwardTransformer, v => v, default) { }
        public DoubleMarshallingInfo(
            Func<double, double> forwardTransformer,
            Func<double, double> backwardTransformer,
            Func<double, bool> validator)
            : this(validator, forwardTransformer, backwardTransformer, v => v, default) { }
        public DoubleMarshallingInfo(
            Func<double, bool> validator,
            Func<double, double> forwardTransformer,
            Func<double, double> backwardTransformer,
            Func<double, double> sortKeyExtractor,
            double initialValue)
            :base(s =>
                {
                    var v = s.TryParseToDoubleInvariant();
                    return v.HasValue ? validator(v.Value) : false;
                }, 
                 s => forwardTransformer(s.TryParseToDoubleInvariant().Value),
                 s => backwardTransformer(s).ToStringInvariant(),
                 sortKeyExtractor,
                 initialValue)
        {

        }
    }
}
