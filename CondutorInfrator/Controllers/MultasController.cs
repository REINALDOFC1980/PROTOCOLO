using CondutorInfrator.Models;
using CondutorInfrator.Util;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace CondutorInfrator.Controllers
{
    [AuthorizeActionFilterAtribute]
    [Authorize]
    public class MultasController : Controller
    {


        public static bool SituacaoAIT(string mut_ait)
        {
            //verificando  situacao do AIT
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VloCampo", "AIT_Vencido");
            parameters.Add("@VloBusca1", mut_ait);
            parameters.Add("@VloBusca2", "");
            var Statusait = DapperORM.DapperORM.ReturnList<MultasModel>("STb_Multa_Localizar_Dapper", parameters).FirstOrDefault<MultasModel>().Statusait;

            return Statusait;
        }


        // GET: Multas        
        public ActionResult Multas()
        {

            //Exibindo a informação de Processo Pendente
            var cpf_logado = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;

            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@VloCampo", "CadastroExistente");
            parameter.Add("@VloBusca1", cpf_logado);
            parameter.Add("@VloBusca2", "");

            var Processo = DapperORM.DapperORM.ReturnList<DadosExistenteModel>("STb_Pessoa_Localizar_Dapper", parameter).FirstOrDefault<DadosExistenteModel>().Processo;

            TempData["vloTotal"] = Processo;
            return View();
        }

        public JsonResult Act_Multas(string searchPhrase, int current = 1, int rowCount = 10)
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

            List<MultasModel> result = DapperORM.DapperORM.ReturnList<MultasModel>("STb_MultaLista_Localizar_Dapper", parameters).ToList();


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









    }

}
