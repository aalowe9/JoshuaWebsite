﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
                        case "ID":
                            d.ID = x.AsPrimitive().Value.ToString();
                            break;
                        case "TextContent":
                            d.TextContent = x.AsPrimitive().Value.ToString();
                            break;
                        case "DateStamp":
                            d.DateString = x.AsPrimitive().Value.ToString();
                            d.DateTime = DateTime.Parse(d.DateString);
                            break;
                        case "Synopsis":
                            d.Synopsis = x.AsPrimitive().Value.ToString();
                            break;
                    }
                   var r = new Random();
                    if (r.Next(0, 2) == 0)
                    {
                        d.Rand = 0;
                    }
                    else
                    {
                        d.Rand = 1;
                    }
                }

                allDates.Add(d);
            }

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