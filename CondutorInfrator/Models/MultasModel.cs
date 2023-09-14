using System.ComponentModel;

namespace CondutorInfrator.Models
{
    public class MultasModel
    {

        [DisplayName("Número AIT")]
        public string Mut_AIT { get; set; }

        [DisplayName("Placa")]
        public string Mut_PlacaVeiculo { get; set; }

        [DisplayName("Código Infração")]
        public string Mut_CodigoInfracao { get; set; }

        [DisplayName("Data/hora Infração")]
        public string Mut_DataInfracao { get; set; }

        [DisplayName("Descrição")]
        public string Mut_DescricaoInfracao { get; set; }

        [DisplayName("Descrição da Infração")]
        public string Mut_DescricaoInfracaoCompleto { get; set; }

        [DisplayName("Situação")]
        public string Mut_StatusAIT { get; set; }

        [DisplayName("Status")]
        public bool Statusait { get; set; }

        public string Rec_Veiculo_UF { get; set; }

        [DisplayName("Total de Registro")]
        public int QtdRegistroTabela { get; set; }

        public int status { get; set; }
        public string tipopessoa { get; set; }

        [DisplayName("Serviços")]
        public string Servico { get; set; }


        //public string Pro_Processo { get; set; }




    }
}

