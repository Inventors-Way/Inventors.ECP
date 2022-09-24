namespace Inventors.ECP.Logging
{
    public interface ILogConfigVisitor
    {
        void Accept(BasicLogging config);

        void Accept(SeqLogging config);
    }
}