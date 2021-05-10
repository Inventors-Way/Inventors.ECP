using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Inventors.ECP
{    
    [Serializable]
    public class IncompatibleDeviceException :
        Exception
    {
        public IncompatibleDeviceException(String message) : base(message) { }
        public IncompatibleDeviceException(String message, Exception inner) : base(message, inner) { }
        protected IncompatibleDeviceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public IncompatibleDeviceException() { }
    }

    [Serializable]
    public class InvalidMasterRequestException : Exception
    {
        public InvalidMasterRequestException(String message) : base(message) { }
        public InvalidMasterRequestException(String message, Exception inner) : base(message, inner) { }
        protected InvalidMasterRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public InvalidMasterRequestException() { }
    }

    [Serializable]
    public class InvalidMessageException :
        Exception
    {
        public InvalidMessageException(String message) : base(message) { }
        public InvalidMessageException(String message, Exception inner) : base(message, inner) { }
        protected InvalidMessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public InvalidMessageException() { }
    }

    [Serializable]
    public class InvalidSlaveResponseException : Exception
    {
        public InvalidSlaveResponseException(String message) : base(message) { }
        public InvalidSlaveResponseException(String message, Exception inner) : base(message, inner) { }
        protected InvalidSlaveResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public InvalidSlaveResponseException() { }
    }

    [Serializable]
    public class PacketFormatException :
      Exception
    {
        public PacketFormatException(String message) : base(message) { }
        public PacketFormatException(String message, Exception inner) : base(message, inner) { }
        protected PacketFormatException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public PacketFormatException() { }
    }

    [Serializable]
    public class SlaveNotRespondingException :
       Exception
    {
        public SlaveNotRespondingException(String message) : base(message) { }
        public SlaveNotRespondingException(String message, Exception inner) : base(message, inner) { }
        protected SlaveNotRespondingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public SlaveNotRespondingException() { }
    }

    [Serializable]
    public class FunctionNotAcknowledgedException :
       Exception
    {
        public FunctionNotAcknowledgedException(String message) : base(message) { }
        public FunctionNotAcknowledgedException(String message, Exception inner) : base(message, inner) { }
        protected FunctionNotAcknowledgedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public FunctionNotAcknowledgedException() { }

        public int ErrorCode
        {
            get
            {
                if (int.TryParse(Message, out int result))
                {
                    return result;
                }

                return -1;
            }
        }
    }
}
