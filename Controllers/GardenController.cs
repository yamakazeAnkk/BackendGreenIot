using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GreenIotApi.Services;
using GreenIotApi.DTOs;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/gardens")]
    public class GardenController : ControllerBase
    {
        private readonly FirestoreService _firestoreService;
        private readonly IMapper _mapper;

        public GardenController(FirestoreService firestoreService, IMapper mapper)
        {
            _firestoreService = firestoreService;
            _mapper = mapper;
        }

       

        // GET: Get a garden by ID
        
    }
}
