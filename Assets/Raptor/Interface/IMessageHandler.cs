namespace Raptor.Interface
{
    public interface IMessageHandler<T>
    {
        void Handle(Message<T> message);
    }
}
