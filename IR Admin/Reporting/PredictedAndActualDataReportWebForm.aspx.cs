using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IR_Admin.Db;
using IR_Admin.Helpers;
using Microsoft.Reporting.WebForms;

namespace IR_Admin.Reporting
{
    public partial class PredictedAndActualDataReportWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IR_DBEntities entities = new IR_DBEntities();
                Guid projectId = new Guid(Request.QueryString["projectId"]);
                using (var _context = entities)
                {
                    var project = _context.CountryBasicProjectDatas.Find(projectId);

                    DetailedReportDataSet dataSet = new DetailedReportDataSet();
                    DetailedReportDataSet.DetailedReportDataTableDataTable dataTable = dataSet.DetailedReportDataTable;

                    DetailedReportDataSet.FinalReportDataTableDataTable plannedDataDataTable =
                        dataSet.FinalReportDataTable;

                    ReportParameter[] parameters = new ReportParameter[] { };
                    int? totalRightHoldersForAllTheProject = 0;

                    var query = from beneficiary in _context.BeneficiariesRecords
                        join flexible in _context.FlexibleInterventions
                            on beneficiary.ProjectId equals project.Id
                        where beneficiary.FlexibleInterventionsId.Equals(flexible.Id) && flexible.ParentId != Guid.Empty
                        select new DetailedReportModel()
                        {
                            BeneficiariesRecordTable = beneficiary,
                            FlexibleFieldsTable = flexible
                        };
                    var queryListedData = query.ToList();
                    int counter = 0;


                    var groupedData = queryListedData
                        .Select(group => new
                        {
                            Category = ProjectHelper.FindCategoryNameById(group.BeneficiariesRecordTable.CategoryId),
                            group.FlexibleFieldsTable.Values,
                            group.BeneficiariesRecordTable.AgeGroup,
                            group.BeneficiariesRecordTable.Disability,
                            group.BeneficiariesRecordTable.Gender,

                        }).OrderBy(a => a.AgeGroup).ThenBy(a => a.Gender).ThenBy(a => a.Disability);



                    List<FieledSectors> dataList = new List<FieledSectors>();
                    Dictionary<string, List<FieledSectors>>  categoryData = new Dictionary<string, List<FieledSectors>>();

                    foreach (var group in groupedData)
                    {
                        string parentCategory = group.Category;
                        string values = group.Values;
                        string ageGroup = group.AgeGroup;
                        string gender = group.Gender;
                        string disability = group.Disability;
                        string[] valuePairs = values.Split(new string[] {"],["}, StringSplitOptions.None);

                        foreach (string pair in valuePairs)
                        {
                            string[] parts = pair.Replace("[", "").Replace("]", "").Split(',');

                            string category = parts[0].Trim();
                            int value = int.Parse(parts[1]);

                            FieledSectors dataItem = new FieledSectors
                            {
                                ParentCategory = parentCategory,
                                Category = category,
                                Value = value,
                                AgeGroup = ageGroup,
                                Gender = gender,
                                Disability = disability
                            };

                            if (!categoryData.ContainsKey(category))
                            {
                                categoryData[category] = new List<FieledSectors>();
                            }

                            categoryData[category].Add(dataItem);
                        }
                    }

                    foreach (KeyValuePair<string, List<FieledSectors>> keyValuePair in categoryData)
                    {
                        var res = keyValuePair.Value
                            .GroupBy(entry => new {entry.Category, entry.AgeGroup, entry.Gender, entry.Disability})
                            .Select(group => new
                            {
                                group.Key.Category,
                                group.Key.AgeGroup,
                                group.Key.Disability,
                                group.Key.Gender,
                                Value = group.Sum(v => v.Value)
                            });


                        DetailedReportDataSet.DetailedReportDataTableRow newRow =
                            dataTable.NewDetailedReportDataTableRow();
                        Dictionary<(string, string, string), Action<string>> propertyMap =
                            new Dictionary<(string, string, string), Action<string>>
                            {
                                {("0-5", "Male", "WithDisability"), value => newRow.MaleW0 = value},
                                {("0-5", "Male", "WithoutDisability"), value => newRow.MaleWOUT0 = value},
                                {("0-5", "Female", "WithDisability"), value => newRow.FemaleW0 = value},
                                {("0-5", "Female", "WithoutDisability"), value => newRow.FemaleWOUT0 = value},
                                {("6-12", "Male", "WithDisability"), value => newRow.MaleW6 = value},
                                {("6-12", "Male", "WithoutDisability"), value => newRow.MaleWOUT6 = value},
                                {("6-12", "Female", "WithDisability"), value => newRow.FemaleW6 = value},
                                {("6-12", "Female", "WithoutDisability"), value => newRow.FemaleWOUT6 = value},
                                {("13-17", "Male", "WithDisability"), value => newRow.MaleW13 = value},
                                {("13-17", "Male", "WithoutDisability"), value => newRow.MaleWOUT13 = value},
                                {("13-17", "Female", "WithDisability"), value => newRow.FemaleW13 = value},
                                {("13-17", "Female", "WithoutDisability"), value => newRow.FemaleWOUT13 = value},
                                {("18-29", "Male", "WithDisability"), value => newRow.MaleW18 = value},
                                {("18-29", "Male", "WithoutDisability"), value => newRow.MaleWOUT18 = value},
                                {("18-29", "Female", "WithDisability"), value => newRow.FemaleW18 = value},
                                {("18-29", "Female", "WithoutDisability"), value => newRow.FemaleWOUT18 = value},
                                {("30-49", "Male", "WithDisability"), value => newRow.MaleW30 = value},
                                {("30-49", "Male", "WithoutDisability"), value => newRow.MaleWOUT30 = value},
                                {("30-49", "Female", "WithDisability"), value => newRow.FemaleW30 = value},
                                {("30-49", "Female", "WithoutDisability"), value => newRow.FemaleWOUT30 = value},
                                {("50-69", "Male", "WithDisability"), value => newRow.MaleW50 = value},
                                {("50-69", "Male", "WithoutDisability"), value => newRow.MaleWOUT50 = value},
                                {("50-69", "Female", "WithDisability"), value => newRow.FemaleW50 = value},
                                {("50-69", "Female", "WithoutDisability"), value => newRow.FemaleWOUT50 = value},
                                {("70-79", "Male", "WithDisability"), value => newRow.MaleW70 = value},
                                {("70-79", "Male", "WithoutDisability"), value => newRow.MaleWOUT70 = value},
                                {("70-79", "Female", "WithDisability"), value => newRow.FemaleW70 = value},
                                {("70-79", "Female", "WithoutDisability"), value => newRow.FemaleWOUT70 = value},
                                {("80+", "Male", "WithDisability"), value => newRow.MaleW80 = value},
                                {("80+", "Male", "WithoutDisability"), value => newRow.MaleWOUT80 = value},
                                {("80+", "Female", "WithDisability"), value => newRow.FemaleW80 = value},
                                {("80+", "Female", "WithoutDisability"), value => newRow.FemaleWOUT80 = value}
                            };

                        foreach (var item in res)
                        {
                            (string, string, string) key = (item.AgeGroup, item.Gender, item.Disability);
                            if (propertyMap.TryGetValue(key, out var assignAction))
                            {
                                assignAction(item.Value.ToString());
                            }
                        }

                        newRow.ProjectName = "Actual --- " + keyValuePair.Key;
                        
                        dataTable.AddDetailedReportDataTableRow(newRow);
                    }

                    var plannedProjectData =
                        entities.PredictedFlexibleInterventionsDatas.FirstOrDefault(p => p.ProjectId == projectId);
                    var normalPlannedData =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(
                            plannedProjectData.Values);
                    List<ProjectDataFromUser> customDataList = normalPlannedData
                        .SelectMany(listOfDict => listOfDict.Select(dict => new ProjectDataFromUser()
                        {
                            Category = dict["Category"],
                            Disability = dict["Disability"].Equals("W")
                                ? Disability.WithDisability
                                : Disability.WithoutDisability,
                            Gender = dict["Gender"].Equals("Male") ? Gender.Male : Gender.Female,
                            AgeGroup = dict["Age Group"],
                            Components = string.Join(",",
                                dict.Where(kvp =>
                                    kvp.Key != "Category" && kvp.Key != "Disability" && kvp.Key != "Gender" &&
                                    kvp.Key != "Age Group"))
                        }))
                        .ToList();

                    var plannedGroupedData = customDataList
                        .Select(group => new
                        {
                            group.Components,
                            group.AgeGroup,
                            group.Disability,
                            group.Gender,

                        }).OrderBy(a => a.AgeGroup).ThenBy(a => a.Gender).ThenBy(a => a.Disability);

                    List<FieledSectors> plannedDataList = new List<FieledSectors>();
                    Dictionary<string, List<FieledSectors>> plannedCategoryData =
                        new Dictionary<string, List<FieledSectors>>();

                    foreach (var group in plannedGroupedData)
                    {
                        string values = group.Components;
                        string ageGroup = group.AgeGroup;
                        string gender = group.Gender.ToString();
                        string disability = group.Disability.ToString();
                        string[] valuePairs = values.Split(new string[] {"],["}, StringSplitOptions.None);

                        foreach (string pair in valuePairs)
                        {
                            string[] parts = pair.Replace("[", "").Replace("]", "").Split(',');

                            string category = parts[0].Trim();
                            int value = int.Parse(parts[1]);

                            FieledSectors dataItem = new FieledSectors
                            {
                                Category = category,
                                Value = value,
                                AgeGroup = ageGroup,
                                Gender = gender,
                                Disability = disability
                            };

                            if (!plannedCategoryData.ContainsKey(category))
                            {
                                plannedCategoryData[category] = new List<FieledSectors>();
                            }

                            plannedCategoryData[category].Add(dataItem);
                        }
                    }

                    foreach (KeyValuePair<string, List<FieledSectors>> keyValuePairForPlannedData in
                        plannedCategoryData)
                    {
                        var resForPlannedData = keyValuePairForPlannedData.Value
                            .GroupBy(entry => new {entry.Category, entry.AgeGroup, entry.Gender, entry.Disability})
                            .Select(group => new
                            {
                                group.Key.Category,
                                group.Key.AgeGroup,
                                group.Key.Disability,
                                group.Key.Gender,
                                Value = group.Sum(v => v.Value)
                            });
                        DetailedReportDataSet.FinalReportDataTableRow plannedNewRow =
                            plannedDataDataTable.NewFinalReportDataTableRow();
                        Dictionary<(string, string, string), Action<string>> propertyMapForPlannedData =
                            new Dictionary<(string, string, string), Action<string>>
                            {
                                {("0-5", "Male", "WithDisability"), value => plannedNewRow.MaleW0 = value},
                                {("0-5", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT0 = value},
                                {("0-5", "Female", "WithDisability"), value => plannedNewRow.FemaleW0 = value},
                                {("0-5", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT0 = value},
                                {("6-12", "Male", "WithDisability"), value => plannedNewRow.MaleW6 = value},
                                {("6-12", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT6 = value},
                                {("6-12", "Female", "WithDisability"), value => plannedNewRow.FemaleW6 = value},
                                {("6-12", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT6 = value},
                                {("13-17", "Male", "WithDisability"), value => plannedNewRow.MaleW13 = value},
                                {("13-17", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT13 = value},
                                {("13-17", "Female", "WithDisability"), value => plannedNewRow.FemaleW13 = value},
                                {("13-17", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT13 = value},
                                {("18-29", "Male", "WithDisability"), value => plannedNewRow.MaleW18 = value},
                                {("18-29", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT18 = value},
                                {("18-29", "Female", "WithDisability"), value => plannedNewRow.FemaleW18 = value},
                                {("18-29", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT18 = value},
                                {("30-49", "Male", "WithDisability"), value => plannedNewRow.MaleW30 = value},
                                {("30-49", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT30 = value},
                                {("30-49", "Female", "WithDisability"), value => plannedNewRow.FemaleW30 = value},
                                {("30-49", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT30 = value},
                                {("50-69", "Male", "WithDisability"), value => plannedNewRow.MaleW50 = value},
                                {("50-69", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT50 = value},
                                {("50-69", "Female", "WithDisability"), value => plannedNewRow.FemaleW50 = value},
                                {("50-69", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT50 = value},
                                {("70-79", "Male", "WithDisability"), value => plannedNewRow.MaleW70 = value},
                                {("70-79", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT70 = value},
                                {("70-79", "Female", "WithDisability"), value => plannedNewRow.FemaleW70 = value},
                                {("70-79", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT70 = value},
                                {("80+", "Male", "WithDisability"), value => plannedNewRow.MaleW80 = value},
                                {("80+", "Male", "WithoutDisability"), value => plannedNewRow.MaleWOUT80 = value},
                                {("80+", "Female", "WithDisability"), value => plannedNewRow.FemaleW80 = value},
                                {("80+", "Female", "WithoutDisability"), value => plannedNewRow.FemaleWOUT80 = value}
                            };

                        foreach (var item in resForPlannedData)
                        {
                            (string, string, string) key = (item.AgeGroup, item.Gender, item.Disability);
                            if (propertyMapForPlannedData.TryGetValue(key, out var assignAction))
                            {
                                assignAction(item.Value.ToString());
                            }
                        }

                        plannedNewRow.Name = "Planned --- " + keyValuePairForPlannedData.Key;

                        totalRightHoldersForAllTheProject += project.DirectRightHolders;

                        plannedDataDataTable.AddFinalReportDataTableRow(plannedNewRow);
                    }
                    var users = _context.AssignUsers.Where(u => u.ProjectId == projectId).ToList();
                    List<string> usersNameList = new List<string>();
                    foreach (AssignUser user in users)
                        usersNameList.Add(_context.AspNetUsers.FirstOrDefault(u=>u.Id.Equals(user.UserId.ToString())).UserName);
                    
                    parameters = new ReportParameter[]
                    {
                        new ReportParameter("ReportParameterProjectName", project.Name),

                        new ReportParameter("ReportParameterDateTime", project.CreateDate.Value.ToString("yyyy-M-d dddd")),
                        new ReportParameter("ReportParameterCompletedBy", string.Join(", ", usersNameList)),
                    };
                   

                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", (DataTable) dataTable);
                    ReportDataSource reportDataSource1 = new ReportDataSource("DataSet2", (DataTable) plannedDataDataTable);

                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    ReportViewer1.LocalReport.ReportPath =
                        Server.MapPath("~/bin/PredictedAndActualDataReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource1);
                    ReportViewer1.LocalReport.SetParameters(parameters);
                    ReportViewer1.LocalReport.Refresh();
                    ReportViewer1.DataBind();
                }
            }
        }
    }
}