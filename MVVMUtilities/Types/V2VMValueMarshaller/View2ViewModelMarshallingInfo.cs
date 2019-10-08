using System;

namespace MVVMUtilities.Types
{
    public class View2ViewModelMarshallingInfo<TInView, TInModel>
    {
        public Func<TInView, bool> Validator { get; }
        public Func<TInView, TInModel> ForwardConverter { get; }
        public Func<TInModel, TInView> BackwardConverter { get; }
        public Func<TInModel, double> SortKeyExtractor { get; }
        public TInModel InitialValue { get; }

        public bool HasSortKey { get; }

        public View2ViewModelMarshallingInfo(
            Func<TInView, TInModel> forwardConverter,
            Func<TInModel, TInView> backwardConverter,
            TInModel initialValue)
            : this(null, forwardConverter, backwardConverter, null, initialValue)
        {

        }

        public View2ViewModelMarshallingInfo(
            Func<TInView, bool> validator,
            Func<TInView, TInModel> forwardConverter,
            Func<TInModel, TInView> backwardConverter,
            TInModel initialValue)
            : this(validator, forwardConverter, backwardConverter, null, initialValue)
        {

        }
        public View2ViewModelMarshallingInfo(
            Func<TInView, bool> validator,
            Func<TInView, TInModel> forwardConverter,
            Func<TInModel, TInView> backwardConverter,
            Func<TInModel, double> sortKeyExtractor,
            TInModel initialValue)
        {
            Validator = validator ?? (_ => true);
            ForwardConverter = forwardConverter ?? throw new ArgumentNullException();
            BackwardConverter = backwardConverter ?? throw new ArgumentNullException();
            SortKeyExtractor = sortKeyExtractor ?? (_ => default);
            InitialValue = initialValue;

            HasSortKey = sortKeyExtractor != null;
        }
    }
}
