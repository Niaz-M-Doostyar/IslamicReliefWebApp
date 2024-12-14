using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IR_Admin.Db;

namespace IR_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DetailedReportsController : Controller
    {
        public ActionResult Index()
        {
            IR_DBEntities entities = new IR_DBEntities();
            ViewBag.Years = entities.CountryBasicProjectDatas.ToList().GroupBy(r => r.CreateDate.Value.Year).Select(d => d.Key).ToList();
            ViewBag.Projects = entities.CountryBasicProjectDatas.ToList();
            return View();
        }
    }
}