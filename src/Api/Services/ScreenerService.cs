using Api.Core;
using Binance.Net.Clients;
using Microsoft.Extensions.Options;
using Skender.Stock.Indicators;

namespace Api.Services;

public class ScreenerService(
    DataProvider dataProvider,
    BinanceSocketClient binanceClient,
    IOptionsMonitor<WatchList> options
)
{
    private readonly DataProvider _dataProvider = dataProvider;
    private readonly BinanceSocketClient _binanceClient = binanceClient;
    private readonly IEnumerable<string> _watchList = options.CurrentValue;

    /// <summary>
    /// MAMA Strategy: ALMA + MACD.
    /// MACD line crosses over and above the signal line, combined with the stock price going above the ALMA line </summary>
    /// <param name="days"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<MaMaRecord>> GetMAMA(int days)
    {
        var quotes = await _dataProvider.GetQuotes(_watchList, _binanceClient);

        var almaResults = quotes.GetAlma(days);
        var macdResults = quotes.GetMacd();

        Guard.Against.NullOrEmpty(almaResults, nameof(almaResults), "ALMA results cannot be null.");
        Guard.Against.NullOrEmpty(macdResults, nameof(macdResults), "MACD results cannot be null.");

        var combinedResults = almaResults.Cast<object>().Concat(macdResults.Cast<object>());

        var groupedResults = combinedResults.GroupBy(result =>
        {
            return result switch
            {
                AlmaResult almaResult => almaResult.Date,
                MacdResult macdResult => macdResult.Date,
                _ => throw new InvalidOperationException("Unknown result type")
            };
        });

        var aggregatedResults = groupedResults.Select(group =>
        {
            var date = group.Key;
            var almaResult = group.OfType<AlmaResult>().FirstOrDefault();
            var macdResult = group.OfType<MacdResult>().FirstOrDefault();

            return new MaMaRecord(
                date,
                almaResult?.Alma ?? 0,
                macdResult?.Macd ?? 0,
                macdResult?.Signal ?? 0,
                macdResult?.Histogram ?? 0
            );
        });

        return aggregatedResults;
    }
}
