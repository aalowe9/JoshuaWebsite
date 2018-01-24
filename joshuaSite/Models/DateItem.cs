using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace joshuaSite.Models
{
    public class DateItem
    {
        public string ID { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public string DateString { get; set; }
        [DataType(DataType.MultilineText)]
        public string TextContent { get; set; }
        public List<Image> Images { get; set; }
        public bool SpecialDate { get; set; }
        [DataType(DataType.MultilineText)]
        public string Synopsis { get; set; }
        public int Rand { get; set; }
        public DateTime FullDateStamp { get; set; }
    }

    public class Image
    {
        public string Name { get; set; }
        [Required]
        public string ImagePath { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Caption { get; set; }
    }

    public class YearContainer
    {
        public string Year { get; set; }
        public List<DateItem> DateItems { get; set; }
    }
}