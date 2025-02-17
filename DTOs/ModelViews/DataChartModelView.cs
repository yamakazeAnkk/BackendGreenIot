using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenIotApi.DTOs.ModelViews
{
    public class DataChartModelView
    {
        public string Id { get; set; }
        public float Temperature { get; set; }
     
        public float Humidity { get; set; }
  
        public float SoilMoisture { get; set; }

        public float LightLevel { get; set; }

        public float CoPpm { get; set; }


        
    }
}