namespace MedCheck.ViewModels
{
    public class CheckInputViewModel
    {
        public int DiseaseId { get; set; }
        public List<int> MedicineIds { get; set; } = new();
        public string? PatientNote { get; set; }
    }

    public class CheckResultViewModel
    {
        public int CheckId { get; set; }
        public string DiseaseName { get; set; } = string.Empty;
        public List<CheckItemResult> Items { get; set; } = new();
        public decimal TotalOriginal { get; set; }
        public decimal TotalOptimized { get; set; }
        public decimal Savings { get; set; }
        public List<InteractionWarning> Interactions { get; set; } = new();
    }

    public class CheckItemResult
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsRelevant { get; set; }
        public bool IsDuplicate { get; set; }
        public string? IssueReason { get; set; }
        public AlternativeMed? Alternative { get; set; }
    }

    public class AlternativeMed
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SavingPercent { get; set; }
    }

    public class InteractionWarning
    {
        public string MedA { get; set; } = string.Empty;
        public string MedB { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
