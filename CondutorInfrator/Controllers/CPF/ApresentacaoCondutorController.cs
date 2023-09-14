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

namespace CondutorInfrator.Controllers

{
    [AuthorizeActionFilterAtribute]
    [Authorize]
    public class ApresentacaoCondutorController : Controller
    {
        EmailController rotinaemail = new EmailController();
        Funcao funcao = new Funcao();
        private static ProcessoViewModel global_processoview = new ProcessoViewModel();

        public ActionResult Buscar_FotoPerfil_Condutor(string con_cpf)
        {
            var caminho_foto_Perfil = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];
            var FotoPerfilCondutor = funcao.Buscar_IDFotoPerfil_Condutor(con_cpf);

            if (!System.IO.File.Exists(caminho_foto_Perfil + @"\PERFIL_" + FotoPerfilCondutor + ".jpeg"))
                return File(@"~\img\bg-upload.jpg", "image/jpeg");
            else
                return File(caminho_foto_Perfil + @"\PERFIL_" + FotoPerfilCondutor + ".jpeg", "image/jpeg");
        }

        public ActionResult SituacaoProcesso(string pro_numero)
        {

            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "1");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            return View(processoViewModel);
        }

        //pegando os dados do proprietario
        public ActionResult Buc_CondutorInfrator(string mut_ait)
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
                parameters.Add("@VloCampo", "Condutor_Existente");
                parameters.Add("@VloBusca1", _cpf);
                parameters.Add("@VloBusca2", _ait);
                parameters.Add("@VloBusca3", "");
                ErroModel resultado = DapperORM.DapperORM.ReturnList<ErroModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ErroModel>();

                if(resultado.ID == "1")
                {
                    return Json(new { resultado.ID }, JsonRequestBehavior.AllowGet);
                }

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

                return Json(new { redirectUrl = Url.Action("confirmacaodados", "apresentacaocondutor"), isRedirect = true }, JsonRequestBehavior.AllowGet);

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
                if (funcao.StatusProcesso(global_processoview.Mut_AIT, "1") != 0 && 
                    funcao.StatusProcesso(global_processoview.Mut_AIT, "1") != 2 &&
                    funcao.StatusProcesso(global_processoview.Mut_AIT, "1") != 6 &&
                    funcao.StatusProcesso(global_processoview.Mut_AIT, "1") != 20)
                {
                    return RedirectToAction("processosabertos", "processo");

                }
                else
                {

                    //buscando os dados para abrir a pagina
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@VloCampo", "ConfirmarDados");
                    parameters.Add("@VloBusca1", global_processoview.Ptr_CPF_CNPJ);
                    parameters.Add("@VloBusca2", global_processoview.Mut_AIT);
                    parameters.Add("@VloBusca3", global_processoview.Con_CPF);
                    ProcessoViewModel VCM = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters).FirstOrDefault<ProcessoViewModel>();

                    VCM.Ptr_CPFRepreLegal = global_processoview.Ptr_CPFRepreLegal;
                    VCM.Ptr_NomeRepreLegal = global_processoview.Ptr_NomeRepreLegal;

                    return View(VCM);

                }
                   

            }
            catch (Exception ex)
            {
                return RedirectToAction("login", "LogOut");
            }

        }

        [HttpPost]
        public JsonResult procgerarprocesso()
        {
            try
            {

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_CPF_CNPJ", Request.Params["Ptr_CPF_CNPJ"]);
                parameters.Add("@Cod_CPF_CNPJ", Request.Params["Con_CPF"]);
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
                parameters.Add("@Con_Cadastrado", "SIM");
                parameters.Add("@Con_ConfirmarAceite", "SIM");

                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Gerar_Processo_CondutorInfrator_Dapper", parameters);

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
                    return Json(new { redirectUrl = Url.Action("anexadocumento", "apresentacaocondutor"), isRedirect = true, pro_numero = Pro_numero }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { redirectUrl = Url.Action("anexadocumento_defesa", "defesaautuacao"), isRedirect = true, pro_numero = Pro_numero }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// TELA ANEXAR DOCUMENTACAO
        /// </summary>
        //so pode ser chamado pelo Proc_GerarProcesso
        public ActionResult AnexaDocumento(string pro_Numero)
        {
            try
            {
                //Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", pro_Numero);
                parameters_.Add("@VloBusca2", "");
                parameters_.Add("@VloBusca3", "1");

               ProcessoViewModel processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

               if (processoViewModel == null)
                    return RedirectToAction("processosabertos", "processo");

                return View(processoViewModel);

            }
            catch (Exception)
            {
                return RedirectToAction("LogOut", "autenticacao");
            }

        }


        public JsonResult Analisar_FotoPerfil_SituacaoAIT(string con_cpf, string mut_ait)
        {
            try

            {

                //analisando situacao da Foto
                var FotoPerfilCondutor = funcao.Buscar_IDFotoPerfil_Condutor(con_cpf);
                var CaminhoFotoPerfil = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];
                var fotoperfil = System.IO.File.Exists(CaminhoFotoPerfil + "PERFIL_" + FotoPerfilCondutor + ".JPEG");

                return Json(new { fotoperfil }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
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

            var ErroModel = funcao.AnalisarAIT(ait_numero, CPF_Requerente, "Stb_AnalisarAIT_CondutorInfrator_Dapper");

            if (ErroModel != null)
            {
                id = ErroModel.ID;
                mensagem_erro = ErroModel.resposta;
                Identificacao = ErroModel.NIdentificacao;

                 if (ErroModel.ID == "CI_Erro")
                  return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
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


        [HttpPost]
        public JsonResult FinalizarProcesso()
        {

            try
            {
                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                var pro_numero = Request.Params["Pro_Numero"];
                var con_cpf = Request.Params["Con_CPF"];
                var mut_ait = Request.Params["Mut_AIT"];
                var StsPro_Descricao = Request.Params["StsPro_Descricao"];


                //limpa a tabela caso tenho registro do processo 
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pro_Codigo", pro_numero);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Anexo_Delete_Dapper", parameters);

                HttpFileCollectionBase files = Request.Files;

                string path = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"];
                string pasta = path + pro_numero;
                path = pasta + @"\";

                //pegando as imagens antigas e salvando na pasta erroIMG
                if (StsPro_Descricao == "Liberado para Correção")
                {
                    var Destino = path + "\\_ErroIMG\\";
                    funcao.CriarPasta(Destino);
                    funcao.CopyArquivo(path, Destino);
                }


                if (StsPro_Descricao == "Pendente documentação")
                    funcao.CriarPasta(path);


                //Upload das img
                for (int i = 0; i < files.Count; i++)
                {
                    string Nome = Request.Files.AllKeys[i];
                    HttpPostedFileBase file = files[i];

                    var NomeDoc = Nome + "_" + pro_numero + ".jpeg";

                    if (file.ContentType == "image/jpeg")
                    {
                        try
                        {
                            WebImage img = new WebImage(file.InputStream);

                            if (img.ImageFormat != "jpeg")
                            {
                                return Json(new { formatoimg = false, arq_corrompido = funcao.NomeAnexo(Nome) }, JsonRequestBehavior.AllowGet);
                            }
                            //Salvando IMG
                            if (img.Width > 1000)
                                img.Resize(1000, 1000);

                                img.Save(path + NomeDoc);

                            //verifica se salvou na pasta
                            if (funcao.ArquivoExistente(path + NomeDoc) == false)
                            {
                                return Json(new { error = true });
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { formatoimg = false, arq_corrompido = funcao.NomeAnexo(Nome) }, JsonRequestBehavior.AllowGet);
                        }

                        //salvado nas tabelas anexo e historico
                        funcao.AnexoArquivo_banco(pro_numero, Nome, NomeDoc);
                        
                        //salvando a imagem na tabela AnexoImagem
                        funcao.SalvarImgbanco(path + NomeDoc, Nome, pro_numero);
                    }
                }

                //pegando o id da foto
                var FotoPerfilCondutor = funcao.Buscar_IDFotoPerfil_Condutor(con_cpf);
                var CaminhoFotoPerfil = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];

                //salvando novamente a foto do perfil
                if (!Directory.Exists(path + "FOTOCNH_CONDUTOR_" + pro_numero + ".JPEG"))
                    System.IO.File.Copy(CaminhoFotoPerfil + "PERFIL_" + FotoPerfilCondutor + ".JPEG", path + "FOTOCNH_CONDUTOR_" + pro_numero + ".JPEG");

                //salvando na tablea Anexo e AnexoProcesso
                funcao.AnexoArquivo_banco(pro_numero, "FOTOCNH_CONDUTOR", "FOTOCNH_CONDUTOR_" + pro_numero + ".JPEG");
                
                //salvando a imagem na tabela AnexoImagem
                funcao.SalvarImgbanco(path + "FOTOCNH_CONDUTOR_" + pro_numero + ".JPEG", "PERFIL_" + FotoPerfilCondutor, pro_numero);


                return Json(new { redirectUrl = Url.Action("VisualizarAnexos", "apresentacaocondutor"), isRedirect = true, error = false, pro_numero = pro_numero }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var erro = ex;
                rotinaemail.RotinhaEmail("reinaldo@gruporecursos.com.br", "", "Erro_Sistema", erro.ToString(), "", "", "","","");
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult EncaminharParaCondutor(ProcessoViewModel processoModel)
        {
            //***ESSE BLOCO VAI PARA TELA DE VISUALIZAÇÃO

            //Mudando o status do processo
            DynamicParameters parameters_h = new DynamicParameters();
            parameters_h.Add("@Pro_Codigo", processoModel.Pro_Numero);
            parameters_h.Add("@HisPro_StsPro_Id", 1);
            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("Stb_Historico_Status_inserir_Dapper", parameters_h).FirstOrDefault<ProcessoViewModel>();


            // caso nao exista emitir um erro
            if (processoViewModel == null)
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);

            //pegando o processo para anexar os documento
            var Status = processoViewModel.status;
            if (Status != 0)
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);


            //--confirmando o processo
            var email = processoModel.Con_Email;   //Request.Params["Con_Email"];
            var nome = processoModel.Ptr_Nome;    //Request.Params["Ptr_Nome"];
            var vlo1 = processoModel.Mut_AIT;    //Request.Params["Mut_AIT"];
            var vlo2 = processoModel.Vec_Placa; //Request.Params["Vec_Placa"];
            var pro_numero = processoModel.Pro_Numero;
            // EmailController emails = new EmailController();
            rotinaemail.RotinhaEmail(email, nome, "Confirmação_Processo", vlo1, vlo2, "", "", "", "");
            
            return Json(new { redirectUrl = Url.Action("SituacaoProcesso", "apresentacaocondutor"), isRedirect = true, error = false, pro_numero }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult VisualizarAnexos(string pro_numero)
        {

            DynamicParameters parameters_ = new DynamicParameters();
            parameters_.Add("@VloCampo", "Processo");
            parameters_.Add("@VloBusca1", pro_numero);
            parameters_.Add("@VloBusca2", "");
            parameters_.Add("@VloBusca3", "1");

            var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

            return View(processoViewModel);
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
        public JsonResult AceitarProcesso(string pro_numero, string Mut_ait)
        {
            try
            {
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
                parameter.Add("@Pro_Numero", pro_numero);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Processo_Aceitar_Dapper", parameter);

                return Json(new { redirectUrl = Url.Action("situacaoprocesso", "apresentacaoCondutor"), isRedirect = true, pro_numero, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
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
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult Download(string id, string ext, string view)
        {
            string contentType = "";
            string caminhoArquivo = "";
            string nomeArquivoV = "";
            string extensao = "";


            if (HttpContext.Session["NUMERO_PROCESSO"] != null)
            {
                var numero_processo = HttpContext.Session["NUMERO_PROCESSO"].ToString();

                caminhoArquivo = ConfigurationManager.AppSettings["Vlo_CaminhoAnexo"] + numero_processo + "\\" + id + ext;
                extensao = Path.GetExtension(caminhoArquivo);
                nomeArquivoV = id + extensao;
                if (extensao.Equals(".pdf"))
                    contentType = "application/pdf";
                if (extensao.Equals(".jpg") || extensao.Equals(".jpeg"))
                    contentType = "application/image";
            }

            if (System.IO.File.Exists(caminhoArquivo))
            {
                return File(caminhoArquivo, contentType, nomeArquivoV);
            }
            else
            {
                return RedirectToAction(view, "processo");
            }
        }


        public ActionResult ImprimirNA (string numeroait)
        {
            CrystalReport crystalR = new CrystalReport();
            crystalR.Formulario_NAI (numeroait);

            return View();
        }


    }
}