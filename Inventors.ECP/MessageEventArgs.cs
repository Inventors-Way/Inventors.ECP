using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP
{
    public class MessageEventArgs<T> : EventArgs
    {
        public T Message { get; private set; }

        public MessageEventArgs(T message)
        {
            Message = message;
        }
    }
}
