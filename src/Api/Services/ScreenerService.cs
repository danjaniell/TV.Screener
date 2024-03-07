using Binance.Net.Clients;
using Skender.Stock.Indicators;

namespace Api.Services;

public class ScreenerService(DataProvider dataProvider, BinanceRestClient binanceClient)
{
    private readonly DataProvider _dataProvider = dataProvider;
    private readonly BinanceRestClient _binanceClient = binanceClient;

    public async Task<IEnumerable<AlmaResult>> GetAlma(
        int days,
        CancellationToken cancellationToken
    )
    {
        var results = (
            await _dataProvider.GetQuotes("SOLUSDT", _binanceClient, cancellationToken)
        ).GetAlma(9);

        if (results is null)
        {
            return [];
        }

        return results;
    }
}
