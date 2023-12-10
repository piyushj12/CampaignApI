using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Campaigns.Api;
using Campaigns.Data;
using Campaigns.Models;
using Campaigns.Services.Interfaces;
using CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Campaigns.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignsController : ControllerBase
    {
        private readonly ILogger<CampaignsController> _logger;
        private readonly ICampaignInterface _campaigns;

        public CampaignsController(ILogger<CampaignsController> logger, ICampaignInterface campaigns)
        {
            this._campaigns = campaigns;
            this._logger = logger;
        }


        [HttpPost]
        public async Task<ActionResult> AddCampaign([FromBody] CampaignModel campaignModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Model is not correct");
                return BadRequest(ModelState);
            }

            try
            {
                var addedCampaign = await _campaigns.AddCampaigns(campaignModel);
                return Ok(addedCampaign);
            }
            catch (CampaignNameAlreadyExist ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest($"Campaign with the same name:{campaignModel.CampaignName} already exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding campaign" + ex.Message);
                return StatusCode(500, "error occured");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCampaign([FromQuery] [EmptyGuidValidator] Guid guid, [FromBody] CampaignModel campaignModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCampaign = await _campaigns.UpdatingCampaign(guid, campaignModel);
                if(updatedCampaign == null)
                {
                    _logger.LogError("Campaign could not found");
                    return NotFound($"Campaign doesn't exist for the given {guid}");
                }
                return Ok(updatedCampaign);
            }
            catch(CampaignNameAlreadyExist ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest($"Campaign with the same name:{campaignModel.CampaignName} already exist");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error Updating Campaign" + ex.Message);
                return StatusCode(500, "error occured on the server side");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCampaign([FromQuery] [EmptyGuidValidator] Guid guid)
        {
            try
            {
                var deleteCampaign = await _campaigns.DeletingCampaign(guid);
                if(!deleteCampaign)
                {
                    _logger.LogError("Campaign Could not be found");
                    return NotFound($"Campaign doesn't exist for the given {guid}");
                }
                return Ok(deleteCampaign);
            }
            catch(CampaignActiveException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest($"Campaign with the Campaign Id: {guid} is in Active state, thus can't delete it");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error Deleting the Campaign" + ex.Message);
                return StatusCode(500, "Interal server error");
            }
        }


        [HttpGet("GetCampaignById")]
        public async Task<ActionResult> GetCampaignById([FromQuery][EmptyGuidValidator] Guid campaignId)
        {
            try
            {
                var fetchedCampaign = await _campaigns.GettingCampaignById(campaignId);
                if(fetchedCampaign == null)
                {
                    _logger.LogError($"Campaign with the Campaign Id: {campaignId} could not be found");
                    return NotFound($"Campaign Not found with the id: {campaignId} ");
                }
                return Ok(fetchedCampaign);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in getting the campaign" + ex.Message);
                return StatusCode(500, "Server Error");
            }

        }

        [HttpGet("GetFilteredCampaigns")]
        public async Task<ActionResult> GetCampaigns(
            [FromQuery] string? searchName,
            [FromQuery] string? searchState,
            [FromQuery] string? searchDay,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortField = "CampaignName",
            [FromQuery] string sortDirection = "asc"
            )

        {

            try
            {
                var (fetchedCampaigns, totalCampaigns) = await _campaigns.GetFilteredCampaigns(searchName, searchState, searchDay,
                    pageIndex, pageSize, sortField, sortDirection);

                if (fetchedCampaigns == null)
                {
                    _logger.LogError("Campaigns not found");
                    return NotFound("Campaign Not Found");
                }
                var response = new
                {
                    filteredCampaigns = fetchedCampaigns,
                    TotalCampaigns = totalCampaigns
                };
                return Ok(response);


            }
            catch(InvalidStateOrDayException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Server error" + ex.Message);
                return StatusCode(500, "Server Error");
            }
        }

        [HttpGet("JustMatchingCampaigns")]
        public async Task<ActionResult<List<CampaignModel>>> JustMatchingCampaigns([FromQuery] List<States> states, [FromQuery] decimal loanAmount)
        {
            
            try
            {
                if (!states.Any() || loanAmount <= 0)
                {
                    return BadRequest("Invalid states or loan amount.");
                }
                var filteredCampaigns = await _campaigns.MatchingCampaigns(states, loanAmount);
                if (filteredCampaigns == null)
                {
                    _logger.LogError("Not found");
                    return NotFound("Campaigns with matching attributed not found");
                }
                return Ok(filteredCampaigns);
            }
            catch (Exception ex)

            {
                _logger.LogError("Exception occured" + ex.Message);
                return BadRequest("error");
            }
            
        }

        [HttpGet("MatchingCampaignByLeadLimit")]
        public async Task<ActionResult<List<CampaignModel>>> GetCampaignsByLeadLimit([FromQuery] List<States> states,[FromQuery] decimal loanAmount,[FromQuery] decimal cost)
        {
            try
            {
                var fetchedCampaigns = await _campaigns.GettingCampaignsByLeadLimit(states, loanAmount, cost);
                if(fetchedCampaigns == null)
                {
                    _logger.LogError("Could not find the campaigns");
                    return NotFound("Could not found the campaigns");
                }
                return Ok(fetchedCampaigns);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error in fetching campaigns on the server" + ex.Message);
                return BadRequest("Server error");
            }
        } 
    }
}

