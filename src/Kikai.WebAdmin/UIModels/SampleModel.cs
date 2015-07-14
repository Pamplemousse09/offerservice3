using Kikai.Internal.Contracts.Objects;

namespace Kikai.WebAdmin.UIModels
{
    public class SampleModel
    {
        public MainstreamStudySampleObject StudySample { get; set; }

        public bool Exists { get; set; }

        public SampleModel()
        {
            Exists = false;
        }
    }
}