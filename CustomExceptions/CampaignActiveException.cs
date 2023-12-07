using System;
namespace CustomExceptions
{
	public class CampaignActiveException: Exception
	{

		public CampaignActiveException()
		{

		}
		public CampaignActiveException(Guid campaignId)
			:base($"Campaign with the Campaign Id:{campaignId} is in Active state, thus can't delete it")
		{
		}
	}
}

