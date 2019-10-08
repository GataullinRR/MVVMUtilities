namespace MVVMUtilities.Types
{
    public class ObjectMarshaller : View2ViewModelValueMarshaller<string, object>
    {
        public ObjectMarshaller(View2ViewModelMarshallingInfo<string, object> marshallingInfo) 
            : base(marshallingInfo)
        {
        }
    }

    public class ObjectMarshaller<T> : View2ViewModelValueMarshaller<string, T>
    {
        public ObjectMarshaller(View2ViewModelMarshallingInfo<string, T> marshallingInfo)
            : base(marshallingInfo)
        {
        }
    }
}
