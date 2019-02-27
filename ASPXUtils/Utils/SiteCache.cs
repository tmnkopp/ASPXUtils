using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Web.Configuration;

namespace ASPXUtils
{
    public static class SiteCache
    { 
        public static void Add(string _CacheKey, DataSet objDataSet)
        {
            if (WebConfigurationManager.AppSettings["Debugging"] != null)
            {
                if (WebConfigurationManager.AppSettings["Debugging"]== "true")
                {
                    return; 
                }
            }
            HttpRuntime.Cache.Insert(_CacheKey, objDataSet);
        }
        public static void Remove(string _CacheKey)
        { 
            HttpRuntime.Cache.Remove(_CacheKey);  
        } 
        public static DataSet GetDataSet(string _CacheKey)
        {
            return (DataSet)HttpRuntime.Cache[_CacheKey]; 
        }
    }
}
