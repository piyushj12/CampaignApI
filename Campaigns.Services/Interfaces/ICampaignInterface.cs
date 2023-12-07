using System;
using Campaigns.Models;
using MongoDB.Driver;

namespace Campaigns.Services.Interfaces
{
	public interface ICampaignInterface
	{
		Task<CampaignModel> AddCampaigns(CampaignModel campaignModel);

		Task<CampaignModel> UpdatingCampaign(Guid guid, CampaignModel campaignModel);

		Task<bool> DeletingCampaign(Guid campaignId);

		Task<CampaignModel> GettingCampaignById(Guid campaignId);

        Task<(IEnumerable<CampaignModel> Items, long Total)> GetFilteredCampaigns(
             string? searchName,
             string? searchState,
             string? searchDay,
             int pageIndex = 1,
             int pageSize = 10,
             string sortField = "CampaignName",
             string sortDirection = "asc");

    }
}

