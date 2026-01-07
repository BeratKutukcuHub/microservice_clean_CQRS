using AbstractionBlocks.Common.Domain;
namespace AbstractionBlocks.Common.Application.Interfaces
{
    public interface IApplicationDispatcher
    {
        Task Dispatch(IEnumerable<IEventDomain> command);
    }
}
