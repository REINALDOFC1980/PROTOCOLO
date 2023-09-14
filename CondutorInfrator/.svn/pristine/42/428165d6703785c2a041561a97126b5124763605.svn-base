using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Configuration;
using System.Web;

namespace CondutorInfrator.Rpt_class
{
    public class CrystalReport
    {

        string ServerName = ConfigurationManager.AppSettings["rpt_servername"];
        string DatabaseName = ConfigurationManager.AppSettings["rpt_databasename"];
        string UserName = ConfigurationManager.AppSettings["rpt_username"];
        string Password = ConfigurationManager.AppSettings["rpt_password"];


        public void Formulario_Defesa(string numeroait, string peticao)
        {
            string rpt = ConfigurationManager.AppSettings["rpt_Requerimento_Defesa"];
            EmitirRPT_defesa(numeroait, peticao, rpt);
        }

        public void Formulario_JARI(string numeroait, string peticao)
        {
            string rpt = ConfigurationManager.AppSettings["rpt_Requerimento_JARI"];
            EmitirRPT_defesa(numeroait, peticao, rpt);
        }


        public void Formulario_NAI(string numeroait)
        {
            string rpt = ConfigurationManager.AppSettings["rpt_pathdatweb"];
            EmitirRPT_Condutor(numeroait, rpt);
        }


        public void EmitirRPT_defesa(string vlobusca, string vlobusca2, string rpt)
        {

            string rptfile = rpt;
            try
            {
                using (ReportDocument report = new ReportDocument())
                {
                    try
                    {
                        report.Load(rptfile);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        report.DataSourceConnections[0].SetConnection(ServerName, DatabaseName, UserName, Password);
                    }
                    catch (Exception)
                    {
                    }

                    report.ParameterFields[0].CurrentValues.Clear();
                    report.ParameterFields[0].CurrentValues.AddValue(vlobusca);
                    report.ParameterFields[1].CurrentValues.Clear();
                    report.ParameterFields[1].CurrentValues.AddValue(vlobusca2);

                    ExportOptions exportOptions = new ExportOptions();
                    exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;

                    HttpContext.Current.Response.Buffer = false;
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.ClearHeaders();

                    report.ExportToHttpResponse(exportOptions, HttpContext.Current.Response, false, "Report");
                }

            }
            catch (Exception) { }
        }




        public void EmitirRPT_Condutor(string vlobusca, string rpt)
        {

            string rptfile = rpt;
            try
            {
                using (ReportDocument report = new ReportDocument())
                {
                    try
                    {
                        report.Load(rptfile);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        report.DataSourceConnections[0].SetConnection(ServerName, DatabaseName, UserName, Password);
                    }
                    catch (Exception)
                    {
                    }

                    report.ParameterFields[0].CurrentValues.Clear();
                    report.ParameterFields[0].CurrentValues.AddValue(vlobusca);

                    ExportOptions exportOptions = new ExportOptions();
                    exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;

                    HttpContext.Current.Response.Buffer = false;
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.ClearHeaders();


                    report.ExportToHttpResponse(exportOptions, HttpContext.Current.Response, false, "Report");
                }

            }
            catch (Exception) { }
        }

    }


}





