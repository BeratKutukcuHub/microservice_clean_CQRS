using AbstractionBlocks.Common.Domain;
namespace AbstractionBlocks.Common.Application.Interfaces
{
    public interface IEventApplicationHandler<T> where T : IEventDomain
    {
        Task Handle(T reqEvent);
    }
}
