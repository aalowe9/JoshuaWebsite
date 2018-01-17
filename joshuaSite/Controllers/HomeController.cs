using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using joshuaSite.Models;

namespace joshuaSite.Controllers
{
    public class HomeController : Controller
    {
        private static string tableName = "JoshuaDateTable";

        // GET: Home
        public  ActionResult Index()
        {
            var credentials = new BasicAWSCredentials("AKIAJPKNX6O34KAXNZUA", "A+QAJxiu+8Xj/UBEA+e8Gj15Pj3f9cwi3uHtO7ED");
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUWest1);



            Table DateTable = Table.LoadTable(client, tableName);
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("TextContent", ScanOperator.IsNotNull);
            Search search = DateTable.Scan(scanFilter);

            List<Document> documentList = new List<Document>();
            documentList = search.GetNextSet();

            List<DateItem> allDates = new List<DateItem>();
            foreach (var doc in documentList)
            {
                DateItem d = new DateItem();

                foreach (var e in doc.GetAttributeNames())
                {
                    var x = doc[e];
                    switch (e)
                    {
                        case "TextContent":
                            d.TextContent = x.AsPrimitive().Value.ToString();
                            break;
                        case "DateStamp":
                            d.DateString = x.AsPrimitive().Value.ToString();
                            break;
                    }
                   
                }

                allDates.Add(d);
            }


            return View("Index");
        }
    }
}