using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingOnline.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}