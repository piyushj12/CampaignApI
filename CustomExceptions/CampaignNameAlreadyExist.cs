using System;
namespace CustomExceptions
{
	public class CampaignNameAlreadyExist: Exception
	{

		public CampaignNameAlreadyExist()
		{

		}
		public CampaignNameAlreadyExist(string campaignName)
			:base($"Campaign with the same name:{campaignName} already exist")
		{

		}
	}
}

