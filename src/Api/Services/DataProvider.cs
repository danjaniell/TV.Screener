using Binance.Net.Clients;
using Skender.Stock.Indicators;

namespace Api.Services;

public class DataProvider
{
    private const short YEAR_COUNT = 3;

    public async Task<IEnumerable<Quote>> GetQuotes(
        string ticker,
        BinanceRestClient binanceClient,
        CancellationToken cancellationToken
    )
    {
        DateTime currentDate = DateTime.UtcNow.Date;
        var tasks = new List<Task<Quote>>();

        for (int i = 0; i < (YEAR_COUNT * 365); i++)
        {
            DateTime date = currentDate.AddDays(-i);

            var task = Task.Run(async () =>
            {
                var candlestickData = await binanceClient.SpotApi.ExchangeData.GetKlinesAsync(
                    ticker,
                    Binance.Net.Enums.KlineInterval.OneDay,
                    startTime: date,
                    endTime: date,
                    ct: cancellationToken
                );

                decimal openPrice = candlestickData.Data.First().OpenPrice;
                decimal highPrice = candlestickData.Data.First().HighPrice;
                decimal lowPrice = candlestickData.Data.First().LowPrice;
                decimal closePrice = candlestickData.Data.First().ClosePrice;
                decimal volume = candlestickData.Data.First().Volume;

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

            tasks.Add(task);
        }

        var quotes = await Task.WhenAll(tasks);
        return quotes;
    }
}
