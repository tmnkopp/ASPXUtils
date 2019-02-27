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
using System.Web.Caching;
using System.Text;
using System.Xml;
using ASPXUtils;
using ASPXUtils.Data;

namespace ASPXUtils
{

    public class LogDAL : DataEntity<Log>
    {
        public LogDAL() { }


        public static int Add(Log obj)
        {
            ResetCache();
            SqlHelper objSqlHelper = new SqlHelper();
            string sSql = "";
            sSql = @" INSERT INTO wutil_Log(		
LogTypeID,		
Severity,		
Message,		
Exception,		
IPAddress,		
UserID,		
PageURL,		
ReferrerURL,		
CreatedOn		
)VALUES(		
@LogTypeID,		
@Severity,		
@Message,		
@Exception,		
@IPAddress,		
@UserID,		
@PageURL,		
@ReferrerURL,		
@CreatedOn		
); 
SELECT SCOPE_IDENTITY() ;";
            objSqlHelper.AddParameterToSQLCommand("@LogTypeID", SqlDbType.VarChar, Convert.ToInt32(obj.LogTypeID));
            objSqlHelper.AddParameterToSQLCommand("@Severity", SqlDbType.VarChar, Convert.ToInt32(obj.Severity));
            objSqlHelper.AddParameterToSQLCommand("@Message", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.Message));
            objSqlHelper.AddParameterToSQLCommand("@Exception", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.Exception));
            objSqlHelper.AddParameterToSQLCommand("@IPAddress", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.IPAddress));
            objSqlHelper.AddParameterToSQLCommand("@UserID", SqlDbType.VarChar, Convert.ToInt32(obj.UserID));
            objSqlHelper.AddParameterToSQLCommand("@PageURL", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.PageURL));
            objSqlHelper.AddParameterToSQLCommand("@ReferrerURL", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.ReferrerURL));
            objSqlHelper.AddParameterToSQLCommand("@CreatedOn", SqlDbType.DateTime, Convert.ToDateTime(DateTime.Now));

            int _id = objSqlHelper.GetExecuteScalarByCommand(sSql);
            objSqlHelper.Dispose();
            return _id;
        }
        public static void Update(Log obj)
        {
            ResetCache();

            SqlHelper objSqlHelper = new SqlHelper();
            string sSql = "";
            sSql = @" Update wutil_Log SET
 
		LogTypeID = @LogTypeID,
		Severity = @Severity,
		Message = @Message,
		Exception = @Exception,
		IPAddress = @IPAddress,
		UserID = @UserID,
		PageURL = @PageURL,
		ReferrerURL = @ReferrerURL,
		CreatedOn = @CreatedOn
 WHERE ID = @ID;";

            objSqlHelper.AddParameterToSQLCommand("@ID", SqlDbType.VarChar, Convert.ToInt32(obj.ID));
            objSqlHelper.AddParameterToSQLCommand("@LogTypeID", SqlDbType.VarChar, Convert.ToInt32(obj.LogTypeID));
            objSqlHelper.AddParameterToSQLCommand("@Severity", SqlDbType.VarChar, Convert.ToInt32(obj.Severity));
            objSqlHelper.AddParameterToSQLCommand("@Message", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.Message));
            objSqlHelper.AddParameterToSQLCommand("@Exception", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.Exception));
            objSqlHelper.AddParameterToSQLCommand("@IPAddress", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.IPAddress));
            objSqlHelper.AddParameterToSQLCommand("@UserID", SqlDbType.VarChar, Convert.ToInt32(obj.UserID));
            objSqlHelper.AddParameterToSQLCommand("@PageURL", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.PageURL));
            objSqlHelper.AddParameterToSQLCommand("@ReferrerURL", SqlDbType.VarChar, ASPXUtils.CommonUtils.GetDataString(obj.ReferrerURL));
            objSqlHelper.AddParameterToSQLCommand("@CreatedOn", SqlDbType.DateTime, Convert.ToDateTime(obj.CreatedOn));

            objSqlHelper.GetExecuteNonQueryByCommand(sSql);
            objSqlHelper.Dispose();

        }
        public static void Delete(int ID)
        {
            ResetCache();
            SqlHelper objSqlHelper = new SqlHelper();
            string sSql = "";
            sSql = " DELETE FROM wutil_Log WHERE ID = @ID;";
            objSqlHelper.AddParameterToSQLCommand("@ID", SqlDbType.VarChar, Convert.ToString(ID));
            objSqlHelper.GetExecuteNonQueryByCommand(sSql);
            objSqlHelper.Dispose();
        }

        public static void ResetCache()
        {
            ASPXUtils.SiteCache.Remove(typeof(LogDAL).ToString().ToUpper());
        }

        public List<Log> GetAll()
        {
            string _sql = "Select * From wutil_Log";
            return base.GetListFromDS(base.GetDataSet(_sql));
        }
        public override Log LoadFromDataRow(DataRow oDataRow)
        {
            Log obj = new Log();
            obj.ID = Convert.ToInt32(oDataRow["ID"]);
            obj.LogTypeID = Convert.ToInt32(oDataRow["LogTypeID"]);
            obj.Severity = Convert.ToInt32(oDataRow["Severity"]);
            obj.Message = ASPXUtils.CommonUtils.GetDataString(oDataRow["Message"]);
            obj.Exception = ASPXUtils.CommonUtils.GetDataString(oDataRow["Exception"]);
            obj.IPAddress = ASPXUtils.CommonUtils.GetDataString(oDataRow["IPAddress"]);
            obj.UserID = Convert.ToInt32(oDataRow["UserID"]);
            obj.PageURL = ASPXUtils.CommonUtils.GetDataString(oDataRow["PageURL"]);
            obj.ReferrerURL = ASPXUtils.CommonUtils.GetDataString(oDataRow["ReferrerURL"]);
            obj.CreatedOn = Convert.ToDateTime(oDataRow["CreatedOn"]);

            return obj;
        }

    }// End Class 

}
