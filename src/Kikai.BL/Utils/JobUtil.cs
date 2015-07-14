using Kikai.BL.DTO;
using Kikai.BL.IUtils;
using System.Collections.Generic;

namespace Kikai.BL.Utils
{
    public class JobUtil : IJobUtil
    {
        public string SampleListAsString(IEnumerable<SampleObject> samples)
        {
            string sampleList = string.Empty;

            //Getting all the sampleIds as comma separated list
            foreach (var sample in samples)
            {
                sampleList += sample.SampleId + ",";
            }

            //Removing the last comma
            sampleList = sampleList.Remove(sampleList.Length - 1);

            return sampleList;
        }
    }
}