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
    public async Task<
        IEnumerable<(DateTime, IEnumerable<double>, IEnumerable<(double, double, double)>)>
    > GetMAMA(int days)
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
            var almaValue = group.OfType<AlmaResult>().Select(x => x.Alma ?? 0);
            var macdValue = group
                .OfType<MacdResult>()
                .Select(x => (x.Macd ?? 0, x.Signal ?? 0, x.Histogram ?? 0));
            return (date, almaValue, macdValue);
        });

        return aggregatedResults;
    }
}
