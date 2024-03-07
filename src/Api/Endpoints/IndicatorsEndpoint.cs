using Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Skender.Stock.Indicators;
using Wolverine.Http;

namespace Api.Endpoints;

public class IndicatorsEndpoint(ScreenerService screenerService)
{
    private readonly ScreenerService _screenerService = screenerService;

    [WolverineGet("/getAlma")]
    public async Task<Results<Ok<IEnumerable<AlmaResult>>, NotFound>> GetAlma(HttpContext context)
    {
        var cancellationToken = context.RequestAborted;
        return
            (await _screenerService.GetAlma(9, cancellationToken))
                is IEnumerable<AlmaResult> results
            ? TypedResults.Ok(results)
            : TypedResults.NotFound();
    }
}
