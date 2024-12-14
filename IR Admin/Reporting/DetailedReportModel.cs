using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IR_Admin.Db;
using IR_Admin.Helpers;

namespace IR_Admin.Reporting
{
    public class DetailedReportModel
    {
        public BeneficiariesRecord BeneficiariesRecordTable { get; set; }
        public FlexibleIntervention FlexibleFieldsTable { get; set; }

    }
    public class AgeGroupData
    {
        public string AgeGroup { get; set; }
        public int TotalValue { get; set; }
    }

    public class DisabilityGroupData
    {
        public string Disability { get; set; }
        public List<AgeGroupData> AgeGroups { get; set; }
    }

    public class GenderGroupData
    {
        public string Gender { get; set; }
        public List<DisabilityGroupData> DisabilityGroups { get; set; }
    }

    public class CategoryData
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public List<GenderGroupData> GenderGroups { get; set; }
    }
    public class ProjectsData
    {
        public Guid? ProjectId { get; set; }
        public List<CategoryData> CategoryDataList { get; set; }
    }
}