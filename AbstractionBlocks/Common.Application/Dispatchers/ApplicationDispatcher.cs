using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection; 
namespace AbstractionBlocks.Common.Application.Dispatchers
{
    public class ApplicationDispatcher : IApplicationDispatcher
    {
        private readonly IServiceProvider _provider;
        private readonly ILoggerService<ApplicationDispatcher> _logger;
        public ApplicationDispatcher(IServiceProvider provider, ILoggerService<ApplicationDispatcher> logger)
        {
            _provider = provider;
            _logger = logger;
        }
        public async Task Dispatch(IEnumerable<IEventDomain> events)
        {
            foreach (var domainEvent in events)
            {
                var handlerType = typeof(IEventApplicationHandler<>)
                    .MakeGenericType(domainEvent.GetType());
                var handler = _provider.GetService(handlerType);
                if (handler == null)
                {
                    _logger.Warning("No handler found for event", new
                    {
                        EventType = domainEvent.GetType().Name
                    });
                    continue;
                }
                try
                {
                    await ((dynamic)handler).Handle((dynamic)domainEvent);
                }
                catch (System.Exception ex) 
                {
                    throw new System.Exception(
                        $"Failed to dispatch event {domainEvent.GetType().Name}", ex);
                }
            }
        }
    }
}
