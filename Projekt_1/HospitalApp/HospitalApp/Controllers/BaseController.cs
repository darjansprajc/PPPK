using HospitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using HospitalApp.Utilities;
using Microsoft.Extensions.Logging;

namespace HospitalApp.Controllers
{
    public class BaseController<T> : Controller
    {
        protected readonly ILogger<T> _logger;
        protected readonly DbHospitalContext _dbContext;

        public BaseController(ILogger<T> logger, DbHospitalContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var doctor = HttpContext.Session.GetObject<Doctor>("SelectedDoctor");
            ViewBag.Doctor = doctor;
            base.OnActionExecuting(context);
        }
    }
}
