using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using joshuaSite.Models;
using joshuaSite.Utilities;

namespace joshuaSite.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public  ActionResult Index()
        {
            FormsAuthentication.SignOut();
            List<DateItem> allDates = DynamoHelper.GetAllDates("JWLDateTable");
            var dateGroups = allDates.GroupBy(m => m.DateTime.Year).OrderBy(m => m.Key);
            List<YearContainer> years = new List<YearContainer>();

            foreach (var groupingByClassA in dateGroups)
            {
                YearContainer y = new YearContainer();
                y.Year = groupingByClassA.Key.ToString();
                y.DateItems = new List<DateItem>();

                var ordered = groupingByClassA.OrderBy(m => m.DateTime);
                //iterating through values
                foreach (var d in ordered)
                {
                   y.DateItems.Add(d);
                }

                years.Add(y);
            }


            return View("Index", years);
        }
    }
}