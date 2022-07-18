using System;
using MarketScreener.Contracts;

namespace MarketScreener.Jobs
{
	public class MagnetStrategy : IMagnetStrategy
	{
		private readonly IMarketData _marketData;

		public MagnetStrategy(IMarketData marketData)
		{
			_marketData = marketData;
		}

		public void SayHello()
        {
			Console.WriteLine("Hello!");
        }
	}
}

