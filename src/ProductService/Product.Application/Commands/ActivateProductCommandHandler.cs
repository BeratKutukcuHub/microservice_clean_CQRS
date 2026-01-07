using AbstractionBlocks.Common.Exception.Logger;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;
namespace ProductService.Product.Application.Commands
{
    public record ActivateProductCommand(Guid Id) : IRequest<bool>;
    public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<ActivateProductCommandHandler> _logger;
        public ActivateProductCommandHandler(
            IUnitOfWork uow,
            ILoggerService<ActivateProductCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public async Task<bool> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new ProductNotFoundException(request.Id);
            product.Activate();
            await _uow.ProductRepository.UpdateAsync(product);
            _logger.Information("Product.Activated", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = product.Id,
                ProductName = product.Name
            });
            return true;
        }
    }
}
