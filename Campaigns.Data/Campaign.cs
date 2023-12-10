using System;
using Campaigns.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Campaigns.Data
{
	public class Campaign
	{

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("campaignId")]
        public Guid CampaignId { get; set; }

        [BsonElement("campaignName")]
        public string CampaignName { get; set; }

        [BsonElement("daysOfWeek")]
        public List<DayOfWeek> DaysOfWeek { get; set; }

        [BsonElement("startDate")]
        public DateTime? StartDate { get; set; }

        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }

        [BsonElement("states")]
        public List<States> States { get; set; }

        [BsonElement("minLoanAmount")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? MinLoanAmount { get; set; }

        [BsonElement("maxLoanAmount")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? MaxLoanAmount { get; set; }

        [BsonElement("dailySpendLimit")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? DailySpendLimit { get; set; }

        [BsonElement("dailyLeadLimit")]
        public int? DailyLeadLimit { get; set; }

        [BsonElement("activeIndicator")]
        public bool? ActiveIndicator { get; set; }

        [BsonElement("underReviewIndicator")]
        public bool? UnderReviewIndicator { get; set; }
    }
}

