﻿using CondutorInfrator.Models;
using CondutorInfrator.Util;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace CondutorInfrator.Controllers
{
    [AuthorizeActionFilterAtribute]
    [Authorize]
    public class ProcessoController : Controller
    {
        Funcao funcao = new Funcao();
        private static ProcessoViewModel global_processoview = new ProcessoViewModel();

        //funcao.Md5FromStringUTF8(mut_ait)
        public int ProcessoRecebidos()
        {
            //verificando se existe processo recebidos para o condutor, emite uma mensagem
            var cpf_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value; ;
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@VloCampo", "CadastroExistente");
            parameter.Add("@VloBusca1", cpf_logado);
            parameter.Add("@VloBusca2", "");

            var TotalProcesso = DapperORM.DapperORM.ReturnList<DadosExistenteModel>("STb_Pessoa_Localizar_Dapper", parameter).FirstOrDefault<DadosExistenteModel>().Processo;

            return TotalProcesso;
        }


        [HttpPost]
        public ActionResult TipoProcesso(string mut_ait, string tipopessoa, string placa)
        {
            var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@VloCampo", "Mul_AIT");
            parameters.Add("@VloBusca1", mut_ait);
            parameters.Add("@VloBusca2", tipopessoa);
            parameters.Add("@VloBusca3", CPF_Requerente);
            
            var multasModel = DapperORM.DapperORM.ReturnList<MultasModel>("STb_Multa_Localizar_Dapper", parameters).FirstOrDefault<MultasModel>();

            ViewData["tipopessoa"] = tipopessoa;

            return View(multasModel);
        }

        /// <summary>
        /// TELA BUSCAR CONSDUTOR INFRATOR
        /// </summary>
        [HttpPost]
        public JsonResult Proc_CondutorInfrator(string mut_ait, string tipopessoa)
        {
            try
            {
                //verificando  situacao do AIT
                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                var mensagem_erro = "";
                var id = "";
                var Identificacao = "";

                var ErroModel = funcao.AnalisarAIT(mut_ait, CPF_Requerente, "Stb_AnalisarAIT_CondutorInfrator_Dapper");

                if (ErroModel != null)
                {
                    id = ErroModel.ID;
                    mensagem_erro = ErroModel.resposta;
                    Identificacao = ErroModel.NIdentificacao;

                    if (ErroModel.ID == "CI_Erro")
                        return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }

                if (tipopessoa == "FÍSICA")
                {
                    if (id == "13" || id == "21")
                        return Json(new { redirectUrl = Url.Action("anexadocumento", "apresentacaocondutor"), isRedirect = true, pro_Numero = Identificacao, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("Buc_CondutorInfrator", "ApresentacaoCondutor"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (id == "13" || id == "21")
                        return Json(new { redirectUrl = Url.Action("anexadocumento", "Juridico_ApresentacaoCondutor"), isRedirect = true, pro_Numero = Identificacao, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("Buc_PJCondutorInfrator", "Juridico_ApresentacaoCondutor"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult Proc_Defesa(string mut_ait, string tipopessoa)
        {
            try
            {
                //verificando  situacao do AIT
                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                var mensagem_erro = "";
                var id = "";
                var Identificacao = "";
                var Processo_Numero = "";
                var statusProcesso = 0;

                var ErroModel_CI = funcao.AnalisarAIT(mut_ait, CPF_Requerente, "Stb_AnalisarAIT_DefesaAutuacao_CI_Dapper");

                if (ErroModel_CI != null)
                {
                    id = ErroModel_CI.ID;
                    mensagem_erro = ErroModel_CI.resposta;
                    Identificacao = ErroModel_CI.NIdentificacao;

                    if (ErroModel_CI.ID == "DA_Erro")
                        return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }

                //Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", CPF_Requerente);
                parameters_.Add("@VloBusca2", mut_ait);
                parameters_.Add("@VloBusca3", "2");

                ProcessoViewModel processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                if (processoViewModel != null)
                {
                    Processo_Numero = processoViewModel.Pro_Numero;
                    statusProcesso = processoViewModel.HisPro_StsPro_Id;
                }


                if (tipopessoa == "FÍSICA")
                {
                    if (statusProcesso == 13 || statusProcesso == 21)
                        return Json(new { redirectUrl = Url.Action("anexadocumento_defesa", "defesaautuacao", new { }), isRedirect = true, pro_Numero = Processo_Numero, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("confirmacaodados_defesa", "defesaautuacao"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (statusProcesso == 13 || statusProcesso == 21)
                        return Json(new { redirectUrl = Url.Action("anexadocumento_defesa", "juridico_defesaautuacao", new { }), isRedirect = true, id, pro_Numero = Processo_Numero, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("confirmacaodados_defesa", "juridico_defesaautuacao"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult Proc_JARI(string mut_ait, string tipopessoa)
        {
            try
            {
                //verificando  situacao do AIT
                var CPF_Requerente = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                var mensagem_erro = "";
                var id = "";
                var Identificacao = "";
                var Processo_Numero = "";
                var statusProcesso = 0;

                var ErroModel_CI = funcao.AnalisarAIT(mut_ait, CPF_Requerente, "Stb_AnalisarAIT_JARI_CI_Dapper");
                if (ErroModel_CI != null)
                {
                    id = ErroModel_CI.ID;
                    mensagem_erro = ErroModel_CI.resposta;
                    Identificacao = ErroModel_CI.NIdentificacao;

                    if (ErroModel_CI.ID == "JA_Erro")
                        return Json(new { redirectUrl = Url.Action("processosabertos", "processo"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }

                //Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", CPF_Requerente);
                parameters_.Add("@VloBusca2", mut_ait);
                parameters_.Add("@VloBusca3", "8");

                ProcessoViewModel processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                if (processoViewModel != null)
                {
                    Processo_Numero = processoViewModel.Pro_Numero;
                    statusProcesso = processoViewModel.HisPro_StsPro_Id;
                }

                if (tipopessoa == "FÍSICA")
                {
                    if (statusProcesso == 13 || statusProcesso == 21)
                        return Json(new { redirectUrl = Url.Action("anexadocumento_jari", "jari", new { }), isRedirect = true, pro_Numero = Processo_Numero, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("confirmacaodados_jari", "jari"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (statusProcesso == 13 || statusProcesso == 21)
                        return Json(new { redirectUrl = Url.Action("anexadocumento_jari", "juridico_jari", new { }), isRedirect = true, pro_Numero = Processo_Numero, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("confirmacaodados_jari", "juridico_jari"), isRedirect = true, id, mensagem_erro, Identificacao }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ProcessosAbertos()
        {

            TempData["vloTotal"] = ProcessoRecebidos();

            return View();
        }



        public JsonResult Act_ProcessoAbertos(string searchPhrase, int current = 1, int rowCount = 10)
        {
            int vloTotal = 0;
            string chave = Request.Form.AllKeys.Where(k => k.StartsWith("sort")).First();
            string ordenacao = Request[chave];
            string campo = chave.Replace("sort[", string.Empty).Replace("]", string.Empty);

            var cpf_cnpj_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageNumber", current);
            parameters.Add("@RowspPage", rowCount);
            parameters.Add("@Campo", campo);
            parameters.Add("@Ordem", ordenacao);
            parameters.Add("@cpf_cnpj", cpf_cnpj_logado);
            parameters.Add("@ValorPesquisado", searchPhrase);

            List<ProcessoViewModel> result = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_ProcessoAbertosLista_Localizar_Dapper", parameters).ToList();


            if (result.Count > 0)
                vloTotal = result[0].QtdRegistroTabela;

            return Json(new
            {
                rows = result,
                current,
                rowCount,
                total = vloTotal,
            }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// PROCESSOS RECEBIDOS
        /// </summary>
        public ActionResult Processo()
        {
            var cpf_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value; ;

            TempData["vloTotal"] = ProcessoRecebidos();

            return View();
        }

        public JsonResult Act_Processo(string searchPhrase, int current = 1, int rowCount = 10)
        {
            int vloTotal = 0;
            string chave = Request.Form.AllKeys.Where(k => k.StartsWith("sort")).First();
            string ordenacao = Request[chave];
            string campo = chave.Replace("sort[", string.Empty).Replace("]", string.Empty);

            var cpf_cnpj_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageNumber", current);
            parameters.Add("@RowspPage", rowCount);
            parameters.Add("@Campo", campo);
            parameters.Add("@Ordem", ordenacao);
            parameters.Add("@cpf_cnpj", cpf_cnpj_logado);
            parameters.Add("@ValorPesquisado", searchPhrase);

            List<ProcessoViewModel> result = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_ProcessoLista_Localizar_Dapper", parameters).ToList();

            if (result.Count != 0)
                vloTotal = result[0].QtdRegistroTabela;


            return Json(new
            {
                rows = result,
                current,
                rowCount,
                total = vloTotal,
            }, JsonRequestBehavior.AllowGet);

        }

        ///<summary>
        /// SITUACAO PROCESSO
        /// </summary>
        ///É CHAMADA NA TELA DE PROCESSO ABERTO E VERIFICA PRA QUAL VAI SER ENCAMINHADA : APRESENTAÇÃO OU AUTUAÇÃO 
        [HttpPost]
        public JsonResult ProcSituacaoProcesso(string pro_numero)
        {
            try
            {
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Pro_Numero");
                parameters_.Add("@VloBusca1", pro_numero);
                parameters_.Add("@VloBusca2", "");
                parameters_.Add("@VloBusca3", "");

                var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();


                if (processoViewModel.Ptr_TipoPessoa == "FÍSICA")
                {
                    if (processoViewModel.Pro_Svc_Id == 1)
                        return Json(new { redirectUrl = Url.Action("situacaoprocesso", "apresentacaocondutor"), isRedirect = true, pro_Numero = pro_numero }, JsonRequestBehavior.AllowGet);
                    else if (processoViewModel.Pro_Svc_Id == 2)
                        return Json(new { redirectUrl = Url.Action("situacaoprocesso_defesa", "defesaautuacao"), isRedirect = true, pro_Numero = pro_numero }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("situacaoprocesso_jari", "jari"), isRedirect = true, pro_Numero = pro_numero }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (processoViewModel.Pro_Svc_Id == 1)
                        return Json(new { redirectUrl = Url.Action("situacaoprocesso", "Juridico_ApresentacaoCondutor"), isRedirect = true, pro_Numero = pro_numero }, JsonRequestBehavior.AllowGet);
                    else if (processoViewModel.Pro_Svc_Id == 2)
                        return Json(new { redirectUrl = Url.Action("situacaoprocesso_defesa", "juridico_defesaautuacao"), isRedirect = true, pro_Numero = pro_numero }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("situacaoprocesso_jari", "juridico_jari"), isRedirect = true, pro_Numero = pro_numero }, JsonRequestBehavior.AllowGet);

                }


            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// ANEXOS 
        /// </summary>
        [HttpPost]
        public JsonResult Procanexadocumento(string pro_numero, string mut_ait, int idservico)
        {
            try
            {
                ////Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", pro_numero);
                parameters_.Add("@VloBusca2", "");
                parameters_.Add("@VloBusca3", idservico);

                var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                // caso nao exista emitir um erro
                if (processoViewModel == null)
                    return Json(new { error = true }, JsonRequestBehavior.AllowGet);

                //   var Pro_Numero = processoViewModel.Pro_Numero;
                var statusprocesso = processoViewModel.HisPro_StsPro_Id;


                if (processoViewModel.Ptr_TipoPessoa == "FÍSICA")
                {
                    if (processoViewModel.Pro_Svc_Id == 1)
                        return Json(new { redirectUrl = Url.Action("anexadocumento", "apresentacaocondutor"), isRedirect = true, pro_Numero = pro_numero, Mut_ait = mut_ait, statusprocesso }, JsonRequestBehavior.AllowGet);
                    else
                    if (processoViewModel.Pro_Svc_Id == 2)
                        return Json(new { redirectUrl = Url.Action("anexadocumento_defesa", "defesaautuacao"), isRedirect = true, pro_Numero = pro_numero, Mut_ait = mut_ait, statusprocesso }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("anexadocumento_jari", "jari"), isRedirect = true, pro_Numero = pro_numero, Mut_ait = mut_ait, statusprocesso }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (processoViewModel.Pro_Svc_Id == 1)
                        return Json(new { redirectUrl = Url.Action("anexadocumento", "Juridico_ApresentacaoCondutor"), isRedirect = true, pro_Numero = pro_numero, Mut_ait = mut_ait, statusprocesso }, JsonRequestBehavior.AllowGet);
                    else
                      if (processoViewModel.Pro_Svc_Id == 2)
                        return Json(new { redirectUrl = Url.Action("anexadocumento_defesa", "juridico_defesaautuacao"), isRedirect = true, pro_Numero = pro_numero, Mut_ait = mut_ait, statusprocesso }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { redirectUrl = Url.Action("anexadocumento_jari", "juridico_jari"), isRedirect = true, pro_Numero = pro_numero, Mut_ait = mut_ait, statusprocesso }, JsonRequestBehavior.AllowGet);


                }

            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ProcConfirmacaoProcesso(string mut_ait, string pro_numero)
        {
            try
            {
                ////Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@VloCampo", "Processo");
                parameters_.Add("@VloBusca1", pro_numero);
                parameters_.Add("@VloBusca2", "");
                parameters_.Add("@VloBusca3", 1);

                var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("STb_Processo_Localizar_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();


                if (processoViewModel.Ptr_TipoPessoa == "FÍSICA")
                    return Json(new { redirectUrl = Url.Action("confirmacaoprocesso", "ApresentacaoCondutor"), isRedirect = true, Mut_ait = mut_ait, Pro_numero = pro_numero }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { redirectUrl = Url.Action("confirmacaoprocesso", "juridico_ApresentacaoCondutor"), isRedirect = true, Mut_ait = mut_ait, Pro_numero = pro_numero }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Analisar_Condutor_Infrator(string mut_ait)
        {
            var processoexistente = "NÃO";
            try
            {
                var cpf_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value; ;
                ////Buscando o processo gerado
                DynamicParameters parameters_ = new DynamicParameters();
                parameters_.Add("@PRT_AIT", mut_ait);
                parameters_.Add("@CPF_Requerente", cpf_logado);
                var processoViewModel = DapperORM.DapperORM.ReturnList<ProcessoViewModel>("Stb_Verificar_Processo_CI_Dapper", parameters_).FirstOrDefault<ProcessoViewModel>();

                if (processoViewModel != null)
                    processoexistente = "SIM";

                return Json(new { processoexistente, mut_ait }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }















































    }
}