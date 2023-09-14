using CondutorInfrator.Models;
using Dapper;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace CondutorInfrator.Controllers.CNPJ
{
    public class Juridico_PerfilController : Controller
    {

        [Authorize]
        public ActionResult juridico_perfil()
        {
            var Id_user_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "Pes_id");
            parameters.Add("@VloBusca1", Id_user_logado);
            parameters.Add("@VloBusca2", "");

            PessoaViewModel pessoaViewModel = DapperORM.DapperORM.ReturnList<PessoaViewModel>("STb_Pessoa_Localizar_Dapper", parameters).FirstOrDefault<PessoaViewModel>();

            //Pegando a última pege antes do perfil
            Session["Referrer"] = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();

            return View(pessoaViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Proc_Alterar(PessoaViewModel pessoaViewModel)
        {
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
                parameters.Add("@Pes_FotoCNH", "");
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


    }
}