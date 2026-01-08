using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Messaging.Interfaces;
using AbstractionBlocks.Common.Messaging.Events;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductEntity = ProductService.Product.Domain.Product;

namespace ProductService.Product.Application.Commands
{
    public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        int Stock,
        string? Category) : IRequest<Guid>;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<CreateProductCommandHandler> _logger;
        private readonly IEventBus _eventBus;

        public CreateProductCommandHandler(
            IUnitOfWork uow,
            ILoggerService<CreateProductCommandHandler> logger,
            IEventBus eventBus)
        {
            _uow = uow;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = ProductEntity.Create(
                request.Name,
                request.Description,
                request.Price,
                request.Stock,
                request.Category,
                _uow.CurrentUser.UserId);

            var productId = await _uow.ProductRepository.AddAsync(product);

            _logger.Information("Product.Created", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = productId,
                ProductName = request.Name,
                Price = request.Price,
                Stock = request.Stock
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

            return productId;
        }
    }
}
