using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace MVVMUtilities.Types
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T Old { get; }
        public T New { get; }

        public ValueChangedEventArgs(T old, T @new)
        {
            Old = old;
            New = @new;
        }
    }

    public class View2ViewModelValueMarshaller<TInView, TInModel>
        : NotifiableObjectTemplate, IDataErrorInfo, IViewValueProvider<TInView>
    {
        public static implicit operator TInModel(View2ViewModelValueMarshaller<TInView, TInModel> marshaller)
        {
            return marshaller.ModelValue;
        }

        readonly View2ViewModelMarshallingInfo<TInView, TInModel> _marshallingInfo;
        bool _valueChecked = true;

        public event EventHandler<ValueChangedEventArgs<TInView>> ValueChanged;
        public event EventHandler<ValueChangedEventArgs<TInModel>> ModelValueChanged;

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (columnName == nameof(Value))
                {
                    var isValid = checkValue();
                    if (!isValid)
                    {
                        return "Validation error";
                    }
                }

                return "";
            }
        }
        string IDataErrorInfo.Error => null;

        /// <summary>
        /// Для View
        /// </summary>
        public TInView Value
        {
            get => _propertyHolder.Get(() => default(TInView));
            set
            {
                _valueChecked = false;
                _propertyHolder.Set(value);
            }
        }
        
        /// <summary>
        /// Для ViewModel
        /// </summary>
        public TInModel ModelValue
        {
            get
            {
                if (!_valueChecked)
                {
                    checkValue();
                }

                return _propertyHolder.Get(() => default(TInModel));
            }
            private set => _propertyHolder.Set(value);
        }
        public double SortKey
        {
            get => _marshallingInfo.SortKeyExtractor(ModelValue);
        }
        public TInView AcceptedValue
        {
            get => _marshallingInfo.BackwardConverter(ModelValue);
        }

        object IViewValueProvider.Value
        {
            get => Value;
            set => Value = (TInView)value;
        }
        object IViewValueProvider.AcceptedValue => AcceptedValue;

        public Func<TInView, bool> Validator => _marshallingInfo.Validator;

        bool checkValue()
        {
            _valueChecked = true;
            var valid = Validator(Value);
            if (valid)
            {
                ModelValue = _marshallingInfo.ForwardConverter(Value);
            }

            return valid;
        }

        public View2ViewModelValueMarshaller(View2ViewModelMarshallingInfo<TInView, TInModel> marshallingInfo)
        {
            _marshallingInfo = marshallingInfo ?? throw new ArgumentNullException();

            if (marshallingInfo.HasSortKey)
            {
                _propertyHolder.CreateDependency(nameof(SortKey), nameof(ModelValue));
            }

            Assert(marshallingInfo.InitialValue);

            _propertyHolder.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(Value))
                {
                    var ea = new ValueChangedEventArgs<TInView>((TInView)e.OldValue, (TInView)e.NewValue);
                    ValueChanged?.Invoke(this, ea);
                }
                else if (e.PropertyName == nameof(ModelValue))
                {
                    var ea = new ValueChangedEventArgs<TInModel>((TInModel)e.OldValue, (TInModel)e.NewValue);
                    ModelValueChanged?.Invoke(this, ea);
                }
            };
            _propertyHolder.CreateDependency(nameof(AcceptedValue), nameof(Value));
        }
        public void AssertModelValue()
        {
            Assert(ModelValue);
        }
        public void Assert(TInModel value)
        {
            ModelValue = value;
            Value = _marshallingInfo.BackwardConverter(value);
        }
    }
}
