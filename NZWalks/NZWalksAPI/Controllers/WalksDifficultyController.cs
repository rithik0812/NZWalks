using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Repository;

namespace NZWalksAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalkDifficultiesController : Controller
    {
        private readonly IWalksDifficultyRepository walkDifficultyRepository;
        private readonly IMapper mapper;

        public WalkDifficultiesController(IWalksDifficultyRepository walkDifficultyRepository, IMapper mapper)
        {
            this.walkDifficultyRepository = walkDifficultyRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "reader")]
        public async Task<IActionResult> GetAllWalkDifficulties()
        {
            var walkDifficultiesDomain = await walkDifficultyRepository.GetAllAsync();

            // Convert Domain to DTOs
            var walkDifficultiesDTO = mapper.Map<List<Models.DTO.WalkDifficulty>>(walkDifficultiesDomain);

            return Ok(walkDifficultiesDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkDifficultyById")]
        [Authorize(Roles = "reader")]
        public async Task<IActionResult> GetWalkDifficultyById(Guid id)
        {
            var walkDifficulty = await walkDifficultyRepository.GetAsync(id);
            if (walkDifficulty == null)
            {
                return NotFound();
            }

            // Convert Domain to DTOs
            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);

            return Ok(walkDifficultyDTO);
        }

        [HttpPost]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> AddWalkDifficultyAsync(
            Models.DTO.AddWalksDifficultyRequestDTO addWalkDifficultyRequest)
        {
            // validation checks 

            if (ValidateAddWalksDifficulty(addWalkDifficultyRequest) == false)
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domain model
            var walkDifficultyDomain = new Models.Domain.WalkDifficulty
            {
                Code = addWalkDifficultyRequest.Code
            };

            // Call repository
            walkDifficultyDomain = await walkDifficultyRepository.AddAsync(walkDifficultyDomain);

            // Convert Domain to DTO
            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficultyDomain);

            // Return response
            return CreatedAtAction(nameof(GetWalkDifficultyById),
                new { ID = walkDifficultyDTO.ID }, walkDifficultyDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> UpdateWalkDifficultyAsync(Guid id,
            Models.DTO.UpdateWalksDiffcultyRequestDTO updateWalkDifficultyRequest)
        {

            // validation checks 

            if (ValidateUpdateWalksDifficulty(updateWalkDifficultyRequest) == false)
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domiain Model
            var walkDifficultyDomain = new Models.Domain.WalkDifficulty
            {
                Code = updateWalkDifficultyRequest.Code
            };

            // Call repository to update
            walkDifficultyDomain = await walkDifficultyRepository.UpdateAsync(id, walkDifficultyDomain);

            if (walkDifficultyDomain == null)
            {
                return NotFound();
            }

            // Convert Domain to DTO
            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficultyDomain);

            // Return response
            return Ok(walkDifficultyDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> DeleteWalkDifficulty(Guid id)
        {
            var walkDifficultyDomain = await walkDifficultyRepository.DeleteAsync(id);
            if (walkDifficultyDomain == null)
            {
                return NotFound();
            }

            // Convert to DTO
            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficultyDomain);
            return Ok(walkDifficultyDTO);
        }

        #region Private Methods

        // validation of Add walksDifficulty endpoint : 
        private bool ValidateAddWalksDifficulty(Models.DTO.AddWalksDifficultyRequestDTO addWalksDifficultyRequest)
        {
            if (addWalksDifficultyRequest == null)
            {
                ModelState.AddModelError(nameof(addWalksDifficultyRequest), "Request for adding a region cannot be null");

                return false;
            }

            if (string.IsNullOrWhiteSpace(addWalksDifficultyRequest.Code))
            {
                ModelState.AddModelError(nameof(addWalksDifficultyRequest.Code), $"{nameof(addWalksDifficultyRequest.Code)} cannot be null/ empty / whitespace");
            }
            
            // if any of the validation checks fail then return false
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            // if the all the validation chacks pass then return true
            return true;

        }

        // validation of Update walksDiffiulty endpoint : 
        private bool ValidateUpdateWalksDifficulty(Models.DTO.UpdateWalksDiffcultyRequestDTO updateWalksDifficultyRequest)
        {
            if (updateWalksDifficultyRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalksDifficultyRequest), "Request for adding a region cannot be null");

                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalksDifficultyRequest.Code))
            {
                ModelState.AddModelError(nameof(updateWalksDifficultyRequest.Code), $"{nameof(updateWalksDifficultyRequest.Code)} cannot be null/ empty / whitespace");
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
