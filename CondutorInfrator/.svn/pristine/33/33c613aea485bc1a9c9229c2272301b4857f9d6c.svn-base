using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CondutorInfrator.Models
{
    public class AutenticacaoModel
    {
        public string Usu_id { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Senha")]
        public string Usu_Senha { get; set; }

        [DisplayName("CPF / CNPJ / PASSAPORTE")]
        public string Usu_Login { get; set; }
        [DisplayName("Nome usuário")]
        public string Usu_Nome { get; set; }
        [DisplayName("Expirado")]
        public bool Usu_Expirado { get; set; }


        public string Pes_Id { get; set; }
        [DisplayName("Tipo Pessoa")]
        public string Pes_TipoPessoa { get; set; }
        [DisplayName("Data da aprovação SIPRO")]
        public string Pes_DataAprovadoSipro { get; set; }
        [DisplayName("E-mail")]
        public string Pes_Email { get; set; }
        [HiddenInput]
        public string ReturnUrl { get; set; }
        public string Autenticar_ReturnUrl { get; set; }





    }



}