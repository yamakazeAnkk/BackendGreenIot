using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenIotApi.Models
{
    public class CheckSensorRequest
    {
        public string UserId { get; set; } // ID người dùng
        public DateTime Date { get; set; }
    }
}