using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Campaigns.Data
{
	public class DailyRecord
	{
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("campaignId")]
        public Guid CampaignId { get; set; }

        [BsonElement("checkedDate")]
        public DateTime CheckedDate { get; set; }

        [BsonElement("checkedCount")]
        public int CheckedCount { get; set; }

        [BsonElement("checkedSpendAmount")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal CheckedSpendAmount { get; set; }
	}
}

