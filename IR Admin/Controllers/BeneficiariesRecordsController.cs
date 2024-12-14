using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IR_Admin.Db;
using IR_Admin.Helpers;
using IR_Admin.Reporting;

namespace IR_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BeneficiariesRecordsController : Controller
    {
        private IR_DBEntities db = new IR_DBEntities();

        // GET: BeneficiariesRecords
        public ActionResult Index()
        {
            return View(db.CountryBasicProjectDatas.ToList());
        }

        // GET: BeneficiariesRecords/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BeneficiariesRecord beneficiariesRecord = db.BeneficiariesRecords.Find(id);
            if (beneficiariesRecord == null)
            {
                return HttpNotFound();
            }
            return View(beneficiariesRecord);
        }

        // GET: BeneficiariesRecords/Create
        public ActionResult Create()
        {
            MultiModels model = new MultiModels();
            model.Users = db.AspNetUsers.ToList();
            model.Countries = new CountriesModel(){Countries = CountriesModel.GetCountries()};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MultiModels multiModels, FormCollection formCollection)
        {
            string[] userIds = formCollection["AssignUser"].Split(','); ;
           

            string[] categories = multiModels.CategoryModel.Name.Split(',');

            multiModels.CountryBasicProjectDataModel.Id = Guid.NewGuid();
            multiModels.CountryBasicProjectDataModel.Roll = "User";
            multiModels.CountryBasicProjectDataModel.CreateDate = DateTime.Now;
            multiModels.CountryBasicProjectDataModel.Country = multiModels.Countries.SelectedCountry;
            multiModels.CountryBasicProjectDataModel.IsOneDonar = multiModels.CountryBasicProjectDataModel.Donar_Partner.Split(',').Length;
            multiModels.CountryBasicProjectDataModel.ProjectLocation = formCollection["AssignUser"].ToString();

            multiModels.FlexibleInterventionModel.Id = Guid.NewGuid();
            multiModels.FlexibleInterventionModel.ParentId = Guid.Empty;
            multiModels.FlexibleInterventionModel.ProjectId = multiModels.CountryBasicProjectDataModel.Id;

            foreach (string category in categories)
            {
                multiModels.CategoryModel = new Category() {Id = Guid.NewGuid(), Name = category, ProjectId = multiModels.CountryBasicProjectDataModel.Id};
                db.Categories.Add(multiModels.CategoryModel);
                db.SaveChanges();
            }

            db.FlexibleInterventions.Add(multiModels.FlexibleInterventionModel);
            db.CountryBasicProjectDatas.Add(multiModels.CountryBasicProjectDataModel);
            foreach (string userId in userIds)
            {
                db.AssignUsers.Add(new AssignUser()
                {
                    Id = Guid.NewGuid(),
                    ProjectId = multiModels.CountryBasicProjectDataModel.Id,
                    UserId = new Guid(userId)
                });
            }
            
            db.SaveChanges();
            ViewBag.Message = "Success";
            MultiModels m = new MultiModels();
            m.Users = db.AspNetUsers.ToList();
            m.Countries = new CountriesModel() { Countries = CountriesModel.GetCountries() };
            return View(m);

        }

        // GET: BeneficiariesRecords/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MultiModels m = new MultiModels();
            m.CountryBasicProjectDataModel = db.CountryBasicProjectDatas.Find(id);
            m.FlexibleInterventionModel =  db.FlexibleInterventions.FirstOrDefault(f => f.ProjectId==id);
            m.CategoryModel= new Category(){Name = string.Join(",",db.Categories.Where(c => c.ProjectId == id).ToList().ConvertAll(ca => ca.Name))};
        
            m.Users = db.AspNetUsers.ToList();
            var countries = new CountriesModel() { Countries = CountriesModel.GetCountries() };
            countries.SelectedCountry = db.CountryBasicProjectDatas.Find(id).Country;
            m.Countries = countries;
           
            return View(m);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MultiModels multiModels, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {

                string[] userIds = formCollection["AssignUser"].Split(',');
                string projectId = formCollection["ProjectId"].ToString();
                multiModels.CountryBasicProjectDataModel.ImplementationPeriodFrom = DateTime.Parse(formCollection["ImplementationPeriodFrom"].ToString());
                multiModels.CountryBasicProjectDataModel.ImplementationPeriodTo = DateTime.Parse(formCollection["ImplementationPeriodTo"].ToString());
                
                CountryBasicProjectData existingProject = db.CountryBasicProjectDatas.Find(new Guid(projectId));
                existingProject.Name = multiModels.CountryBasicProjectDataModel.Name;
                existingProject.Country = multiModels.Countries.SelectedCountry;
                existingProject.Donar_Partner = multiModels.CountryBasicProjectDataModel.Donar_Partner;
                existingProject.ImplementationPeriodFrom = multiModels.CountryBasicProjectDataModel.ImplementationPeriodFrom;
                existingProject.ImplementationPeriodTo = multiModels.CountryBasicProjectDataModel.ImplementationPeriodTo;
                existingProject.ProjectLocation = formCollection["AssignUser"].ToString();
                existingProject.DirectRightHolders = multiModels.CountryBasicProjectDataModel.DirectRightHolders;
                existingProject.Duration = multiModels.CountryBasicProjectDataModel.Duration;
                existingProject.IRWProjectPinCode = multiModels.CountryBasicProjectDataModel.IRWProjectPinCode;
                existingProject.PrimarySector = multiModels.CountryBasicProjectDataModel.PrimarySector;
                existingProject.SDGGoalIndicator = multiModels.CountryBasicProjectDataModel.SDGGoalIndicator;
                existingProject.ScopeOfProject = multiModels.CountryBasicProjectDataModel.ScopeOfProject;
                existingProject.SeasonalProjectType = multiModels.CountryBasicProjectDataModel.SeasonalProjectType;
                existingProject.Region = multiModels.CountryBasicProjectDataModel.Region;
                existingProject.TypeOfProject = multiModels.CountryBasicProjectDataModel.TypeOfProject;
                existingProject.IsOneDonar = multiModels.CountryBasicProjectDataModel.Donar_Partner.Split(',').Length;
                db.Entry(existingProject).State = EntityState.Modified;
                db.SaveChanges();
               
                FlexibleIntervention existingFlexibleFields = db.FlexibleInterventions.FirstOrDefault(f => f.ProjectId == new Guid(projectId));
                existingFlexibleFields.Fields = multiModels.FlexibleInterventionModel.Fields;
                db.Entry(existingFlexibleFields).State = EntityState.Modified;
                db.SaveChanges();
                db.AssignUsers.RemoveRange(db.AssignUsers.Where(u=>u.ProjectId == new Guid(projectId)).ToList());
                db.SaveChanges();
                foreach (string userId in userIds)
                {
                    db.AssignUsers.Add(new AssignUser()
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = new Guid(projectId),
                        UserId = new Guid(userId)
                    });
                    db.SaveChanges();
                }

                ViewBag.Message = "Success";
                MultiModels m = new MultiModels();
                m.CountryBasicProjectDataModel = db.CountryBasicProjectDatas.Find(new Guid(projectId));
                m.FlexibleInterventionModel = db.FlexibleInterventions.FirstOrDefault(f => f.ProjectId == new Guid(projectId));
                m.CategoryModel = new Category() { Name = string.Join(",", db.Categories.Where(c => c.ProjectId == new Guid(projectId)).ToList().ConvertAll(ca => ca.Name)) };

                m.Users = db.AspNetUsers.ToList();
                var countries = new CountriesModel() { Countries = CountriesModel.GetCountries() };
                countries.SelectedCountry = db.CountryBasicProjectDatas.Find(new Guid(projectId)).Country;
                m.Countries = countries;
                return View(m);
            }
            return View();
        }
        
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CountryBasicProjectData project = db.CountryBasicProjectDatas.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            List<BeneficiariesRecord> beneficiariesRecordList = db.BeneficiariesRecords.Where(b => b.ProjectId == id).ToList();
            if (beneficiariesRecordList == null)
                return HttpNotFound();
            List<Category> categories = db.Categories.Where(c => c.ProjectId == id).ToList();
            List<AssignUser> user = db.AssignUsers.Where(u => u.ProjectId == id).ToList();
            foreach (BeneficiariesRecord beneficiariesRecord in beneficiariesRecordList)
            {
                db.FlexibleInterventions.Remove(db.FlexibleInterventions.FirstOrDefault(p=>p.ProjectId == id));
                db.SaveChanges();
            }
            db.AssignUsers.RemoveRange(user);
            db.Categories.RemoveRange(categories);
            db.BeneficiariesRecords.RemoveRange(beneficiariesRecordList);
            db.CountryBasicProjectDatas.Remove(project);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            BeneficiariesRecord beneficiariesRecord = db.BeneficiariesRecords.Find(id);
            db.BeneficiariesRecords.Remove(beneficiariesRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult CreatePredictedData(string id)
        {
            var projectData = db.CountryBasicProjectDatas.FirstOrDefault(p => p.Id.Equals(new Guid(id)));
            var categoryByProjectId = db.Categories.Where(c => c.ProjectId == projectData.Id).ToList();
            var components = db.FlexibleInterventions.FirstOrDefault(b => b.ProjectId == projectData.Id);

            UserCategory dataTable = new UserCategory();
            List<UserCategory> dataTableData = dataTable.CategoriesWithFixedFields(categoryByProjectId);
            List<UserComponents> componentsList = new UserComponents().GetUserComponents(components.Id, components.Fields.Split(','));
            ViewBag.Components = componentsList;
            ViewBag.Model = dataTableData;
            ViewBag.ProjectData = projectData;
            return View();
        }
        [HttpPost]
        public ActionResult CreatePredictedData(string projectId, string data = "")
        {
            var normalData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(data);
            db.PredictedFlexibleInterventionsDatas.Add(new PredictedFlexibleInterventionsData()
            {
                Id = Guid.NewGuid(), Fields = projectId, Values = data, CreatedTime = DateTime.Now,
                ProjectId = new Guid(projectId)
            });
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditProjectPlannedData(string id, bool message = false)
        {
            var projectData = db.CountryBasicProjectDatas.FirstOrDefault(p => p.Id.Equals(new Guid(id)));
            var plannedProjectData = db.PredictedFlexibleInterventionsDatas.FirstOrDefault(p => p.ProjectId==new Guid(id));

            var normalData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(plannedProjectData.Values);

            List<ProjectDataFromUser> customDataList = normalData
                .SelectMany(listOfDict => listOfDict.Select(dict => new ProjectDataFromUser()
                {
                    Category = dict["Category"],
                    Disability = dict["Disability"].Equals("W") ? Disability.WithDisability : Disability.WithoutDisability,
                    Gender = dict["Gender"].Equals("Male") ? Gender.Male : Gender.Female,
                    AgeGroup = dict["Age Group"],
                    ProjectId = new Guid(id),
                    FlexibleFieldId = plannedProjectData.Id,
                    Components = string.Join(",", dict.Where(kvp => kvp.Key != "Category" && kvp.Key != "Disability" && kvp.Key != "Gender" && kvp.Key != "Age Group"))
                }))
                .ToList();

            IOrderedEnumerable<GroupedDataViewModelForUserEditFeature> groupedData = customDataList.Select(group => new GroupedDataViewModelForUserEditFeature()
            {
                FlexibleFieldId = group.FlexibleFieldId,
                CategoryName = group.Category,
                Disability = group.Disability.ToString(),
                Gender = group.Gender.ToString(),
                AgeGroup = group.AgeGroup,
                Value = ProjectHelper.ConvertStringToList(group.Components)

            }).OrderBy(a => a.CategoryName).ThenBy(a => a.Disability).ThenBy(a => a.Gender).ThenBy(a => a.AgeGroup);
            ViewBag.PlannedProjectDataId = plannedProjectData.Id;
            ViewBag.ComponentsDataForEdit = groupedData;
            var c = groupedData.Select(item => item.Value).Distinct().First();
            List<string> components = new List<string>();
            foreach (KeyValuePair<string, int> keyValuePair in c)
                components.Add(keyValuePair.Key);
            ViewBag.Components = components;
            ViewBag.ProjectData = projectData;

            return View();
        }
        [HttpPost]
        public ActionResult EditProjectPlannedData(string id, string data = "")
        {
            try
            {
                var plannedProjectData = db.PredictedFlexibleInterventionsDatas.Find(new Guid(id));
                plannedProjectData.Values = data;
                db.Entry(plannedProjectData).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
