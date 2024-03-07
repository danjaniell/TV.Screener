using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot;
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
                var task = FetchQuoteAsync(binanceClient, ticker, date);
                tasks.Add(task);
            }
        }

        var quotes = (await Task.WhenAll(tasks)).Where(x => x != null);
        return quotes!;
    }

    private static async Task<Quote?> FetchQuoteAsync(
        BinanceSocketClient binanceClient,
        string ticker,
        DateTime date
    )
    {
        var candle = await binanceClient.SpotApi.ExchangeData.GetKlinesAsync(
            ticker,
            KlineInterval.OneDay,
            startTime: date,
            endTime: date
        );

        Guard.Against.Null(candle, nameof(candle), "Cannot be null.");

        return candle.Data.Result.FirstOrDefault() is BinanceSpotKline result
            ? new Quote()
            {
                Date = date,
                Open = result.OpenPrice,
                High = result.HighPrice,
                Low = result.LowPrice,
                Close = result.ClosePrice,
                Volume = result.Volume
            }
            : null;
    }
}
