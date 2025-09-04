using HospitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Controllers
{
    public class MedicalDocumentController : BaseController<MedicalDocumentController>
    {
        public MedicalDocumentController(ILogger<MedicalDocumentController> logger, DbHospitalContext dbContext) 
            : base(logger, dbContext)
        {
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var document = _dbContext.MedicalDocuments
                .Include(md => md.Doctor)
                .Include(md => md.Patient)
                .FirstOrDefault(md => md.Id == id);

            if (document == null)
                return NotFound();

            return PartialView("_DetailsModal", document);
        }

        [HttpPost]
        public IActionResult Add(string Title, string Sickness, string Diagnosis, DateTime StartOfSickness, DateTime? EndOfSickness, int PatientId, int DoctorId)
        {
            var document = new MedicalDocument
            {
                Title = Title,
                Sickness = Sickness,
                Diagnosis = Diagnosis,
                StartOfSickness = DateOnly.FromDateTime(StartOfSickness),
                EndOfSickness = EndOfSickness.HasValue ? DateOnly.FromDateTime(EndOfSickness.Value) : null,
                PatientId = PatientId,
                DoctorId = DoctorId
            };

            _dbContext.MedicalDocuments.Add(document);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", "Patient", new { id = PatientId });
        }

        [HttpPost]
        public IActionResult Edit(int Id, string Title, string Sickness, string Diagnosis, DateTime StartOfSickness, DateTime? EndOfSickness)
        {
            var document = _dbContext.MedicalDocuments.FirstOrDefault(md => md.Id == Id);
            if (document == null)
                return NotFound();

            document.Title = Title;
            document.Sickness = Sickness;
            document.Diagnosis = Diagnosis;
            document.StartOfSickness = DateOnly.FromDateTime(StartOfSickness);
            document.EndOfSickness = EndOfSickness.HasValue ? DateOnly.FromDateTime(EndOfSickness.Value) : null;

            _dbContext.SaveChanges();

            return RedirectToAction("Details", "Patient", new { id = document.PatientId });

        }

    }
}
