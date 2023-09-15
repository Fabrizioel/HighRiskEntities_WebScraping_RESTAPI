using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using WebScrapingHighRisk.Models;
using System.Net;
using System.Diagnostics;

namespace WebScrapingHighRisk.Controllers
{
    [Route("searchRiskEntity")]
    [ApiController]
    public class WebScrapingController : ControllerBase
    {
        [HttpGet]
        public async Task<HighRiskEntityListReponse> GetScrapingResult()
        {
            string requestData = HttpContext.Request.Query["name"];

            HighRiskEntityListReponse highEntityRiskList = new();
            List<HighRiskEntity> entitiesAtRisk = new();

            HttpClient httpClient = new();
            string encodedString = WebUtility.UrlEncode(requestData);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            HttpResponseMessage response = await httpClient.GetAsync($"https://offshoreleaks.icij.org/search?q={encodedString}&c=&j=&d=");
            Stream stream = await response.Content.ReadAsStreamAsync();
            HtmlDocument doc = new();
            doc.Load(stream);

            var entities = doc.DocumentNode.SelectNodes("//td//a[contains(@class, 'font-weight-bold text-dark')]");
            var jurisdictions = doc.DocumentNode.SelectNodes("//td[contains(@class, 'jurisdiction')]");
            var linkedTo = doc.DocumentNode.SelectNodes("//td[contains(@class, 'country')]"); ;
            var dataFrom = doc.DocumentNode.SelectNodes("//td[contains(@class, 'source text-nowrap')]//a");

            if (entities != null)
            {
                highEntityRiskList.TotalHits = entities.Count;
                int index = 0;
                foreach (var item in entities)
                {
                    HighRiskEntity highRiskEntity = new(
                        item.InnerHtml.Trim(),
                        jurisdictions[index].InnerHtml.Trim(),
                        linkedTo[index].InnerHtml.Trim(),
                        dataFrom[index].InnerHtml.Trim()
                    );
                    entitiesAtRisk.Add(highRiskEntity);
                    index++;
                }

                highEntityRiskList.HighRiskEntities = entitiesAtRisk;
            }

            return highEntityRiskList;


        }
    }
}
