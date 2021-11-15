using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventors.ECP.TestFramework
{
    public class PropertyBinder<T>
        where T : INotifyPropertyChanged
    {
        private readonly Control owner;
        private readonly Dictionary<string, Action> eventHandlers = new Dictionary<string, Action>();

        public T Model { get; }

        public PropertyBinder(T model, Control owner)
        {
            this.Model = model;
            this.owner = owner;
            this.Model.PropertyChanged += (s, e) => Execute(e.PropertyName); ;
        }

        public void Bind(string propertyName, Action handler)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) ||
                         (Model.GetType().GetRuntimeProperty(propertyName) != null),
                         "Check that the property name exists for the ViewModel.");

            if (!eventHandlers.ContainsKey(propertyName))
            {
                eventHandlers.Add(propertyName, handler);
                Execute(propertyName);
            }
        }

        private void Execute(string propertyName)
        {
            if (eventHandlers.ContainsKey(propertyName))
            {
                if (owner.InvokeRequired)
                {
                    owner.BeginInvoke(new Action(() => eventHandlers[propertyName]()));
                }
                else
                {
                    eventHandlers[propertyName]();
                }
            }
        }
    }
}
