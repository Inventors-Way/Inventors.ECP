using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Utility
{
    public class LockedVariable<T>
    {
        private T value;

        public LockedVariable() { }

        public LockedVariable(T value)
        {
            this.value = value;
        }

        public T Get(object lockObject)
        {
            lock (lockObject)
            {
                return value;
            }
        }

        public void Set(object lockObject, T value)
        {
            lock (lockObject)
            {
                this.value = value;
            }
        }
    }
}
