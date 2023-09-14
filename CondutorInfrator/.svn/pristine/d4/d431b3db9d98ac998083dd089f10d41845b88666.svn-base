using CondutorInfrator.Models;
using CondutorInfrator.Util;
using Dapper;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CondutorInfrator.Controllers
{
    public class EmailController : Controller
    {
        readonly Funcao funca = new Funcao();

        [HttpPost]
        [AllowAnonymous]
        public JsonResult CorrigirEmail(string Usu_Login, string Usu_Senha, string Pes_Email)
        {

            try
            {
                //Buscando o cliente
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@VloCampo", "Correcao_email");
                parameters.Add("@VloBusca1", Usu_Login);
                parameters.Add("@VloBusca2", Usu_Senha);
                parameters.Add("@VloBusca3", Pes_Email);

                var result = DapperORM.DapperORM.ReturnList<AutenticacaoModel>("STb_Usuario_Localizar_Dapper", parameters).FirstOrDefault<AutenticacaoModel>();
                if (result.Usu_Login != "ok")
                    return Json(new { mensagem = result.Usu_Login }, JsonRequestBehavior.AllowGet);


                EmailController email = new EmailController();
                email.RotinhaEmail(Pes_Email, result.Usu_Nome, "Confirmação", "", "", "", "","","");

                return Json(new { redirectUrl = Url.Action("login", "autenticacao"), isRedirect = true, mensagem = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { redirectUrl = Url.Action("login", "autenticacao"), isRedirect = true, error = true }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        [AllowAnonymous]
        public JsonResult EncaminharEmail(string email, string tipoEmail, string nome, string vlo1, string vlo2, string link, string observacao)
        {
            try
            {   //Envio do Email
                string pes_email = Request.Params["Pes_Email"];

                var resultado = RotinhaEmail(email, nome, tipoEmail, vlo1, vlo2, "", "", link, observacao);
                return Json(new { redirectUrl = Url.Action("login", "autenticacao"), isRedirect = true, emailexistente = resultado }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { redirectUrl = Url.Action("login", "autenticacao"), isRedirect = true, error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public bool RotinhaEmail(string email, string nome, string tipo, string vlo1, string vlo2, string vlo3, string vlo4, string link, string observacao)
        {

            //verificando se existe
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@VloCampo", "CadastroExistente");
            parameter.Add("@VloBusca1", email);
            parameter.Add("@VloBusca2", "");
            var resultado = DapperORM.DapperORM.ReturnList<DadosExistenteModel>("STb_Pessoa_Localizar_Dapper", parameter).FirstOrDefault<DadosExistenteModel>();

            var nomepessoa = "";

            //verificando para ver se o email existe no momento das rotinas abaixo     
            if (tipo == "ReenvioConfirmação" || tipo == "Redefine_Senha")
            {
                if (resultado.Email == 0)
                    return false;
            }

            if (nome == "" || nome == null)
                nomepessoa = resultado.NomePessoa;
            else
                nomepessoa = nome;

            //liberando para uso da tela de alteração          
            if (tipo == "Redefine_Senha")
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Email", email);
                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Liberar_Atualizacao_Dapper", parameters);
            };

            //verificando se foi enviado
            RotinaEmail rotinaEmail = new RotinaEmail();

            var envio = rotinaEmail.EmailEstrutura(email, nomepessoa, tipo, vlo1, vlo2, vlo3, vlo4, link, observacao);
           
            return envio;

        }

        //verificando se o email existe em nossa base
        [AllowAnonymous]
        public ActionResult ConfirmacaoCadastroEmail(string r)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Email", r);

            var Email = DapperORM.DapperORM.ReturnList<DadosExistenteModel>("Stb_Confirmar_Cadastro_Dapper", parameters).FirstOrDefault<DadosExistenteModel>().Email;

            if (Email == 1)
                return RedirectToAction("login", "autenticacao");

            return RedirectToAction("confirmacaoEmail", "pessoa");
        }



        [AllowAnonymous]
        public ActionResult ConviteCadastroEmail()
        {
            return RedirectToAction("login", "autenticacao");
        }



    }
}