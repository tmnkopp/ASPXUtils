using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Data;
using System.Web.Configuration;

namespace ASPXUtils.Data
{
    public class GenericDAL<T>
    {

        public GenericDAL(Type t)
        {

        }
        public static string CacheString = "{0}";

        public static int Insert(T obj)
        {
            SqlHelper objSqlHelper = new SqlHelper();
            string sql = "";
            string Tablename = "";
            int id = 0;
            Type type = typeof(T);
            StringBuilder SQL = new StringBuilder();
            StringBuilder HTML = new StringBuilder();
            try
            {
                foreach (PropertyInfo field in obj.GetType().GetProperties())
                {
                    foreach (DatafieldAttribute attr in field.GetCustomAttributes(typeof(DatafieldAttribute), true))
                    {
                        if (!attr.IsPrimary)
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(attr.FieldName);
                            SQL.AppendFormat("{{0}}{0},", field.Name);
                            switch (attr.DataType)
                            {
                                case SqlDbType.Int:
                                    objSqlHelper.AddParameterToSQLCommand("@" + field.Name, SqlDbType.Int, CommonUtils.GetDataInt(propertyInfo.GetValue(obj, null)));
                                    break;
                                case SqlDbType.DateTime:
                                    objSqlHelper.AddParameterToSQLCommand("@" + field.Name, SqlDbType.DateTime, CommonUtils.GetDataDateTime(propertyInfo.GetValue(obj, null)));
                                    break;
                                default:
                                    objSqlHelper.AddParameterToSQLCommand("@" + field.Name, SqlDbType.VarChar, CommonUtils.GetDataString(propertyInfo.GetValue(obj, null)));
                                    break;
                            }
                        }
                    }
                }

                foreach (DataEntityAttribute attr in type.GetCustomAttributes(typeof(DataEntityAttribute), true))
                {
                    Tablename = attr.TableName;
                }

                string fields = string.Format(SQL.ToString(), "");
                string vals = string.Format(SQL.ToString(), "@");

                sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT SCOPE_IDENTITY() ;", Tablename, fields, vals);
                sql = sql.Replace(",)", ")");
                id = objSqlHelper.GetExecuteScalarByCommand(sql);

            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("<br>" + sql);
                HttpContext.Current.Response.Write("<br>" + ex.Message + " " + ex.StackTrace);
                throw;

            }
            finally
            {
                objSqlHelper.Dispose();
            }
            objSqlHelper.Dispose();
            ResetCache(type);
            return id;
        }

        public static void Update(T obj)
        {
            SqlHelper objSqlHelper = new SqlHelper();
            string sql = "";
            string Tablename = "";
            string KeyName = "";
            int KeyValue = 0;
            int id = 0;
            Type type = typeof(T);
            StringBuilder SQL = new StringBuilder();
            try
            {
                foreach (PropertyInfo field in obj.GetType().GetProperties())
                {
                    foreach (DatafieldAttribute attr in field.GetCustomAttributes(typeof(DatafieldAttribute), true))
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(attr.FieldName);

                        if (!attr.IsPrimary)
                        {
                            SQL.AppendFormat("{0}=@{0},", field.Name);
                            switch (attr.DataType)
                            {
                                case SqlDbType.Int:
                                    objSqlHelper.AddParameterToSQLCommand("@" + field.Name, SqlDbType.Int, CommonUtils.GetDataInt(propertyInfo.GetValue(obj, null)));
                                    break;
                                case SqlDbType.DateTime:
                                    objSqlHelper.AddParameterToSQLCommand("@" + field.Name, SqlDbType.DateTime, CommonUtils.GetDataDateTime(propertyInfo.GetValue(obj, null)));
                                    break;
                                default:
                                    objSqlHelper.AddParameterToSQLCommand("@" + field.Name, SqlDbType.VarChar, CommonUtils.GetDataString(propertyInfo.GetValue(obj, null)));
                                    break;
                            }
                        }
                        else
                        {
                            KeyName = field.Name;
                            KeyValue = CommonUtils.GetDataInt(propertyInfo.GetValue(obj, null));
                            objSqlHelper.AddParameterToSQLCommand("@" + KeyName, SqlDbType.Int, KeyValue);
                        }

                    }
                }

                foreach (DataEntityAttribute attr in type.GetCustomAttributes(typeof(DataEntityAttribute), true))
                {
                    Tablename = attr.TableName; 
                }

                string fields = string.Format(SQL.ToString(), "");
                fields = fields.Remove(fields.Length - 1);
                sql = string.Format("UPDATE {0} SET {1} WHERE {2}=@{3};  ", Tablename, fields, KeyName, KeyName);

                Log.Insert(sql);
                //HttpContext.Current.Response.Write("<br>" + sql);

                objSqlHelper.GetExecuteNonQueryByCommand(sql);

            }
            catch (Exception ex)
            {
                Log.Insert(string.Format("GENERICDAL EXCEPTION: {0} sql:{1}  StackTrace:{2}"
                    , ex.Message
                    , sql
                    ,  ex.StackTrace)) ;
            }
            finally
            {
                objSqlHelper.Dispose();
            }
            objSqlHelper.Dispose();
            ResetCache(type);

        }

        public static void Delete(T obj)
        {
            SqlHelper objSqlHelper = new SqlHelper();
            string sql = "";
            string Tablename = "";
            string KeyName = "";
            int KeyValue = 0;
            Type type = typeof(T);
            StringBuilder SQL = new StringBuilder();
            try
            {
                foreach (PropertyInfo field in obj.GetType().GetProperties())
                {
                    foreach (DatafieldAttribute attr in field.GetCustomAttributes(typeof(DatafieldAttribute), true))
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(attr.FieldName);
                        if (attr.IsPrimary)
                        {
                            KeyName = field.Name;
                            KeyValue = CommonUtils.GetDataInt(propertyInfo.GetValue(obj, null));
                            objSqlHelper.AddParameterToSQLCommand("@" + KeyName, SqlDbType.Int, KeyValue);
                        }
                    }
                }

                foreach (DataEntityAttribute attr in type.GetCustomAttributes(typeof(DataEntityAttribute), true))
                {
                    Tablename = attr.TableName;
                }

                sql = string.Format("DELETE FROM {0} WHERE {1}=@{1};  ", Tablename, KeyName);

                //HttpContext.Current.Response.Write("<br>" + sql); 
                objSqlHelper.GetExecuteNonQueryByCommand(sql);

            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("<br>" + sql);
                HttpContext.Current.Response.Write("<br>" + ex.Message + " " + ex.StackTrace);

            }
            finally
            {
                objSqlHelper.Dispose();
            }
            objSqlHelper.Dispose();
            ResetCache(type);
        }
        public static void ResetCache(Type type)
        {
            SiteCache.Remove(string.Format(CacheString, type.ToString()));
        }

        public static List<T> GetAll()
        {

            StringBuilder HTML = new StringBuilder();
            Type type = typeof(T);
            string CacheKey = type.ToString();
            //HTML.AppendFormat("type {0}<br>", type);
            string Tablename = "";
             
            foreach (DataEntityAttribute attr in type.GetCustomAttributes(typeof(DataEntityAttribute), true))
            {
                Tablename = attr.TableName; 
            }
            string sql = string.Format("Select * From {0} ", Tablename);
            //HTML.AppendFormat("TABLENAME {0}<br>", sql);

            DataSet objDataSet = SiteCache.GetDataSet(string.Format(CacheString, type.ToString()));
            if (objDataSet == null)
            {
                //HttpContext.Current.Response.Write("<br>" + string.Format(CacheString, type.ToString()));
                SqlHelper objSqlHelper = new SqlHelper();
                objDataSet = objSqlHelper.GetDatasetByCommand(sql);
                objSqlHelper.Dispose();
                SiteCache.Add(string.Format(CacheString, type.ToString()), objDataSet);
            }

            /*
             LOAD LIST
             */
            T instance = (T)Activator.CreateInstance(type);

            Dictionary<string, IEnumerable<Attribute>> DataFieldAttCache = new Dictionary<string, IEnumerable<Attribute>>();
            PropertyInfo[] properties = instance.GetType().GetProperties();

            List<T> objList = new List<T>();

            foreach (DataRow oDataRow in objDataSet.Tables[0].Rows)
            {
                instance = (T)Activator.CreateInstance(type);
                foreach (PropertyInfo propertyInfo in properties)
                {

                    IEnumerable<Attribute> DatafieldAttributes;
                    if (!DataFieldAttCache.ContainsKey(propertyInfo.Name))
                    {
                        var attributes = (IEnumerable<Attribute>)propertyInfo.GetCustomAttributes(typeof(DatafieldAttribute), false);
                        DataFieldAttCache.Add(propertyInfo.Name, attributes);
                        DatafieldAttributes = DataFieldAttCache[propertyInfo.Name];
                    }
                    DatafieldAttributes = (IEnumerable<Attribute>)DataFieldAttCache[propertyInfo.Name];
                    foreach (DatafieldAttribute attr in DatafieldAttributes)
                    {
                        DataColumn column = oDataRow.Table.Columns[attr.FieldName];
                        if (column!= null)
                        {
                            switch (column.DataType.ToString())
                            {
                                case "System.Int32":
                                    propertyInfo.SetValue(instance, CommonUtils.GetDataInt(oDataRow[column.ColumnName]), null);
                                    break;
                                case "System.DateTime":
                                    propertyInfo.SetValue(instance, CommonUtils.GetDataDateTime(oDataRow[column.ColumnName]), null);
                                    break;
                                case "System.Boolean":
                                    propertyInfo.SetValue(instance, CommonUtils.GetDataBoolian(oDataRow[column.ColumnName]), null);
                                    break;
                                case "System.Decimal":
                                    object val = CommonUtils.GetDataString(oDataRow[column.ColumnName]);
                                    propertyInfo.SetValue(instance, Convert.ChangeType(val, propertyInfo.PropertyType), null);
                                    break;
                                default:
                                    propertyInfo.SetValue(instance, CommonUtils.GetDataString(oDataRow[column.ColumnName]), null);
                                    break;
                            } 
                        }

                    }
                }
                objList.Add(instance);
            }

            /*  LOAD LIST  */

 
            return objList;
        }


    }// End Class 
}
