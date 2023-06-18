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
        public async Task<IActionResult> GetAllRegionsAsync()
        {
            // this method returns all the Regions in the DB then maps them into a DTO 

            var DomainRegionsList = await _regionRepository.GetAllAsync();

            var DTORegionsList = _mapper.Map<List<Models.DTO.Region>>(DomainRegionsList); // the argument is the source for the mapping  

            return Ok(DTORegionsList);

        }

        [HttpGet]
        [Route(("{id:guid}"))]
        [ActionName("GetSingleRegionAsync")]
        public async Task<IActionResult> GetSingleRegionAsync(Guid id)
        {

            // this method tries the find a region by id and maps to a DTO, if it cant find it then shows not found

            var DomainRegionByID = await _regionRepository.GetSingleAsync(id);

            if (DomainRegionByID == null)
            {
                return NotFound();
            }

            var DTORegionByID = _mapper.Map<Models.DTO.Region>(DomainRegionByID); // the argument is the source for the mapping  

            return Ok(DTORegionByID);

        }

        [HttpPost]

        public async Task<IActionResult> AddRegionByIDAsync(Models.DTO.AddRegionsRequestDTO addRegion) 
        {
            // convert the user input to domain model so that we can pass it into the add method in the repo.add method
            // (note: we set the guid id in the repo.add method so the user wont set that param)

            var DomainRegionObject = new Models.Domain.Region
            {
                Code = addRegion.Code,
                Name = addRegion.Name,
                Area = addRegion.Area,
                Lat = addRegion.Lat,
                Long = addRegion.Long,
                Population = addRegion.Population
            };


            // exceute the add method and add domain model region and get back the added object 

            var AddedRegionObject = await _regionRepository.AddRegionsAsync(DomainRegionObject);

            // if successful then convert the returned domain object that was added to the DB to a DTO object

            var DTORegionObject = new Models.DTO.Region
            {
                ID = AddedRegionObject.ID,
                Code = AddedRegionObject.Code,
                Name = AddedRegionObject.Name,
                Area = AddedRegionObject.Area,
                Lat = AddedRegionObject.Lat,
                Long = AddedRegionObject.Long,
                Population = AddedRegionObject.Population
            };

            // when the region is addedd sucessfully send a 201 post request success code 

            return CreatedAtAction(nameof(GetSingleRegionAsync), new { ID = DTORegionObject.ID }, DTORegionObject);

        }

        [HttpDelete]
        [Route(("{id:guid}"))]
        public async Task<IActionResult> DeleteRegionByIDAsync(Guid id)
        {
            // check if the region by that id exists in DB 
            // if it exists then remove it and return the object that was deleted

            var DomainDeletedRegion = await _regionRepository.DeleteRegionAsync(id);

            if (DomainDeletedRegion == null)
            {
                return NotFound();
            }

            // map the delted object to a DTO object and return it 

            var DTODeletedRegion = new Models.DTO.Region
            {
                ID = DomainDeletedRegion.ID,
                Code = DomainDeletedRegion.Code,
                Name = DomainDeletedRegion.Name,
                Area = DomainDeletedRegion.Area,
                Lat = DomainDeletedRegion.Lat,
                Long = DomainDeletedRegion.Long,
                Population = DomainDeletedRegion.Population
            };

            return Ok(DTODeletedRegion);
        }

        [HttpPut]
        [Route(("{id:guid}"))]

    } 
}
