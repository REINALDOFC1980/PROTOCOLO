using CondutorInfrator.Models;
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
    public class PerfilController : Controller
    {
        [Authorize(Roles = "FÍSICA")]
        public ActionResult Perfil_CPF()
        {

            var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Pes_id");
            parameters.Add("@VloBusca1", Id_user_logado);
            parameters.Add("@VloBusca2", "");

            PessoaViewModel pessoaViewModel = DapperORM.DapperORM.ReturnList<PessoaViewModel>("STb_Pessoa_Localizar_Dapper", parameters).FirstOrDefault<PessoaViewModel>();


            if (pessoaViewModel.Pes_Estrangeiro == "Sim")
                return RedirectToAction("Perfil_Estrangeiro", "perfil", pessoaViewModel);


            var Foto_Perfil = pessoaViewModel.Pes_FotoCNH;
            var caminho_foto_Perfil = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];

            if (!System.IO.File.Exists(caminho_foto_Perfil + @"\PERFIL_" + Foto_Perfil + ".jpeg"))
                ViewData["FotoPerfil"] = "Não";
            else
                ViewData["FotoPerfil"] = "Sim";

            //Pegando a última pege antes do perfil
            Session["Referrer"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();

            return View(pessoaViewModel);
        }

        public ActionResult Perfil_Estrangeiro()
        {

            var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Pes_id");
            parameters.Add("@VloBusca1", Id_user_logado);
            parameters.Add("@VloBusca2", "");

            PessoaViewModel pessoaViewModel = DapperORM.DapperORM.ReturnList<PessoaViewModel>("STb_Pessoa_Localizar_Dapper", parameters).FirstOrDefault<PessoaViewModel>();


            var Foto_Perfil = pessoaViewModel.Pes_FotoCNH;
            var caminho_foto_Perfil = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];

            if (!System.IO.File.Exists(caminho_foto_Perfil + @"\PERFIL_" + Foto_Perfil + ".jpeg"))
                ViewData["FotoPerfil"] = "Não";
            else
                ViewData["FotoPerfil"] = "Sim";

            //Pegando a última pege antes do perfil
            Session["Referrer"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();

            return View(pessoaViewModel);
        }

        [Authorize(Roles = "FÍSICA")]
        public ActionResult FotoCnhPerfil()
        {
            var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;


            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Pes_id");
            parameters.Add("@VloBusca1", Id_user_logado);
            parameters.Add("@VloBusca2", "");

            PessoaViewModel pessoaViewModel = DapperORM.DapperORM.ReturnList<PessoaViewModel>("STb_Pessoa_Localizar_Dapper", parameters).FirstOrDefault<PessoaViewModel>();


            var caminho_foto_Perfil = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];
            var Foto_Perfil = pessoaViewModel.Pes_FotoCNH;

            if (!System.IO.File.Exists(caminho_foto_Perfil + @"\PERFIL_" + Foto_Perfil + ".jpeg"))
                return File(@"~\img\bg-upload.jpg", "image/jpeg");
            else
                return File(caminho_foto_Perfil + @"\PERFIL_" + Foto_Perfil + ".jpeg", "image/jpeg");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Proc_PessoaAlterar(PessoaViewModel pessoaViewModel)
        {
            //upload da foto
            string path = ConfigurationManager.AppSettings["Vlo_CaminhoFotoPerfil"];
            var Foto_Perfil = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

            HttpFileCollectionBase files = Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                string Nome = Request.Files.AllKeys[0];
                HttpPostedFileBase file = files[0];
                var NomeDoc = Nome + Foto_Perfil + ".JPEG";

                if (file.ContentLength != 0)
                {

                    try
                    {
   
                        WebImage img = new WebImage(file.InputStream);
                        if (img.ImageFormat == "jpeg")
                        {
                            if (img.Width > 1000)
                                img.Resize(1000, 1000);
                            img.Save(path + NomeDoc);
                        }
                        else
                          return Json(new { formatoimg = false }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        return Json(new { formatoimg = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else //caso nao tenha foto manter o ja existe!
                {
                    Foto_Perfil = "";
                }

            }


            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pes_Id", pessoaViewModel.Pes_Id);
                parameters.Add("@Pes_CPF_CNPJ", pessoaViewModel.Pes_CPF_CNPJ);
                parameters.Add("@Pes_Nome", pessoaViewModel.Pes_Nome);
                parameters.Add("@Pes_NomeFantasia", pessoaViewModel.Pes_NomeFantasia);
                parameters.Add("@Pes_TipoEmpresa", pessoaViewModel.Pes_TipoEmpresa);
                parameters.Add("@Pes_RG", pessoaViewModel.Pes_RG);
                parameters.Add("@Pes_RegistroCNH", pessoaViewModel.Pes_RegistroCNH);
                parameters.Add("@Pes_CNHValidade", pessoaViewModel.Pes_CNHValidade);
                parameters.Add("@Pes_UF_CNH", pessoaViewModel.Pes_UF_CNH);

                parameters.Add("@Pes_FotoCNH", Foto_Perfil);
                parameters.Add("@Pes_Telefone", pessoaViewModel.Pes_Telefone);
                parameters.Add("@Pes_Celular", pessoaViewModel.Pes_Celular);
                parameters.Add("@Pes_Email", pessoaViewModel.Pes_Email);

                parameters.Add("@End_id", pessoaViewModel.End_id);
                parameters.Add("@End_CEP", pessoaViewModel.End_CEP);
                parameters.Add("@End_Logradouro", pessoaViewModel.End_Logradouro);
                parameters.Add("@End_Numero", pessoaViewModel.End_Numero);
                parameters.Add("@End_Bairro", pessoaViewModel.End_Bairro);
                parameters.Add("@End_Cidade", pessoaViewModel.End_Cidade);
                parameters.Add("@End_Estado", pessoaViewModel.End_Estado);
                parameters.Add("@End_Complemento", pessoaViewModel.End_Complemento);
                parameters.Add("@End_Pais", pessoaViewModel.End_Pais);
                parameters.Add("@Usu_Senha", pessoaViewModel.Usu_Senha);


                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Pessoa_Alterar_Dapper", parameters);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }

        }


        //OK Homologado
        [AllowAnonymous]
        [HttpPost]
        public JsonResult EmailExistes(string email)
        {
            try
            {
                var cpf_cnpj = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@VloCampo", "CadastroExistente");
                parameters.Add("@VloBusca1", email);
                parameters.Add("@VloBusca2", cpf_cnpj);


                var Resultado = DapperORM.DapperORM.ReturnList<DadosExistenteModel>("STb_Pessoa_Localizar_Dapper", parameters).FirstOrDefault<DadosExistenteModel>().EmailPessoa;

                return Json(new { email = Resultado, erro = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  erro = true }, JsonRequestBehavior.AllowGet);
            }
        }



        //PERFIL CNPJ !!dar um melhorada 
        [Authorize(Roles = "JURÍDICO")]
        public ActionResult Perfil_cnpj()
        {

            var CPF_CNPJ_LOGADO = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
            var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;

            Session["Referrer"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();

            ViewData["DOC"] = "DOC_SOCIOS";
            ViewData["CON"] = "CONTRATO_SOCIAL";
            ViewData["TERMO"] = "TERMO_RESPONSABILIDADE";
            ViewData["PRO"] = "PROCURACAO";
            ViewData["DOC_PROCURACAO"] = "DOC_PROCURACAO";
            ViewData["ARQUIVO"] = "";

            //VERIFICANDO SE TODOS OS ANEXOS NAO OBRIGATORIOS EXISTEM
            string[] anexo = new string[2];
            anexo[0] = "PROCURACAO.PDF";
            anexo[1] = "DOC_PROCURACAO.PDF";

            foreach (string texto in anexo)
            {
                var caminhoArquivo = ConfigurationManager.AppSettings["Vlo_CaminhoAnexoCNPJ"] + CPF_CNPJ_LOGADO + "\\" + texto;
                if (!System.IO.File.Exists(caminhoArquivo))
                {

                    switch (texto)
                    {
                        case "PROCURACAO.PDF":
                            ViewData["ARQUIVO"] = "PROCURACAO";
                            break;

                    };
                }
            }

            //verificando se a procuração existe ja que não é obrigatório
            var PROCURACAO = ConfigurationManager.AppSettings["Vlo_CaminhoAnexoCNPJ"] + CPF_CNPJ_LOGADO + "\\" + "DOC_PROCURACAO.PDF";
            if (System.IO.File.Exists(PROCURACAO))
                ViewData["DOC_PROCURACAO"] = "true";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Pes_id");
            parameters.Add("@VloBusca1", Id_user_logado);
            parameters.Add("@VloBusca2", "");

            PessoaViewModel pessoaViewModel = DapperORM.DapperORM.ReturnList<PessoaViewModel>("STb_Pessoa_Localizar_Dapper", parameters).FirstOrDefault<PessoaViewModel>();

            return View(pessoaViewModel);
        }

        //OK Homologado      
        [AllowAnonymous]
        [HttpPost]

        public JsonResult Proc_Pessoa_Analise_Inserir(PessoaViewModel pessoaViewModel)
        {
            try
            {    //Upload
                var cpf_cnpj_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                string path = ConfigurationManager.AppSettings["Vlo_CaminhoAnexoCNPJ"] + cpf_cnpj_logado + '\\';

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    string Nome = Request.Files.AllKeys[0];
                    var NomeDoc = Nome + ".PDF";

                    HttpPostedFileBase file = files[0];
                    if (file.ContentType == "application/pdf" && file.ContentLength != 0)
                        file.SaveAs(path + NomeDoc);

                }

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@Pes_CPF_CNPJ", pessoaViewModel.Pes_CPF_CNPJ);
                parameters.Add("@Pes_Nome", pessoaViewModel.Pes_Nome);
                parameters.Add("@Pes_NomeFantasia", pessoaViewModel.Pes_NomeFantasia);
                parameters.Add("@Pes_Telefone", pessoaViewModel.Pes_Telefone);
                parameters.Add("@Pes_Celular", pessoaViewModel.Pes_Celular);
                parameters.Add("@Pes_Email", pessoaViewModel.Pes_Email);
                parameters.Add("@Pes_TipoEmpresa", pessoaViewModel.Pes_TipoEmpresa);
                parameters.Add("@Pes_Qtd_Veiculo", pessoaViewModel.Pes_Qtd_Veiculo);

                parameters.Add("@End_CEP", pessoaViewModel.End_CEP);
                parameters.Add("@End_Logradouro", pessoaViewModel.End_Logradouro);
                parameters.Add("@End_Numero", pessoaViewModel.End_Numero);
                parameters.Add("@End_Bairro", pessoaViewModel.End_Bairro);
                parameters.Add("@End_Cidade", pessoaViewModel.End_Cidade);
                parameters.Add("@End_Estado", pessoaViewModel.End_Estado);
                parameters.Add("@End_Complemento", pessoaViewModel.End_Complemento);
                parameters.Add("@End_Pais", pessoaViewModel.End_Pais);

                parameters.Add("@CPFProcurador", pessoaViewModel.CPFProcurador);
                parameters.Add("@CPFSocio1", pessoaViewModel.CPFSocio1);

                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Pessoa_Juridica_Analise_Inserir_Dapper", parameters);


                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        //DOWNALOAD PERFIL
        [Authorize]
        public ActionResult Download(string id, string ext)
        {
            string contentType = "";
            string caminhoArquivo = "";
            string nomeArquivoV = "";
            string extensao = "";
            string cpf_cnpj_logado = "";


            cpf_cnpj_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

            caminhoArquivo = ConfigurationManager.AppSettings["Vlo_CaminhoAnexoCNPJ"] + cpf_cnpj_logado + "\\" + id + ext;
            extensao = Path.GetExtension(caminhoArquivo);
            nomeArquivoV = id + extensao;
            if (extensao.Equals(".pdf"))
                contentType = "application/pdf";
            if (extensao.Equals(".jpg") || extensao.Equals(".jpeg"))
                contentType = "application/image";

            if (System.IO.File.Exists(caminhoArquivo))
            {
                return File(caminhoArquivo, contentType, nomeArquivoV);
            }
            else
            {
                return RedirectToAction("Perfil_cnpj", "perfil");
            }

        }


        [HttpPost]
        public JsonResult AnexarFotoPerfilPDF()
        {
            try
            {
                var cpf_cnpj_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

                string path = ConfigurationManager.AppSettings["Vlo_CaminhoAnexoCNPJ"] + cpf_cnpj_logado + '\\';

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    string Nome = Request.Files.AllKeys[0];
                    var NomeDoc = Nome + ".PDF";

                    HttpPostedFileBase file = files[0];
                    if (file.ContentLength != 0)
                        file.SaveAs(path + NomeDoc);

                }

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ExcluirAnexos(string TipoAnexo)
        {
            try
            {
                var CPF_CNPJ_LOGADO = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
                var CaminhoAnexoCNPJ1 = ConfigurationManager.AppSettings["Vlo_CaminhoAnexoCNPJ"] + CPF_CNPJ_LOGADO + "\\" + "PROCURACAO.PDF";
                var CaminhoAnexoCNPJ2 = ConfigurationManager.AppSettings["Vlo_CaminhoAnexoCNPJ"] + CPF_CNPJ_LOGADO + "\\" + "DOC_PROCURACAO.PDF";

                if (!System.IO.File.Exists(CaminhoAnexoCNPJ1) || !System.IO.File.Exists(CaminhoAnexoCNPJ2))
                {
                    return Json(new { error = true, messagem = "Caminho inexistente" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    System.IO.File.Delete(CaminhoAnexoCNPJ1);
                    System.IO.File.Delete(CaminhoAnexoCNPJ2);
                }
            }

            catch (Exception ex)
            {
                return Json(new { error = true, messagem = ex }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = false }, JsonRequestBehavior.AllowGet);


        }


















    }
}