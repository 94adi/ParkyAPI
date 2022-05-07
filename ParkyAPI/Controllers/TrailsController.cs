using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/trails")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : Controller
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepository,
                                       IMapper mapper)
        {
            _trailRepository = trailRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Get list of trails.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var Trails = _trailRepository.GetTrails();

            var TrailsDto = new List<TrailDto>();

            foreach(var Trail in Trails)
            {
                TrailsDto.Add(_mapper.Map<TrailDto>(Trail));
            }

            return Ok(TrailsDto);
        }


        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="id">Id of the trail</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int id)
        {
            var Trail = _trailRepository.GetTrail(id);

            if(Trail == null)
            {
                return NotFound();
            }

            var TrailDto = _mapper.Map<TrailDto>(Trail);

            return Ok(TrailDto);
        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="id">Id of the trail</param>
        /// <returns></returns>
        [HttpGet("GetTrailInNationalPark/{id:int}", Name = "GetTrailInNationalPark")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailInNationalPark(int id)
        {
            var objList = _trailRepository.GetTrailsInNationalPark(id);
            var objDto = new List<TrailDto>();
            if (objList == null)
            {
                return NotFound();
            }

            foreach(var obj in objList)
            {
                 objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if(trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepository.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }

            var TrailObj = _mapper.Map<Trail>(trailDto);

            var result = _trailRepository.CreateTrail(TrailObj);
            
            if(result == false)
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {TrailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { id = TrailObj.Id }, TrailObj);
        }

        [HttpPatch("{id:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int id, [FromBody]TrailUpdateDto trailDto)
        {
            if (trailDto == null || id != trailDto.Id)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);

            var result = _trailRepository.UpdateTrail(trailObj);

            if (result == false)
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{id:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int id)
        {
            if (!_trailRepository.TrailExists(id))
            {
                return NotFound();
            }

            var trailObj = _trailRepository.GetTrail(id);

            var result = _trailRepository.DeleteTrail(trailObj);

            if (result == false)
            {
                ModelState.AddModelError("", $"Something went wrong when deleting  the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }
    }
}
