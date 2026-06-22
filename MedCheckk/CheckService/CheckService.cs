using MedCheck.Data;
using MedCheck.Models;
using MedCheck.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MedCheck.Services
{
    public class CheckService
    {
        private readonly AppDbContext _db;
        public CheckService(AppDbContext db) => _db = db;

        public async Task<List<Disease>> SearchDiseasesAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return await _db.Diseases.OrderBy(d => d.NameUz).ToListAsync();

            var ql = q.ToLower();
            return await _db.Diseases
                .Where(d => d.NameUz.ToLower().Contains(ql) ||
                            d.Symptoms.ToLower().Contains(ql))
                .OrderBy(d => d.NameUz)
                .ToListAsync();
        }

        public async Task<List<Medicine>> SearchMedicinesAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return new();
            var ql = q.ToLower();
            return await _db.Medicines
                .Where(m => m.Name.ToLower().Contains(ql) ||
                            m.NameUz.ToLower().Contains(ql) ||
                            m.GenericName.ToLower().Contains(ql))
                .OrderBy(m => m.Name)
                .Take(15)
                .ToListAsync();
        }

        public async Task<CheckResultViewModel> AnalyzeAsync(CheckInputViewModel input)
        {
            var disease = await _db.Diseases
                .Include(d => d.MedicineLinks)
                .FirstOrDefaultAsync(d => d.Id == input.DiseaseId);

            var medicines = await _db.Medicines
                .Include(m => m.GenericAlternative)
                .Where(m => input.MedicineIds.Contains(m.Id))
                .ToListAsync();

            var relevantIds = disease?.MedicineLinks.Select(l => l.MedicineId).ToHashSet() ?? new();

            // Interaksiyalar
            var interactions = await _db.DrugInteractions
                .Include(i => i.MedicineA).Include(i => i.MedicineB)
                .Where(i => input.MedicineIds.Contains(i.MedicineAId) &&
                            input.MedicineIds.Contains(i.MedicineBId))
                .ToListAsync();

            var items = new List<CheckItem>();
            var resultItems = new List<CheckItemResult>();

            // Takroriy generic tekshirish
            var genericGroups = medicines
                .GroupBy(m => m.GenericName.ToLower())
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1).Select(m => m.Id))
                .ToHashSet();

            decimal totalOriginal = 0, totalOptimized = 0;

            foreach (var med in medicines)
            {
                bool isRelevant = !relevantIds.Any() || relevantIds.Contains(med.Id);
                bool isDuplicate = genericGroups.Contains(med.Id);

                string? issue = null;
                if (!isRelevant) issue = $"Bu dori '{disease?.NameUz}' kasalligiga mo'ljallanmagan";
                else if (isDuplicate) issue = $"Bir xil ta'sirli dori allaqachon tanlangan ({med.GenericName})";

                AlternativeMed? alt = null;
                if (med.GenericAlternative != null && !med.IsGeneric)
                {
                    var saving = (int)Math.Round((1 - med.GenericAlternative.Price / med.Price) * 100);
                    alt = new AlternativeMed
                    {
                        Id = med.GenericAlternative.Id,
                        Name = med.GenericAlternative.Name,
                        Price = med.GenericAlternative.Price,
                        SavingPercent = saving > 0 ? saving : 0
                    };
                }

                totalOriginal += med.Price;
                totalOptimized += (alt != null && isRelevant && !isDuplicate) ? alt.Price : med.Price;

                items.Add(new CheckItem
                {
                    MedicineId = med.Id,
                    Quantity = 1,
                    IsRelevant = isRelevant,
                    IsDuplicate = isDuplicate,
                    IssueReason = issue,
                    SuggestedAlternativeId = alt?.Id
                });

                resultItems.Add(new CheckItemResult
                {
                    MedicineId = med.Id,
                    Name = med.Name,
                    GenericName = med.GenericName,
                    Category = med.Category,
                    Price = med.Price,
                    Quantity = 1,
                    IsRelevant = isRelevant,
                    IsDuplicate = isDuplicate,
                    IssueReason = issue,
                    Alternative = alt
                });
            }

            // Saqlash
            var check = new Check
            {
                DiseaseId = disease?.Id,
                PatientNote = input.PatientNote,
                TotalOriginalPrice = totalOriginal,
                TotalOptimizedPrice = totalOptimized,
                Savings = totalOriginal - totalOptimized,
                Items = items
            };
            _db.Checks.Add(check);
            await _db.SaveChangesAsync();

            return new CheckResultViewModel
            {
                CheckId = check.Id,
                DiseaseName = disease?.NameUz ?? "Noma'lum",
                Items = resultItems,
                TotalOriginal = totalOriginal,
                TotalOptimized = totalOptimized,
                Savings = totalOriginal - totalOptimized,
                Interactions = interactions.Select(i => new InteractionWarning
                {
                    MedA = i.MedicineA.Name,
                    MedB = i.MedicineB.Name,
                    Severity = i.Severity,
                    Description = i.Description
                }).ToList()
            };
        }
    }
}
