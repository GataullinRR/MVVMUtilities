using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace MVVMUtilities.Types
{
    public class ValueBinder<TModel> 
    {
        public static implicit operator ValueBinder<object>(ValueBinder<TModel> binder)
        {
            return new ValueBinder<object>(
                tryViewToModel,
                mv => binder.ModelToView(castToModel(mv)),
                mv => binder.ValueRangeValidator(castToModel(mv)),
                mv => (object)binder.CoerceBeforeShow(castToModel(mv)),
                binder.Validator);

            TModel castToModel(object mv)
            {
                return (TModel)Convert.ChangeType(mv, typeof(TModel));
            }


            bool tryViewToModel(string serialized, out object modelValue)
            {
                var isOk = binder.TryViewToModel(serialized, out TModel mv);
                modelValue = (object)mv;

                return isOk;
            }
        }   

        class Parser
        {
            readonly Func<string, TModel> _parser;

            public Parser(Func<string, TModel> parser)
            {
                _parser = parser;
            }

            public bool TryParse(string value, out TModel modelValue)
            {
                try
                {
                    modelValue = _parser(value);

                    return true;
                }
                catch
                {
                    modelValue = default;

                    return false;
                }
            }
        }

        public CommonEx.TryParseDelegate<TModel> TryViewToModel { get; }
        public Func<TModel, string> ModelToView { get; }
        public Func<TModel, bool> ValueRangeValidator { get; }
        public Func<TModel, TModel> CoerceBeforeShow { get; }
        public Func<string, bool> Validator { get; }

        public ValueBinder(Func<string, TModel> viewToModel, Func<TModel, string> modelToView, Func<TModel, bool> valueRangeValidator)
            : this(new Parser(viewToModel).TryParse, modelToView, valueRangeValidator)
        {

        }
        public ValueBinder(CommonEx.TryParseDelegate<TModel> viewToModel, Func<TModel, string> modelToView, Func<TModel, bool> valueRangeValidator)
            : this(viewToModel, modelToView, valueRangeValidator, v => v)
        {

        }
        public ValueBinder(
            CommonEx.TryParseDelegate<TModel> viewToModel,
            Func<TModel, string> modelToView,
            Func<TModel, bool> valueRangeValidator,
            Func<TModel, TModel> coerceBeforeShow)
        {
            TryViewToModel = viewToModel ?? throw new ArgumentNullException(nameof(viewToModel));
            ModelToView = modelToView ?? throw new ArgumentNullException(nameof(modelToView));
            ValueRangeValidator = valueRangeValidator ?? throw new ArgumentNullException(nameof(valueRangeValidator));
            CoerceBeforeShow = coerceBeforeShow ?? throw new ArgumentNullException(nameof(coerceBeforeShow));
            Validator = viewValue =>
            {
                var isOk = viewToModel(viewValue, out TModel modelValue);
                if (isOk)
                {
                    isOk &= ValueRangeValidator(modelValue);
                }

                return isOk;
            };
        }

        ValueBinder(
            CommonEx.TryParseDelegate<TModel> tryViewToModel, 
            Func<TModel, string> modelToView, 
            Func<TModel, bool> valueRangeValidator, 
            Func<TModel, TModel> coerceBeforeShow, 
            Func<string, bool> validator)
        {
            TryViewToModel = tryViewToModel ?? throw new ArgumentNullException(nameof(tryViewToModel));
            ModelToView = modelToView ?? throw new ArgumentNullException(nameof(modelToView));
            ValueRangeValidator = valueRangeValidator ?? throw new ArgumentNullException(nameof(valueRangeValidator));
            CoerceBeforeShow = coerceBeforeShow ?? throw new ArgumentNullException(nameof(coerceBeforeShow));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }
    }

    public interface IEditableViewValueProvider : IDataErrorInfo, INotifyPropertyChanged
    {
        string Value { get; set; }
    }

    public class ValueVM<TModel> : INotifyPropertyChanged, IEditableViewValueProvider
    {
        class Parser
        {
            readonly Func<string, TModel> _parser;

            public Parser(Func<string, TModel> parser)
            {
                _parser = parser;
            }

            public bool TryParse(string value, out TModel modelValue)
            {
                try
                {
                    modelValue = _parser(value);

                    return true;
                }
                catch
                {
                    modelValue = default;

                    return false;
                }
            }
        }

        readonly CommonEx.TryParseDelegate<TModel> _tryViewToModel;
        readonly Func<TModel, string> _modelToView;

        public event PropertyChangedEventHandler PropertyChanged;

        string value;
        public string Value
        {
            get => value;
            set
            {
                if (IsEditable)
                {
                    if (_tryViewToModel(value, out TModel modelValue) &&
                        _valueRangeValidator(modelValue))
                    {
                        ModelValue = modelValue;
                    }

                    this.value = value;
                }
            }
        }


        TModel modelValue;
        public TModel ModelValue
        {
            get => modelValue;
            set
            {
                modelValue = value;
                updateValueFromModel();

                Set?.Invoke(modelValue);
            }
        }
        void updateValueFromModel()
        {
            value = _modelToView(_coerceBeforeShow(ModelValue));
            PropertyChanged?.Invoke(this, nameof(Value));
        }

        public Func<string, bool> Validator { get; }
        readonly Func<TModel, bool> _valueRangeValidator;

        public Action<TModel> Set { get; set; }
        public bool IsEditable { get; set; } = true;

        public string Error => "Error";
        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Value) && Validator != null)
                {
                    if (!Validator(Value))
                    {
                        return "Error";
                    }
                }

                return "";
            }
        }

        readonly Func<TModel, TModel> _coerceBeforeShow;

        public ValueVM(Func<string, TModel> viewToModel, Func<TModel, string> modelToView, Func<TModel, bool> valueRangeValidator)
            : this(new Parser(viewToModel).TryParse, modelToView, valueRangeValidator)
        {

        }
        public ValueVM(CommonEx.TryParseDelegate<TModel> viewToModel, Func<TModel, string> modelToView, Func<TModel, bool> valueRangeValidator)
            :this(viewToModel, modelToView, valueRangeValidator, v => v)
        {

        }
        public ValueVM(
            CommonEx.TryParseDelegate<TModel> viewToModel, 
            Func<TModel, string> modelToView, 
            Func<TModel, bool> valueRangeValidator, 
            Func<TModel, TModel> coerceBeforeShow)
        {
            _tryViewToModel = viewToModel ?? throw new ArgumentNullException(nameof(viewToModel));
            _modelToView = modelToView ?? throw new ArgumentNullException(nameof(modelToView));
            _valueRangeValidator = valueRangeValidator ?? throw new ArgumentNullException(nameof(valueRangeValidator));
            _coerceBeforeShow = coerceBeforeShow ?? throw new ArgumentNullException(nameof(coerceBeforeShow));
            Validator = viewValue =>
            {
                var isOk = viewToModel(viewValue, out TModel modelValue);
                if (isOk)
                {
                    isOk &= _valueRangeValidator(modelValue);
                }

                return isOk;
            };
        }

        public void AssertValue()
        {
            updateValueFromModel();
        }
    }
}