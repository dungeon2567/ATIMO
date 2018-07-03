// verifica_cpf_cnpj

function verifica_cpf_cnpj ( valor ) {
    // Garante que o valor é uma string
    valor = valor.toString();
    
    // Remove caracteres inválidos do valor
    valor = valor.replace(/[^0-9]/g, '');
 
    // Verifica CPF
    if ( valor.length === 11 ) {
        return 'CPF';
    } 
    else if ( valor.length === 14 ) { // Verifica CNPJ
        return 'CNPJ';
    } 
    else {
        return false;
    }
    
}
 
/*
 calc_digitos_posicoes
*/
function calc_digitos_posicoes(digitos, posicoes)
{
    var soma_digitos = 0;

    // Garante que o valor é uma string
    digitos = digitos.toString();

    for ( var i = 0; i < digitos.length; i++  ) {
        // Preenche a soma com o dígito vezes a posição
        soma_digitos = soma_digitos + ( digitos[i] * posicoes );
 
        // Subtrai 1 da posição
        posicoes--;
 
        // Parte específica para CNPJ
        // Ex.: 5-4-3-2-9-8-7-6-5-4-3-2
        if ( posicoes < 2 ) {
            // Retorno a posição para 9
            posicoes = 9;
        }
    }
 
    // Captura o resto da divisão entre soma_digitos dividido por 11
    // Ex.: 196 % 11 = 9
    soma_digitos = soma_digitos % 11;
 
    // Verifica se soma_digitos é menor que 2
    if ( soma_digitos < 2 ) {
        // soma_digitos agora será zero
        soma_digitos = 0;
    } else {
        // Se for maior que 2, o resultado é 11 menos soma_digitos
        // Ex.: 11 - 9 = 2
        // Nosso dígito procurado é 2
        soma_digitos = 11 - soma_digitos;
    }
 
    // Concatena mais um dígito aos primeiro nove dígitos
    // Ex.: 025462884 + 2 = 0254628842
    var cpf = digitos + soma_digitos;
 
    // Retorna
    return cpf;
    
}
 
    /*
        Valida CPF
    */
    function valida_cpf( valor ) {
        // Garante que o valor é uma string
        valor = valor.toString();
    
        // Remove caracteres inválidos do valor
        valor = valor.replace(/[^0-9]/g, '');
 
 
        // Captura os 9 primeiros dígitos do CPF
        // Ex.: 02546288423 = 025462884
        var digitos = valor.substr(0, 9);
 
        // Faz o cálculo dos 9 primeiros dígitos do CPF para obter o primeiro dígito
        var novo_cpf = calc_digitos_posicoes( digitos, 10 );
 
        // Faz o cálculo dos 10 dígitos do CPF para obter o último dígito
        var novo_cpf = calc_digitos_posicoes( novo_cpf, 11 );
 
        // Verifica se o novo CPF gerado é idêntico ao CPF enviado
        if ( novo_cpf === valor ) {
            // CPF válido
            return true;
        } else {
            // CPF inválido
            return false;
        }
    
    } // valida_cpf
 
    /*
        valida_cnpj
    */
    function valida_cnpj ( valor ) {
        // Garante que o valor é uma string
        valor = valor.toString();
    
        // Remove caracteres inválidos do valor
        valor = valor.replace(/[^0-9]/g, '');
 
    
        // O valor original
        var cnpj_original = valor;
 
        // Captura os primeiros 12 números do CNPJ
        var primeiros_numeros_cnpj = valor.substr( 0, 12 );
 
        // Faz o primeiro cálculo
        var primeiro_calculo = calc_digitos_posicoes( primeiros_numeros_cnpj, 5 );
 
        // O segundo cálculo é a mesma coisa do primeiro, porém, começa na posição 6
        var segundo_calculo = calc_digitos_posicoes( primeiro_calculo, 6 );
 
        // Concatena o segundo dígito ao CNPJ
        var cnpj = segundo_calculo;
 
        // Verifica se o CNPJ gerado é idêntico ao enviado
        if ( cnpj === cnpj_original ) {
            return true;
        }
        
        // Retorna falso por padrão
        return false;
    }
 
    /*
        valida_cpf_cnpj
   */
    function valida_cpf_cnpj ( valor ) {
        // Verifica se é CPF ou CNPJ
        var valida = verifica_cpf_cnpj( valor );
 
        // Garante que o valor é uma string
        valor = valor.toString();
    
        // Remove caracteres inválidos do valor
        valor = valor.replace(/[^0-9]/g, '');
 
        // Valida CPF
        if ( valida === 'CPF' ) {
            // Retorna true para cpf válido
            return valida_cpf( valor );
        } 
            // Valida CNPJ
        else if ( valida === 'CNPJ' ) {
            // Retorna true para CNPJ válido
            return valida_cnpj( valor );
        } 
            // Não retorna nada
        else {
            return false;
        }
    }
 
    /*
        formata_cpf_cnpj
    */
    function formata_cpf_cnpj(valor) {
        // O valor formatado
        var formatado = false;

        // Verifica se é CPF ou CNPJ
        var valida = verifica_cpf_cnpj(valor);

        // Garante que o valor é uma string
        valor = valor.toString();

        // Remove caracteres inválidos do valor
        valor = valor.replace(/[^0-9]/g, '');


        // Valida CPF
        if (valida === 'CPF') {
            // Verifica se o CPF é válido
            if (valida_cpf(valor)) {
                // Formata o CPF ###.###.###-##
                formatado = valor.substr(0, 3) + '.';
                formatado += valor.substr(3, 3) + '.';
                formatado += valor.substr(6, 3) + '-';
                formatado += valor.substr(9, 2) + '';
            }
        }
            // Valida CNPJ
        else if (valida === 'CNPJ') {
            // Verifica se o CNPJ é válido
            if (valida_cnpj(valor)) {
                // Formata o CNPJ ##.###.###/####-##
                formatado = valor.substr(0, 2) + '.';
                formatado += valor.substr(2, 3) + '.';
                formatado += valor.substr(5, 3) + '/';
                formatado += valor.substr(8, 4) + '-';
                formatado += valor.substr(12, 14) + '';
            }
        }

        // Retorna o valor 
        return formatado;
    }