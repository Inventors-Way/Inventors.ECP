# Change Log

## Revision 1.5.0

The main purpose of this update is to enable DeviceSlaves to include error response codes
in NACK's in the same way as they are provided by physical devices implementing the ECP
serial communication protocol.

This is a breaking change and will require an update to code implementing ECP slaves on .NET.

- Changed the Accept(DeviceFunction func) in FunctionListeners to return the errorCode(int) of the operation. If no error occured then the FunctionListeners should return zero (0) [Breaking Change].
- Changed Code property in FunctionDispatcher and MessageDispatcher to an auto property
- Minor changes to conform to the coding style.
