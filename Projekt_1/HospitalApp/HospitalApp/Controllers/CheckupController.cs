using HospitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace HospitalApp.Controllers
{
    public class CheckupController : BaseController<CheckupController>
    {
        public CheckupController(ILogger<CheckupController> logger, DbHospitalContext dbContext) : base(logger, dbContext)
        {
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var checkup = _dbContext.Checkups
                .Include(c => c.Type)
                .Include(c => c.CheckupDoctors).ThenInclude(cd => cd.Doctor)
                .Include(c => c.CheckupImages)
                .Include(c => c.Patient)
                .FirstOrDefault(c => c.Id == id);

            if (checkup == null)
                return NotFound();

            ViewBag.CheckupTypes = _dbContext.CheckupTypes.ToList();
            ViewBag.Doctors = _dbContext.Doctors.ToList();

            return PartialView("_DetailsModal", checkup);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string Title, string Description, DateTime Time, int TypeId, int PatientId, List<int> DoctorIds, List<IFormFile> CheckupImages)
        {
            var checkup = new Checkup
            {
                Title = Title,
                Description = Description,
                Time = Time,
                TypeId = TypeId,
                PatientId = PatientId
            };

            _dbContext.Checkups.Add(checkup);
            await _dbContext.SaveChangesAsync();

            foreach (var doctorId in DoctorIds)
            {
                _dbContext.CheckupDoctors.Add(new CheckupDoctor { CheckupId = checkup.Id, DoctorId = doctorId });
            }

            if (CheckupImages != null)
            {
                foreach (var file in CheckupImages)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine("wwwroot/images", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        _dbContext.CheckupImages.Add(new CheckupImage
                        {
                            CheckupId = checkup.Id,
                            Name = file.FileName,
                            ImgPath = "/images/" + fileName
                        });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Details", "Patient", new { id = PatientId });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, string Title, string Description, DateTime Time, int TypeId, List<int> DoctorIds, List<IFormFile> CheckupImages)
        {
            var checkup = _dbContext.Checkups
                .Include(c => c.CheckupDoctors)
                .Include(c => c.CheckupImages)
                .FirstOrDefault(c => c.Id == Id);

            if (checkup == null)
                return NotFound();

            checkup.Title = Title;
            checkup.Description = Description;
            checkup.Time = Time;
            checkup.TypeId = TypeId;

            _dbContext.CheckupDoctors.RemoveRange(checkup.CheckupDoctors);
            foreach (var doctorId in DoctorIds)
            {
                _dbContext.CheckupDoctors.Add(new CheckupDoctor { CheckupId = checkup.Id, DoctorId = doctorId });
            }

            if (CheckupImages != null)
            {
                foreach (var file in CheckupImages)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine("wwwroot/images", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        _dbContext.CheckupImages.Add(new CheckupImage
                        {
                            CheckupId = checkup.Id,
                            Name = file.FileName,
                            ImgPath = "/images/" + fileName
                        });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            ViewBag.CheckupTypes = _dbContext.CheckupTypes.ToList();
            ViewBag.Doctors = _dbContext.Doctors.ToList();

            return RedirectToAction("Details", "Patient", new { id = checkup.PatientId });
        }

        [HttpGet]
        public IActionResult DownloadImage(int id)
        {
            var image = _dbContext.CheckupImages.FirstOrDefault(img => img.Id == id);
            if (image == null || string.IsNullOrEmpty(image.ImgPath))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImgPath.TrimStart('/'));
            var fileName = image.Name ?? Path.GetFileName(filePath);
            var mimeType = "application/octet-stream";
            return PhysicalFile(filePath, mimeType, fileName);
        }
    }
}
