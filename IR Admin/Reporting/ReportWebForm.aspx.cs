using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IR_Admin.Db;
using IR_Admin.Helpers;
using Microsoft.Reporting.WebForms;

namespace IR_Admin.Reporting
{
    public partial class ReportWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IR_DBEntities entities = new IR_DBEntities();
                int year = int.Parse(Request.QueryString["year"]);
                using (var _context = entities)
                {
                    var projects = _context.CountryBasicProjectDatas.Where(p=>p.CreateDate.Value.Year == year).ToList();
                    
                    DetailedReportDataSet dataSet = new DetailedReportDataSet();
                    DetailedReportDataSet.DetailedReportDataTableDataTable dataTable = dataSet.DetailedReportDataTable;
                    ReportParameter[] parameters = new ReportParameter[] { };
                    int? totalRightHoldersForAllTheProject = 0;
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
                        var queryListedData = query.ToList();
                        int counter = 0;
                        foreach (var q in queryListedData)
                        {
                            queryListedData[counter++].FlexibleFieldsTable.Values = ProjectHelper.CalculateSumFromInput(q.FlexibleFieldsTable.Values).ToString();
                        }
                        var groupedData = queryListedData.GroupBy(entry => new { entry.BeneficiariesRecordTable.AgeGroup, entry.BeneficiariesRecordTable.Gender, entry.BeneficiariesRecordTable.Disability })
                            .Select(group => new
                            {
                                group.Key.AgeGroup,
                                group.Key.Gender,
                                group.Key.Disability,
                                TotalValue = group.Sum(entry => int.Parse(entry.FlexibleFieldsTable.Values))
                            }).OrderBy(a=>a.AgeGroup).ThenBy(a => a.Gender).ThenBy(a => a.Disability); 


                        DetailedReportDataSet.DetailedReportDataTableRow newRow = dataTable.NewDetailedReportDataTableRow();
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

                        foreach (var item in groupedData)
                        {
                            (string, string, string) key = (item.AgeGroup, item.Gender, item.Disability);
                            if (propertyMap.TryGetValue(key, out var assignAction))
                            {
                                assignAction(item.TotalValue.ToString());
                            }
                        }

                        newRow.ProjectName = projectData.Name;
                        newRow.DonarOrPartner = projectData.Donar_Partner;
                        totalRightHoldersForAllTheProject += projectData.DirectRightHolders;
                        parameters = new ReportParameter[]
                        {
                            new ReportParameter("ReportParameterCountry", projectData.Country),
                            new ReportParameter("ReportParameterTotalNumberOfProjectsPerYear", projects.Count.ToString()),
                            new ReportParameter("ReportParameterTotalProjectsWithActivitiesInRecentYear", year.ToString()),
                            new ReportParameter("ReportParameterDirectRightHolders", totalRightHoldersForAllTheProject.ToString()),
                            new ReportParameter("ReportParameterTotalIndirectRightHolders", (totalRightHoldersForAllTheProject*6.2).ToString()),
                        };
                        dataTable.AddDetailedReportDataTableRow(newRow);
                    }
                    ReportDataSource reportDataSource = new ReportDataSource("BeneficiaryDataSet",(DataTable) dataTable);
                 
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/bin/DetailedReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    ReportViewer1.LocalReport.SetParameters(parameters);
                    ReportViewer1.LocalReport.Refresh();
                    ReportViewer1.DataBind();
                }
            }
        }
    }
}