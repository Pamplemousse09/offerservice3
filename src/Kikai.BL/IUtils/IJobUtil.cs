using Kikai.BL.DTO;
using System.Collections.Generic;

namespace Kikai.BL.IUtils
{
    interface IJobUtil
    {
        string SampleListAsString(IEnumerable<SampleObject> samples);
    }
}
