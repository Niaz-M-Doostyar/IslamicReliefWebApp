using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IR_Admin.Db;

namespace IR_Admin.Helpers
{
    public class ProjectDataFromUser
    {
        public Guid ProjectId { get; set; }
        public string Category { get; set; }
        public Gender Gender { get; set; }
        public Disability Disability { get; set; }
        public string AgeGroup { get; set; }
        public DateTime Month { get; set; }
        public string Components { get; set; }
        public Guid FlexibleFieldId { get; set; }
    }
    public class UserCategory
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<UserDataTable> StaticFields { get; set; }
        
        public List<UserCategory> Categories = new List<UserCategory>();

        public List<UserCategory> CategoriesWithFixedFields(List<Category> categories)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                UserDataTable dt = new UserDataTable();
                var sfiled = dt.FillUserDataTable();
                Categories.Add(new UserCategory(){CategoryId=categories[i].Id,CategoryName = categories[i].Name,StaticFields = sfiled});
            }

            return Categories;
        }
    }

    public class UserComponents
    {
        public Guid ComponentId { get; set; }
        public string ComponentName { get; set; }
        public List<UserComponents> ComponentsList { get; set; }

        public UserComponents()
        {
            ComponentsList = new List<UserComponents>();
        }

        public List<UserComponents> GetUserComponents(Guid campId, string[] componentsNames)
        {
            foreach (string component in componentsNames)
            {
                ComponentsList.Add(new UserComponents(){ComponentId = campId,ComponentName = component});
            }

            return ComponentsList;
        }
    }
    public class UserDataTable
    {
        public List<UserDataTable> ListOfUserDataTables;

        public UserDataTable()
        {
            ListOfUserDataTables = new List<UserDataTable>();
        }
        public string Disability { get; set; }
        public Gender Gender { get; set; }
        public string AgeGroup { get; set; }
        public string Components { get; set; }

        public List<UserDataTable> FillUserDataTable()
        {
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "0-5" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "6-12" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "13-17" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "18-29" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "30-49" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "50-69" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "70-79" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Female, AgeGroup = "80+" });

                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "0-5" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "6-12" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "13-17" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "18-29" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "30-49" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "50-69" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "70-79" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Female, AgeGroup = "80+" });

                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "0-5" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "6-12" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "13-17" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "18-29" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "30-49" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "50-69" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "70-79" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W", Gender = Gender.Male, AgeGroup = "80+" });

                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "0-5" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "6-12" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "13-17" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "18-29" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "30-49" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "50-69" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "70-79" });
                ListOfUserDataTables.Add(new UserDataTable() { Disability = "W/Out", Gender = Gender.Male, AgeGroup = "80+" });
                return ListOfUserDataTables;
        }
    }
    public class GroupedDataViewModelForUserEditFeature
    {
        public Guid FlexibleFieldId { get; set; }
        public string CategoryName { get; set; }
        public string Disability { get; set; }
        public string Gender { get; set; }
        public string AgeGroup { get; set; }
        public List<KeyValuePair<string, int>> Value { get; set; }
    }
}