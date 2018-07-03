/*$.mask.masks = {
	'phone'     : { mask : '(99) 9999-9999' },
	'phone-us'  : { mask : '(999) 9999-9999' },
	'cpf'       : { mask : '999.999.999-99' },
	'cnpj'      : { mask : '99.999.999/9999-99' },
	'date'      : { mask : '39/19/9999' }, //uk date
	'date-us'   : { mask : '19/39/9999' },
	'cep'       : { mask : '99999-999' },
	'time'      : { mask : '29:69' },
	'cc'        : { mask : '9999 9999 9999 9999' }, //credit card mask
	'integer'   : { mask : '999.999.999.999', type : 'text' },
	'decimal'   : { mask : '99,999.999.999.999', type : 'text', defaultValue: '000' },
	'decimal-us'    : { mask : '99.999,999,999,999', type : 'text', defaultValue: '000' },
	'signed-decimal'    : { mask : '99,999.999.999.999', type : 'text', defaultValue : '+000' },
	'signed-decimal-us' : { mask : '99,999.999.999.999', type : 'text', defaultValue : '+000' }
}*/


$.mask.masks = $.extend($.mask.masks,{
	msk:{ mask: '999-999' },
	other_msk:{ mask: '6666' },
	another_one:{ mask: '7777' , type:'reverse' },
	dose:{ mask: '99'},
	termina_em:{ mask: '39/19/9999', autoTab:false },
	phone_9digito:{ mask: '(99) 99999-9999'},
	inteiros: { mask: '999999999999' },

});


function procura_no_array(needle, haystack){
	for(var i=0;i<haystack.length;i++){
		if(haystack[i] == needle){
			return i;
		}
	}
	return -1;
}


(function($){
	$(function(){
        $('input:text').setMask();

		var ddds = ['11) 9','12) 9','13) 9','14) 9','15) 9','16) 9','17) 9','18) 9','19) 9','21) 9','22) 9','24) 9','27) 9','28) 9','91) 9','92) 9','93) 9','94) 9','95) 9','96) 9','97) 9','98) 9','99) 9'];

		// SP 11 - 19
		// RJ 21,22,24
		// ES 27,28
		// PA 91, 93, 94
		// AM 92, 97
		// RR 95
		// AP 96
		// MA 98, 99
		

		$('#id_telefone, #id_celular, #id_celular_validacao, #id_telefone').keydown(function(event) {
			if( procura_no_array($(this).val().substring(1,6), ddds) >= 0){
				$(this).attr('alt','phone_9digito');
			}else{
				$(this).attr('alt','phone');
            }

			$(this).setMask();
		});
	});

})(jQuery);
