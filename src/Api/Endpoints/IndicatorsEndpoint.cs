using Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Skender.Stock.Indicators;
using Wolverine.Http;

namespace Api.Endpoints;

public class IndicatorsEndpoint(ScreenerService screenerService)
{
    private readonly ScreenerService _screenerService = screenerService;

    [WolverineGet("/getAlma")]
    public async Task<
        Results<
            Ok<IEnumerable<(DateTime, IEnumerable<double>, IEnumerable<(double, double, double)>)>>,
            NotFound
        >
    > GetAlma()
    {
        return
            (await _screenerService.GetMAMA(9))
                is IEnumerable<(
                    DateTime,
                    IEnumerable<double>,
                    IEnumerable<(double, double, double)>
                )> results
            ? TypedResults.Ok(results)
            : TypedResults.NotFound();
    }
}
