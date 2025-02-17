using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GreenIotApi.Models;
using GreenIotApi.DTOs;
using GreenIotApi.DTOs.ModelEdits;
using GreenIotApi.DTOs.ModelViews;

namespace GreenIotApi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SensorData, SensorDataDto>().ReverseMap();
            CreateMap<Device, DeviceDto>().ReverseMap();
            CreateMap<Garden, GardenDto>().ReverseMap();
            CreateMap<Garden, GardenModelEdit>().ReverseMap();
            CreateMap<SensorData, DataChartModelView>().ReverseMap();
        }
    }
}