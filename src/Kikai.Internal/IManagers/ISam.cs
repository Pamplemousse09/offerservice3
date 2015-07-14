using Kikai.Internal.Contracts.Objects;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Kikai.Internal.IManagers
{
    public interface ISam
    {
        XDocument GetMainstreamStudySamples(int studyId);

        XDocument GetOpenSampleAttributes(string sampleList);

        IEnumerable<ServiceErrorObject> GetServiceErrors(XDocument serviceResponse);

        IEnumerable<MainstreamStudySampleObject> GetMainstreamStudySampleResponse(XDocument serviceResponse);

        IEnumerable<OpenSampleObject> GetOpenSampleAttributeResponse(XDocument serviceResponse);

        MainstreamStudySampleResponseObject ProcessGetMainstreamStudySample(int studyId);

        OpenSampleAttributeResponseObject ProcessGetOpenSampleAttributes(string sampleList);
    }
}
