using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
