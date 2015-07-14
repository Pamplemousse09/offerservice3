using Kikai.BL.DTO;

namespace Kikai.BL.IRepository
{
    public interface IProviderRepository
    {
        ProviderObject SelectByProviderId(string providerId);

        void Update(ProviderObject obj);

        void Insert(ProviderObject obj);

        void Delete(object id);
    }
}
