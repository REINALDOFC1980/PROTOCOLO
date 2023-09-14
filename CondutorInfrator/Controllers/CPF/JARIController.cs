using CondutorInfrator.Models;
using CondutorInfrator.Rpt_class;
using CondutorInfrator.Util;
using Dapper;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using Microsoft.SqlServer.Server;

namespace CondutorInfrator.Controllers
{
    public class JARIController : Controller
    {
   

        private static string connectionString = ConfigurationManager.ConnectionStrings["CONEXAO_APCI"].ToString();

        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["vlo_apijari"]);
        HttpClient client;

        public JARIController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }

        Funcao funcao = new Funcao();
        EmailController rotinaemail = new EmailController();

        //!!!! add em uma unica controle
        public ActionResult Download(string id, string ext, string view, string pro_numero)
        {
            var contentType = "";

            var caminhoArquivo = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + "\\" + ext;
            var extensao = Path.GetExtension(caminhoArquivo);
            var nomeArquivoV = ext;

            if (extensao.Equals(".pdf") || extensao.Equals(".PDF"))
                contentType = "application/pdf";
            if (extensao.Equals(".jpeg") || extensao.Equals(".JPEG"))
                contentType = "image/jpeg";

            if (System.IO.File.Exists(caminhoArquivo))
            {
                Response.AppendHeader("target", "_blank");
                return File(caminhoArquivo, contentType);
            }
            else
            {
                return RedirectToAction("SituacaoProcesso_jari", "JARI", new { pro_numero });
            }
        }


        public ActionResult ConfirmacaoDados_JARI(string mut_ait)
        {
            var _ptr_cpf_cnpj = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
            try
            {
                if (funcao.StatusProcesso(mut_ait, "8") != 0)
                    return RedirectToAction("processosabertos", "processo");

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@VloCampo", "Proprietario");
                parameters.Add("@VloBusca1", _ptr_cpf_cnpj);
                parameters.Add("@VloBusca2", mut_ait);
                parameters.Add("@VloBusca3", "");
                ProcessoViewModel VCM = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ProcessoViewModel>();

                return View(VCM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("login", "LogOut");
            }
        }

        [HttpPost]
        public JsonResult ProcGerarProcesso_JARI()
        {
            try
            {
                // gerando o processo
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_CPF_CNPJ", Request.Params["Ptr_CPF_CNPJ"]);
                parameters.Add("@Vec_Placa", Request.Params["Vec_Placa"]);
                parameters.Add("@Vec_RENAVAN", Request.Params["Vec_RENAVAN"]);
                parameters.Add("@Vec_Ano", Request.Params["Vec_Ano"]);
                parameters.Add("@Vec_Marca", Request.Params["Vec_Marca"]);
                parameters.Add("@Mut_AIT", Request.Params["Mut_AIT"]);
                parameters.Add("@Mut_CodigoInfracao", Request.Params["Mut_CodigoInfracao"]);
                parameters.Add("@Mut_DescricaoInfracao", Request.Params["Mut_DescricaoInfracao"]);
                parameters.Add("@Mut_DataInfracao", Request.Params["Mut_DataInfracao"]);
                parameters.Add("@Ptr_CPFRepreLegal", Request.Params["Ptr_CPFRepreLegal"]);
                parameters.Add("@Ptr_NomeRepreLegal", Request.Params["Ptr_NomeRepreLegal"]);


                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Gerar_Processo_JARI_Dapper", parameters);

                ////Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", Request.Params["Ptr_CPF_CNPJ"]);

                parameters_.Add("@VloBusca2", Request.Params["Mut_AIT"]);
                parameters_.Add("@VloBusca3", "8");

                var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                // caso nao exista emitir um erro
                if (processoViewModel == null)
                    return Json(new { error = true }, JsonRequestBehavior.AllowGet);

                var Pro_Numero = processoViewModel.Pro_Numero;
                var Mut_ait = processoViewModel.Mut_AIT;

                return Json(new { redirectUrl = Url.Action("anexadocumento_jari", "jari"), isRedirect = true, pro_Numero = Pro_Numero, mut_ait = Mut_ait }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AnexaDocumento_JARI(string pro_Numero, string mut_ait)
        {
            try
            {
                //situacao caso o usuario click em voltar e evitar anexar duas vezes
                if (funcao.StatusProcesso(pro_Numero, "8") != 13 && funcao.StatusProcesso(pro_Numero, "8") != 21)
                    return RedirectToAction("processosabertos", "processo");


                //verificando se existe processo de condutor aberto para o AIT
                if (funcao.StatusProcesso(mut_ait, "1") == 4)
                    ViewData["ProcessoExistente"] = "Sim";
                else
                    ViewData["ProcessoExistente"] = "Nao";

                //Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", pro_Numero);
                parameters_.Add("@VloBusca2", "");
                parameters_.Add("@VloBusca3", "8");

                ProcessoViewModel processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                if (processoViewModel == null || processoViewModel.HisPro_StsPro_Id != 13 && processoViewModel.HisPro_StsPro_Id != 21)
                    return RedirectToAction("processosabertos", "processo");

                return View(processoViewModel);

            }
            catch (Exception)
            {
                return RedirectToAction("LogOut", "autenticacao");
            }

        }

        public ActionResult SituacaoProcesso_jari(string pro_numero)
        {


            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "8");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            processoViewModel.Anexos = funcao.ObterAnexosPorDemanda(pro_numero);

            if (processoViewModel == null)
                return RedirectToAction("processosabertos", "processo");


            return View(processoViewModel);
        }

        [HttpPost]
        public JsonResult AnalisarAIT()//usada apenas na finalização
        {

            //VERIFICANDO ERROS NO AIT
            var pro_numero = Request.Params["Pro_Numero"];
            var ait_numero = Request.Params["Mut_AIT"];

            var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
            var mensagem_erro = "";
            var id = "";
            var Identificacao = "";

            var ErroModel = funcao.AnalisarAIT(ait_numero, CPF_Requerente, "Stb_AnalisarAIT_JARI_CI_Dapper");

            if (ErroModel != null)
            {
                id = ErroModel.ID;
                mensagem_erro = ErroModel.resposta;
                Identificacao = ErroModel.NIdentificacao;

                //  if (ErroModel.ID == "JA_Erro")
                //    return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult VisualizarAnexo(string pro_numero, string Restricao, string tempestivo)
        {


            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "8");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();
            processoViewModel.Restricao = Restricao;
            processoViewModel.tempestivo = tempestivo;

            processoViewModel.Anexos = funcao.ObterAnexosPorDemanda(pro_numero);

            if (processoViewModel == null)
                return RedirectToAction("processosabertos", "processo");


            return View(processoViewModel);
        }

        [HttpPost]
        public JsonResult FinalizarAberturaJARI(HttpPostedFileBase[] COMPROBATORIOS)
        {
            try
            {

                var pro_numero = Request.Params["Pro_Numero"];
                var ait_numero = Request.Params["Mut_AIT"];

                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

                //Deleta caso tenha na base
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_Codigo", pro_numero);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Anexo_Delete_Dapper", parameters);

                var Restricao = 1;
                var NomeDoc = "";
                var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;
                var path = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + "\\";
                var pathComprobatorio = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + "\\COMPROBATORIOS\\";


                //craiado as pastas
                funcao.CriarPasta(path);
                funcao.CriarPasta(pathComprobatorio);
                //funcao.CriarPasta(pathTemp);

                HttpFileCollectionBase files = Request.Files;

                //Upload das img
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string Nome = Request.Files.AllKeys[i];

                    //upload  pdf
                    if (file.ContentType == "application/pdf")
                    {
                        if (Nome == "COMPROBATORIOS")
                            NomeDoc = i + Nome + "_" + pro_numero + ".PDF";
                        else
                            NomeDoc = Nome + "_" + pro_numero + ".PDF";

                        file.SaveAs(path + NomeDoc);
                     
                        //verifica se salvou na pasta
                        if (funcao.ArquivoExistente(path + NomeDoc) == false)
                        {
                            return Json(new { img = true });
                        }

                        //--Convertendo PDF em IMG, salvando no banco e deletando a pasta TEMP
                        ConvertPDFtoIMG convertPDFtoIMG = new ConvertPDFtoIMG();

                        var retorno = convertPDFtoIMG.ConvertPdfToImage(NomeDoc, pro_numero, Nome);
                        if (retorno == 0)
                        return Json(new { formatoimg = false, imagemerro = NomeDoc }, JsonRequestBehavior.AllowGet);

                    
                        //salvado nas tabelas anexo e historico
                        funcao.AnexoArquivo_banco(pro_numero, Nome, NomeDoc);

                    }
                    
                    else
                    {
                        if (Nome == "FORMULARIO_INDICACAO" || Nome == "DOC_REQUERENTE")
                            Restricao = 6; //virificar se existe algum anexo em branco 
                    }
                }

                var tempestivo = "true";
                var ErroModel = funcao.AnalisarAIT(ait_numero, CPF_Requerente, "Stb_AnalisarAIT_JARI_CI_Dapper");

                if (ErroModel.ID == "JA_Erro_1")
                    tempestivo = "false";


                return Json(new { redirectUrl = Url.Action("VisualizarAnexo", "jari"), isRedirect = true, pro_numero,  Restricao , tempestivo }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                var erro = ex;
                rotinaemail.RotinhaEmail("reinaldo@gruporecursos.com.br", "", "Erro_Sistema", erro.ToString(), "", "", "","","");
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RealizarAbertura()
        {
            string Restricao = Request.Params["Restricao"];
            string tempestivo = Request.Params["tempestivo"];
            string pro_numero = Request.Params["Pro_Numero"];

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                using (SqlTransaction tran = sqlCon.BeginTransaction())
                {
                    try
                    {
                        DynamicParameters parameters1 = new DynamicParameters();
                        parameters1.Add("@pro_Numero", pro_numero);
                        parameters1.Add("@Req_TipoEmpresa", "");
                        parameters1.Add("@Prt_restricao", Restricao);
                        parameters1.Add("@NumeroProtocoloWeb", dbType: DbType.String, direction: ParameterDirection.Output, size: 20);

                        sqlCon.Execute("STb_Integracao_Protocolo_JARI_Dapper_api", parameters1, tran, commandType: CommandType.StoredProcedure);
                        var NumeroProtocolo = parameters1.Get<string>("NumeroProtocoloWeb");


                        //ação API
                        JariSipModel sipModel = new JariSipModel();

                        sipModel.recAitNumero = Request.Params["Mut_AIT"];
                        sipModel.recJariProcesso = NumeroProtocolo;
                        sipModel.recJariDataabertura = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");    ///  Convert.ToDateTime(datas).ToString("yyyy-MM-dd");
                        sipModel.recJariUsuariocadastro = "WEB";
                        sipModel.aplicarEfeitoSuspensivo = Convert.ToBoolean(tempestivo);//corrgir verificar a data

                        string data = JsonConvert.SerializeObject(sipModel);
                        StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = client.PostAsync(client.BaseAddress, content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            tran.Commit();
                            return Json(new { redirectUrl = Url.Action("situacaoprocesso_jari", "jari"), isRedirect = true, pro_numero = pro_numero }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            tran.Rollback();
                            return Json(new { error = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json(new { error = true });


                    }
                }

            }
        }


        public ActionResult ImprimirFormulario(string numeroait, string peticao)
        {
            CrystalReport crystalR = new CrystalReport();
            crystalR.Formulario_JARI(numeroait, peticao);

            return View();
        }

    }
}