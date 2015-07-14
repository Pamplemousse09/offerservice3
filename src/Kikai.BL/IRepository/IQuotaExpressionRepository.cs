using Kikai.Internal.Contracts.Objects;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
   public interface IQuotaExpressionRepository
    {
        IEnumerable<SteamStudyObject> SelectAll();
        SteamStudyObject SelectByID(object sampleId);
        void Insert(SteamStudyObject obj);
        void Update(SteamStudyObject obj);
        void Delete(object sampleId);
    }
}
