using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Database
{
    public class DatabaseAccess
    {
        SqlConnection connection;

        public DatabaseAccess()
        {
            connection= new SqlConnection();
        }

        public DatabaseAccess(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        private string listToString(List<string> list, bool insertQuotes=true)
        {
            string result = "(";

            for (int i = 0; i < list.Count; i++) 
            {
                if (insertQuotes)
                {
                    result += "'" + list[i] + "'";
                }
                else
                {
                    result += list[i];
                }

                if (i == list.Count - 1) 
                {
                    result += ")";
                }
                else
                {
                    result += ", ";
                }
            }

            return result;
        }

        private string listToString(List<(string, string)> list, bool insertQuotes=true)
        {
            List<string> merged = new List<string>();

            foreach ((string a, string b) in list)
            {
                merged.Add(a + " " + b);
            }

            return listToString(merged, insertQuotes);
        }


        public void CreateTable(string tableName, List<(string, string)> fields)
        {
            SqlCommand cm;
            string query = "CREATE TABLE " + tableName + " ";

            query += listToString(fields, false);

            cm = new SqlCommand(query, connection);

            connection.Open();
            cm.ExecuteNonQuery();
            connection.Close();
        }

        public void Insert(string tableName, List<string> values)
        {
            SqlCommand cm;
            string query = "INSERT INTO " + tableName + " ";

            query += " VALUES " + listToString(values);

            cm = new SqlCommand(query, connection);

            connection.Open();
            cm.ExecuteNonQuery();
            connection.Close();

            if (tableName != "Changes")
            {
                Insert("Changes", new List<string> { tableName, "Insert", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
        }

        public string FindAll(string tableName) 
        {
            SqlCommand cm;
            SqlDataReader sdr;
            string result = "";

            string query = "SELECT * FROM " + tableName;

            connection.Open();
            cm = new SqlCommand(query, connection);

            try
            {
                sdr = cm.ExecuteReader();
            }
            catch (Exception)
            {
                return "";
            }

            while (sdr.Read())
            {
                for (int i = 0; i < sdr.FieldCount; i++)
                {
                    result += sdr.GetValue(i).ToString() + " ";
                }

                result += "\n";
            }

            connection.Close();

            return result;
        }

        public string FindEqual(string tableName, List<(string, string)> conditions)
        {
            SqlCommand cm;
            SqlDataReader sdr;
            string result = "";

            string query = "SELECT * FROM " + tableName + " WHERE ";

            foreach ((string field, string value) in conditions)
            {
                query += field + " = " + "'" + value + "'";

                if ((field, value) != conditions[conditions.Count - 1])
                {
                    query += " AND ";
                }
            }

            connection.Open();
            cm = new SqlCommand(query, connection);

            try
            {
                sdr = cm.ExecuteReader();
            }
            catch (Exception)
            {
                return "";
            }

            while (sdr.Read())
            {
                for (int i = 0; i < sdr.FieldCount; i++) 
                {
                    result += sdr.GetValue(i).ToString() + " ";
                }

                result += "\n";
            }

            connection.Close();

            return result;
        }
   
        public void Update(string tableName, List<(string, string)> values, List<(string, string)> conditions)
        {
            SqlCommand cm;
            string query = "UPDATE " + tableName + " SET ";

            foreach ((string field, string value) in values)
            {
                query += field + " = " + "'" + value + "'";

                if ((field, value) != values[values.Count - 1])
                {
                    query += ", ";
                }
            }

            query += " WHERE ";

            foreach ((string field, string value) in conditions)
            {
                query += field + " = " + "'" + value + "'";

                if ((field, value) != conditions[conditions.Count - 1])
                {
                    query += " AND ";
                }
            }

            cm = new SqlCommand(query, connection);

            connection.Open();
            cm.ExecuteNonQuery();
            connection.Close();

            if (tableName != "Changes")
            {
                Insert("Changes", new List<string> { tableName, "Update", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
        }
    }

    class Program
    {
        public static void Main()
        {

        }
    }
}
