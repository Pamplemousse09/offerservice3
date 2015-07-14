using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Data;
using System.Configuration;

namespace Kikai.BL
{
    public class DataUtils
    {

        public List<T> GetList<T>(string query, params object[] parameterValues) where T : class
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KikaiDB"].ConnectionString))
                {
                    return conn.Query<T>(query, GenerateParameters(parameterValues)).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public int ExcecuteCommand(string query, params object[] parameterValues)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KikaiDB"].ConnectionString))
                {
                    return conn.Execute(query, GenerateParameters(parameterValues));
                }
            }
            catch
            {
                throw;
            }
        }
        /*
        protected SqlParameter[] CreateParameters(object[] parameterValues)
        {
            SqlParameter[] parameters = new SqlParameter[parameterValues.Count()];
            if (parameterValues != null)
            {
                int index = 1;
                foreach (object paramValue in parameterValues)
                {
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@P" + index;
                    if (paramValue == null)
                        param.Value = DBNull.Value;
                    else
                        param.Value = paramValue;
                    parameters[index - 1] = param;
                    index++;

                }
            }
            return parameters;
        }
        */
        protected DynamicParameters GenerateParameters(object[] parameterValues)
        {
            DynamicParameters parameters = new DynamicParameters();
            try
            {
                if (parameterValues != null)
                {
                    int index = 1;
                    foreach (object paramValue in parameterValues)
                    {
                        if (paramValue == null)
                            parameters.Add("@P" + index, null);
                        else
                            parameters.Add("@P" + index, paramValue);
                        index++;
                    }
                }
            }
            catch
            {
                throw;
            }
            return parameters;
        }
    }
}
