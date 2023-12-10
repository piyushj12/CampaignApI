using System;
using System.Linq;
using AutoMapper;
using Campaigns.Data;
using Campaigns.Models;
using Campaigns.Services;
using Campaigns.Services.Interfaces;
using Campaigns.Services.Repository;
using CustomExceptions;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;

namespace Campaigns.Services.Services
{
	public class CampaignServices: ICampaignInterface
	{
		private readonly CampaignMongoRepository _campaignMongoRepository;
		private readonly DailyRecordRepository _dailyRecordRepository;

		private readonly IMemoryCache _memoryCache;

		private readonly IMapper _mapper;

		public CampaignServices(CampaignMongoRepository repo, IMemoryCache memoryCache, DailyRecordRepository dailyRepo)
		{
			this._campaignMongoRepository = repo;
			this._memoryCache = memoryCache;
			this._mapper = GetMapper();
			this._dailyRecordRepository = dailyRepo;
		}

		//IMapping Configuration
		private IMapper GetMapper()
		{
			var configuration = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<CampaignModel, Campaign>();
				cfg.CreateMap<Campaign, CampaignModel>();
			});

			return configuration.CreateMapper();
		}

		//Adding Campaign

        public async Task<CampaignModel> AddCampaigns(CampaignModel campaignModel)
        {

			campaignModel.CampaignId = Guid.NewGuid();

			var entity = _mapper.Map<Campaign>(campaignModel);

			//checking if the campaign with the same name exist
			var IsCampaignExist = await _campaignMongoRepository.IsCampaignNameExist(entity);

			if(IsCampaignExist == true)
			{
				throw new CampaignNameAlreadyExist(campaignModel.CampaignName);
			}


            await _campaignMongoRepository.AddCampaignToDb(entity);

			return campaignModel;
        }


		//Updating Campaign
        public async Task<CampaignModel> UpdatingCampaign(Guid guid, CampaignModel campaignModel)
        {
			var campaign = _mapper.Map<Campaign>(campaignModel);

			var IsCampaignAlreadyExist = await _campaignMongoRepository.IsCampaignNameExist(campaign);
			if (IsCampaignAlreadyExist)
			{
				throw new CampaignNameAlreadyExist(campaignModel.CampaignName);
			}

			await _campaignMongoRepository.UpdateCampaignToDB(guid, campaign);
			return campaignModel;

        }


		//Deleting Campaign
        public async Task<bool> DeletingCampaign(Guid campaignId)
        {

			var IsActive = await _campaignMongoRepository.IsCampaignActive(campaignId);
			if(IsActive == true)
			{
                throw new CampaignActiveException(campaignId);
            }

			var response = await _campaignMongoRepository.DeleteCampaignFromDB(campaignId);
		
			return response;
        }

		//Get Campaign By Id

        public async Task<CampaignModel> GettingCampaignById(Guid campaignId)
        {

			var campaign = await _campaignMongoRepository.GetCampaignByIdFromDb(campaignId);

			return _mapper.Map<CampaignModel>(campaign);
            
        }

		//Get Filtered Campaigns 

        public async Task<(IEnumerable<CampaignModel> Items, long Total)> GetFilteredCampaigns(
			string? searchName, string? searchState,
			string? searchDay, int pageIndex = 1,
			int pageSize = 10, string sortField = "CampaignName",
			string sortDirection = "asc")

		{
			var (campaign, totalCampaigns) = await _campaignMongoRepository.GetFilteredCampaigns(searchName, searchState, searchDay,
				pageIndex, pageSize, sortField, sortDirection);

			var campaignModels =  _mapper.Map<IEnumerable<CampaignModel>>(campaign);
			return (campaignModels, totalCampaigns);
        }



        public async Task<List<CampaignModel>> MatchingCampaigns(List<States> states, decimal loanAmount)
        {
			var fethedCampaigns = await _campaignMongoRepository.GetMatchingCampaigns(states, loanAmount);
			return _mapper.Map<List<CampaignModel>>(fethedCampaigns);
        }

        public async Task<List<CampaignModel>> GettingCampaignsByLeadLimit(List<States> states, decimal loanAmount, decimal cost)
        {

			var todayDate = DateTime.Today;
			var validCampaigns = new List<CampaignModel>();

			var matchingCampaigns = await _campaignMongoRepository.GetMatchingCampaigns(states, loanAmount);

			foreach(var campaign in matchingCampaigns)
			{
				if(!campaign.ActiveIndicator.HasValue || !campaign.ActiveIndicator.Value ||
					campaign.UnderReviewIndicator.HasValue && campaign.UnderReviewIndicator.Value)
				{
					continue;
				}

				var dailyRecord = await _dailyRecordRepository.GetOrCreateDailyRecord(campaign.CampaignId, todayDate);

                if (campaign.DailyLeadLimit.HasValue && campaign.DailyLeadLimit.Value > 0 && dailyRecord.CheckedCount >= campaign.DailyLeadLimit.Value)
                {
                    continue; // Skip if lead limit exceeded
                }

                if (campaign.DailySpendLimit.HasValue && dailyRecord.CheckedSpendAmount + cost >= campaign.DailySpendLimit.Value)
				{
					continue;
				}

				await _dailyRecordRepository.UpdateDailyRecord(dailyRecord, cost);

				validCampaigns.Add(_mapper.Map<CampaignModel>(campaign));
			}

			return validCampaigns;
        }

    }
}



