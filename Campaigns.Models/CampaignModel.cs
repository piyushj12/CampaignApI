using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace Campaigns.Models
{
	public class CampaignModel
	{
		[JsonProperty("campaignId")]
		public Guid CampaignId { get; set; }

        [Required]
        [JsonProperty("campaignName")]
        public string CampaignName { get; set; }

        [Required]
        [MinLength(2)]
        [JsonProperty("daysOfWeek")]
        public List<DayOfWeek> DaysOfWeek { get; set; }

        [JsonProperty("startDate")]
 
        public DateTime? StartDate { get; set; }


        [JsonProperty("endDate")]
        [DateValidation("StartDate")]
        public DateTime? EndDate { get; set; }

        [Required]
        [MinLength(1)]
        [JsonProperty("states")]
        public List<States> States { get; set; }

        [JsonProperty("minLoanAmount")]
        public decimal? MinLoanAmount { get; set; }

        [JsonProperty("maxLoanAmount")]
        public decimal? MaxLoanAmount { get; set; }

        [JsonProperty("dailySpendLimit")]
        public decimal? DailySpendLimit { get; set; }

        [JsonProperty("dailyLeadLimit")]
        public int? DailyLeadLimit { get; set; }

        [JsonProperty("activeIndicator")]
        public bool? ActiveIndicator { get; set; }

        [JsonProperty("underReviewIndicator")]
        public bool? UnderReviewIndicator { get; set;  }

	}
}

