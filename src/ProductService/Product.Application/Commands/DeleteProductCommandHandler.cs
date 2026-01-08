using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Messaging.Interfaces;
using AbstractionBlocks.Common.Messaging.Events;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;

namespace ProductService.Product.Application.Commands
{
    public record DeleteProductCommand(Guid Id) : IRequest<bool>;

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<DeleteProductCommandHandler> _logger;
        private readonly IEventBus _eventBus;

        public DeleteProductCommandHandler(
            IUnitOfWork uow,
            ILoggerService<DeleteProductCommandHandler> logger,
            IEventBus eventBus)
        {
            _uow = uow;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new ProductNotFoundException(request.Id);

            product.SoftDelete(_uow.CurrentUser.UserId);
            await _uow.ProductRepository.UpdateAsync(product);

            _logger.Information("Product.Deleted", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = product.Id,
                ProductName = product.Name
            });

            // Publish integration events from domain
            foreach (var domainEvent in product.Events)
            {
                if (domainEvent is IntegrationEvent integrationEvent)
                {
                    await _eventBus.PublishAsync(integrationEvent, cancellationToken);
                }
            }

            product.ClearEvents();

            return true;
        }
    }
}
