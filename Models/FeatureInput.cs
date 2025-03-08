using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenIotApi.Models
{
    public class FeatureInput
    {
        public float? Temperature { get; set; }
        public float? Humidity { get; set; }
        public float? SoilMoisture { get; set; }
        public float? CoPpm { get; set; }
        public float? LightLevel { get; set; }
        public float? IsRaining { get; set; }
       
    }
}