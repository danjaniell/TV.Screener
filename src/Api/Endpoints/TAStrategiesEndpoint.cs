using Api.Core;
using Api.Services;
using Wolverine.Http;

namespace Api.Endpoints;

public class TAStrategiesEndpoint(ScreenerService screenerService)
{
    private readonly ScreenerService _screenerService = screenerService;

    [WolverineGet("/getAlma")]
    public async Task<IEnumerable<MaMaRecord>> GetAlma()
    {
        var result = await _screenerService.GetMAMA(9);

        return result;
    }
}
