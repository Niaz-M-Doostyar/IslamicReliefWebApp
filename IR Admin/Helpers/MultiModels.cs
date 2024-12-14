using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IR_Admin.Db;

namespace IR_Admin.Helpers
{
    public class MultiModels
    {
        public BeneficiariesRecord BeneficiariesRecordModel { get; set; }
        public CountryBasicProjectData CountryBasicProjectDataModel { get; set; }
        public Category CategoryModel { get; set; }
        public FlexibleIntervention FlexibleInterventionModel { get; set; }
        public List<AspNetUser> Users { get; set; }
        public CountriesModel Countries { get; set; }

    }
}