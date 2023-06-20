using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Repository;
using System.Runtime.CompilerServices;

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
            // validation checks 

            if (ValidateAddRegionAsync(addRegion) == false) 
            { 
                return BadRequest(ModelState);  
            }

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

        public async Task<IActionResult> UpdateRegionByIDAsync([FromRoute] Guid id, [FromBody] Models.DTO.UpdateRegionRequestDTO updateRegion) 
        {
            // validation checks 
            if (ValidateUpdateRegionAsync(updateRegion) == false) 
            {
                BadRequest(ModelState);
            }

            // Convert the updateRegion object to domain model

            var DomainUpdateRegionObject = new Models.Domain.Region
            {
                Code = updateRegion.Code,
                Name = updateRegion.Name,
                Area = updateRegion.Area,
                Lat = updateRegion.Lat,
                Long = updateRegion.Long,
                Population = updateRegion.Population
            };

            // Try and update region using repository 

            var UpdatedRegion = await _regionRepository.UpdateRegionAsync(id, DomainUpdateRegionObject);

            // if the repo send back null then send a NotFound

            if (UpdatedRegion == null) 
            {
                return NotFound(); 
            }

            // if the update succeeds then convert the returned updated object to a DTO and send OkResponse 

            var DTOUpdatedRegion = new Models.DTO.Region
            {
                ID = UpdatedRegion.ID,
                Code = UpdatedRegion.Code,
                Name = UpdatedRegion.Name,
                Area = UpdatedRegion.Area,
                Lat = UpdatedRegion.Lat,
                Long = UpdatedRegion.Long,
                Population = UpdatedRegion.Population
            };

            return Ok(DTOUpdatedRegion);
        }

        #region Private Methods

        // validation of AddRegion endpoint : 
        private bool ValidateAddRegionAsync(Models.DTO.AddRegionsRequestDTO addRegionsRequest) 
        {
            if (addRegionsRequest == null) 
            {
                ModelState.AddModelError(nameof(addRegionsRequest),"Request for adding a region cannot be null");

                return false;
            }

            if (string.IsNullOrWhiteSpace(addRegionsRequest.Code)) 
            {
                ModelState.AddModelError(nameof(addRegionsRequest.Code), $"{nameof(addRegionsRequest.Code)} cannot be null/ empty / whitespace");
            }

            if (string.IsNullOrWhiteSpace(addRegionsRequest.Name))
            {
                ModelState.AddModelError(nameof(addRegionsRequest.Name), $"{nameof(addRegionsRequest.Name)} cannot be null/ empty / whitespace");
            }

            if (addRegionsRequest.Area <= 0)
            {
                ModelState.AddModelError(nameof(addRegionsRequest.Area), $"{nameof(addRegionsRequest.Area)} cannot be less than or equal to 0");
            }

            if (addRegionsRequest.Lat == 0)
            {
                ModelState.AddModelError(nameof(addRegionsRequest.Lat), $"{nameof(addRegionsRequest.Lat)} cannot be equal to 0");
            }

            if (addRegionsRequest.Long == 0)
            {
                ModelState.AddModelError(nameof(addRegionsRequest.Long), $"{nameof(addRegionsRequest.Long)} cannot be equal to 0");
            }

            if (addRegionsRequest.Population <= 0)
            {
                ModelState.AddModelError(nameof(addRegionsRequest.Population), $"{nameof(addRegionsRequest.Population)} cannot be less than or equal to 0");
            }

            // if any of the validation checks fail then return false
            if (ModelState.ErrorCount > 0)
            {
                return false; 
            }

            // if the all the validation chacks pass then return true
            return true;

        }

        // validation for update endpoint
        private bool ValidateUpdateRegionAsync(Models.DTO.UpdateRegionRequestDTO updateRegionsRequest)
        {
            if (updateRegionsRequest == null)
            {
                ModelState.AddModelError(nameof(updateRegionsRequest), "Request for adding a region cannot be null");

                return false;
            }

            if (string.IsNullOrWhiteSpace(updateRegionsRequest.Code))
            {
                ModelState.AddModelError(nameof(updateRegionsRequest.Code), $"{nameof(updateRegionsRequest.Code)} cannot be null/ empty / whitespace");
            }

            if (string.IsNullOrWhiteSpace(updateRegionsRequest.Name))
            {
                ModelState.AddModelError(nameof(updateRegionsRequest.Name), $"{nameof(updateRegionsRequest.Name)} cannot be null/ empty / whitespace");
            }

            if (updateRegionsRequest.Area <= 0)
            {
                ModelState.AddModelError(nameof(updateRegionsRequest.Area), $"{nameof(updateRegionsRequest.Area)} cannot be less than or equal to 0");
            }

            if (updateRegionsRequest.Lat == 0)
            {
                ModelState.AddModelError(nameof(updateRegionsRequest.Lat), $"{nameof(updateRegionsRequest.Lat)} cannot be equal to 0");
            }

            if (updateRegionsRequest.Long == 0)
            {
                ModelState.AddModelError(nameof(updateRegionsRequest.Long), $"{nameof(updateRegionsRequest.Long)} cannot be equal to 0");
            }

            if (updateRegionsRequest.Population <= 0)
            {
                ModelState.AddModelError(nameof(updateRegionsRequest.Population), $"{nameof(updateRegionsRequest.Population)} cannot be less than or equal to 0");
            }

            // if any of the validation checks fail then return false
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            // if the all the validation chacks pass then return true
            return true;

        }


        #endregion

    }
}
