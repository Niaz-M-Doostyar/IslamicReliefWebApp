using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using IR_Admin.Db;
using IR_Admin.Models;
using Newtonsoft.Json;

namespace IR_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {

        // GET: Dashboard
        public ActionResult Index()
        {
           

            List<DataPoint> dataPoints1 = new List<DataPoint>();

            dataPoints1.Add(new DataPoint("Total Beneficiary", 50));
            dataPoints1.Add(new DataPoint("Total Direct Rightholders", 25));
            dataPoints1.Add(new DataPoint("Total Indirect Rightholders", 25));

            ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1);

            List<DataPoint> dataPoints2 = new List<DataPoint>();

            dataPoints2.Add(new DataPoint("Male", 40));
            dataPoints2.Add(new DataPoint("Female", 60));
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2);

            List<DataPoint> dataPoints3 = new List<DataPoint>();

            dataPoints3.Add(new DataPoint("Host Community (Male With Disability)", 50));
            dataPoints3.Add(new DataPoint("Host Community (Male Without Disability)", 10));
            dataPoints3.Add(new DataPoint("IDP (Male With Disability)", 30));
            dataPoints3.Add(new DataPoint("IDP (Male Without Disability)", 30));
            dataPoints3.Add(new DataPoint("Refugee (Male With Disability)", 17));
            dataPoints3.Add(new DataPoint("Refugee (Male Without Disability)", 17));

            ViewBag.DataPoints3 = JsonConvert.SerializeObject(dataPoints3);

            List<DataPoint> dataPoints4 = new List<DataPoint>();
            dataPoints4.Add(new DataPoint("Host Community (Female With Disability)", 50));
            dataPoints4.Add(new DataPoint("Host Community (Female Without Disability)", 10));
            dataPoints4.Add(new DataPoint("IDP (Female With Disability)", 30));
            dataPoints4.Add(new DataPoint("IDP (Female Without Disability)", 30));
            dataPoints4.Add(new DataPoint("Refugee (Female With Disability)", 17));
            dataPoints4.Add(new DataPoint("Refugee (Female Without Disability)", 17));

            ViewBag.DataPoints4 = JsonConvert.SerializeObject(dataPoints4);

            List<DataPoint> dataPoints5 = new List<DataPoint>();

            dataPoints5.Add(new DataPoint("Economics", 1));
            dataPoints5.Add(new DataPoint("Physics", 2));
            dataPoints5.Add(new DataPoint("Literature", 4));
            dataPoints5.Add(new DataPoint("Chemistry", 4));
            dataPoints5.Add(new DataPoint("Literature", 9));
            dataPoints5.Add(new DataPoint("Physiology or Medicine", 11));
            dataPoints5.Add(new DataPoint("Peace", 13));

            ViewBag.DataPoints5 = JsonConvert.SerializeObject(dataPoints5);
            return View();
        }
        
    }
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}
