using System;
using Campaigns.Data;
using Campaigns.Models;
using CustomExceptions;
using MongoDB.Driver;

namespace Campaigns.Services.Repository
{
	public class CampaignMongoRepository
	{

		private readonly IMongoCollection<Campaign> _repo;

		public CampaignMongoRepository(MongoDbSettings settings)
		{
			var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DataBaseName);
			_repo = database.GetCollection<Campaign>("Campaigns");
		}


		public async Task AddCampaignToDb(Campaign campaign)
		{
			await _repo.InsertOneAsync(campaign);
		}

		public async Task<bool> UpdateCampaignToDB(Guid campaignId, Campaign campaign)
		{
			var filter = Builders<Campaign>.Filter.Eq(x => x.CampaignId, campaignId);
			var replace = _repo.Find(filter).Limit(1).First();

			campaign.Id = replace.Id;

			var updatedCampaign = _repo.FindOneAndReplace(filter, campaign);

			if(updatedCampaign == null)
			{
				return false;
			}
			return true;
		}


		public async Task<bool> DeleteCampaignFromDB(Guid campaignId)
		{
			var filter = Builders<Campaign>.Filter.Eq(x => x.CampaignId, campaignId);
			
			var IsDeleteSuccesful = await _repo.FindOneAndDeleteAsync(filter);
			return IsDeleteSuccesful!=null; 
		}


		public async Task<Campaign> GetCampaignByIdFromDb(Guid campaignId)
		{
			var filter = Builders<Campaign>.Filter.Eq(x => x.CampaignId, campaignId);
			var fetchedCampaign = _repo.Find(filter).Limit(1).First();

			return fetchedCampaign;
		}


		public async Task<(IEnumerable<Campaign> Items, long Total)> GetFilteredCampaigns(
			string? searchName, string? searchState,
            string? searchDay, int pageIndex = 1,
			int pageSize = 10, string sortField = "CampaignName",
			string sortDirection = "asc")
		{
			var filter = SearchFilter(searchName, searchState, searchDay);

			var sortdefinition = GetSortedDefinition(sortField, sortDirection);

			int skipCount = (pageIndex - 1) * pageSize;

			var result = await _repo
				.Find(filter)
				.Sort(sortdefinition)
				.Skip(skipCount)
				.Limit(pageSize)
				.ToListAsync();

			

			var totalCampaigns = await _repo.CountDocumentsAsync(filter);


			return (result, totalCampaigns);
			
			
		}


		public FilterDefinition<Campaign> SearchFilter(string? searchName, string? searchState, string? searchDay)
		{

			var filterBuilder = Builders<Campaign>.Filter;
			FilterDefinition<Campaign> filter = filterBuilder.Empty;

			if (!string.IsNullOrEmpty(searchName))
				filter &= filterBuilder.Eq(c => c.CampaignName, searchName);
			if (!string.IsNullOrEmpty(searchState))
			{ 
				if(Enum.TryParse<States>(searchState, out var stateEnum))
				{
                    filter &= filterBuilder.Eq("States", stateEnum);
                }
				else
				{
					throw new InvalidStateOrDayException("Invalid State");
				}
			}
				
			if (!string.IsNullOrEmpty(searchDay))
			{
                if (Enum.TryParse<DayOfWeek>(searchDay, out var dayEnum))
                {
                    filter &= filterBuilder.Eq("DaysOfWeek", dayEnum);
                }
                else
                {
                    throw new InvalidStateOrDayException("Invalid Day");
                }
            }
               

            return filter;

		}






        // Common Methods 

        public SortDefinition<Campaign> GetSortedDefinition(string sortField, string sortDirection)
        {
            //Validations

            var validSortFields = new HashSet<string> {"campaignname", "campaignid", "startdate",
                "enddate", "minloanamount", "maxloanamount", "dailyleadlimit",
                "activeindicator", "underreviewindicator", "daysofweek", "states"};

            var validSortDirections = new HashSet<string> { "asc", "desc" };

            if (!validSortFields.Contains(sortField?.ToLower()))
                throw new ArgumentException("Invalid Sort Field");
            if (!validSortDirections.Contains(sortDirection?.ToLower()))
                throw new ArgumentException("Invalid Sort Direction");


            switch (sortField?.ToLower())
            {
                case "campaignname":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.CampaignName) : Builders<Campaign>.Sort.Ascending(x => x.CampaignName);
                case "minloanamount":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.MinLoanAmount) : Builders<Campaign>.Sort.Ascending(x => x.MinLoanAmount);
                case "maxloanamount":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.MaxLoanAmount) : Builders<Campaign>.Sort.Ascending(x => x.MaxLoanAmount);
                case "dailyspendlimit":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.DailySpendLimit) : Builders<Campaign>.Sort.Ascending(x => x.DailySpendLimit);
                case "dailyleadlimit":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.DailyLeadLimit) : Builders<Campaign>.Sort.Ascending(x => x.DailyLeadLimit);
                case "startdate":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.StartDate) : Builders<Campaign>.Sort.Ascending(x => x.StartDate);
                case "enddate":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.EndDate) : Builders<Campaign>.Sort.Ascending(x => x.EndDate);
                case "activeindicator":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.ActiveIndicator) : Builders<Campaign>.Sort.Ascending(x => x.ActiveIndicator);
                case "underreviewindicator":
                    return sortDirection == "desc" ? Builders<Campaign>.Sort.Descending(x => x.UnderReviewIndicator) : Builders<Campaign>.Sort.Ascending(x => x.UnderReviewIndicator);
                // Add additional cases as needed for other fields
                default:
                    return Builders<Campaign>.Sort.Ascending(x => x.CampaignName); // Default sorting

            }
        }


        public async Task<bool> IsCampaignActive(Guid campaignId)
		{
			var filter = Builders<Campaign>.Filter.Eq(x => x.CampaignId, campaignId);
            var campaignToBeDeleted = _repo.Find(filter).Limit(1).First();
            if (campaignToBeDeleted.ActiveIndicator == true)
            {
                return true;
            }
			return false; 
        }


        public async Task<bool> IsCampaignNameExist(Campaign campaign)
		{
			return await _repo.Find(c => c.CampaignName == campaign.CampaignName).AnyAsync();
		}


	}
}

