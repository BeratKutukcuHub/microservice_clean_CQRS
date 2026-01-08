using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Messaging.Interfaces;
using AbstractionBlocks.Common.Messaging.Events;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;

namespace ProductService.Product.Application.Commands
{
    public record UpdateStockCommand(Guid ProductId, int Quantity) : IRequest<bool>;

    public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<UpdateStockCommandHandler> _logger;
        private readonly IEventBus _eventBus;

        public UpdateStockCommandHandler(
            IUnitOfWork uow,
            ILoggerService<UpdateStockCommandHandler> logger,
            IEventBus eventBus)
        {
            _uow = uow;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new ProductNotFoundException(request.ProductId);

            var oldStock = product.Stock;
            product.UpdateStock(request.Quantity, "Manual stock update", _uow.CurrentUser.UserId);
            await _uow.ProductRepository.UpdateAsync(product);

            _logger.Information("Product.StockUpdated", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = product.Id,
                ProductName = product.Name,
                OldStock = oldStock,
                NewStock = request.Quantity
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
