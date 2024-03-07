using Binance.Net.Clients;
using Skender.Stock.Indicators;

namespace Api.Services;

public class DataProvider
{
    private const short YEAR_COUNT = 3;

    public async Task<IEnumerable<Quote>> GetQuotes(
        IEnumerable<string> watchList,
        BinanceSocketClient binanceClient
    )
    {
        DateTime currentDate = DateTime.UtcNow.Date;
        var tasks = new List<Task<Quote?>>();
        for (int i = 0; i < (YEAR_COUNT * 365); i++)
        {
            DateTime date = currentDate.AddDays(-i);

            foreach (string ticker in watchList)
            {
                var task = Task.Run(async () =>
                {
                    var candlestickData = await binanceClient.SpotApi.ExchangeData.GetKlinesAsync(
                        ticker,
                        Binance.Net.Enums.KlineInterval.OneDay,
                        startTime: date,
                        endTime: date
                    );

                    Guard.Against.Null(candlestickData, nameof(candlestickData), "Cannot be null.");

                    if (!candlestickData.Data.Result.Any())
                    {
                        return null;
                    }

                    decimal openPrice = candlestickData.Data.Result.First().OpenPrice;
                    decimal highPrice = candlestickData.Data.Result.First().HighPrice;
                    decimal lowPrice = candlestickData.Data.Result.First().LowPrice;
                    decimal closePrice = candlestickData.Data.Result.First().ClosePrice;
                    decimal volume = candlestickData.Data.Result.First().Volume;

                    return new Quote()
                    {
                        Date = date,
                        Open = openPrice,
                        High = highPrice,
                        Low = lowPrice,
                        Close = closePrice,
                        Volume = volume
                    };
                });

                if (task is not null)
                {
                    tasks.Add(task);
                }
            }
        }

        var quotes = await Task.WhenAll(tasks);
        return quotes!;
    }
}
