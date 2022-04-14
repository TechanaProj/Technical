using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IFFCO.DAILYWG.Web.Areas.M2.Controllers
{
    [Area("M2")]
    public class HomeController : BaseController<ViewModelDashBoardChart>
    {
        public IActionResult Index()
        {
            return View();
        }                
    }
}