using Kikai.Internal.Contracts.Objects;
using System.Collections.Generic;

namespace Kikai.WebAdmin.UIModels
{
    public class MainstreamStudySampleResponseModel
    {
        public IEnumerable<SampleModel> SampleList { get; set; }

        public IEnumerable<ServiceErrorObject> Errors { get; set; }
    }
}