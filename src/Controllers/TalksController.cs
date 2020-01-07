using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository campRepository, IMapper mapper)
        {
            _campRepository = campRepository;
            _mapper = mapper;
        }

        [HttpGet("{moniker}/{talkId}")]
        public async Task<ActionResult<TalkModels>> Get(string moniker, int talkId, bool includeSpeakers = false)
        {
            try
            {
                var result = await _campRepository.GetTalkByMonikerAsync(moniker, talkId, includeSpeakers);
                
                return _mapper.Map<TalkModels>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(500, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<TalkModels[]>> SearchByDate(string moniker, bool includeSpeakers = false)
        {
            try
            {
                var search = await _campRepository.GetTalksByMonikerAsync(moniker, includeSpeakers);

                if (!search.Any())
                {
                    return NotFound();
                }
                return _mapper.Map<TalkModels[]>(search);
            }
            catch (Exception)
            {
                return this.StatusCode(500, "Database Failure");
            }
        }
    }
}
