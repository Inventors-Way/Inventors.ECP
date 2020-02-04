using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Inventors.ECP.Profiling
{
    public class NotifyPropertyChanged : 
        INotifyPropertyChanged
    {
        private readonly object lockObject = new object();

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<string, PropertyChangedEventArgs> eventArgs = new Dictionary<string, PropertyChangedEventArgs>();

        protected object LockObject => lockObject;

        private PropertyChangedEventArgs GetEventArgs(string propertyName)
        {
            if (!eventArgs.ContainsKey(propertyName))
            {
                eventArgs.Add(propertyName, new PropertyChangedEventArgs(propertyName));
            }

            return eventArgs[propertyName];
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) ||
                         (this.GetType().GetRuntimeProperty(propertyName) != null),
                         "Check that the property name exists for this instance.");

            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, GetEventArgs(propertyName));

                return true;
            }

            return false;
        }

        protected bool SetPropertyLocked<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) ||
                         (this.GetType().GetRuntimeProperty(propertyName) != null),
                         "Check that the property name exists for this instance.");

            lock (lockObject)
            {
                if (!EqualityComparer<T>.Default.Equals(field, newValue))
                {
                    field = newValue;
                    PropertyChanged?.Invoke(this, GetEventArgs(propertyName));

                    return true;
                }
            }

            return false;
        }

        protected T NotifyIfChanged<T>(T current, T newValue, [CallerMemberName]string propertyName = null)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) ||
                         (this.GetType().GetRuntimeProperty(propertyName) != null),
                         "Check that the property name exists for this instance.");

            if (!EqualityComparer<T>.Default.Equals(current, newValue))
            {
                PropertyChanged?.Invoke(this, GetEventArgs(propertyName));
            }

            return newValue;
        }

        protected void Notify(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, GetEventArgs(propertyName));
        }
    }
}
