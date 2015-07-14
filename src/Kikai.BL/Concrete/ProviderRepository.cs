using Kikai.BL.DTO;
using Kikai.BL.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace Kikai.BL.Concrete
{
    public class ProviderRepository : IGenericRepository<ProviderObject>, IProviderRepository
    {
        public IEnumerable<ProviderObject> SelectAll()
        {
            return new DataUtils().GetList<ProviderObject>("EXEC Provider_GetAll").ToList();
        }

        public ProviderObject SelectByID(object id)
        {
            return new DataUtils().GetList<ProviderObject>("EXEC Provider_GetById @Id=@P1", id).ToList().SingleOrDefault();
        }

        public ProviderObject SelectByProviderId(string providerId)
        {
            //var context = new KikaiDB();
            return new DataUtils().GetList<ProviderObject>("EXEC Provider_GetByProviderId @ProviderId='" + providerId + "'").SingleOrDefault();
            //return new KikaiDB().Database.SqlQuery<ProviderObject>("EXEC Provider_GetByApiUser @ApiUser=@P1", apiUser).SingleOrDefault();
            //var provider = context.Database.SqlQuery<ProviderObject>("SELECT TOP 1 * from Providers with (nolock) where Providers.ApiUser='" + apiUser + "'").SingleOrDefault();
            //return provider;
        }

        public void Insert(ProviderObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Provider_Add @ProviderId=@P1, 
                                    @WelcomeUrlCode=@P2, 
                                    @Enabled=@P3",
                                    obj.ProviderId,
                                    obj.WelcomeURLCode,
                                    obj.Enabled);
        }

        public void Update(ProviderObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Provider_Update @Id=@P1,
                                    @ProviderId=@P2, 
                                    @WelcomeUrlCode=@P3, 
                                    @Enabled=@P4",
                                    obj.Id,
                                    obj.ProviderId,
                                    obj.WelcomeURLCode,
                                    obj.Enabled);
        }

        public void Delete(object id)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Provider_Delete @Id=@P1", id);
        }
    }
}
