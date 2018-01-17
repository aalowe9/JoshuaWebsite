using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace joshuaSite.Models
{
    public class DateItem
    {
        public DateTime DateTime { get; set; }
        public string DateString { get; set; }
        public string TextContent { get; set; }
        public List<Image> Images { get; set; }
    }

    public class Image
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Caption { get; set; }
    }
}