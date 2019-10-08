using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace MVVMUtilities.Types
{
    [Serializable]
    public class RichPropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public RichPropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    [Serializable]
    public class NotifiablePropertyHolder
    {
        readonly Dictionary<string, dynamic> _properties = new Dictionary<string, dynamic>();
        // <свойство, зависимые от него свойства>
        readonly Dictionary<string, List<string>> _dependencies = new Dictionary<string, List<string>>(); 

        readonly object _sender;
        readonly Func<PropertyChangedEventHandler> _handler;

        /// <summary>
        /// Вызывается до ассоциированного с классом PropertyChangedEventHandler
        /// </summary>
        public event EventHandler<RichPropertyChangedEventArgs> PropertyChanged = delegate { };

        public NotifiablePropertyHolder(Func<PropertyChangedEventHandler> handler, object sender)
        {
            _handler = handler;
            _sender = sender;
        }

        /// <summary>
        /// Doesn't raise event if the value being set the same as the old value set. If value is a ValueType
        /// then for testing equality <see cref="object.Equals(object)"/> is used. If reference type
        /// then <see cref="object.ReferenceEquals(object, object)"/>. This method is suposed as an optimization of 
        /// <see cref="Set(dynamic, string)"/> for certain cases.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        public void SetOnlyIfDifferent(dynamic value, [CallerMemberName]string propertyName = "")
        {
            set(value, propertyName, true);
        }
        public void Set(dynamic value, [CallerMemberName]string propertyName = "")
        {
            set(value, propertyName, false);
        }
        void set(dynamic value, string propertyName, bool setOnlyIfDifferent)
        {
            var hasOldValueSet = _properties.ContainsKey(propertyName);
            var oldValue = hasOldValueSet 
                ? _properties[propertyName] 
                : null;
            var newValue = value;
            var objectsAreSame = hasOldValueSet 
                && oldValue != null
                && newValue != null
                && oldValue.GetType() == newValue.GetType()
                && (oldValue.GetType().IsValueType
                    ? object.Equals(oldValue, newValue)
                    : object.ReferenceEquals(oldValue, newValue));
            if (setOnlyIfDifferent && objectsAreSame)
            {

            }
            else
            {
                _properties[propertyName] = newValue;

                var handler = _handler();
                if (handler != null)
                {
                    if (hasOldValueSet)
                    {
                        OnPropertyChanged(new RichPropertyChangedEventArgs(propertyName, (object)oldValue, (object)newValue));
                    }
                    handler.Invoke(_sender, propertyName);
                    notifyDependingProperties();
                }

                void notifyDependingProperties()
                {
                    if (_dependencies.ContainsKey(propertyName))
                    {
                        var dependingProperties = _dependencies[propertyName];
                        var alreadyNotified = new List<string>(dependingProperties.Count);
                        for (int i = 0; i < dependingProperties.Count; i++)
                        {
                            var property = dependingProperties[i];
                            if (alreadyNotified.NotContains(property))
                            {
                                handler.Invoke(_sender, property);
                                alreadyNotified.Add(property);
                                i = 0;
                            }
                        }
                    }
                }
            }
        }

        public dynamic GetOrNull([CallerMemberName]string propertyName = "")
        {
            if (!_properties.ContainsKey(propertyName))
            {
                _properties[propertyName] = null;
            }

            return _properties[propertyName];
        }
        public dynamic Get(Func<object> factory, [CallerMemberName]string propertyName = "")
        {
            if (!_properties.ContainsKey(propertyName))
            {
                Set(factory(), propertyName);
            }

            return _properties[propertyName];
        }

        protected virtual void OnPropertyChanged(RichPropertyChangedEventArgs e)
        {
            PropertyChanged.Invoke(_sender, e);
        }

        /// <summary>
        /// Симулирует вызов PropertyChanged для <see cref="dependingPropertyName"/> всякий раз, когда изменяется <see cref="sourcePropertyName"/>
        /// </summary>
        /// <param name="dependingPropertyName">Свойство, значение которого зависит от другого свойства (источника)</param>
        /// <param name="sourcePropertyName">Источник</param>
        public void CreateDependency(string dependingPropertyName, string sourcePropertyName)
        {
            if (!_dependencies.ContainsKey(sourcePropertyName))
            {
                _dependencies[sourcePropertyName] = new List<string>();
            }
            _dependencies[sourcePropertyName].Add(dependingPropertyName);
        }
        /// <summary>
        /// Симулирует вызов PropertyChanged для <see cref="dependingPropertyName"/> всякий раз, когда изменяется <see cref="sourcePropertyName"/>
        /// </summary>
        /// <param name="dependingPropertyName">Свойство, значение которого зависит от другого свойства (источника)</param>
        /// <param name="sourcePropertyName">Источник</param>
        public void CreateExternalDependency(string dependingPropertyName, string sourcePropertyName, 
            INotifyPropertyChanged externalObject)
        {
            externalObject.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == sourcePropertyName)
                {
                    _handler()?.Invoke(_sender, dependingPropertyName);
                }
            };
        }
        /// <summary>
        /// Симулирует вызов PropertyChanged для <see cref="dependingPropertyName"/> всякий раз, когда изменяется <see cref="externalObject"/>
        /// </summary>
        /// <param name="dependingPropertyName">Свойство, значение которого зависит от другого свойства (источника)</param>
        public void CreateExternalDependency(string dependingPropertyName, INotifyPropertyChanged externalObject)
        {
            externalObject.PropertyChanged += (o, e) =>
            {
                _handler()?.Invoke(_sender, dependingPropertyName);
            };
        }
    }
}
