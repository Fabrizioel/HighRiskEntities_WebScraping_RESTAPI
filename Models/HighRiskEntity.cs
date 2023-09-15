namespace WebScrapingHighRisk.Models
{
    public class HighRiskEntity
    {
        public string? Entity { get; set; }

        public string? Jurisdiction { get; set; }

        public string? LinkedTo { get; set; }

        public string? DataFrom { get; set; }

        public HighRiskEntity(string? entity, string? jurisdiction, string? linkedTo, string? dataFrom) 
        {
            Entity = entity;
            Jurisdiction = jurisdiction;
            LinkedTo = linkedTo;
            DataFrom = dataFrom;
        }
    }
}
