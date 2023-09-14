$(document).ready(function () {
    $('#GravarDados').click(function () {

        //Validaçao
        if ($('#Pes_CPF_CNPJ').val() === "") {
            $("#vcpf").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivCPF').addClass('has-error');
            $('#Pes_CPF_CNPJ').focus();
            return false;
        }

        if ($('#Pes_Nome').val() === "") {
            $("#vnome").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivNome').addClass('has-error');
            $('#Pes_Nome').focus();
            return false;
        }

        if ($('#Pes_NomeFatasia').val() === "") {
            $("#vNomeFatasia").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivNomeFatasia').addClass('has-error');
            $('#Pes_NomeFatasia').focus();
            return false;
        }

        //if ($('#Pes_Representante').val() === "") {
        //    $("#vNomeRepresentante").fadeIn(300).delay(1500).fadeOut(300);
        //    $('#DivNomeRepresentante').addClass('has-error');
        //    $('#Pes_Representante').focus();
        //    return false;
        //}

        if ($('#End_CEP').val() === "") {
            $("#vcep").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivCEP').addClass('has-error');
            $('#End_CEP').focus();
            return false;
        }

        if ($('#End_Logradouro').val() === "") {
            $("#vlogradouro").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivLogradouro').addClass('has-error');
            $('#End_Logradouro').focus();
            return false;
        }

        if ($('#End_Bairro').val() === "") {
            $("#vbairro").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivBairro').addClass('has-error');
            $('#End_Bairro').focus();
            return false;
        }

        if ($('#End_Pais').val() === "") {
            $("#vPais").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivPais').addClass('has-error');
            $('#End_Pais').focus();
            return false;
        }

        if ($('#Pes_Celular').val() === "") {
            $("#vcelular").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivCelular').addClass('has-error');
            $('#Pes_Celular').focus();
            return false;
        }


        if ($('#End_Cidade').val() === "") {
            $("#vcidade").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivCidade').addClass('has-error');
            $('#End_Cidade').focus();
            return false;
        }

        if ($('#End_Estado').val() === "") {
            $('#DivEstado').addClass('has-error');
            $('#End_Estado').focus();
            return false;
        }


        if ($('#Pes_Email').val() === "") {
            $("#vemail").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivEmail').addClass('has-error');
            $('#Pes_Email').focus();
            return false;
        }


        if ($('#Usu_Senha').val() === "") {
            $("#vsenha").fadeIn(300).delay(1500).fadeOut(300);
            $('#DivSenha').addClass('has-error');
            $('#Usu_Senha').focus();
            return false;
        }


    });
});

   