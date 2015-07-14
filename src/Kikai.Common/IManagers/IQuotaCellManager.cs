using System;

namespace Kikai.Common.IManagers
{
    public interface IQuotaCellManager
    {
        bool InitializeQuotaCells(Guid offerId);
    }
}
