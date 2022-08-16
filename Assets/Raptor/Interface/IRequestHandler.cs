namespace Raptor.Interface
{
    public interface IRequestHandler<T>
    {
        void Handle(Sequence<T> request);
    }
}