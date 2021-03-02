using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HiSpaceListingService.DTO;
using HiSpaceListingService.BO;
using HiSpaceListingService.Services;
using HiSpaceListingService.Models;

namespace HiSpaceListingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicReturnCalculatorController : ControllerBase
    {

        IBasicReturnCalculatorService _BRCService;
        public BasicReturnCalculatorController()
        {
            _BRCService = new BasicReturnCalculatorService();
        }                

        [Route("Post")]
        [HttpPost]
        public ActionResult Post([FromBody] BasicReturnCalculatorDTO basicReturnCalculatorDTO)
        {
                if(ModelState.IsValid)
                {
                    try
                    {
                        _BRCService.Initialize(basicReturnCalculatorDTO);
                        BasicReturnCalculatorResponse BRCResponse = _BRCService
                                                                        .GetResponse();
                        return Ok(BRCResponse);
                    }
                    catch(Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                    
                }
                return BadRequest(basicReturnCalculatorDTO);
        }
    }
}