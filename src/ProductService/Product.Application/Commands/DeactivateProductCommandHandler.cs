using AbstractionBlocks.Common.Exception.Logger;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;
namespace ProductService.Product.Application.Commands
{
    public record DeactivateProductCommand(Guid Id) : IRequest<bool>;
    public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<DeactivateProductCommandHandler> _logger;
        public DeactivateProductCommandHandler(
            IUnitOfWork uow,
            ILoggerService<DeactivateProductCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public async Task<bool> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new ProductNotFoundException(request.Id);
            product.Deactivate();
            await _uow.ProductRepository.UpdateAsync(product);
            _logger.Information("Product.Deactivated", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = product.Id,
                ProductName = product.Name
            });
            return true;
        }
    }
}
