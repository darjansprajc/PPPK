using HospitalApp.Models;
using HospitalApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HospitalApp.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger, DbHospitalContext dbContext)
             : base(logger, dbContext)
        {
        }

        public IActionResult Index()
        {
            var doctors = _dbContext.Doctors.ToList();
            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        public IActionResult SelectDoctor(int doctorId)
        {
            var doctor = _dbContext.Doctors.Find(doctorId);
            if (doctor != null)
            {
                HttpContext.Session.SetObject("SelectedDoctor", doctor);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
