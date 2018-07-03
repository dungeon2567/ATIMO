/*Códigos reutilizáveis em qualquer parte do projeto*/

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
};

jQuery.fn.load = function (callback) { $(window).on("load", callback) };