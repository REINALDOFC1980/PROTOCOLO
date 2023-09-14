﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace CondutorInfrator.Models
{
    public class ProcessoViewModel
    {

        public int Pro_Id { get; set; }
        [DisplayName("Número Processo")]
        public string Pro_Numero { get; set; }
        [DisplayName("Número Processo")]
        public string Pro_Processo { get; set; }

        [DisplayName("Data do Processo")]
        public string Pro_DataCriacao { get; set; }
        [DisplayName("Id Proprietario")]
        public string Pro_Ptr_Id { get; set; }
        [DisplayName("ID Condutor")]
        public string Pro_Con_Id { get; set; }
        [DisplayName("ID Multa")]
        public string Pro_Mut_Id { get; set; }

        [DisplayName("ID Veiculo")]
        public string Pro_Vec_Id { get; set; }
        [DisplayName("")]
        public string Pro_ConfirmarProcesso { get; set; }
        [DisplayName("Data de confirmação")]
        public string Pro_DT_ConfirmacaoProcesso { get; set; }
        [DisplayName("")]
        public string Pro_UsuarioRevisor { get; set; }
        [DisplayName("")]
        public string Pro_UsuarioSupervisor { get; set; }

        //Proprietario
        [DisplayName("")]
        public string Ptr_Id { get; set; }
        [DisplayName("Nome Completo")]
        public string Ptr_Nome { get; set; }
        [DisplayName("CNPJ/CPF")]
        public string Ptr_CPF_CNPJ { get; set; }
        [DisplayName("Celular/Telefone Fixo")]
        public string Ptr_Telefone { get; set; }
        [DisplayName("Celular")]
        public string Ptr_Celular { get; set; }
        [DisplayName("Registro CNH")]
        public string Ptr_RegistroCNH { get; set; }
        [DisplayName("Validade CNH")]
        public string Ptr_CNHValidade { get; set; }
        [DisplayName("Situacão CNH")]
        public string Ptr_StatusCNH { get; set; }
        [DisplayName("RG")]
        public string Ptr_RG { get; set; }
        [DisplayName("E-mail")]
        public string Ptr_Email { get; set; }
        [DisplayName("Id Endereço")]
        public string Ptr_EndPro_Id { get; set; }
        [DisplayName("Estrangeiro?")]
        public string Ptr_Estrangeiro { get; set; }
        [DisplayName("Nome Fantasia")]
        public string Ptr_NomeFantasia { get; set; }
        [DisplayName("Tipo Empresa")]
        public string Ptr_TipoEmpresa { get; set; }
        [DisplayName("CPF Representante")]
        public string Ptr_CPFRepreLegal { get; set; }
        [DisplayName("Nome do Representante")]
        public string Ptr_NomeRepreLegal { get; set; }
        [DisplayName("Tipo Pessoa")]
        public string Ptr_TipoPessoa { get; set; }





        [DisplayName("CEP")]
        public string Ptr_EndPro_CEP { get; set; }
        [DisplayName("Logradouro")]
        public string Ptr_EndPro_Logradouro { get; set; }
        [DisplayName("Complemento")]
        public string Ptr_EndPro_Complemento { get; set; }
        [DisplayName("Número")]
        public string Ptr_EndPro_Numero { get; set; }
        [DisplayName("Bairro")]
        public string Ptr_EndPro_Bairro { get; set; }
        [DisplayName("Cidade")]
        public string Ptr_EndPro_Cidade { get; set; }
        [DisplayName("UF")]
        public string Ptr_EndPro_Estado { get; set; }
        [DisplayName("Pais")]
        public string Ptr_EndPro_Pais { get; set; }



        //Condutor
        [DisplayName("")]
        public string Con_Id { get; set; }
        [DisplayName("Nome Completo")]
        public string Con_Nome { get; set; }
        [DisplayName("CPF")]
        public string Con_CPF { get; set; }
        [DisplayName("Celular/Telefone Fixo")]
        public string Con_Telefone { get; set; }
        [DisplayName("Celular")]
        public string Con_Celular { get; set; }
        [DisplayName("Registro CNH")]
        public string Con_RegistroCNH { get; set; }
        [DisplayName("Validade CNH")] 
        public string Con_CNHValidade { get; set; }
        [DisplayName("UF CNH")]
        public string Con_UF_CNH { get; set; }
        [DisplayName("Validade CNH")]
        public string Con_StatusCNH { get; set; }
        [DisplayName("RG")]
        public string Con_RG { get; set; }
        [DisplayName("E-mail")]
        public string Con_Email { get; set; }
        [DisplayName("")]
        public string Con_EndPro_Id { get; set; }
        [DisplayName("")]
        public string Con_FotoCNH { get; set; }
        [DisplayName("Estrangeiro?")]
        public string Con_Estrangeiro { get; set; }
        [DisplayName("Usuario Cadastrado?")]
        public string Con_Cadastrado { get; set; }
        [DisplayName("Confirmar Aceite")]
        public string Con_ConfirmarAceite { get; set; }





        [DisplayName("CEP")]
        public string Con_EndPro_CEP { get; set; }
        [DisplayName("Logradouro")]
        public string Con_EndPro_Logradouro { get; set; }
        [DisplayName("Complemento")]
        public string Con_EndPro_Complemento { get; set; }
        [DisplayName("Número")]
        public string Con_EndPro_Numero { get; set; }
        [DisplayName("Bairro")]
        public string Con_EndPro_Bairro { get; set; }
        [DisplayName("Cidade")]
        public string Con_EndPro_Cidade { get; set; }
        [DisplayName("UF")]
        public string Con_EndPro_Estado { get; set; }
        [DisplayName("Pais")]
        public string Con_EndPro_Pais { get; set; }








        //Multas
        [DisplayName("")]
        public string Mut_Id { get; set; }
        [DisplayName("AIT")]
        public string Mut_AIT { get; set; }
        [DisplayName("Código infração")]
        public string Mut_CodigoInfracao { get; set; }
        [DisplayName("Descrição da infração")]
        public string Mut_DescricaoInfracao { get; set; }
        [DisplayName("Data da infração")]
        public string Mut_DataInfracao { get; set; }
        [DisplayName("Situação")]
        public string Mut_StatusAIT { get; set; }
        

        //veiculo
        [DisplayName("")]
        public string Vec_Id { get; set; }
        [DisplayName("Placa")]
        public string Vec_Placa { get; set; }
        [DisplayName("RENAVAN")]
        public string Vec_RENAVAN { get; set; }
        [DisplayName("Ano")]
        public string Vec_Ano { get; set; }
        [DisplayName("Marca")]
        public string Vec_Marca { get; set; }
        [DisplayName("UF_Veiculo")]
        public string Rec_Veiculo_UF { get; set; }



        //StatusProcesso        
        public int HisPro_StsPro_Id { get; set; }
        public string StsPro_Descricao { get; set; }
        public string StsPro_Orientacao { get; set; }
        public string Mot_Descricao { get; set; }

        public string Motivo_reprovacao { get; set; }



        //outros
        public int QtdRegistroTabela { get; set; }
        public int Pro_Svc_Id { get; set; }
        public string Ass_Nome { get; set; }
        public int status { get; set; }
        public int AnexoPendente { get; set; }

        public int ativarmensagem { get; set; }

        public string Restricao { get; set; }
        public string tempestivo { get; set; }
        
        public HttpPostedFileBase[] COMPROBATORIOS { get; set; }


        public List<AnexoModel> Anexos { get; set; }













    }
}