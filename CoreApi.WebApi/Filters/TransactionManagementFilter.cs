using System.Threading.Tasks;
using CoreApi.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreApi.WebApi.Filters
{
    public class TransactionManagementFilter : IAsyncActionFilter
    {
        private readonly DataContext _dataContext;

        public TransactionManagementFilter(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await _dataContext.Database.BeginTransactionAsync();

            try
            {
                await next();
                if (_dataContext.ChangeTracker.HasChanges())
                {
                    await _dataContext.SaveChangesAsync();
                }
                await _dataContext.Database.CurrentTransaction.CommitAsync();
            }
            catch
            {
                if (_dataContext.Database.CurrentTransaction != null)
                    _dataContext.Database.RollbackTransaction();

                throw;
            }
        }
    }
}