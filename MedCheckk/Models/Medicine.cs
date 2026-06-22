using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedCheck.Models
{
    public class Medicine
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string NameUz { get; set; } = string.Empty;

        [MaxLength(200)]
        public string GenericName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Form { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Dosage { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Indications { get; set; } = string.Empty;

        [MaxLength(300)]
        public string SideEffects { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Contraindications { get; set; } = string.Empty;

        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        public bool IsGeneric { get; set; }
        public bool RequiresPrescription { get; set; }
        public int DailyDosage { get; set; } = 1;

        public int? GenericAlternativeId { get; set; }
        public Medicine? GenericAlternative { get; set; }

        public ICollection<MedicineDiseaseLink> DiseaseLinks { get; set; } = new List<MedicineDiseaseLink>();
        public ICollection<DrugInteraction> InteractionsA { get; set; } = new List<DrugInteraction>();
        public ICollection<DrugInteraction> InteractionsB { get; set; } = new List<DrugInteraction>();
        public ICollection<CheckItem> CheckItems { get; set; } = new List<CheckItem>();
    }

    public class Disease
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string NameUz { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Symptoms { get; set; } = string.Empty;

        public ICollection<MedicineDiseaseLink> MedicineLinks { get; set; } = new List<MedicineDiseaseLink>();
    }

    public class MedicineDiseaseLink
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; } = null!;
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; } = null!;
        public int Relevance { get; set; } = 100;
    }

    public class DrugInteraction
    {
        public int Id { get; set; }
        public int MedicineAId { get; set; }
        public Medicine MedicineA { get; set; } = null!;
        public int MedicineBId { get; set; }
        public Medicine MedicineB { get; set; } = null!;

        [MaxLength(20)]
        public string Severity { get; set; } = "Medium";

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    public class Check
    {
        public int Id { get; set; }
        public int? DiseaseId { get; set; }
        public Disease? Disease { get; set; }
        public string? PatientNote { get; set; }
        public decimal TotalOriginalPrice { get; set; }
        public decimal TotalOptimizedPrice { get; set; }
        public decimal Savings { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<CheckItem> Items { get; set; } = new List<CheckItem>();
    }

    public class CheckItem
    {
        public int Id { get; set; }
        public int CheckId { get; set; }
        public Check Check { get; set; } = null!;
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public bool IsRelevant { get; set; } = true;
        public bool IsDuplicate { get; set; }
        public string? IssueReason { get; set; }
        public int? SuggestedAlternativeId { get; set; }
        public Medicine? SuggestedAlternative { get; set; }
    }

    public class Doctor
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Hospital { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal ConsultationFee { get; set; }

        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<DoctorConsultation> Consultations { get; set; } = new List<DoctorConsultation>();
    }

    public class DoctorConsultation
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        [Required, MaxLength(200)]
        public string PatientName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PatientPhone { get; set; }

        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
