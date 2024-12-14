using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IR_Admin.Db;
using IR_Admin.Helpers;
using IR_Admin.Reporting;
using Microsoft.AspNet.Identity;

namespace IR_Admin.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private IR_DBEntities db = new IR_DBEntities();

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            bool isAdmin = User.IsInRole("Admin");
            if (isAdmin)
                return View(db.CountryBasicProjectDatas.ToList());

            List<CountryBasicProjectData> projectsList = new List<CountryBasicProjectData>();
            var assignProjects = db.AssignUsers.Where(u => u.UserId.ToString().Equals(userId)).ToList();
            foreach (AssignUser assignProject in assignProjects)
            {
                projectsList.Add(db.CountryBasicProjectDatas.FirstOrDefault(p => p.Id.Equals(assignProject.ProjectId)));
            }
            return View(projectsList);
        }
        public ActionResult ListProjectWithMonths(Guid id)
        {
            var projectData = db.CountryBasicProjectDatas.FirstOrDefault(p => p.Id.Equals(id));

            if (projectData != null)
            {
                var u = ProjectHelper.FindProvinceNameByUserId(User.Identity.GetUserId());
                var projectDataByMonth = db.BeneficiariesRecords
                    .Where(c => c.ProjectId == projectData.Id && c.ProvinceId== u)
                    .GroupBy(b => new { b.Month, b.ProjectId })
                    .OrderBy(g => g.Key.Month)
                    .ToList();

                List<GroupingKey> groupingKey = new List<GroupingKey>();
                foreach (var temp in projectDataByMonth)
                {
                    groupingKey.Add(new GroupingKey() { Month = temp.Key.Month, ProjectId = temp.Key.ProjectId.ToString() });
                }
                ViewBag.ProjectDataByMonth = groupingKey;
                ViewBag.ProjectId = projectData.Id;
                return View();
            }
            return View();
        }
        [HttpGet]
        public ActionResult Details(string id)
        {
            var projectData = db.CountryBasicProjectDatas.FirstOrDefault(p => p.Id.Equals(new Guid(id)));
            var categoryByProjectId = db.Categories.Where(c => c.ProjectId == projectData.Id).ToList();
            var components = db.FlexibleInterventions.FirstOrDefault(b => b.ProjectId==projectData.Id);

            UserCategory dataTable = new UserCategory();
            List<UserCategory> dataTableData = dataTable.CategoriesWithFixedFields(categoryByProjectId);
            List<UserComponents> componentsList = new UserComponents().GetUserComponents(components.Id, components.Fields.Split(','));
            ViewBag.Components = componentsList;
            ViewBag.Model = dataTableData;
            ViewBag.ProjectData = projectData;
            return View();
        }
        
        // GET: Projects/Create
        public ActionResult Create(Guid id)
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        public ActionResult Create(string data,string projectId, string month, HttpPostedFileBase file)
        {
            try
            {
                var normalData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(data);

                List<ProjectDataFromUser> customDataList = normalData
                    .SelectMany(listOfDict => listOfDict.Select(dict => new ProjectDataFromUser()
                    {
                        Category = dict["Category"],
                        Disability = dict["Disability"].Equals("W")?Disability.WithDisability:Disability.WithoutDisability ,
                        Gender = dict["Gender"].Equals("Male")?Gender.Male:Gender.Female,
                        AgeGroup = dict["Age Group"],
                        ProjectId = new Guid(projectId),
                        Month = DateTime.Parse(month),
                        Components = string.Join(",", dict.Where(kvp => kvp.Key != "Category" && kvp.Key != "Disability" && kvp.Key != "Gender" && kvp.Key != "Age Group"))
                    }))
                    .ToList();
                var comp = db.FlexibleInterventions.FirstOrDefault(f => f.ProjectId.ToString().Equals(projectId));
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string directoryPath = Server.MapPath("~/Content/UploadedPdfs");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, fileName);
                file.SaveAs(filePath);
                foreach (ProjectDataFromUser fromUser in customDataList)
                {
                    FlexibleIntervention components = new FlexibleIntervention()
                    {
                        Id = Guid.NewGuid(), ParentId = comp.Id, ProjectId = new Guid(projectId),
                        Fields = comp.Fields, Values = fromUser.Components,
                        FilePath = fileName
                    };
                    db.FlexibleInterventions.Add(components);
                    BeneficiariesRecord beneficiariesRecord = new BeneficiariesRecord()
                    {
                        Disability = fromUser.Disability.ToString(),
                        Gender = fromUser.Gender.ToString(),
                        CategoryId = db.Categories.FirstOrDefault(c => c.Name.Equals(fromUser.Category)).Id,
                        Id = Guid.NewGuid(),
                        CreatedTime = DateTime.Now,
                        Month = string.Format(month),
                        ProjectId = new Guid(projectId),
                        AgeGroup = fromUser.AgeGroup,
                        FlexibleInterventionsId = components.Id,
                        ProvinceId = ProjectHelper.FindProvinceNameByUserId(User.Identity.GetUserId().ToString())
                    };
                    db.BeneficiariesRecords.Add(beneficiariesRecord);
                    db.SaveChanges();
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(string id, string month, bool message = false)
        {
            var projectData = db.CountryBasicProjectDatas.FirstOrDefault(p => p.Id.Equals(new Guid(id)));

            var query = from beneficiary in db.BeneficiariesRecords
                            join flexible in db.FlexibleInterventions
                                on beneficiary.ProjectId equals projectData.Id
                            where beneficiary.FlexibleInterventionsId.Equals(flexible.Id) && beneficiary.Month.ToString().Equals(month)
                            select new DetailedReportModel()
                            {
                                BeneficiariesRecordTable = beneficiary,
                                FlexibleFieldsTable = flexible
                            };
                var queryListedData = query.ToList();

                var groupedData = queryListedData.Select(group => new GroupedDataViewModelForUserEditFeature()
                {
                    FlexibleFieldId = group.FlexibleFieldsTable.Id,
                    CategoryName = ProjectHelper.FindCategoryNameById(group.BeneficiariesRecordTable.CategoryId),
                    Disability = group.BeneficiariesRecordTable.Disability,
                    Gender = group.BeneficiariesRecordTable.Gender,
                    AgeGroup = group.BeneficiariesRecordTable.AgeGroup,
                    Value = ProjectHelper.ConvertStringToList(group.FlexibleFieldsTable.Values)

                }).OrderBy(a => a.CategoryName).ThenBy(a => a.Disability).ThenBy(a => a.Gender).ThenBy(a => a.AgeGroup);

            var categoryByProjectId = db.Categories.Where(c => c.ProjectId == projectData.Id).ToList();
            var components = db.FlexibleInterventions.FirstOrDefault(b => b.ProjectId == projectData.Id);

            UserCategory dataTable = new UserCategory();
            List<UserCategory> dataTableData = dataTable.CategoriesWithFixedFields(categoryByProjectId);
            List<UserComponents> componentsList = new UserComponents().GetUserComponents(components.Id, components.Fields.Split(','));
            ViewBag.Components = componentsList;
            ViewBag.ComponentsDataForEdit = groupedData;
            ViewBag.Month = month;
            ViewBag.ProjectData = projectData;
            if (message)
                ViewBag.Message = "Success";
            return View();
        }
        
        [HttpPost]
        public ActionResult Edit(string projectId, string month, string data = "")
        {
            try
            {
                var normalData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(data);

                List<ProjectDataFromUser> customDataList = normalData
                    .SelectMany(listOfDict => listOfDict.Select(dict => new ProjectDataFromUser()
                    {
                        Category = dict["Category"],
                        Disability = dict["Disability"].Equals("W")
                            ? Disability.WithDisability
                            : Disability.WithoutDisability,
                        Gender = dict["Gender"].Equals("Male") ? Gender.Male : Gender.Female,
                        AgeGroup = dict["Age Group"],
                        ProjectId = new Guid(projectId),
                        Month = DateTime.Parse(month), FlexibleFieldId = new Guid(dict["FlexibleFieldId"]),
                        Components = string.Join(",",
                            dict.Where(kvp =>
                                kvp.Key != "Category" && kvp.Key != "Disability" && kvp.Key != "Gender" &&
                                kvp.Key != "Age Group" && kvp.Key != "FlexibleFieldId"))
                    }))
                    .ToList();
                foreach (ProjectDataFromUser fromUser in customDataList)
                {
                    var flexibleFieldRecord = db.FlexibleInterventions.Find(fromUser.FlexibleFieldId);
                    flexibleFieldRecord.Values = fromUser.Components;
                    db.Entry(flexibleFieldRecord).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                return Json(new { response = true }); 
            }
            catch
            {
                return View();
            }
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(string id,string month)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<BeneficiariesRecord> beneficiariesRecordList = db.BeneficiariesRecords.Where(b => b.ProjectId ==new Guid(id) && b.Month.Equals(month)).ToList();
            if (beneficiariesRecordList == null)
                return HttpNotFound();
            foreach (BeneficiariesRecord beneficiariesRecord in beneficiariesRecordList)
            {
                db.FlexibleInterventions.Remove(db.FlexibleInterventions.Find(beneficiariesRecord.FlexibleInterventionsId));
                db.SaveChanges();
            }
            db.BeneficiariesRecords.RemoveRange(beneficiariesRecordList);
            db.SaveChanges();
            return RedirectToAction("ListProjectWithMonths","Projects",new {id=id});
        }
        
        public ActionResult Download(string id, string month, bool message = false)
        {
            var projectData = db.CountryBasicProjectDatas.FirstOrDefault(p => p.Id.Equals(new Guid(id)));

            var query = from beneficiary in db.BeneficiariesRecords
                join flexible in db.FlexibleInterventions
                    on beneficiary.ProjectId equals projectData.Id
                where beneficiary.FlexibleInterventionsId.Equals(flexible.Id) && beneficiary.Month.Equals(month)
                select new DetailedReportModel()
                {
                    BeneficiariesRecordTable = beneficiary,
                    FlexibleFieldsTable = flexible
                };
            var queryListedData = query.ToList();

            var categoryByProjectId = db.Categories.Where(c => c.ProjectId == projectData.Id).ToList();
            var components = queryListedData.FirstOrDefault(b => b.FlexibleFieldsTable.ProjectId == projectData.Id);
            UserCategory dataTable = new UserCategory();
            List<UserCategory> dataTableData = dataTable.CategoriesWithFixedFields(categoryByProjectId);
            List<UserComponents> componentsList = new UserComponents().GetUserComponents(components.FlexibleFieldsTable.Id, components.FlexibleFieldsTable.Fields.Split(','));
            string filePath = Server.MapPath("~/Content/UploadedPdfs/"+components.FlexibleFieldsTable.FilePath);
            
            if (System.IO.File.Exists(filePath))
            {
                return File(filePath, "application/pdf", components.FlexibleFieldsTable.FilePath);
            }
            else
            {
                return HttpNotFound();
            }
            
        }
    }
}
