using CondutorInfrator.Models;
using CondutorInfrator.Util;
using Dapper;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CondutorInfrator.Controllers
{
    public class Juridica_PessoaController : Controller
    {

        Funcao funcao = new Funcao();

        [AllowAnonymous]
        public ActionResult Cad_PessoaCNPJ()
        {
            return View();
        }
        //OK Homologado
        [AllowAnonymous]
        [HttpPost]
        public JsonResult BuscarDadosPessoa(string VloBusca1)
        {
            DadosExistenteModel dadosExistenteModel = funcao.VerificarExistencia(VloBusca1);

            var Resultado = dadosExistenteModel;

            try
            {
                return Json(new { Resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {  error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        //Dados da CNH
        [AllowAnonymous]
        [HttpPost]
        public JsonResult PesquisarDados(string _cpf, string tipo)
        {
            try
            {
                DadosExistenteModel dadosExistenteModel = funcao.VerificarExistencia(_cpf);


                if (dadosExistenteModel.APCI == 1)
                {
                    return Json(new { existe = true }, JsonRequestBehavior.AllowGet);
                }



                return Json(new { Resultado = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult RotinaSalvarPJ()
        {
            try
            {


                //Salvando o registro
                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@Pes_CPF_CNPJ", Request.Params["Pes_CPF_CNPJ"]);
                parameters.Add("@Pes_Nome", Request.Params["Pes_Nome"]);
                parameters.Add("@Pes_NomeFantasia", Request.Params["Pes_NomeFantasia"]);
                parameters.Add("@Pes_TipoEmpresa", null);
                parameters.Add("@Pes_RegistroCNH", "");
                parameters.Add("@Pes_CNHValidade", "");
                parameters.Add("@Pes_UF_CNH", "");
                parameters.Add("@Pes_RG", null);

                parameters.Add("@Pes_Telefone", Request.Params["Pes_Telefone"]);
                parameters.Add("@Pes_Celular", Request.Params["Pes_Celular"]);
                parameters.Add("@Pes_Email", Request.Params["Pes_Email"]);

                parameters.Add("@Pes_Estrangeiro", null);
                parameters.Add("@Pes_TipoPessoa", "JURÍDICA");

                parameters.Add("@End_CEP", Request.Params["End_CEP"]);
                parameters.Add("@End_Logradouro", Request.Params["End_Logradouro"]);
                parameters.Add("@End_Numero", Request.Params["End_Numero"]);
                parameters.Add("@End_Bairro", Request.Params["End_Bairro"]);
                parameters.Add("@End_Cidade", Request.Params["End_Cidade"]);
                parameters.Add("@End_Estado", Request.Params["End_Estado"]);
                parameters.Add("@End_Complemento", Request.Params["End_Complemento"]);
                parameters.Add("@End_Pais", Request.Params["End_Pais"]);

                parameters.Add("@Usu_Senha", Request.Params["Usu_Senha"]);


                DapperORM.DapperORM.ExecuteWithouReturn("Stb_Pessoa_Inserir_Dapper", parameters);

                //Envio do Email
                string pes_email = Request.Params["Pes_Email"];
                string Pes_nome = Request.Params["Pes_Nome"];

                EmailController email = new EmailController();
                var resultado = email.RotinhaEmail(pes_email, Pes_nome, "Confirmação CNPJ", "", "", "", "","","");


                return Json(new { redirectUrl = Url.Action("login", "autenticacao"), isRedirect = true, envioemail = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { redirectUrl = Url.Action("login", "autenticacao"), isRedirect = true, error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        [AllowAnonymous]
        public ActionResult Back()
        {
            string urlAnterior = Session["Referrer"].ToString();
            Response.Redirect(urlAnterior); //Redireciona pra url anterior.

            return View();
        }


        [AllowAnonymous]
        public ActionResult Status_Cadastro(string CPF_CNPJ)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pes_Ana_CNPJ", CPF_CNPJ);
            var Resultado = DapperORM.DapperORM.ReturnList<HistoricoStatusModel>("Stb_Historico_Analise_Localizar_Dapper", parameters).FirstOrDefault<HistoricoStatusModel>();

            return View(Resultado);
        }
    }
}