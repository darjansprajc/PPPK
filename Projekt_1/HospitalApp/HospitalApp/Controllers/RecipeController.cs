using HospitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace HospitalApp.Controllers
{
    public class RecipeController : BaseController<RecipeController>
    {
        public RecipeController(ILogger<RecipeController> logger, DbHospitalContext dbContext) 
            : base(logger, dbContext)
        {
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var recipe = _dbContext.Recipes
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .FirstOrDefault(r => r.Id == id);

            if (recipe == null)
                return NotFound();

            return PartialView("_DetailsModal", recipe);
        }

        [HttpPost]
        public IActionResult Add(string Medicine, string Description, DateTime StartOfMedication, DateTime EndOfMedication, int PatientId, int DoctorId)
        {
            var recipe = new Recipe
            {
                Medicine = Medicine,
                Description = Description,
                StartOfMedication = DateOnly.FromDateTime(StartOfMedication),
                EndOfMedication = DateOnly.FromDateTime(EndOfMedication),
                PatientId = PatientId,
                DoctorId = DoctorId
            };

            _dbContext.Recipes.Add(recipe);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", "Patient", new { id = PatientId });
        }

        [HttpPost]
        public IActionResult Edit(int Id, string Medicine, string Description, DateTime StartOfMedication, DateTime EndOfMedication)
        {
            var recipe = _dbContext.Recipes.FirstOrDefault(r => r.Id == Id);
            if (recipe == null)
                return NotFound();

            recipe.Medicine = Medicine;
            recipe.Description = Description;
            recipe.StartOfMedication = DateOnly.FromDateTime(StartOfMedication);
            recipe.EndOfMedication = DateOnly.FromDateTime(EndOfMedication);

            _dbContext.SaveChanges();

            return RedirectToAction("Details", "Patient", new { id = recipe.PatientId });
        }
    }
}
