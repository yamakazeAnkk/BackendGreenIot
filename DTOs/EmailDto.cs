using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenIotApi.DTOs
{
    public class EmailDto
    {
        public string To { get; set; }   
        public string Subject { get; set; } 
        public string Body { get; set; }
    }

}