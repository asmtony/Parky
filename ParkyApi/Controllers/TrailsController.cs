using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyApi.Models;
using ParkyApi.Models.Dtos;
using ParkyApi.Repository.IRepository;

namespace ParkyApi.Controllers
{
    [Route("api/v{version:apiVersion}/trails")]
    //[Route("api/[controller]")]
    [ApiController]
   // [ApiExplorerSettings(GroupName = "ParkyOpenApiSpecTrail")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        public readonly ITrailRepository _trailRepository;
        public readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepository, IMapper mapper)
        {
            _trailRepository = trailRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(List<TrailDto>))]        
        public IActionResult GetTrails()
        {
            var objList = _trailRepository.GetTrails();
            var objDto = new List<TrailDto>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }


        /// <summary>
        /// Get individual trails
        /// </summary>
        /// <param name="trailId">The national park Id</param>
        /// <returns></returns>
        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepository.GetTrail(trailId);


            if (obj == null)
                return NotFound();

            var objDto = _mapper.Map<TrailDto>(obj);

            return Ok(objDto);
        }

        /// <summary>
        /// Get individual trails by National park name
        /// </summary>
        /// <param name="nationalParkId">The national park Id</param>
        /// <returns></returns>
        [HttpGet("[Action]/{nationalParkId:int}", Name = "GetTrailInNationalPark")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrailInNationalPark(int nationalParkId)
        {
            var objList = _trailRepository.GetTrailsInNationalPark(nationalParkId);

            if (objList == null)
                return NotFound();

            List<TrailDto> trailList = new List<TrailDto>();
            foreach (var obj in objList)
            {
                trailList.Add(_mapper.Map<TrailDto>(obj));
            }
            

            return Ok(trailList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody]TrailCreateDto trailDtos)
        {
            if (trailDtos == null)
            {
                return BadRequest(ModelState);
            }
            if(_trailRepository.TrailExists(trailDtos.Name))
            {
                ModelState.AddModelError("", "National Park Exists");
                return StatusCode(404, ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDtos);
            if (!_trailRepository.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new { TrailId = trailObj.Id}, trailObj);
        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateTrail(int TrailId, [FromBody] TrailUpdateDto trailDtos)
        {
            if (trailDtos == null && TrailId!= trailDtos.Id)
            {
                return BadRequest(ModelState);
            }
            
            var trailObj = _mapper.Map<Trail>(trailDtos);
            if (!_trailRepository.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
                
        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if(!_trailRepository.TrailExists(trailId))
            {
                return NotFound();
            }


            var trailObj = _trailRepository.GetTrail(trailId);
            if (!_trailRepository.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
