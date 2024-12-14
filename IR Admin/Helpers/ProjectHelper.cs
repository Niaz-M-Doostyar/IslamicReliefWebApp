using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IR_Admin.Db;
using ISO3166;

namespace IR_Admin.Helpers
{
    public class ProjectHelper
    {
        public static CountryBasicProjectData FindProjectById(Guid id)
        {
            return new IR_DBEntities().CountryBasicProjectDatas.Find(id);
        }

        public static bool CheckIfProjectDataExistsInPredicatedFlexibleInterventions(Guid projectId)
        {
            return new IR_DBEntities().PredictedFlexibleInterventionsDatas.FirstOrDefault(p =>
                p.ProjectId == projectId) != null
                ? true
                : false;
        }
        public static string GetProjectLocationByUsersId(string userIds)
        {
            List<string> province = new List<string>();
            foreach (string u in userIds.Split(','))
                province.Add(FindProvinceNameById(new IR_DBEntities().AspNetUsers.Find(u).ProvinceId));
            return string.Join(",",province);
        }
        public static string FindProvinceNameById(Guid id)
        {
            return new IR_DBEntities().Provinces.Find(id).Name;
        }
        public static Guid FindProvinceNameByUserId(string id)
        {
            return new IR_DBEntities().AspNetUsers.FirstOrDefault(u=>u.Id.Equals(id)).ProvinceId;
        }
        public static string FindCategoryNameById(Guid id)
        {
            return new IR_DBEntities().Categories.Find(id).Name;
        }
        public static List<Province> GetProvinces()
        {
            return new IR_DBEntities().Provinces.ToList();
        }
        public static int CalculateSumFromInput(string inputData)
        {
            string[] segments = inputData.Split(new char[] { '[', ']', ',' }, StringSplitOptions.RemoveEmptyEntries);
            
            int sum = 0;
            for (int i = 1; i < segments.Length; i += 2)
            {
                if (int.TryParse(segments[i], out int value))
                {
                    sum += value;
                }
            }

            return sum;
        }
        public static List<KeyValuePair<string, int>> ConvertStringToList(string input)
        {
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            
            string[] pairs = input.Split(new[] { "[", "],[", "]" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(',');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    if (int.TryParse(keyValue[1].Trim(), out int value))
                    {
                        result.Add(new KeyValuePair<string, int>(key, value));
                    }
                }
            }

            return result;
        }
    }

    public class CountriesModel
    {
        public List<string> Countries { get; set; }
        public string SelectedCountry { get; set; }

        public static List<string> GetCountries()
        {
            List<string> countries = new List<string>();
            foreach (var country in Country.List) 
                countries.Add(country.Name);
            return countries;
        }
    }

    public class GroupingKey
    {
        public string Month { get; set; }
        public string ProjectId { get; set; }
    }
    public class FieledSectors
    {
        
        public string ParentCategory { get; set; }
        public string Category { get; set; }
        public int Value { get; set; }
        public string AgeGroup { get; set; }
        public string Gender { get; set; }
        public string Disability { get; set; }
    }
}