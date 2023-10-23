﻿using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public interface IUnitOfWork : IDisposable
    {
        void AddEntitySave(Entity entity);

        Task<IActionResult> CommitAsync();
    }
}
