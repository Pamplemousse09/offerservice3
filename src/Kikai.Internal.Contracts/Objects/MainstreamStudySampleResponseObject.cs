using System.Collections.Generic;

namespace Kikai.Internal.Contracts.Objects
{
    public class MainstreamStudySampleResponseObject
    {
        public IEnumerable<MainstreamStudySampleObject> MainstreamStudySamples { get; set; }

        public IEnumerable<ServiceErrorObject> Errors { get; set; }
    }
}
