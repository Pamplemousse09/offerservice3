using Kikai.BL.DTO;
using System;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface ITermRepository
    {
        bool CheckTermForOffer(Guid TermId, Guid OfferId);
        TermObject GetActiveTermForOffer(Guid OfferId);
        TermObject CheckTermValidity(Guid TermId, int GracePeriod);
        IEnumerable<TermObject> GetTermFromOfferId(Guid OfferId);
        void SetNewCPI(TermObject term);
    }
}
