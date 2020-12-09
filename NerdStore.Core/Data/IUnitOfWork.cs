using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }

    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
