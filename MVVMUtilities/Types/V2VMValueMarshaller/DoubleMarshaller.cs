using System;
using Utilities.Extensions;

namespace MVVMUtilities.Types
{
    public class DoubleMarshaller : View2ViewModelValueMarshaller<string, double>
    {
        public DoubleMarshaller(double initialValue)
            : this(initialValue, (string)null, v => v) { }
        public DoubleMarshaller(double initialValue, string formatForBackwardConvertion)
            : this(initialValue, formatForBackwardConvertion, v => v) { }
        public DoubleMarshaller(double initialValue, string formatForBackwardConvertion, Func<double, double> sortKeyExtractor)
            : base(new View2ViewModelMarshallingInfo<string, double>(
                s => s.TryParseToDoubleInvariant().HasValue,
                s => s.TryParseToDoubleInvariant().Value,
                v => formatForBackwardConvertion == null
                    ? v.ToStringInvariant() : v.ToStringInvariant(formatForBackwardConvertion),
                sortKeyExtractor,
                initialValue))
        {
        }

        public DoubleMarshaller(
            double initialValue,
            Func<double, double> forwardTransformer,
            Func<double, double> backwardTransformer)
            : base(new View2ViewModelMarshallingInfo<string, double>(
                s => s.TryParseToDoubleInvariant().HasValue,
                s => forwardTransformer(s.TryParseToDoubleInvariant().Value),
                v => backwardTransformer(v).ToStringInvariant(),
                v => v,
                initialValue))
        {
        }
        public DoubleMarshaller(double initialValue, DoubleMarshallingInfo marshallingInfo)
            : base(new View2ViewModelMarshallingInfo<string, double>(
                marshallingInfo.Validator,
                marshallingInfo.ForwardConverter,
                marshallingInfo.BackwardConverter,
                marshallingInfo.SortKeyExtractor,
                initialValue))
        {
        }

        public DoubleMarshaller(
            double initialValue,
            Func<double, bool> validator)
            : base(new View2ViewModelMarshallingInfo<string, double>(
                s => { var v = s.TryParseToDoubleInvariant(); return v.HasValue ? validator(v.Value) : false; },
                s => s.TryParseToDoubleInvariant().Value,
                v => v.ToStringInvariant(),
                v => v,
                initialValue))
        {
        }
    }
}
