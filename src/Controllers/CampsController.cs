using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using AutoMapper;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Cors;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowReact")]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper)
        {
            _campRepository = campRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CampModels[]>> GetCamps(bool includeTalks = false)
        {
            try
            {
                var listCamps = await _campRepository.GetAllCampsAsync(includeTalks);
                return _mapper.Map<CampModels[]>(listCamps);
            }
            catch (Exception)
            {
                return this.StatusCode(500, "Database Failure");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModels>> Get(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await _campRepository.GetCampAsync(moniker, includeTalks);
                return _mapper.Map<CampModels>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(500, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModels[]>> SearchByDate(DateTime dateTime, bool includeTalks = false)
        {
            try
            {
                var search = await _campRepository.GetAllCampsByEventDate(dateTime, includeTalks);

                if (!search.Any())
                {
                    return NotFound();
                }
                return _mapper.Map<CampModels[]>(search);
            }
            catch (Exception)
            {
                return this.StatusCode(500, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModels>> Post(CampModels model)
        {
            try
            {
                var monikerExits = await _campRepository.GetCampAsync(model.Moniker);

                if (monikerExits != null)
                {
                    return BadRequest("Moniker da ton tai");
                }

                var camp = _mapper.Map<Camp>(model);
                _campRepository.Add(camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Created($"/api/camps/{model.Moniker}", _mapper.Map<CampModels>(camp));
                }
            }
            catch (Exception)
            {

                return this.StatusCode(500, "Database Failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<ActionResult<CampModels>> Delete(string moniker)
        {
            try
            {
                var monikerExits = await _campRepository.GetCampAsync(moniker);

                if (monikerExits == null)
                {
                    return NotFound($"Moniker khong tim thay voi {moniker}");
                }

                _campRepository.Delete(monikerExits);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {

                return this.StatusCode(500, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModels>> Put(string moniker, CampModels models)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker);

                if (oldCamp == null)
                {
                    return NotFound($"Moniker khong tim thay voi {moniker}");
                }

                _mapper.Map(models, oldCamp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModels>(oldCamp);
                }
            }
            catch (Exception)
            {

                return this.StatusCode(500, "Database Failure");
            }

            return BadRequest();
        }
    }
}