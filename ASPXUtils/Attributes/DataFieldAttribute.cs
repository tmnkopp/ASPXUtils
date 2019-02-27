using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ASPXUtils;
using System.Data;

namespace ASPXUtils
{
    public class DatafieldAttribute : Attribute
    {
        public DatafieldAttribute(string s) { 
            this.FieldName = s; 
        }
        public string HasManyTable { get; set; }
        public string HasManyParentIdFieldName { get; set; }
        public string HasManyChildIdFieldName { get; set; }
        public string EVIL { get; set; }

        public string FieldName { get; set; }
        public bool IsPrimary { get; set; } 
        protected SqlDbType dataType = SqlDbType.VarChar;
        public SqlDbType DataType
        {
            get { return this.dataType; }
            set { this.dataType = value; }
        } 

    } 
}
