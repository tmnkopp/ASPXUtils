using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ASPXUtils
{
 
    /// <summary>
    /// Summary description for DBUtils
    /// </summary> 
        public static class DBUtils
        {
           
            public static string[] GetColumns(string dbname, string tablename)
            {
                SqlConnection objSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[dbname].ConnectionString);
                objSqlConnection.Open();

                ArrayList aColumns = new ArrayList();
                SqlCommand myCommand = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tablename + "'", objSqlConnection);
                SqlDataReader objSqlDataReader = myCommand.ExecuteReader();
                int row = 0;
                while (objSqlDataReader.Read())
                {
                    aColumns.Add(Convert.ToString(objSqlDataReader["COLUMN_NAME"].ToString()));
                    row += 1;
                }
                objSqlDataReader.Close();
                String[] _columns = (String[])aColumns.ToArray(typeof(string));
                objSqlConnection.Close();
                return _columns;
            }

            public static List<Column> GetColumnList(string dbname, string tablename)
            {
                SqlConnection objSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[dbname].ConnectionString);
                objSqlConnection.Open();
                List<Column> _columnCollection = new List<Column>();
                SqlCommand myCommand = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tablename + "'", objSqlConnection);
                SqlDataReader objSqlDataReader = myCommand.ExecuteReader();
                int row = 0;
                while (objSqlDataReader.Read())
                {
                    Column objColumn = new Column();
                    objColumn.Name = Convert.ToString(objSqlDataReader["COLUMN_NAME"].ToString());
                    objColumn.DataType = Convert.ToString(objSqlDataReader["DATA_TYPE"].ToString());
                    if (objSqlDataReader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                    {
                        objColumn.Size = Convert.ToInt32(objSqlDataReader["CHARACTER_MAXIMUM_LENGTH"]);
                    }

                    _columnCollection.Add(objColumn);
                    row += 1;
                }
                objSqlDataReader.Close();
                objSqlConnection.Close();
                return _columnCollection;
            }


            public static string[] GetTables(string dbname)
            {
                SqlConnection objSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[dbname].ConnectionString);
                objSqlConnection.Open();

                ArrayList aResult = new ArrayList();
                SqlCommand myCommand = new SqlCommand("SELECT DISTINCT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS", objSqlConnection);
                SqlDataReader objSqlDataReader = myCommand.ExecuteReader();
                int row = 0;
                while (objSqlDataReader.Read())
                {
                    aResult.Add(Convert.ToString(objSqlDataReader["TABLE_NAME"].ToString()));
                    row += 1;
                }
                objSqlDataReader.Close();
                String[] _ret = (String[])aResult.ToArray(typeof(string));
                objSqlConnection.Close();
                return _ret;
            } 

    }


    public struct Column
    {

        private string _name;
        private string _type;
        private int _size;

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string DataType
        {
            get { return _type; }
            set { _type = value; }
        }
 
    }
}
