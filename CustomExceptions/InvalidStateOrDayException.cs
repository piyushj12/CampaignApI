using System;
namespace CustomExceptions
{
	public class InvalidStateOrDayException: Exception
	{
		public InvalidStateOrDayException()
		{
		}

		public InvalidStateOrDayException(string message) : base(message)
		{

		}
	}
}

