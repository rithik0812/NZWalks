using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Repository;

namespace NZWalksAPI.Controllers { 

[ApiController]
[Route("Regions")]
    public class RegionsController : Controller
    {
        private IRegionRepository _regionRepository;
        private IMapper _mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            this._regionRepository = regionRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            var RegionsList = await _regionRepository.GetAllAsync();

            var DTORegionsList = _mapper.Map<List<Models.DTO.Region>>(RegionsList); // the argument is the source for the mapping  

            return Ok(DTORegionsList); 

        }
    } 
}
