using CondutorInfrator.Models;
using CondutorInfrator.Rpt_class;
using CondutorInfrator.Util;
using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml;

namespace CondutorInfrator.Controllers
{
    [AuthorizeActionFilterAtribute]
    [Authorize]
    public class DefesaAutuacaoController : Controller
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["CONEXAO_APCI"].ToString();

        Uri baseAddress = new Uri(ConfigurationManager.AppSettings["vlo_apidefesa"]);
        HttpClient client;

        public DefesaAutuacaoController()
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
                return RedirectToAction("SituacaoProcesso_Defesa", "DefesaAutuacao", new { pro_numero });
            }
        }


        public ActionResult ConfirmacaoDados_Defesa(string mut_ait)
        {
            var _ptr_cpf_cnpj = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
            try
            {
                if (funcao.StatusProcesso(mut_ait, "2") != 0)
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
        public JsonResult ProcGerarProcesso_Defesa()
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

                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Gerar_Processo_DefesaAutuacao_Dapper", parameters);

                ////Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", Request.Params["Ptr_CPF_CNPJ"]);
                parameters_.Add("@VloBusca2", Request.Params["Mut_AIT"]);
                parameters_.Add("@VloBusca3", "2");

                var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                // caso nao exista emitir um erro
                if (processoViewModel == null)
                    return Json(new { error = true }, JsonRequestBehavior.AllowGet);

                var pro_Numero = processoViewModel.Pro_Numero;
                var mut_ait = processoViewModel.Mut_AIT;

                if (processoViewModel.Pro_Svc_Id == 1)
                    return Json(new { redirectUrl = Url.Action("anexadocumento", "apresentacaocondutor"), isRedirect = true, pro_Numero, mut_ait }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { redirectUrl = Url.Action("anexadocumento_defesa", "defesaautuacao"), isRedirect = true, pro_Numero, mut_ait }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        //so pode ser chamado pelo Proc_GerarProcesso
        public ActionResult AnexaDocumento_Defesa(string pro_Numero, string mut_ait)
        {
            try
            {
                //situacao caso o usuario click em voltar e evitar anexar duas vezes
                if (funcao.StatusProcesso(pro_Numero, "2") != 13 && funcao.StatusProcesso(pro_Numero, "2") != 21)
                    return RedirectToAction("processosabertos", "processo");

                //Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", pro_Numero);
                parameters_.Add("@VloBusca2", "");
                parameters_.Add("@VloBusca3", "2");

                ProcessoViewModel processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                if (processoViewModel == null || processoViewModel.HisPro_StsPro_Id != 13 && funcao.StatusProcesso(pro_Numero, "2") != 21)
                    return RedirectToAction("processosabertos", "processo");


                //verificando se existe processo de condutor aberto para o AIT
                if (funcao.StatusProcesso(mut_ait, "1") == 4)
                    ViewData["ProcessoExistente"] = "Sim";
                else
                    ViewData["ProcessoExistente"] = "Nao";

                return View(processoViewModel);

            }
            catch (Exception)
            {
                return RedirectToAction("LogOut", "autenticacao");
            }

        }
        [HttpPost]
        public JsonResult AnalisarAIT(string ait)//usada apenas na finalização
        {

            //VERIFICANDO ERROS NO AIT

            var ait_numero = ait; // Request.Params["Mut_AIT"];

            var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
            var mensagem_erro = "";
            var id = "";
            var Identificacao = "";

            var ErroModel = funcao.AnalisarAIT(ait_numero, CPF_Requerente, "Stb_AnalisarAIT_DefesaAutuacao_CI_Dapper");

            if (ErroModel != null)
            {
                id = ErroModel.ID;
                mensagem_erro = ErroModel.resposta;
                Identificacao = ErroModel.NIdentificacao;

                // if (ErroModel.ID == "DA_Erro")
                // return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult SituacaoProcesso_Defesa(string pro_numero)
        {

            ViewData["CONTRATO_LOCACAO"] = "CONTRATO_LOCACAO_" + pro_numero;
            ViewData["ARQUIVO"] = "";

            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "2");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            processoViewModel.Anexos = funcao.ObterAnexosPorDemanda(pro_numero);

            if (processoViewModel == null)
                return RedirectToAction("processosabertos", "processo");


            return View(processoViewModel);
        }



        public ActionResult VisualizarAnexo(string pro_numero, string Restricao)
        {

            ViewData["CONTRATO_LOCACAO"] = "CONTRATO_LOCACAO_" + pro_numero;

            ViewData["ARQUIVO"] = "";

            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "2");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            processoViewModel.Restricao = Restricao;

            processoViewModel.Anexos = funcao.ObterAnexosPorDemanda(pro_numero);

            if (processoViewModel == null)
                return RedirectToAction("processosabertos", "processo");


            return View(processoViewModel);
        }


        [HttpPost]
        public JsonResult FinalizarAberturaDefesa(HttpPostedFileBase[] COMPROBATORIOS)
        {
            try
            {
                var pro_numero = Request.Params["Pro_Numero"];
                var ait_numero = Request.Params["Mut_AIT"];
                var con_CPF = Request.Params["Con_CPF"];
              

                //VERIFICANDO ERROS NO AIT
                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;


                //Deleta caso tenha na base
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_Codigo", pro_numero);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Anexo_Delete_Dapper", parameters);

                var Restricao = "1";
                var NomeDoc = "";
                var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;
                var path = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + "\\";
               // var pathTemp = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo_Temp"] + pro_numero + "\\";
                var pathComprobatorio = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + "\\COMPROBATORIOS\\";


                //craiado as pastas
                funcao.CriarPasta(path);
                funcao.CriarPasta(pathComprobatorio);
               // funcao.CriarPasta(pathTemp);

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
                        if(retorno == 0)
                        return Json(new { formatoimg = false, imagemerro = NomeDoc }, JsonRequestBehavior.AllowGet);

                        //salvado nas tabelas anexo e historico
                        funcao.AnexoArquivo_banco(pro_numero, Nome, NomeDoc);

                    }
                    else
                    {
                        if (Nome == "FORMULARIO_INDICACAO" || Nome == "DOC_REQUERENTE")
                            Restricao = "6"; //virificar se existe algum anexo em branco 
                    }
                    //ação API              
                    //var ErroModel = funcao.AnalisarAIT(ait_numero, CPF_Requerente, "Stb_AnalisarAIT_DefesaAutuacao_CI_Dapper");
                  
                }
                return Json(new { redirectUrl = Url.Action("VisualizarAnexo", "DefesaAutuacao"), isRedirect = true, pro_numero,   formatoimg = true, imagemerro = "", Restricao }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var erro = ex;
                rotinaemail.RotinhaEmail("reinaldo@gruporecursos.com.br", "", "Erro_Sistema", erro.ToString(), "", "", "","","");
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult EncaminharProcesso()
        {
            var Mut_AIT = Request.Params["Mut_AIT"];
            var pro_numero = Request.Params["pro_numero"];
            var Restricao = Request.Params["Restricao"];
            


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

                        sqlCon.Execute("STb_Integracao_Protocolo_DefesaAutuacao_Dapper_api", parameters1, tran, commandType: CommandType.StoredProcedure);
                        var NumeroProtocolo = parameters1.Get<string>("NumeroProtocoloWeb");

                        //teste sem API
                        //tran.Commit();
                        //return Json(new { redirectUrl = Url.Action("SituacaoProcesso_Defesa", "DefesaAutuacao"), isRedirect = true, pro_numero = pro_numero }, JsonRequestBehavior.AllowGet);

                        //ação API
                        DefesaSipModel defesaSipModel = new DefesaSipModel();

                        defesaSipModel.recAitNumero = Mut_AIT;
                        defesaSipModel.recDpProcesso = NumeroProtocolo;
                        defesaSipModel.recDpDataabertura = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");    ///  Convert.ToDateTime(datas).ToString("yyyy-MM-dd");
                        defesaSipModel.recDpUsuariocadastro = "WEB";

                        string data = JsonConvert.SerializeObject(defesaSipModel);
                        StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = client.PostAsync(client.BaseAddress, content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            tran.Commit();
                            return Json(new { redirectUrl = Url.Action("SituacaoProcesso_Defesa", "DefesaAutuacao"), isRedirect = true, pro_numero }, JsonRequestBehavior.AllowGet);
                        }else
                        {
                            tran.Rollback();
                            return Json(new { error = true });
                        }
                    }catch (Exception)
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
            crystalR.Formulario_Defesa(numeroait, peticao);

            return View();
        }


        public static void Convert_Img_PDF(string caminho)
        {
            //Document Doc = new Document(PageSize.LETTER, 20, 20, 20, 20);
            Document Doc = new Document(PageSize.A4, 2, 2, 2, 2);
            string PDFOutput = caminho + "COMPROBRATORIO.pdf";
            string Folder = caminho;

         //   PdfWriter writer = PdfWriter.GetInstance(Doc, new FileStream(PDFOutput, FileMode.Create, FileAccess.Write, FileShare.Read));
            Doc.Open();

            //verifianco se existe arquivo
            DirectoryInfo di = new DirectoryInfo(Folder);
            FileInfo[] TXTFiles = di.GetFiles("*.jpeg");
            if (TXTFiles.Length != 0)
            {
                foreach (string F in Directory.GetFiles(Folder, "*.jpeg"))
                {
                    if (F != null)
                    {
                        Doc.NewPage();
                        Doc.Add(new Jpeg(new Uri(new FileInfo(F).FullName)));
                    }

                }
                Doc.Close();
            }
            //apagando a pasta tenp com a imagem           

        }
    }


}