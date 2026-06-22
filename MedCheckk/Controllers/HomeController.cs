using Microsoft.AspNetCore.Mvc;
using MedCheck.Data;
using MedCheck.Services;
using MedCheck.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MedCheck.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalMedicines = await _db.Medicines.CountAsync();
            ViewBag.TotalChecks = await _db.Checks.CountAsync();
            ViewBag.TotalSavings = await _db.Checks.SumAsync(c => c.Savings);
            ViewBag.TotalDoctors = await _db.Doctors.CountAsync(d => d.IsAvailable);
            return View();
        }
    }

    public class CheckController : Controller
    {
        private readonly CheckService _svc;
        private readonly AppDbContext _db;

        public CheckController(CheckService svc, AppDbContext db)
        {
            _svc = svc;
            _db = db;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> SearchDisease(string q)
        {
            var list = await _svc.SearchDiseasesAsync(q ?? "");
            return Json(list.Select(d => new { d.Id, d.NameUz, d.Symptoms }));
        }

        [HttpGet]
        public async Task<IActionResult> SearchMedicine(string q)
        {
            var list = await _svc.SearchMedicinesAsync(q ?? "");
            return Json(list.Select(m => new
            {
                m.Id,
                m.Name,
                m.NameUz,
                m.GenericName,
                m.Category,
                m.Price,
                m.IsGeneric,
                m.RequiresPrescription,
                m.Manufacturer,
                m.Country
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(CheckInputViewModel model)
        {
            model.MedicineIds ??= new List<int>();
            if (model.DiseaseId == 0 || !model.MedicineIds.Any())
            {
                TempData["Error"] = "Kasallik va kamida 1 ta dori tanlang!";
                return RedirectToAction("Index");
            }
            var result = await _svc.AnalyzeAsync(model);
            return View("Result", result);
        }

        public async Task<IActionResult> History()
        {
            var checks = await _db.Checks
                .Include(c => c.Disease)
                .Include(c => c.Items).ThenInclude(i => i.Medicine)
                .OrderByDescending(c => c.CreatedAt)
                .Take(50)
                .ToListAsync();
            return View(checks);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var check = await _db.Checks
                .Include(c => c.Disease)
                .Include(c => c.Items).ThenInclude(i => i.Medicine)
                .Include(c => c.Items).ThenInclude(i => i.SuggestedAlternative)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (check == null) return NotFound();
            return View(check);
        }
    }

    public class DoctorController : Controller
    {
        private readonly AppDbContext _db;
        public DoctorController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var doctors = await _db.Doctors.OrderByDescending(d => d.IsAvailable).ToListAsync();
            return View(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> Contact(int doctorId, string patientName, string? patientPhone, string message)
        {
            if (string.IsNullOrWhiteSpace(patientName) || string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Ism va xabarni kiriting!";
                return RedirectToAction("Index");
            }
            _db.DoctorConsultations.Add(new Models.DoctorConsultation
            {
                DoctorId = doctorId,
                PatientName = patientName,
                PatientPhone = patientPhone,
                Message = message
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Xabaringiz shifokorga yuborildi!";
            return RedirectToAction("Index");
        }
    }
}