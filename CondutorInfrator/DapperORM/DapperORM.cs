﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace CondutorInfrator.DapperORM
{
    public class DapperORM
    {


        private static string connectionString = ConfigurationManager.ConnectionStrings["CONEXAO_APCI"].ToString();



        public static void ExecuteWithouReturn(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                try
                {
                    sqlCon.Open();
                    sqlCon.Execute(procedureName, param,  commandType: CommandType.StoredProcedure);
                }
                catch (Exception)
                {

                    throw;
                }
                

            }
        }


        public static T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                try
                {
                    sqlCon.Open();
                    return (T)Convert.ChangeType(sqlCon.Execute(procedureName, param,  commandType: CommandType.StoredProcedure), typeof(T));
                }
                catch (Exception)
                {
                   
                    throw;
                }
                

            }
        }

        public static IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                
                try
                {
                    sqlCon.Open();
                    return sqlCon.Query<T>(procedureName, param, commandTimeout: 0, commandType: CommandType.StoredProcedure) ;
                }
                catch (Exception)
                {
                    throw;
                }
                

            }
        }
    }
}