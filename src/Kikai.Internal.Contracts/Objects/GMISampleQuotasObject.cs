using System.Collections.Generic;

namespace Kikai.Internal.Contracts.Objects
{
    public class GMISampleQuotasObject
    {
        public List<GMISampleObject> GMISampleQuotasList { get; set; }

        public List<GMIQuotaObject> GMIQuotasList { get; set; }
    }
}
