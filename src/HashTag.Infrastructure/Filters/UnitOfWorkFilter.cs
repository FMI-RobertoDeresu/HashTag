using HashTag.Contracts;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HashTag.Infrastructure.Filters
{
    public class UnitOfWorkFilter : ActionFilterAttribute
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkFilter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.ModelState.IsValid && !_unitOfWork.IsCompleted)
                _unitOfWork.RollbackAsync().Wait();

            if (!_unitOfWork.IsCompleted)
                _unitOfWork.CommitAsync().Wait();
        }
    }
}