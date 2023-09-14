

//verificando se é estrageiro pra desabilitar campos
function func_estrangeiro( status) {

    if (status === "Sim") {
        $("#label_cpf").empty();
        $("#label_cpf").append("N° do Passaporte");
        $("#Pes_CPF_CNPJ").attr('maxlength', '50');

        $('#Pes_RG').prop('readonly', true);
        $('#End_CEP').prop('readonly', true);
        $('#End_Logradouro').prop('readonly', true);
        $('#End_Numero').prop('readonly', true);
        $('#End_Bairro').prop('readonly', true);
        $('#End_Complemento').prop('readonly', true);
        $('#Pes_Celular').prop('readonly', true);

        $("#End_CEP").val("00.000-000");
        $("#End_Logradouro").val("Sem informação");
        $("#End_Numero").val("000");
        $("#End_Bairro").val("Sem informação");
        $("#End_Complemento").val("");
        $("#End_Cidade").val("Sem informação");
        $("#End_Estado").val("--");

    }
    else {
        $("#label_cpf").empty();
        $("#label_cpf").append("CPF");
        $("#Pes_CPF_CNPJ").attr('maxlength', '11');

        $('#Pes_RG').prop('readonly', false);
        $('#End_CEP').prop('readonly', false);
        $('#End_Logradouro').prop('readonly', false);
        $('#End_Numero').prop('readonly', false);
        $('#End_Bairro').prop('readonly', false);
        $('#End_Complemento').prop('readonly', false);
        $('#Pes_Celular').prop('readonly', false);

    }
}

//baixando IMG
function func_upload(Campo, Imagen) {
    var file = document.getElementById(Campo).files[0];
    var readImg = new FileReader();
    readImg.readAsDataURL(file);
    readImg.onload = function (e) {
        $(Imagen).attr('src', e.target.result).fadeIn();
    };
}


function func_validandoemail(email) {
    if (email !== "") {
        //desabilitando o submit do form
        $("form").submit(function () { return false; });
        //atribuindo o valor do campo
        var sEmail = email;
        var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!filter.test(sEmail)) {
            return false;
        }
        else {
            return true;
        }
    }
     
}

function func_inputHandler(masks, max, event) {
    var c = event.target;
    var v = c.value.replace(/\D/g, '');
    var m = c.value.length > max ? 1 : 0;
    VMasker(c).unMask();
    VMasker(c).maskPattern(masks[m]);
    c.value = VMasker.toPattern(v, masks[m]);
}



function campo_obrigatorio(alert, div, campo) {       
    $(alert).fadeIn(300).delay(1500).fadeOut(300);
    $(div).addClass('has-error');
    $(campo).focus();
}


//CADASTRO PESSOA FÍSICA
//Limpar campos especificos
function func_limparcampos() {


   //$('#form').on('click', function () {
        $('input').not('.noclear').val('');
    //});

    //$("#Pes_CPF_CNPJ").val("");
    //$('#Pes_Nome').val("");
    //$('#Pes_NomeFatasia').val("");
    ////$('#Pes_TipoEmpresa').val(0);     
    //$("#Pes_RegistroCNH").val("");
    //$("#Pes_CNHValidade").val("");
    //$("#Pes_RG").val("");
    //$("#End_CEP").val("");
    //$("#End_Logradouro").val("");
    //$("#End_Numero").val("");
    //$("#End_Bairro").val("");
    //$("#End_Complemento").val("");
    //$("#End_Cidade").val("");
    //$("#End_Estado").val("");
    //$("#End_Pais").val("");
    //$("#Pes_Celular").val("");
    //$("#Pes_Email").val("");
    //$("#Pes_ConfirmarEmail").val("");
    //$("#Usu_Senha").val("");
    //$("#Usu_ConfirmarSenha").val("");
    //$('#Pes_CPF_CNPJ').focus();
}



//Validando CPF
function func_validador_cpf() {
    "user_strict";
    function r(r) {
        for (var t = null,
            n = 0; 9 > n; ++n)t += r.toString().charAt(n) * (10 - n);
        var i = t % 11; return i = 2 > i ? 0 : 11 - i;
    } function t(r) {
        for (var t = null, n = 0; 10 > n; ++n)t += r.toString().charAt(n) * (11 - n);
        var i = t % 11;
        return i = 2 > i ? 0 : 11 - i;
    } var n = "False",
        i = "True";
    this.gera = function () {
        for (var n = "",
            i = 0; 9 > i;
            ++i)n += Math.floor(9 * Math.random()) + "";
        var o = r(n),
            a = n + "-" + o + t(n + "" + o);
        return a;
    },
        this.valida = function (o) {
            for (var a = o.replace(/\D/g, ""),
                u = a.substring(0, 9),
                f = a.substring(9, 11),
                v = 0; 10 > v; v++)
                if ("" + u + f === "" + v + v + v + v + v + v + v + v + v + v + v)
                    return n;
            var c = r(u), e = t(u + "" + c);
        return f.toString() === c.toString() + e.toString() ? i : n;
        };
}


function func_validando_cnpj(cnpj_){
    var cnpj = cnpj_;
    var valida = new Array(6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2);
    var dig1 = new Number;
    var dig2 = new Number;

    exp = /\.|\-|\//g;
    cnpj = cnpj.toString().replace(exp, "");
    var digito = new Number(eval(cnpj.charAt(12) + cnpj.charAt(13)));

    for (i = 0; i < valida.length; i++) {
        dig1 += (i > 0 ? (cnpj.charAt(i - 1) * valida[i]) : 0);
        dig2 += cnpj.charAt(i) * valida[i];
    }
    dig1 = (((dig1 % 11) < 2) ? 0 : (11 - (dig1 % 11)));
    dig2 = (((dig2 % 11) < 2) ? 0 : (11 - (dig2 % 11)));

    if (((dig1 * 10) + dig2) != digito) {

        //swal({
        //    title: "CNPJ Invalido",
        //    text: "Favor preencher o CNPJ corretamente.",
        //    closeModal: false

        //}).then(function () {
        //    swal.close();
        //    func_limparcampos();
        //    $('#Pes_CPF_CNPJ').val('');
        //    $('#Pes_CPF_CNPJ').focus();

        //});
        return false;
    }

}
function func_rotinaenvioemail(_email, _nome, _tipo, _MyUsefulUrl, _vlo1, _vlo2 ) {
    //
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ email: _email, nome: _nome, tipo: _tipo, vlo1: _vlo1, vlo2: _vlo2 }),
        url: _MyUsefulUrl,
        success: function (data) {
            if (data.error === true) {
                posLoad();
                swal("Ops!", "Houve uma falha de comunicação, o email não foi encaminhado.", "warning")
                    .then(() => {
                        if (data.isRedirect) {
                            window.location.href = data.redirectUrl;
                        }
                    });
            } else {
                if (_tipo === "Confirmação") {  
                    posLoad();
                    swal("Procedimento realizado com sucesso!", "Verifique seu e-mail para ativar a conta.", "success")
                        .then(() => {
                            window.location.href = data.redirectUrl;
                        });
                               
                }

                if (_tipo === "ReenvioConfirmação") {
                    if (data.email_existe === 0) {
                        posLoad();
                        $("#vmensagem").fadeIn(500).delay(3000).fadeOut(500);
                        return false;
                    }
                    else {
                        posLoad();
                        swal("Procedimento realizado com sucesso!", "Verifique seu e-mail para ativar a conta.", "success")
                            .then(() => {
                                window.location.href = data.redirectUrl;
                            });
                    }
                }                      
                

                if (_tipo === "Confirmação CNPJ") {  
                    posLoad();
                      swal("Procedimento realizado com sucesso!", "Solicitação para analisar as informações e documentações do cadastro enviada. Acompanhe as atualizações, após aprovada os serviços estarão disponíveis.", "success")
                        .then(() => {
                            window.location.href = data.redirectUrl;
                        });
                }         


                if (_tipo === "Convite") {
                    posLoad();
                    swal("Envio de convite","encaminhado para: " + _email)
                        .then(() => {
                            if (data.isRedirect) {
                                window.location.href = data.redirectUrl;
                            }
                        });
                } 

                if (_tipo === "Redefine_Senha") {
                    if (data.email_existe === 0) {
                        posLoad();
                        $("#vmensagem").fadeIn(500).delay(3000).fadeOut(500);
                        return false;
                    }
                    else {
                        posLoad();
                        swal("Envio encaminhado com sucesso", "Você receberá um e-mail com instruções para a alteração da senha.", "success")
                        .then(() => {
                            window.location.href = data.redirectUrl;
                        });                            
                    } 
                }
                if (_tipo === "Confirmação_Processo") {
                    posLoad();
                    swal("Procedimento concluido com sucesso", "Um e-mail de confirmação foi encaminhado para o Condutor Infrator, o mesmo deve aceitar o processo no prazo de três dias corridos para que este seja enviado a Triagem na Transalvador.", "success")
                    .then(() => {
                        window.location = '@Url.Action("processo", "processo")';

                    });
                    
                }
            }           
        },
        error: function ()//erro envioConfirmacaoCadastroEmail
        {
            posLoad();
            swal("Ops!", "Houve uma falha de comunicação, por favor tente novamente mais tarde.", "error")
            .then(() => {
                location.reload();
            });
        }
         
    });
}





function limpa_formulário_cep() {
        // Limpa valores do formulário de cep.
        posLoad();
        $("#End_Logradouro").val("");
        $("#End_Bairro").val("");
        $("#End_Cidade").val("");
        $("#End_Estado").val("");

        $("#Con_EndPro_Logradouro").val("");
        $("#Con_EndPro_Bairro").val("");
        $("#Con_EndPro_Cidade").val("");
        $("#Con_EndPro_Estado").val("");

        $("#uf").val("");
    }

    //Quando o campo cep perde o foco.
    $("#End_CEP").blur(function () {

      ///  if ($('#Pes_Estrangeiro').val() === "Não") {

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
                  
                            swal({
                                title: "CEP não encontrado.",
                                text: "Favor preencher o CEP corretamente.",
                                closeModal: false
                            }).then(function () {
                               // swal.close();
                                limpa_formulário_cep();
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
        //}
       
    });



function verificaData(Data) {
    Data = Data.substring(0, 10);

    var dma = -1;
    var data = Array(3);
    var ch = Data.charAt(0);
    for (i = 0; i < Data.length && ((ch >= '0' &&
        ch <= '9') || (ch == '/' && i != 0));) {
        data[++dma] = '';
        if (ch != '/' && i != 0)
            return false;
        if (i != 0) ch = Data.charAt(++i);
        if (ch == '0') ch = Data.charAt(++i);
        while (ch >= '0' && ch <= '9') {
            data[dma] += ch;
            ch = Data.charAt(++i);
        }
    }
    if (ch != '') return false;
    if (data[0] == '' || isNaN(data[0]) || parseInt(data[0]) < 1) return false;
    if (data[1] == '' || isNaN(data[1]) || parseInt(data[1]) < 1 ||
        parseInt(data[1]) > 12) return false;
    if (data[2] == '' || isNaN(data[2]) || ((parseInt(data[2]) < 0 ||
        parseInt(data[2]) > 99) && (parseInt(data[2]) <
            1900 || parseInt(data[2]) > 9999))) return false;
    if (data[2] < 50) data[2] = parseInt(data[2]) + 2000;
    else if (data[2] < 100) data[2] = parseInt(data[2]) + 1900;
    switch (parseInt(data[1])) {
        case 2: {
            if (((parseInt(data[2]) % 4 != 0 ||
                (parseInt(data[2]) % 100 == 0 && parseInt(data[2]) % 400 != 0))
                && parseInt(data[0]) > 28) || parseInt(data[0]) >
                29) return false; break;
        }
        case 4: case 6: case 9: case 11: {
            if (parseInt(data[0]) >
                30) return false; break;
        }
        default: { if (parseInt(data[0]) > 31) return false; }
    }
    return true;

}


//CNPJ

$("#Con_EndPro_CEP").blur(function () {

    ///  if ($('#Pes_Estrangeiro').val() === "Não") {

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
            $("#Con_EndPro_Logradouro").val("...");
            $("#Con_EndPro_Bairro").val("...");
            $("#uf").val("...");

            //Consulta o webservice viacep.com.br/
            $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {

                if (!("erro" in dados)) {
                    //Atualiza os campos com os valores da consulta.
                    posLoad();
                    $("#Con_EndPro_Logradouro").val(dados.logradouro);
                    $("#Con_EndPro_Bairro").val(dados.bairro);
                    $("#Con_EndPro_Cidade").val(dados.localidade);
                    $("#Con_EndPro_Estado").val(dados.uf);

                    if ($("#Con_EndPro_Logradouro").val() !== "") {
                        $('#Con_EndPro_Logradouro').prop('readonly', true);
                        $('#Con_EndPro_Bairro').prop('readonly', true);
                        $("#Con_EndPro_Numero").focus();

                    }
                    else {
                        $('#Con_EndPro_Logradouro').prop('readonly', false);
                        $('#Con_EndPro_Bairro').prop('readonly', false);
                        $('#Con_EndPro_Logradouro').focus();
                    }

                }
                else {
                    //CEP pesquisado não foi encontrado.
                 
                    swal({
                        title: "CEP não encontrado.",
                        text: "Favor preencher o CEP corretamente.",
                        closeModal: false
                    }).then(function () {
                        limpa_formulário_cep();
                        $('#Con_EndPro_CEP').val('');
                        $('#Con_EndPro_CEP').focus();
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
                limpa_formulário_cep();
                $('#Con_EndPro_CEP').val('');
                $('#Con_EndPro_CEP').focus();
            });
        }
    } //end if.
    else {
        //cep sem valor, limpa formulário.
        limpa_formulário_cep();
    }

    //}
});


