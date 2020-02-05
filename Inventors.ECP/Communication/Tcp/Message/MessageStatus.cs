namespace Inventors.ECP.Communication.Tcp.Message
{
    internal enum MessageStatus
    {
        Normal,
        Success,
        Failure,
        AuthRequired,
        AuthRequested,
        AuthSuccess,
        AuthFailure,
        Removed,
        Disconnecting
    }
}