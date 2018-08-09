using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using PEW_API.Util;
using PEW_API.Filters;
using PEW_API.Models;
using PEW_API.Models.Util;
using System.Dynamic;

namespace PEW_API.Util
{
    public class PEW_Utils
    {
        public DataSet GetQueryDataSet(string queryString, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(queryString, connection);
            
                DataSet dataset = new DataSet();

                adapter.Fill(dataset);

                return dataset;
            }
        }

        public string RunStoredProcedure(SqlCommand cmd, string storedProcedure, string connectionString)
        {
            string JSON = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //  Stored procedure name
                cmd.CommandText = storedProcedure;
                // set it to stored proc
                cmd.CommandType = CommandType.StoredProcedure;
                // add connection
                cmd.Connection = connection;

                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    JSON = JsonConvert.SerializeObject(dt, Formatting.Indented);
                   
                    return JSON;
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        public string RunStoredProcedureOutputs(SqlCommand cmd, string storedProcedure, string connectionString)
        {
            string JSON = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //  Stored procedure name
                cmd.CommandText = storedProcedure;
                // set it to stored proc
                cmd.CommandType = CommandType.StoredProcedure;
                // add connection
                cmd.Connection = connection;

                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Output> outputs = new List<Output>();
                    

                    foreach(SqlParameter p in cmd.Parameters)
                    {
                        if(p.Direction == ParameterDirection.Output)
                        {
                            Output output = new Output(p.ParameterName, p.Value.ToString());
                           
                            outputs.Add(output);
                        }
                    }

                    JSON = JsonOutputList(outputs);

                    return JSON;
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        public string JsonOutputList(List<Output> outputList)
        {
            string jsonString = "[ ";


            foreach(Output o in outputList)
            {
                jsonString += "{ \"" + o.Name.Substring(1) + "\" : \"" + o.Value + "\"}, ";
            }

            jsonString = jsonString.Substring(0, jsonString.Length - 2);

            jsonString += " ]";

            return jsonString;
        }
    }
}