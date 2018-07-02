using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Controllers
{
    [Route("panel")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        private readonly IPanelRepository _panelRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository, IPanelRepository panelRepository)
        {
            _analyticsRepository = analyticsRepository;
            _panelRepository = panelRepository;
        }

        // GET panel/XXXX1111YYYY2222/analytics
        [HttpGet("{banelId}/[controller]")]
        public async Task<IActionResult> Get([FromRoute] string panelId)
        {
            try
            {
                var panel = await _panelRepository.Query()
                .FirstOrDefaultAsync(x => x.Serial.Equals(panelId, StringComparison.CurrentCultureIgnoreCase));

                if (panel == null) return NotFound();

                var analytics = await _analyticsRepository.Query()
                    .Where(x => x.PanelId.Equals(panelId, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();

                var result = new OneHourElectricityListModel
                {
                    OneHourElectricitys = analytics.Select(c => new OneHourElectricityModel
                    {
                        Id = c.Id,
                        KiloWatt = c.KiloWatt,
                        DateTime = c.DateTime
                    })
                };

                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }

        }

        // GET panel/XXXX1111YYYY2222/analytics/day
        [HttpGet("{panelId}/[controller]/day")]
        public async Task<IActionResult> DayResults([FromRoute] string panelId)
        {
            try
            {
                var result = new List<OneDayElectricityModel>();
                var getData = new OneDayElectricityModel();
                var analytics = await _analyticsRepository.Query()
               .Where(x => x.PanelId.Equals(panelId, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
                var a = analytics.OrderByDescending(z => z.DateTime).Last();
                var b = DateTime.Now - a.DateTime;
                int days = (int)b.TotalDays;
                for (int i = 1; i < days + 1; i++)
                {
                    var c = analytics.Where(y => y.DateTime.Date == DateTime.Now.AddDays(-i).Date).ToList();
                    if (c.Count > 0)
                    {
                        getData.DateTime = DateTime.Now.AddDays(-i);
                        var d = c.OrderByDescending(x => x.KiloWatt).First();
                        var e = c.OrderByDescending(x => x.KiloWatt).Last();
                        getData.Maximum = d.KiloWatt;
                        getData.Minimum = e.KiloWatt;
                        getData.Sum = c.Select(w => w.KiloWatt).Sum();
                        getData.Average = c.Select(w => w.KiloWatt).Sum() / c.Count();
                        result.Add(getData);
                    }

                }

                return Ok(result);
            }
            catch (Exception Ex)
            {

                throw;
            }

        }

        // POST panel/XXXX1111YYYY2222/analytics
        [HttpPost("{panelId}/[controller]")]
        public async Task<IActionResult> Post([FromRoute] string panelId, [FromBody] OneHourElectricityModel value)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var oneHourElectricityContent = new OneHourElectricity
                {
                    PanelId = panelId,
                    KiloWatt = value.KiloWatt,
                    DateTime = DateTime.UtcNow
                };

                await _analyticsRepository.InsertAsync(oneHourElectricityContent);

                var result = new OneHourElectricityModel
                {
                    Id = oneHourElectricityContent.Id,
                    KiloWatt = oneHourElectricityContent.KiloWatt,
                    DateTime = oneHourElectricityContent.DateTime
                };

                return Created($"panel/{panelId}/analytics/{result.Id}", result);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}