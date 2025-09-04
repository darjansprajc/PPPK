using HospitalApp.Models;
using HospitalApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace HospitalApp.Controllers
{
    public class PatientController : BaseController<PatientController>
    {
        public PatientController(ILogger<PatientController> logger, DbHospitalContext dbContext)
            : base(logger, dbContext)
        { 
        }

        public IActionResult Index(string search)
        {
            Doctor? doctor = HttpContext.Session.GetObject<Doctor>("SelectedDoctor");

            if (doctor == null)
            {
                return NoContent();
            }

            var patients = string.IsNullOrWhiteSpace(search) ? 
                  _dbContext.Patients
                            .OrderBy(p => p.Id)
                            .ToList()
                : _dbContext.Patients
                    .Where(p =>
                        p.FirstName.ToLower().Contains(search.ToLower()) ||
                        p.LastName.ToLower().Contains(search.ToLower()) ||
                        p.Oib.ToLower().Contains(search.ToLower()))
                    .OrderBy(p => p.Id)
                    .ToList();

            ViewBag.Patients = patients;
            ViewBag.Search = search;
            return View();
        }

        public IActionResult Details(int id)
        {
            var patient = _dbContext.Patients
                .Include(p => p.MedicalDocuments)
                .Include(p => p.Checkups)
                .Include(p => p.Recipes)
                .FirstOrDefault(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.Patient = patient;
            ViewBag.CheckupTypes = _dbContext.CheckupTypes.ToList();
            ViewBag.Doctors = _dbContext.Doctors.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AddPatient(string FirstName, string LastName, string Oib, DateTime DateOfBirth, char Gender)
        {
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Oib) ||
                (Gender != 'M' && Gender != 'F'))
            {
                TempData["Error"] = "Please fill all fields correctly.";
                return RedirectToAction("Index");
            }

            if (_dbContext.Patients.Any(p => p.Oib == Oib))
            {
                TempData["Error"] = "A patient with the same OIB already exists.";
                return RedirectToAction("Index");
            }

            if(Oib.Length != 11 || !Oib.All(char.IsDigit))
            {
                TempData["Error"] = "OIB must be exactly 11 digits.";
                return RedirectToAction("Index");
            }

            var patient = new Patient
            {
                FirstName = FirstName,
                LastName = LastName,
                Oib = Oib,
                DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
                Gender = Gender
            };

            _dbContext.Patients.Add(patient);
            _dbContext.SaveChanges();

            TempData["Success"] = "Patient added successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditPatient(int Id, string FirstName, string LastName, string Oib, DateTime DateOfBirth, char Gender)
        {
            var patient = _dbContext.Patients.FirstOrDefault(p => p.Id == Id);
            if (patient == null)
            {
                return NotFound();
            }

            if (_dbContext.Patients.Any(p => p.Oib == Oib))
            {
                TempData["Error"] = "A patient with the same OIB already exists.";
                return RedirectToAction("Index");
            }

            if (Oib.Length != 11 || !Oib.All(char.IsDigit))
            {
                TempData["Error"] = "OIB must be exactly 11 digits.";
                return RedirectToAction("Index");
            }

            patient.FirstName = FirstName;
            patient.LastName = LastName;
            patient.Oib = Oib;
            patient.DateOfBirth = DateOnly.FromDateTime(DateOfBirth);
            patient.Gender = Gender;

            _dbContext.SaveChanges();

            TempData["Success"] = "Patient updated successfully.";
            return RedirectToAction("Details", new { id = Id });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var patient = _dbContext.Patients
                .Include(p => p.MedicalDocuments)
                .Include(p => p.Checkups)
                .Include(p => p.Recipes)
                .FirstOrDefault(p => p.Id == id);

            if (patient == null)
            {
                TempData["Error"] = "Patient not found.";
                return RedirectToAction("Index");
            }

            _dbContext.MedicalDocuments.RemoveRange(patient.MedicalDocuments);
            _dbContext.Checkups.RemoveRange(patient.Checkups);
            _dbContext.Recipes.RemoveRange(patient.Recipes);
            _dbContext.Patients.Remove(patient);
            _dbContext.SaveChanges();
            _dbContext.SaveChanges();

            TempData["Success"] = "Patient deleted successfully.";
            return RedirectToAction("Index");
        }

        public char CSV_DELIMITER = ';';

        [HttpGet]
        public IActionResult ExportToCsv()
        {
            StringBuilder csvBuilder = new StringBuilder();

            csvBuilder.AppendLine($"Id{CSV_DELIMITER}FirstName{CSV_DELIMITER}LastName{CSV_DELIMITER}Oib{CSV_DELIMITER}DateOfBirth{CSV_DELIMITER}Gender");

            var patients = _dbContext.Patients.OrderBy(p => p.Id).ToList();

            foreach (Patient patient in patients)
            {
                csvBuilder.AppendLine(
                    $"{patient.Id}{CSV_DELIMITER}" +
                    $"{patient.FirstName}{CSV_DELIMITER}" +
                    $"{patient.LastName}{CSV_DELIMITER}" +
                    $"{patient.Oib}{CSV_DELIMITER}" +
                    $"{patient.DateOfBirth:dd.MM.yyyy}{CSV_DELIMITER}" +
                    $"{patient.Gender}");
            }

            var csvBytes = Encoding.GetEncoding("windows-1252").GetBytes(csvBuilder.ToString());
            return File(csvBytes, "text/csv", "Patients.csv");
        }
    }
}
