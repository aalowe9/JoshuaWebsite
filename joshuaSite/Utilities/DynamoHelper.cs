using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using joshuaSite.Models;

namespace joshuaSite.Utilities
{
    public class DynamoHelper
    {

        private static AmazonDynamoDBClient Connect()
        {
            BasicAWSCredentials credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["AccessKey"], ConfigurationManager.AppSettings["SecretKey"]);
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUWest1);

            return client;
        }

        private static DateItem CreateDateItemFromDocument(Document doc)
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
                        d.DateTime = DateTime.ParseExact(d.DateString, "dd-MM-yyyy", null);
                        break;
                    case "ImageURL":
                        Image img = new Image();
                        img.ImagePath = x.AsPrimitive().Value.ToString();
                        d.Images = new List<Image>();
                        d.Images.Add(img); // only 1 image supported for now
                        break;
                    case "Synopsis":
                        d.Synopsis = x.AsPrimitive().Value.ToString();
                        break;
                    case "SpecialDate":
                        d.SpecialDate = x.AsPrimitive().Value.ToString() == "1" ? true : false;

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
            return d;

        }

        private static DateItem ConvertJoshuaDateToDateItem(JoshuaDate jd)
        {
            DateItem d = new DateItem();
            d.Images = new List<Image>();
            var img = new Image();
            img.ImagePath = jd.ImageURL;
            d.Images.Add(img);
            d.DateString = jd.DateStamp;
            d.DateTime = DateTime.ParseExact(d.DateString, "dd-MM-yyyy", null);
            d.ID = jd.ID;
            d.Synopsis = jd.Synopsis;
            d.TextContent = jd.TextContent;
            d.SpecialDate = jd.SpecialDate;
            try
            {
                d.FullDateStamp = DateTime.Parse(jd.FullDateStamp);
            }
            catch (Exception exc)
            {
            }
            return d;
        }

        public static DateItem GetDate(string id)
        {
            using (var client = Connect())
            {
                DynamoDBContext context = new DynamoDBContext(client);
                JoshuaDate x = context.Load<JoshuaDate>(id);
                return ConvertJoshuaDateToDateItem(x);
            }
        }

        public static List<DateItem> GetAllDates(string tableName)
        {
            using (var client = Connect())
            {

                Table DateTable = Table.LoadTable(client, tableName);
                ScanFilter scanFilter = new ScanFilter();
                scanFilter.AddCondition("Synopsis", ScanOperator.IsNotNull);
                Search search = DateTable.Scan(scanFilter);

                List<Document> documentList = new List<Document>();
                documentList = search.GetNextSet();

                List<DateItem> allDates = new List<DateItem>();
                foreach (var doc in documentList)
                {
                    var d = CreateDateItemFromDocument(doc);

                    allDates.Add(d);
                    System.Threading.Thread
                        .Sleep(100); // added because otherwise random seems to pick the same number ???
                }

                return allDates;
            }
        }

        public static void CreateDateItem(DateItem d)
        {
            using (var client = Connect())
            {
                DynamoDBContext context = new DynamoDBContext(client);
                JoshuaDate x = new JoshuaDate
                {
                    ID = d.ID,
                    TextContent = d.TextContent,
                    DateStamp = d.DateString,
                    FullDateStamp = d.FullDateStamp.ToString(CultureInfo.InvariantCulture),
                    Synopsis = d.Synopsis,
                    SpecialDate = d.SpecialDate
                };
                if (d.Images != null)
                {
                    x.ImageURL = d.Images.FirstOrDefault()?.ImagePath;
                }
                context.Save(x);
            }
        }

        public static void UpdateDateItem(DateItem d, HttpPostedFileBase image)
        {
            using (var client = Connect())
            {
                DynamoDBContext context = new DynamoDBContext(client);

                JoshuaDate x = context.Load<JoshuaDate>(d.ID);

                if (image != null)
                {
                    if (d.Images != null)
                    {
                        x.ImageURL = d.Images.FirstOrDefault()?.ImagePath;
                    }
                }
                x.SpecialDate = d.SpecialDate;
                x.Synopsis = d.Synopsis;
                x.TextContent = d.TextContent;
                context.Save(x);
            }
        }

        public static void DeleteDateItem(string id)
        {
            using (var client = Connect())
            {
                DynamoDBContext context = new DynamoDBContext(client);

                context.Delete<JoshuaDate>(id);  
            }
        }
    }

    [DynamoDBTable("JWLDateTable")]
    public class JoshuaDate
    {
        [DynamoDBHashKey] //Partition key
        public string ID
        {
            get; set;
        }
        [DynamoDBProperty]
        public string TextContent
        {
            get; set;
        }
        [DynamoDBProperty]
        public string DateStamp
        {
            get; set;
        }
        [DynamoDBProperty]
        public string FullDateStamp
        {
            get; set;
        }
        [DynamoDBProperty]
        public string ImageURL
        {
            get; set;
        }

        [DynamoDBProperty]
        public string Synopsis
        {
            get; set;
        }

        [DynamoDBProperty]
        public bool SpecialDate
        {
            get; set;
        }
    }
}