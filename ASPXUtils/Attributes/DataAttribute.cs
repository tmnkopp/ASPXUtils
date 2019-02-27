using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ASPXUtils;
using System.Data;

namespace ASPXUtils
{
    public class DataEntityAttribute : Attribute
    {
        public DataEntityAttribute()
        {
        }
        public string TableName { get; set; } 
    }

}
