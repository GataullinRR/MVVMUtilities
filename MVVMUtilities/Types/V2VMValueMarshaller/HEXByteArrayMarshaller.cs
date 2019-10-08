using System;
using System.Linq;
using Utilities;
using Utilities.Extensions;

namespace MVVMUtilities.Types
{
    public class HEXByteArrayMarshaller : View2ViewModelValueMarshaller<string, byte[]>
    {
        public HEXByteArrayMarshaller(byte[] initialValue, bool allowEmptyArrays)
            : base(new View2ViewModelMarshallingInfo<string, byte[]>(
                s =>
                {
                    var parsingResults = s
                        .Split(" ")
                        .Select(hex => CommonUtils.Try(() => Convert.ToByte(hex, 16)))
                        .ToArray();
                    return parsingResults.All(ok => ok) && (allowEmptyArrays ? true : parsingResults.Length > 0);
                },
                s => s.Split(" ").Select(hex => (byte)Convert.ToByte(hex, 16)).ToArray(),
                v => v.Select(b=>b.ToString("X2")).AsString(" "),
                v => v.Length,
                initialValue))
        {
        }
    }
}
