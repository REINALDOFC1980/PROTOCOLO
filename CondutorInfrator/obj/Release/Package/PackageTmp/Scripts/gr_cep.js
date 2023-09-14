

$(document).ready(function () {

    function limpa_formulário_cep() {
        // Limpa valores do formulário de cep.
        posLoad();
        $("#End_Logradouro").val("");
        $("#End_Bairro").val("");
        $("#End_Cidade").val("");
        $("#uf").val("");
    }

    //Quando o campo cep perde o foco.
    $("#End_CEP").blur(function () {
   
        if ($('#Pes_Estrangeiro').val() === "Não" || $('#Pes_Estrangeiro').val() === undefined) {

            //Nova variável "cep" somente com dígitos.
            var cep = $(this).val().replace(/\D/g, '');

            //Verifica se campo cep possui valor informado.
            if (cep !== "") {

                //Expressão regular para validar o CEP.
                var validacep = /^[0-9]{8}$/;

                //Valida o formato do CEP.
                if (validacep.test(cep)) {
                    preLoad();
                    //Preenche os campos com "..." enquanto consulta webservice.
                    $("#End_Logradouro").val("...");
                    $("#End_Bairro").val("...");
                    $("#uf").val("...");

                    //Consulta o webservice viacep.com.br/
                    $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {

                        if (!("erro" in dados)) {
                            //Atualiza os campos com os valores da consulta.
                            posLoad();
                            $("#End_Logradouro").val(dados.logradouro);
                            $("#End_Bairro").val(dados.bairro);
                            $("#End_Cidade").val(dados.localidade);
                            $("#End_Estado").val(dados.uf);
                           // $("#End_CodMunicipio").val(dados.siafi);

                            if ($("#End_Logradouro").val() !== "") {
                                $('#End_Logradouro').prop('readonly', true);
                                $('#End_Bairro').prop('readonly', true);
                                $("#End_Numero").focus();

                            }
                            else {
                                $('#End_Logradouro').prop('readonly', false);
                                $('#End_Bairro').prop('readonly', false);
                                $('#End_Logradouro').focus();
                            }
                           
                        }
                        else {
                            //CEP pesquisado não foi encontrado.
                            limpa_formulário_cep();
                            swal({
                                title: "CEP não encontrado.",
                                text: "Favor preencher o CEP corretamente.",
                                closeModal: false
                            }).then(function () {
                                swal.close();
                                $('#End_CEP').val('');
                                $('#End_CEP').focus();
                            });
                        }
                    });
                } //end if.
                else {
                    //cep é inválido.
                    limpa_formulário_cep();
                    swal({
                        title: "Formato de CEP inválido.",
                        text: "Favor preencher o CEP corretamente.",
                        closeModal: false
                    }).then(function () {
                        swal.close();
                        $('#End_CEP').val('');
                        $('#End_CEP').focus();
                    });
                }
            } //end if.
            else {
                //cep sem valor, limpa formulário.
                limpa_formulário_cep();
            }
        }
    });
});

