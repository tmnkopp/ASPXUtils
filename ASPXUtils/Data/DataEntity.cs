using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using ASPXUtils;

/// <summary>
/// Summary description for DataEntity
/// </summary>
namespace ASPXUtils.Data
{


        public abstract class DataEntity<T>
        {
            public abstract T LoadFromDataRow(DataRow oDataRow);
  
            public virtual string _CacheKey  {   
                get{return Convert.ToString(this.GetType()).ToUpper();}    
            }

            protected DataSet GetDataSet(string _sql) {
                return GetDataSet(new List<SqlParameter>(), _sql, this._CacheKey);
            } 
            protected DataSet GetDataSet(string _sql, string _CacheKey) {
                return GetDataSet(new List<SqlParameter>(), _sql, _CacheKey);
            } 
            protected DataSet GetDataSet(List<SqlParameter> oSqlParameters, string _sql, string _CacheKey)
            { 
                DataSet objDataSet = SiteCache.GetDataSet(_CacheKey); 
                if (objDataSet == null)   { 
                    objDataSet = GetDataSetDirect(oSqlParameters, _sql); 
                    SiteCache.Add(_CacheKey, objDataSet); 
                }
                return objDataSet;
            }  
            protected DataSet GetDataSetDirect(List<SqlParameter> oSqlParameters, string _sql)
            { 
                SqlHelper objSqlHelper = new SqlHelper();
                foreach (SqlParameter p in oSqlParameters) 
                    objSqlHelper.AddParameterToSQLCommand(p.ParameterName, SqlDbType.VarChar, Convert.ToString(p.Value));
                DataSet objDataSet = objSqlHelper.GetDatasetByCommand(_sql);
                objSqlHelper.Dispose(); 
                return objDataSet;
            }
            protected virtual List<T> GetListFromDS(DataSet objDataSet) 
            {
                List<T> objList = new List<T>();
                foreach (DataRow oDataRow in objDataSet.Tables[0].Rows) {
                    objList.Add(LoadFromDataRow(oDataRow));
                }
                return objList;
            } 
        }

    }