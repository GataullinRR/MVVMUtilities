using System;
using Utilities.Extensions;

namespace MVVMUtilities.Types
{
    public class Int32Marshaller : View2ViewModelValueMarshaller<string, int>
    {
        public int MinValue, MaxValue;

        public Int32Marshaller()
            : this(0)
        {
        }
        public Int32Marshaller(int initialValue)
            : this(initialValue, v => true)
        {
        }
        public Int32Marshaller(int initialValue, Func<int, bool> validator)
            : base(new View2ViewModelMarshallingInfo<string, int>(
                s =>
                {
                    var parsed = s.TryParseToInt32Invariant();
                    return parsed.HasValue && validator(parsed.Value);
                },
                s => s.TryParseToInt32Invariant().Value,
                v => v.ToStringInvariant(),
                v => v,
                initialValue))
        {
        }
    }
}
