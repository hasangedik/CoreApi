using System.Threading.Tasks;
using CoreApi.Common.Extensions;
using CoreApi.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenWeatherMap;

namespace CoreApi.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WeathersController : CommonController
    {
        [HttpGet]
        [Route("city/{cityName}")]
        public async Task<ActionResult> GetByCityNameAsync(string cityName)
        {
            var client = new OpenWeatherMapClient("API_KEY");
            var weatherInfo = await client.CurrentWeather.GetByName(cityName: cityName.UrlFriendly(), metric: MetricSystem.Metric);

            return Ok(weatherInfo);
        }
    }
}