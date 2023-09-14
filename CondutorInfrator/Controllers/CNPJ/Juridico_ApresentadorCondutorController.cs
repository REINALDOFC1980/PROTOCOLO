using CondutorInfrator.Models;
using CondutorInfrator.Rpt_class;
using CondutorInfrator.Util;
using Dapper;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CondutorInfrator.Controllers.CNPJ
{


    public class Juridico_ApresentacaoCondutorController : Controller
    {
        Funcao funcao = new Funcao();
        private static ProcessoViewModel global_processoview = new ProcessoViewModel();


        /// <summary>
        /// BUSCAR CONDUTOR
        /// </summary>
        //pegando os dados do proprietario
        public ActionResult Buc_PJCondutorInfrator(string mut_ait)
        {
            var cpf_cnpj_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Proprietario");
            parameters.Add("@VloBusca1", cpf_cnpj_logado);
            parameters.Add("@VloBusca2", mut_ait);
            parameters.Add("@VloBusca3", "");
            ProcessoViewModel processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ProcessoViewModel>();

            return View(processoViewModel);
        }

        //Pesquisar condutor
        [HttpPost]
        public JsonResult PesquisarCondutor(string _cpf, string _ait)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@VloCampo", "Condutor");
                parameters.Add("@VloBusca1", _cpf);
                parameters.Add("@VloBusca2", _ait);
                parameters.Add("@VloBusca3", "");
                ProcessoViewModel VCM = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ProcessoViewModel>();

                return Json(new { VCM }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { VCM = ex.Message, error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// TELA CONFIRMAR DADOS
        /// </summary>
        [HttpPost]
        public JsonResult ProcConfirmacaoDados(ProcessoViewModel processoView)
        {
            try
            {
                global_processoview = processoView;
                return Json(new { redirectUrl = Url.Action("confirmacaodados", "Juridico_ApresentacaoCondutor"), isRedirect = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ConfirmacaoDados()
        {
            try
            {    //verifica se o AIT ja foi feito processo, em caso de confirmar e o usuario volta para pagina
                if (funcao.StatusProcesso(global_processoview.Mut_AIT, "1") != 0)
                    return RedirectToAction("processosabertos", "processo");

                //buscando os dados para abrir a pagina
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@VloCampo", "ConfirmarDados");
                parameters.Add("@VloBusca1", global_processoview.Ptr_CPF_CNPJ);
                parameters.Add("@VloBusca2", global_processoview.Mut_AIT);
                parameters.Add("@VloBusca3", global_processoview.Con_CPF);
                ProcessoViewModel VCM = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ProcessoViewModel>();

                VCM.Con_ConfirmarAceite = global_processoview.Con_ConfirmarAceite;
                VCM.Con_Cadastrado = global_processoview.Con_Cadastrado;
              ///  VCM.Con_UF_CNH = global_processoview.Con_UF_CNH;

                if (global_processoview.Con_Cadastrado == "NÃO")
                {
                    VCM.Con_CPF = global_processoview.Con_CPF;
                    VCM.Con_Nome = global_processoview.Con_Nome;
                    VCM.Con_RegistroCNH = global_processoview.Con_RegistroCNH;
                    VCM.Con_UF_CNH = global_processoview.Con_UF_CNH;
                    VCM.Con_CNHValidade = global_processoview.Con_CNHValidade;
                    VCM.Con_Telefone = global_processoview.Con_Telefone;
                    VCM.Con_Estrangeiro = global_processoview.Con_Estrangeiro;

                    VCM.Con_EndPro_CEP = global_processoview.Con_EndPro_CEP;
                    VCM.Con_EndPro_Logradouro = global_processoview.Con_EndPro_Logradouro;
                    VCM.Con_EndPro_Bairro = global_processoview.Con_EndPro_Bairro;
                    VCM.Con_EndPro_Cidade = global_processoview.Con_EndPro_Cidade;
                    VCM.Con_EndPro_Complemento = global_processoview.Con_EndPro_Complemento;
                    VCM.Con_EndPro_Estado = global_processoview.Con_EndPro_Estado;
                    VCM.Con_EndPro_Numero = global_processoview.Con_EndPro_Numero;
                    VCM.Con_EndPro_Pais = global_processoview.Con_EndPro_Pais;
                 

                }
                return View(VCM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("login", "LogOut");
            }
        }

        //GERAR O PROCESSO
        [HttpPost]
        public JsonResult procgerarprocesso(ProcessoViewModel processoView)
        {

                try
            {   // gerando o processo

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_CPF_CNPJ", Request.Params["Ptr_CPF_CNPJ"]);

                parameters.Add("@Cod_CPF_CNPJ", Request.Params["Con_CPF"]);

                parameters.Add("@Con_Nome", Request.Params["Con_Nome"]);
                parameters.Add("@Con_Telefone", Request.Params["Con_Telefone"]);
                parameters.Add("@Con_RegistroCNH", Request.Params["Con_RegistroCNH"]);
                parameters.Add("@Con_CNHValidade", Request.Params["Con_CNHValidade"]);
                parameters.Add("@Con_UF_CNH", Request.Params["Con_UF_CNH"]);
                

                parameters.Add("@Con_RG", Request.Params["Con_RG"]);
                parameters.Add("@Con_Email", Request.Params["Con_Email"]);
                parameters.Add("@Con_FotoCNH", Request.Params["Con_FotoCNH"]);
                parameters.Add("@Con_Estrangeiro", Request.Params["Con_Estrangeiro"]);
                parameters.Add("@Con_Cadastrado", Request.Params["Con_Cadastrado"]);
                parameters.Add("@Con_ConfirmarAceite", Request.Params["Con_ConfirmarAceite"]);

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


                //Não cadastrado
                parameters.Add("@Con_EndPro_CEP",         Request.Params["Con_EndPro_CEP"]);
                parameters.Add("@Con_EndPro_Logradouro",  Request.Params["Con_EndPro_Logradouro"]);
                parameters.Add("@Con_EndPro_Bairro",      Request.Params["Con_EndPro_Bairro"]);
                parameters.Add("@Con_EndPro_Cidade",      Request.Params["Con_EndPro_Cidade"]);
                parameters.Add("@Con_EndPro_Complemento", Request.Params["Con_EndPro_Complemento"]);
                parameters.Add("@Con_EndPro_Estado",      Request.Params["Con_EndPro_Estado"]);
                parameters.Add("@Con_EndPro_Numero",      Request.Params["Con_EndPro_Numero"]);
                parameters.Add("@Con_EndPro_Pais",        Request.Params["Con_EndPro_Pais"]);


                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Gerar_Processo_CondutorInfrator_Juridico_Dapper", parameters);

                ////Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", Request.Params["Ptr_CPF_CNPJ"]);
                parameters_.Add("@VloBusca2", Request.Params["Mut_AIT"]);
                parameters_.Add("@VloBusca3", "1");

                var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                // caso nao exista emitir um erro
                if (processoViewModel == null)
                    return Json(new { error = true }, JsonRequestBehavior.AllowGet);

                //pegando o processo para anexar os documento
                var Pro_numero = processoViewModel.Pro_Numero;

                if (processoViewModel.Pro_Svc_Id == 1)
                    return Json(new { redirectUrl = Url.Action("anexadocumento", "Juridico_ApresentacaoCondutor"), isRedirect = true, pro_numero = Pro_numero }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { redirectUrl = Url.Action("anexadocumento_defesa", "juridico_defesaautuacao"), isRedirect = true, pro_numero = Pro_numero }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = true, mesageerro = ex }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// TELA ANEXAR DOCUMENTACAO
        /// </summary>
        //so pode ser chamado pelo Proc_GerarProcesso
        public ActionResult AnexaDocumento(string pro_numero)
        {
            try
            {
                //Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", pro_numero);
                parameters_.Add("@VloBusca2", "");
                parameters_.Add("@VloBusca3", "1");

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


        [HttpPost]
        public JsonResult FinalizarAberturaDefesa(HttpPostedFileBase[] COMPROBATORIOS)
        {
            try
            {
                var pro_numero = Request.Params["Pro_Numero"];
                var ait_numero = Request.Params["Mut_AIT"];
                var Con_cpf = Request.Params["Con_CPF"];
                var Con_ConfirmarAceite = Request.Params["Con_ConfirmarAceite"];
                var Con_Cadastrado = Request.Params["Con_Cadastrado"];
                var StsPro_Descricao = Request.Params["StsPro_Descricao"];
                var Nome = "";

                //verificando  situacao do AIT
                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                var mensagem_erro = "";
                var id = "";
                var Identificacao = "";

                var ErroModel = funcao.AnalisarAIT(ait_numero, CPF_Requerente, "Stb_AnalisarAIT_CondutorInfrator_Dapper");

                if (ErroModel != null && ErroModel.ID != "13" && ErroModel.ID != "21")
                {
                    id = ErroModel.ID;
                    mensagem_erro = ErroModel.resposta;
                    Identificacao = ErroModel.NIdentificacao;

                    if (ErroModel.ID == "CI_Erro")
                        return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }

                //Deleta caso tenha na base
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_Codigo", pro_numero);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Anexo_Delete_Dapper", parameters);

                var NomeDoc = "";
                var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;
                var path = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + pro_numero + "\\";
                var pathTemp = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo_Temp"] + pro_numero + "\\";

                if (StsPro_Descricao == "Liberado para Correção")
                {
                    var Destino = path + "\\_ErroIMG\\";

                    funcao.CriarPasta(Destino);
                    funcao.CopyArquivo(path, Destino);
                }

                //craiado as pastas
                if (StsPro_Descricao == "Pendente documentação")
                {
                    funcao.CriarPasta(path);
                    funcao.CriarPasta(pathTemp);

                }


                HttpFileCollectionBase files = Request.Files;

                //Upload das img
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    Nome = Request.Files.AllKeys[i];

                    //upload  pdf
                    if (file.ContentType == "application/pdf")
                    {
                        NomeDoc = Nome + "_" + pro_numero + ".PDF";
                       
                        file.SaveAs(path + NomeDoc);

                        //verifica se salvou na pasta
                        if (funcao.ArquivoExistente(path + NomeDoc) == false)
                        {
                            return Json(new { error = true });
                        }

                        //--Convertendo PDF em IMG, salvando no banco e deletando a pasta TEMP
                        ConvertPDFtoIMG convertPDFtoIMG = new ConvertPDFtoIMG();
                        var retorno = convertPDFtoIMG.ConvertPdfToImage(NomeDoc, pro_numero, Nome);
                        if (retorno == 0)
                            return Json(new { formatoimg = false, imagemerro = NomeDoc, id = "" }, JsonRequestBehavior.AllowGet);
                      
                        //salvado nas tabelas anexo e historico
                        funcao.AnexoArquivo_banco(pro_numero, Nome, NomeDoc);

                    }
                    else //upload  imagem
                    if (file.ContentType == "image/jpeg")
                    {
                        try
                        {
                            NomeDoc = Nome + "_" + pro_numero + ".JPEG";
                            WebImage img = new WebImage(file.InputStream);

                            if (img.ImageFormat != "jpeg")
                                return Json(new { formatoimg = false, imagemerro = funcao.NomeAnexo(Nome), id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                            else
                            {
                                if (img.Width > 800)
                                    img.Resize(680, 800);
                                //salvando a imagem na pasta
                                img.Save(path + NomeDoc);
                                //verifica se salvou na pasta
                                if (funcao.ArquivoExistente(path + NomeDoc) == false)
                                {
                                    return Json(new { error = true });
                                }

                                //Salvando imagem no banco 
                                funcao.SalvarImgbanco(path + NomeDoc, Nome, pro_numero);

                            }
                        }
                        catch (Exception)
                        {
                            return Json(new { formatoimg = false, imagemerro = funcao.NomeAnexo(Nome), id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                        }

                        //--salvado nas tabelas anexo e historico
                        funcao.AnexoArquivo_banco(pro_numero, Nome, NomeDoc);
                    }


                }

                var CaminhoFotoPerfil = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];

                // se caso for cadastrado vai buscar o foto do perfil na pasta, copiar para a nova pasta e salvar na base de dados do protocolo
                    if (Con_Cadastrado == "SIM")
                    {
                        var FotoPerfilCondutor = funcao.Buscar_IDFotoPerfil_Condutor(Con_cpf);

                        if (!Directory.Exists(path + "FOTOCNH_CONDUTOR_" + pro_numero + ".JPEG"))
                        {
                            System.IO.File.Copy(CaminhoFotoPerfil + "PERFIL_" + FotoPerfilCondutor + ".JPEG", path + "FOTOCNH_CONDUTOR_" + pro_numero + ".JPEG");
                            funcao.AnexoArquivo_banco(pro_numero, "FOTOCNH_CONDUTOR", "FOTOCNH_CONDUTOR_" + pro_numero + ".JPEG");
                        }
                    }

                ////deletando a pasta temp criada com as imagens
                //if (Directory.Exists(pathTemp))
                //    Directory.Delete(pathTemp, true);


                return Json(new { redirectUrl = Url.Action("VisualizarAnexo", "juridico_apresentacaocondutor"), isRedirect = true, pro_numero, mensagem_erro, Identificacao, id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {error = true }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult EncaminharParaCondutor(string pro_numero)
        {
            //--finalizando processo o processo
            DynamicParameters parameters1 = new DynamicParameters();
            parameters1.Add("@Pro_Codigo", pro_numero);
            DapperORM.DapperORM.ExecuteWithouReturn("Stb_Historico_Status_inserir_Juridico_Dapper", parameters1);
           
            return RedirectToAction("SituacaoProcesso", "juridico_apresentacaocondutor", new { pro_numero });
        }


        public ActionResult VisualizarAnexo(string pro_numero)
        {
            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "1");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            processoViewModel.Anexos = funcao.ObterAnexosPorDemanda(pro_numero);

            if (processoViewModel == null)
                return RedirectToAction("processosabertos", "processo");

            return View(processoViewModel);
        }


        /// <summary>
        /// TELA SITUAÇÃO
        /// </summary>
        public ActionResult SituacaoProcesso(string pro_numero)
        {
            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "1");

            var caminho  = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"];

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            processoViewModel.Anexos = funcao.ObterAnexosPorDemanda(pro_numero);

            if (processoViewModel == null)
                return RedirectToAction("processosabertos", "processo");

            return View(processoViewModel);
        }

        /// <summary>
        /// CONFIMAÇÃO DO PROCESSO
        /// </summary>
        /// 
        public ActionResult ProcConfirmacaoProcesso(string mut_ait, string pro_numero)
        {
            try
            {
                return Json(new { redirectUrl = Url.Action("confirmacaoprocesso", "ApresentacaoCondutor", new { mut_ait, pro_numero }), isRedirect = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ConfirmacaoProcesso(string pro_numero)
        {

            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "1");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            if (processoViewModel.HisPro_StsPro_Id != 1)
                return RedirectToAction("processosabertos", "processo");

            processoViewModel.Anexos = funcao.ObterAnexosPorDemanda(pro_numero);


            HttpContext.Session["NUMERO_PROCESSO"] = processoViewModel.Pro_Numero;

            ViewData["CONTRATO_LOCACAO"] = "CONTRATO_LOCACAO_" + processoViewModel.Pro_Numero;
            ViewData["ARQUIVO"] = "";

            var caminhoArquivo = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + processoViewModel.Pro_Numero + "\\" + "CONTRATO_LOCACAO_" + processoViewModel.Pro_Numero + ".PDF";
            if (!System.IO.File.Exists(caminhoArquivo))
            {
                if (processoViewModel.Ptr_NomeFantasia != "" && processoViewModel.Ptr_NomeFantasia != null)
                    ViewData["ARQUIVO"] = "erro";
            }
            return View(processoViewModel);
        }

        //!!!! add em uma unica controle
        //pegando os anexos e listando na parte inferior 
        public ActionResult Buscaranexo(string pro_numero, string anexo)
        {
            var caminho = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"];

            string curFile = caminho + pro_numero + @"\" + anexo + pro_numero + ".jpeg";

            if (System.IO.File.Exists(curFile))
            {
                return File(caminho + pro_numero + @"\" + anexo + pro_numero + ".jpeg", "image/jpeg");
            }
            else
            {
                TempData["Imagem"] = "erro";
                return File(@"~\img\semimagens.jpg", "image/jpeg");
            }
        }


        [HttpPost]
        public JsonResult AceitarProcesso(string Pro_numero, string Mut_ait)
        {
            try
            {
                //verificando  situacao do AIT


                //verificando  situacao do AIT
                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                var mensagem_erro = "";
                var id = "";
                var Identificacao = "";

                var ErroModel = funcao.AnalisarAIT(Mut_ait, CPF_Requerente, "Stb_AnalisarAIT_CondutorInfrator_Dapper");


                if (ErroModel != null)
                {
                    if (ErroModel.ID == "CI_Erro")
                    {
                        id = ErroModel.ID;
                        mensagem_erro = ErroModel.resposta;
                        Identificacao = ErroModel.NIdentificacao;
                        return Json(new { redirectUrl = Url.Action("processo", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                    }
                }

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Pro_Numero", Pro_numero);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Processo_Aceitar_Dapper", parameter);

                return Json(new { redirectUrl = Url.Action("situacaoprocesso", "juridico_apresentacaocondutor"), isRedirect = true, pro_numero = Pro_numero, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult RecusarProcesso(string NumeroProcesso)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_Numero", NumeroProcesso);

                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Processo_Recusar_Dapper", parameters);

                return Json(new { redirectUrl = Url.Action("processo", "processo"), isRedirect = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }

        }
     
        public ActionResult PdfDownload()
        {
            string TempFilePath = @"C:\Users\reinaldo\Downloads\Arquivo.pdf";

            if (System.IO.File.Exists(TempFilePath))
            {
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "testPDF.pdf",
                    Inline = false,
                };
                Response.AppendHeader("target", "_blank");
                //Response.AppendHeader("target", "_blank");
                return File(TempFilePath, "application/pdf");
            }
            else
            {
                return null;
            }
        }




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
                return RedirectToAction("situacaoprocesso", "juridico_apresentacaocondutor", new { pro_numero });
            }
        }


        public ActionResult ImprimirNAI(string numeroait)
        {
            CrystalReport crystalR = new CrystalReport();
            crystalR.Formulario_NAI(numeroait);

            return View();
        }
    }
}