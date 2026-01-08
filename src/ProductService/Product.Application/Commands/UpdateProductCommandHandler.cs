using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Messaging.Interfaces;
using AbstractionBlocks.Common.Messaging.Events;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;
using ProductService.Product.Domain;

namespace ProductService.Product.Application.Commands
{
    public record UpdateProductCommand(
        Guid Id,
        string? Name,
        string? Description,
        decimal? Price,
        int? Stock,
        string? Category) : IRequest<Guid>;

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<UpdateProductCommandHandler> _logger;
        private readonly IEventBus _eventBus;

        public UpdateProductCommandHandler(
            IUnitOfWork uow,
            ILoggerService<UpdateProductCommandHandler> logger,
            IEventBus eventBus)
        {
            _uow = uow;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new ProductNotFoundException(request.Id);

            product.UpdateProduct(
                request.Name,
                request.Description,
                request.Price,
                request.Stock,
                request.Category,
                _uow.CurrentUser.UserId);

            await _uow.ProductRepository.UpdateAsync(product);

            _logger.Information("Product.Updated", new
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

            return product.Id;
        }
    }
}
