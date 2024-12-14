using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IR_Admin.Db;
using IR_Admin.Helpers;
using IR_Admin.Reporting;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace IR_Admin.Controllers
{

    [Authorize(Roles = "Admin")]
    public class ReportViewerController : Controller
    {
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            var startDate = DateTime.Parse(collection["startDate"].ToString());
            var endDate = DateTime.Parse(collection["endDate"].ToString());
            IR_DBEntities entities = new IR_DBEntities();
            
            List<ProjectsData> categoriesList = null;
            List<List<ProjectsData>> categoriesListData = new List<List<ProjectsData>>();
            using (var _context = entities)
            {
                var projects = _context.CountryBasicProjectDatas.Where(p => p.CreateDate.Value.Year >= startDate.Year && p.CreateDate.Value.Year<=endDate.Year).ToList();

                DetailedReportDataSet dataSet = new DetailedReportDataSet();
                DetailedReportDataSet.FinalReportDataTableDataTable dataTable = dataSet.FinalReportDataTable;
                DetailedReportDataSet.FinalReportDataTableRow newRow = dataTable.NewFinalReportDataTableRow();


                foreach (CountryBasicProjectData projectData in projects)
                {
                    var query = from beneficiary in _context.BeneficiariesRecords
                        join flexible in _context.FlexibleInterventions
                            on beneficiary.ProjectId equals projectData.Id
                        where beneficiary.FlexibleInterventionsId.Equals(flexible.Id)
                        select new DetailedReportModel()
                        {
                            BeneficiariesRecordTable = beneficiary,
                            FlexibleFieldsTable = flexible
                        };
                    List<Category> categories = new List<Category>();

                    var queryListedData = query.ToList();
                    int counter = 0;
                    foreach (var q in queryListedData)
                    {
                        queryListedData[counter++].FlexibleFieldsTable.Values = ProjectHelper
                            .CalculateSumFromInput(q.FlexibleFieldsTable.Values).ToString();
                    }

                    var groupedData = queryListedData.GroupBy(entry => new
                        {
                            entry.BeneficiariesRecordTable.ProjectId,
                            entry.BeneficiariesRecordTable.CategoryId, entry.BeneficiariesRecordTable.AgeGroup,
                            entry.BeneficiariesRecordTable.Gender, entry.BeneficiariesRecordTable.Disability
                        })
                        .Select(group => new
                        {
                            group.Key.ProjectId,
                            group.Key.CategoryId,
                            group.Key.Gender,
                            group.Key.Disability,
                            group.Key.AgeGroup,
                            TotalValue = group.Sum(entry => int.Parse(entry.FlexibleFieldsTable.Values))
                        }).OrderBy(a => a.CategoryId).ThenBy(a => a.Gender).ThenBy(a => a.Disability)
                        .ThenBy(a => a.AgeGroup);

                    categoriesList = groupedData
                        .GroupBy(p => p.ProjectId)
                        .Select(p => new ProjectsData
                        {
                            ProjectId = p.Key,
                            CategoryDataList = p.GroupBy(g => g.CategoryId)
                                .Select(c => new CategoryData
                                {
                                    CategoryId = c.Key,
                                    Name = ProjectHelper.FindCategoryNameById(c.Key),
                                    GenderGroups = c.GroupBy(gg => gg.Gender)
                                        .Select(gg => new GenderGroupData
                                        {
                                            Gender = gg.Key,
                                            DisabilityGroups = gg.GroupBy(dg => dg.Disability)
                                                .Select(dg => new DisabilityGroupData
                                                {
                                                    Disability = dg.Key,
                                                    AgeGroups = dg.Select(ag => new AgeGroupData
                                                    {
                                                        AgeGroup = ag.AgeGroup,
                                                        TotalValue = ag.TotalValue
                                                    }).ToList()
                                                }).ToList()
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                    categoriesListData.Add(categoriesList);
                }

                List<ProjectsData> listTobeExported = new List<ProjectsData>();
                foreach (List<ProjectsData> projectsDatas in categoriesListData)
                {
                    foreach (ProjectsData data in projectsDatas)
                    {
                        listTobeExported.Add(data);
                    }
                }
                ExportToExcel(listTobeExported);

            }

            return RedirectToAction("DownloadExcelFile");
        }
        public ActionResult DownloadExcelFile()
        {
            var filePath = Server.MapPath("~/FinalReportProjectsData.xlsx");

            if (System.IO.File.Exists(filePath))
            {
               
                string contentType = MimeMapping.GetMimeMapping(filePath);
                
                return File(filePath, contentType, "FinalReportProjectsData.xlsx");
            }
            else
            {
                return HttpNotFound();
            }
        }

        public void ExportToExcel(List<ProjectsData> projects)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                foreach (ProjectsData project in projects)
                {
                    var worksheet = package.Workbook.Worksheets.Add(ProjectHelper.FindProjectById((Guid) project.ProjectId).Name);
                
                ExcelRange rangeToMergeForHeader = worksheet.Cells[1, 1, 1, 13];
                rangeToMergeForHeader.Merge = true;
                worksheet.Cells[1, 1].Value = "Project Data";
                rangeToMergeForHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeToMergeForHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.CornflowerBlue);

                rangeToMergeForHeader.Style.Font.Size = 18;
                rangeToMergeForHeader.Style.Font.Bold = true;
                rangeToMergeForHeader.Style.WrapText = true;
                rangeToMergeForHeader.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rangeToMergeForHeader.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rangeToMergeForHeader.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rangeToMergeForHeader.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                rangeToMergeForHeader.Style.Border.Top.Color.SetColor(Color.White);
                rangeToMergeForHeader.Style.Border.Left.Color.SetColor(Color.White);
                rangeToMergeForHeader.Style.Border.Right.Color.SetColor(Color.White);
                rangeToMergeForHeader.Style.Border.Bottom.Color.SetColor(Color.White);
                rangeToMergeForHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rangeToMergeForHeader.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ExcelRange rangeToMerge = worksheet.Cells[1, 15, 1, 23];
                rangeToMerge.Merge = false;
                

                int row = 1, range = 0, pRow = 2, pCol = 1, lastColNumber = 0, lastRowNumber = 0;

                worksheet.Cells[pRow, pCol].Value = "Username"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Country"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Project Location"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Type Of Project"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Seasonal Project Type"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "IRW Project Pin Code"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Project Name"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Donar of the project"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Donar Partner"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "Primary Sector"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = "SDG Goal Indicator"; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = ""; pCol += 1;
                worksheet.Cells[pRow, pCol].Value = ""; pCol += 1;
                for (int i = 1; i <= 13; i++)
                {
                    worksheet.Cells[pRow, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[pRow, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[pRow, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[pRow, i].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    worksheet.Cells[pRow, i].Style.Font.Size = 12;
                    worksheet.Cells[pRow, i].Style.Font.Bold = true;
                    worksheet.Cells[pRow, i].Style.WrapText = true;
                    worksheet.Cells[pRow, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[pRow, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[pRow, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[pRow, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[pRow, i].Style.Border.Top.Color.SetColor(Color.Black);
                    worksheet.Cells[pRow, i].Style.Border.Left.Color.SetColor(Color.Black);
                    worksheet.Cells[pRow, i].Style.Border.Right.Color.SetColor(Color.Black);
                    worksheet.Cells[pRow, i].Style.Border.Bottom.Color.SetColor(Color.Black);
                }
                
                lastColNumber = pCol;
                pRow += 1;
                pCol = 1;
                
                    var projectBasicData =  new IR_DBEntities().CountryBasicProjectDatas.FirstOrDefault(p => p.Id == project.ProjectId);
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.UserName; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.Country; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.ProjectLocation; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.TypeOfProject; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.SeasonalProjectType; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.IRWProjectPinCode; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.Name; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.IsOneDonar; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.Donar_Partner; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.PrimarySector; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = projectBasicData.SDGGoalIndicator; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = ""; pCol += 1;
                    worksheet.Cells[pRow, pCol].Value = ""; pCol += 1;

                    foreach (var category in project.CategoryDataList)
                    {
                        foreach (var gender in category.GenderGroups)
                        {
                            foreach (var disability in gender.DisabilityGroups)
                            {
                                worksheet.Cells[1, lastColNumber+1].Value = category.Name + " - " + gender.Gender + " - " + disability.Disability;
                                if (IsInSeries(lastColNumber))
                                {
                                    rangeToMerge = worksheet.Cells[1, lastColNumber+1, 1, lastColNumber + 8];
                                    rangeToMerge.Merge = true;
                                    rangeToMerge.Style.Font.Size = 18;
                                    rangeToMerge.Style.Font.Bold = true;
                                    rangeToMerge.Style.WrapText = true;
                                    rangeToMerge.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    rangeToMerge.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    rangeToMerge.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    rangeToMerge.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    rangeToMerge.Style.Border.Top.Color.SetColor(Color.White);
                                    rangeToMerge.Style.Border.Left.Color.SetColor(Color.White);
                                    rangeToMerge.Style.Border.Right.Color.SetColor(Color.White);
                                    rangeToMerge.Style.Border.Bottom.Color.SetColor(Color.White);
                                    rangeToMerge.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    rangeToMerge.Style.Fill.BackgroundColor.SetColor(Color.CornflowerBlue);
                                    rangeToMerge.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rangeToMerge.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                }

                                foreach (var ageGroup in disability.AgeGroups)
                                {
                                    worksheet.Cells[2, lastColNumber+1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[2, lastColNumber+1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    worksheet.Cells[2, lastColNumber+1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[2, lastColNumber+1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                                    worksheet.Cells[2, lastColNumber+1].Style.Font.Size = 12;
                                    worksheet.Cells[2, lastColNumber+1].Style.Font.Bold = true;
                                    worksheet.Cells[2, lastColNumber+1].Style.WrapText = true;
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Top.Color.SetColor(Color.Black);
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Left.Color.SetColor(Color.Black);
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Right.Color.SetColor(Color.Black);
                                    worksheet.Cells[2, lastColNumber+1].Style.Border.Bottom.Color.SetColor(Color.Black);

                                    worksheet.Cells[3, lastColNumber+1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[3, lastColNumber+1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    worksheet.Cells[2, lastColNumber+1].Value = "Age Group " + ageGroup.AgeGroup;
                                    worksheet.Cells[3, lastColNumber+1].Value = ageGroup.TotalValue;
                                    lastColNumber++;
                                }
                                
                            }
                        }


                    }
                }
                 
                var filePath = Server.MapPath("~/FinalReportProjectsData.xlsx");
                package.SaveAs(new FileInfo(filePath));
            }
        }

        public bool IsInSeries(int number)
        {
            if (number < 14) return false;
            if ((number - 14) % 8 == 0) return true;
            return false;
        }
    }
}
