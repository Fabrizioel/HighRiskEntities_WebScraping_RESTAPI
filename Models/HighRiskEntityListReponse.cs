namespace WebScrapingHighRisk.Models
{
    public class HighRiskEntityListReponse
    {
        public int TotalHits { get; set; }

        public List<HighRiskEntity>? HighRiskEntities { get; set; }
    }
}
