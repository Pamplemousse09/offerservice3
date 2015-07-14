using System.Collections.Generic;
namespace Kikai.Internal.IManagers
{
    public interface IRespondentCatalog
    {
        Dictionary<string, string> GetRespondentCatalogueAttributes(string localPid);
        void UpdateRespondentCatalogueAttributes(string localPid, Dictionary<string, string> attributes);
        Dictionary<string, string> ProcessRespondentCatalogAttributesResponse(string respondentCatalogAttributesResponse);
    }
}
