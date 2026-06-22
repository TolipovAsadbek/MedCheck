using Microsoft.EntityFrameworkCore;
using MedCheck.Models;

namespace MedCheck.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<MedicineDiseaseLink> MedicineDiseaseLinks { get; set; }
        public DbSet<DrugInteraction> DrugInteractions { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<CheckItem> CheckItems { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorConsultation> DoctorConsultations { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Medicine>()
                .HasOne(x => x.GenericAlternative)
                .WithMany()
                .HasForeignKey(x => x.GenericAlternativeId)
                .OnDelete(DeleteBehavior.SetNull);

            b.Entity<MedicineDiseaseLink>()
                .HasOne(x => x.Medicine).WithMany(x => x.DiseaseLinks)
                .HasForeignKey(x => x.MedicineId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<MedicineDiseaseLink>()
                .HasOne(x => x.Disease).WithMany(x => x.MedicineLinks)
                .HasForeignKey(x => x.DiseaseId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<DrugInteraction>()
                .HasOne(x => x.MedicineA).WithMany(x => x.InteractionsA)
                .HasForeignKey(x => x.MedicineAId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<DrugInteraction>()
                .HasOne(x => x.MedicineB).WithMany(x => x.InteractionsB)
                .HasForeignKey(x => x.MedicineBId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<CheckItem>()
                .HasOne(x => x.Medicine).WithMany(x => x.CheckItems)
                .HasForeignKey(x => x.MedicineId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<CheckItem>()
                .HasOne(x => x.SuggestedAlternative).WithMany()
                .HasForeignKey(x => x.SuggestedAlternativeId).OnDelete(DeleteBehavior.SetNull);

            b.Entity<CheckItem>()
                .HasOne(x => x.Check).WithMany(x => x.Items)
                .HasForeignKey(x => x.CheckId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<DoctorConsultation>()
                .HasOne(x => x.Doctor).WithMany(x => x.Consultations)
                .HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<Check>()
                .HasOne(x => x.Disease).WithMany()
                .HasForeignKey(x => x.DiseaseId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
