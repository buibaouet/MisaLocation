using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MISA.LocationV2.Web.Controllers
{
    [Authorize]
    public class MonitorController : Controller
    {
        public MonitorController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}