using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GreenIotApi.Models;
using GreenIotApi.DTOs;

namespace GreenIotApi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SensorData, SensorDataDto>().ReverseMap();
        }
    }
}