using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Interfaces;

namespace IdentityService.Application
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
                var handlerType = typeof(IEventApplicationHandler<>).MakeGenericType(domainEvent.GetType());
                dynamic handler = _provider.GetService(handlerType);
                if (handler == null)
                {
                    _logger.Warning($"No handler found for event {domainEvent.GetType().Name}",
                    domainEvent.GetType().Name,
                    "NotFound"
                    , default);
                    continue;
                }
                try
                {
                    await handler.Handle(domainEvent);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to dispatch event {domainEvent.GetType().Name}", ex);
                }
            }
        }
    }
}