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
            // Obtener "name" como parámetro del url
            string requestData = HttpContext.Request.Query["name"];

            // Inicializar instancias para:
            //  - Response
            //  - Listado de "entities"
            //  - HttpClient
            //  - HtmlDocument
            HighRiskEntityListReponse highEntityRiskList = new();
            List<HighRiskEntity> entitiesAtRisk = new();
            HttpClient httpClient = new();
            HtmlDocument doc = new();

            // Cifrar "name" para que sea un query parameter en la url que se llamará
            string encodedString = WebUtility.UrlEncode(requestData);
            
            // Agregar header de "User-Agent" para que la página web no crea que es un bot
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            
            // Obtener la página web y pasarlo a un documento HTML
            HttpResponseMessage response = await httpClient.GetAsync($"https://offshoreleaks.icij.org/search?q={encodedString}&c=&j=&d=");
            Stream stream = await response.Content.ReadAsStreamAsync();
            doc.Load(stream);

            // Cargar listas para cada uno de los campos que entrega esta fuente
            var entities = doc.DocumentNode.SelectNodes("//td//a[contains(@class, 'font-weight-bold text-dark')]");
            var jurisdictions = doc.DocumentNode.SelectNodes("//td[contains(@class, 'jurisdiction')]");
            var linkedTo = doc.DocumentNode.SelectNodes("//td[contains(@class, 'country')]"); ;
            var dataFrom = doc.DocumentNode.SelectNodes("//td[contains(@class, 'source text-nowrap')]//a");

            // En caso se encuentren coincidencias
            if (entities == null) return highEntityRiskList;
            // Asignar total de hits
            highEntityRiskList.TotalHits = entities.Count;
            int index = 0;
            foreach (var item in entities)
            {
                // Crear y agregar entidades con sus respectivos atributos
                HighRiskEntity highRiskEntity = new(
                    item.InnerHtml.Trim(),
                    jurisdictions[index].InnerHtml.Trim(),
                    linkedTo[index].InnerHtml.Trim(),
                    dataFrom[index].InnerHtml.Trim()
                );
                entitiesAtRisk.Add(highRiskEntity);
                index++;
            }

            // Asignar lista de entidades
            highEntityRiskList.HighRiskEntities = entitiesAtRisk;

            return highEntityRiskList;


        }
    }
}
