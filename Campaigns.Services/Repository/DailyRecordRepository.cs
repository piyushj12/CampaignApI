using System;
using Campaigns.Data;
using Campaigns.Models;
using MongoDB.Driver;

namespace Campaigns.Services
{
	public class DailyRecordRepository
	{

		private readonly IMongoCollection<DailyRecord> _record;
		
		public DailyRecordRepository(MongoDbSettings mongoDBSettings)
		{
            var database = new MongoClient(mongoDBSettings.ConnectionString).GetDatabase(mongoDBSettings.DataBaseName);
			_record = database.GetCollection<DailyRecord>("DailyRecord");
        }

		public async Task<DailyRecord> GetOrCreateDailyRecord(Guid campaignId, DateTime date)
		{
			var filter = Builders<DailyRecord>.Filter.Eq(c => c.CampaignId, campaignId) &
				Builders<DailyRecord>.Filter.Eq(c => c.CheckedDate, date.Date);

			var dailyRecord = await _record.Find(filter).FirstOrDefaultAsync();

			if(dailyRecord == null)
			{
				dailyRecord = new DailyRecord
				{
					CampaignId = campaignId,
					CheckedDate = date.Date,
					CheckedCount = 0,
					CheckedSpendAmount = 0

				};
				await _record.InsertOneAsync(dailyRecord);
			}

			return dailyRecord;
		}

		public async Task UpdateDailyRecord(DailyRecord dailyRecord, decimal cost)
		{
			dailyRecord.CheckedCount++;
			dailyRecord.CheckedSpendAmount += cost;

			var filter = Builders<DailyRecord>.Filter.Eq(c => c.Id, dailyRecord.Id);
			var update = Builders<DailyRecord>.Update
				.Set(c => c.CheckedCount, dailyRecord.CheckedCount)
				.Set(c => c.CheckedSpendAmount, dailyRecord.CheckedSpendAmount);

			await _record.UpdateOneAsync(filter, update);
		}

    }
}

