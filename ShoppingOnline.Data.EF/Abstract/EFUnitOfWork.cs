using ShoppingOnline.Infrastructure.Interfaces;

namespace ShoppingOnline.Data.EF.Abstract
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appContext;

        public EFUnitOfWork(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public void Dispose()
        {
            _appContext.Dispose();
        }

        public void Commit()
        {
            _appContext.SaveChanges();
        }
    }
}