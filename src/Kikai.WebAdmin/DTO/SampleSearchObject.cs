using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Kikai.WebAdmin.DTO
{
    public class SampleSearchObject
    {
        public int SampleId { get; set; }

        public int LOI { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<SampleSearchObject> ConvertFromJson(string json)
        {
            return (List<SampleSearchObject>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<SampleSearchObject>));
        }
    }
}