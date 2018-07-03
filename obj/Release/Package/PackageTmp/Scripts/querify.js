(function ($) {
    $.fn.querify = function (fields, indexUrl) {
        var root = this;

        root.data('toQuery', function () {
            var query = [];

            root.children().filter(function () {
                return $(this).data('toQuery') != null;
            }).each(function () {
                var toQuery = $(this).data('toQuery');

                query.push(toQuery());
            });

            return query;
        });

        root.addClass('querify-container');

        root.addClass('form-inline');

        var textEditorTemplate = function (acceptButton) {
            var editor = $('<input type="text" style="width: auto;" class="form-control">');

            editor.createValue = function () {
                var val = editor.val();

                var value = $('<span class="bold-span">"' + val + '"</span>');

                value.data('toQuery', function () {
                    return val;
                });

                return value;
            };

            editor.on('input', function () {
                if (editor.val())
                    acceptButton.attr('disabled', false);
                else
                    acceptButton.attr('disabled', true);
            });

            return editor;
        };

        var ossbStatusEditorTemplate = function (acceptButton) {

            var visita = $('<input type="checkbox"><span> VISITA INICIAL</span>');
            var orcar = $('<input type="checkbox"><span> ORÇAR</span>');
            var orcamento = $('<input type="checkbox"><span> ORÇAMENTO</span>');
            var executando = $('<input type="checkbox"><span> EXECUTANDO</span>');
            var cancelada = $('<input type="checkbox"><span> CANCELADA</span>');
            var finalizada = $('<input type="checkbox"><span> FINALIZADA</span>');

            var editor = visita.add(orcar).add(orcamento).add(executando).add(cancelada).add(finalizada);


            editor.createValue = function () {
                var items = [];

                if (visita.is(':checked')) {
                    items.push("I");
                }

                if (orcar.is(':checked')) {
                    items.push("O");
                }

                if (orcamento.is(':checked')) {
                    items.push("R");
                }

                if (executando.is(':checked')) {
                    items.push("E");
                }

                if (finalizada.is(':checked')) {
                    items.push("F");
                }

                if (cancelada.is(':checked')) {
                    items.push("C");
                }

                var value = $('<span class="bold-span">"' + items.map(function (item) {
                    switch (item) {
                        case "I": return "VISITA INICIAL";
                        case "O": return "ORÇAR";
                        case "R": return "ORÇAMENTO";
                        case "E": return "EXECUTANDO";
                        case "F": return "FINALIZADA";
                        case "C": return "CANCELADA";
                        default: return ""
                    }
                }).join(", ") + '"</span>');

                value.data('toQuery', function () {
                    return items;
                });

                return value;
            }

            var onclick = function () {
                if (editor.is(':checked')) {
                    acceptButton.attr('disabled', false);
                }
                else {
                    acceptButton.attr('disabled', true);
                }
            };

            editor.on('click', onclick);

            return editor;
        };

        var dateEditorTemplate = function (acceptButton) {
            var editor = $('<input type="text"  style="width: auto;" class="form-control">');

            editor.createValue = function () {
                var date = editor.val();

                var value = $('<span class="bold-span">' + date + '</span>');

                value.data('toQuery', function () {
                    return date;
                });

                return value;
            };

            editor.datepicker({
                maxViewMode: 0,
                todayBtn: "linked",
                language: "pt-BR",
                autoclose: true,
                toggleActive: true
            })
              .on('changeDate clearDate', function () {
                  if (editor.datepicker('getDate')) {
                      acceptButton.attr('disabled', false);
                  }
                  else {
                      acceptButton.attr('disabled', true);
                  }

              });


            return editor;
        };

        var dateBetweenEditorTemplate = function (acceptButton) {
            var startEditor = $('<input type="text" class="form-control" style="width: auto;">');
            var endEditor = $('<input type="text" class="form-control" style="width: auto;">');

            startEditor.datepicker({
                maxViewMode: 0,
                todayBtn: "linked",
                language: "pt-BR",
                autoclose: true,
                toggleActive: true
            })
                .on('changeDate clearDate', function () {
                    if (startEditor.datepicker('getDate') && endEditor.datepicker('getDate')) {
                        acceptButton.attr('disabled', false);
                    }
                    else {
                        acceptButton.attr('disabled', true);
                    }

                });

            endEditor.datepicker({
                maxViewMode: 0,
                todayBtn: "linked",
                language: "pt-BR",
                autoclose: true,
                toggleActive: true
            })
                .on('changeDate clearDate', function () {
                    if (startEditor.datepicker('getDate') && endEditor.datepicker('getDate')) {
                        acceptButton.attr('disabled', false);
                    }
                    else {
                        acceptButton.attr('disabled', true);
                    }
                });

            var conector = $("<span>e</span>");

            var container = startEditor
                .add(conector)
                .add(endEditor);

            container.createValue = function () {
                var startDate = startEditor.datepicker('getDate');
                var endDate = endEditor.datepicker('getDate');

                var value = $('<span><span class="bold-span">' + startDate + '</span><span class="light-span">e</span><span class="bold-span">' + endDate + '</span></span>');

                value.data('toQuery', function () {
                    return [startDate, endDate];
                });

                return value;
            };

            return container;
        };

        var intEditorTemplate = function (acceptButton) {
            var editor = $('<input type="number" class="form-control" style="width: auto;">');

            editor.createValue = function () {
                var val = editor.val();

                var value = $('<span class="bold-span">"' + val + '"</span>');

                value.data('toQuery', function () {
                    return val;
                });

                return value;
            };

            editor.on('input', function () {
                if (editor.val())
                    acceptButton.attr('disabled', false);
                else
                    acceptButton.attr('disabled', true);
            });

            return editor;
        };

        var intBetweenEditorTemplate = function (acceptButton) {
            var startEditor = $('<input type="number" class="form-control" style="width: auto;">');
            var endEditor = $('<input type="number" class="form-control" style="width: auto;">');

            var conector = $("<span>e</span>");

            var container = startEditor
                .add(conector)
                .add(endEditor);

            container.createValue = function () {
                var startValue = startEditor.val();
                var endValue = endEditor.val();

                var value = $('<span><span class="bold-span">' + startValue + '</span><span class="light-span">e</span><span class="bold-span">' + endValue + '</span></span>');

                value.data('toQuery', function () {
                    return [startValue, endValue];
                });

                return value;
            };

            startEditor.on('input', function () {
                if (startEditor.val() && endEditor.val())
                    acceptButton.attr('disabled', false);
                else
                    acceptButton.attr('disabled', true);
            });

            endEditor.on('input', function () {
                if (startEditor.val() && endEditor.val())
                    acceptButton.attr('disabled', false);
                else
                    acceptButton.attr('disabled', true);
            });

            return container;
        };

        var types = {
            "text": {
                "=": textEditorTemplate,
                "CONTEM": textEditorTemplate,
                "COMEÇA COM": textEditorTemplate,
            },

            "date": {
                "=": dateEditorTemplate,
                "<": dateEditorTemplate,
                ">": dateEditorTemplate,
                "<=": dateEditorTemplate,
                ">=": dateEditorTemplate,
                "!=": dateEditorTemplate,
                "ENTRE": dateBetweenEditorTemplate,
            },

            "int": {
                "=": intEditorTemplate,
                "<": intEditorTemplate,
                ">": intEditorTemplate,
                "<=": intEditorTemplate,
                ">=": intEditorTemplate,
                "!=": intEditorTemplate,
                "ENTRE": intBetweenEditorTemplate,
            },

            "ossb-status": {
                "ENTRE": ossbStatusEditorTemplate,
            },
        };

        var searchButton = $("<button type='button' data-toggle='tooltip' data-placement='right' title='Clique para pesquisar.' class='btn btn-default'><span class='glyphicon glyphicon-search'/></button>");

        var createButton = $("<button type='button' data-toggle='tooltip' data-placement='right' title='Clique para adicionar um filtro.' class='btn btn-default'><span class='glyphicon glyphicon-plus'/></button>");

        searchButton
            .add(createButton)
            .tooltip();

        searchButton.click(function () {
            window.location.href = indexUrl + "/?query=" + JSON.stringify(root.data('toQuery')());

        });

        createButton.click(function () {
            createButton.tooltip('hide');
            searchButton.tooltip('hide');

            createButton.detach();
            searchButton.detach();

            var fieldSelect = $("<select class='form-control' style='margin-left: 3px; margin-right: 3px;'><option selected style='display: none';></option>" + Object.keys(fields)
                .reduce(function (previous, current) {
                    return previous + "<option val = '" + current + "'>" + current + "</option>";
                }, "") + "</select>");

            var cancelButton = $("<button type='button' class='btn btn-default'><span class='glyphicon glyphicon-remove'/></button>");

            cancelButton.appendTo(root);

            cancelButton.click(function () {
                fieldSelect
                    .nextAll()
                    .detach();

                fieldSelect.detach();

                createButton.appendTo(root);
                searchButton.appendTo(root);
            });

            fieldSelect.insertBefore(cancelButton);

            fieldSelect.change(function () {
                fieldSelect
                    .nextUntil(cancelButton)
                    .detach();

                var field = fieldSelect.val();

                var type = types[fields[field]];

                var operationSelect = $("<select class='form-control' style='margin-left: 3px; margin-right: 3px;'><option selected style='display: none';></option>" + Object.keys(type)
                .reduce(function (previous, current) {
                    return previous + "<option val = '" + current + "'>" + current + "</option>";
                }, "") + "</select>");

                operationSelect.insertBefore(cancelButton);

                operationSelect.change(function () {
                    operationSelect
                        .nextUntil(cancelButton)
                        .detach();

                    var operation = operationSelect.val();

                    var acceptButton = $("<button type='button' class='btn btn-default' style='margin-left: 3px; margin-right: 3px;' disabled><span class='glyphicon glyphicon-ok'/></button>");

                    var valueEditor = type[operation](acceptButton);

                    valueEditor.insertBefore(cancelButton);

                    acceptButton.insertBefore(cancelButton);

                    acceptButton.click(function () {
                        fieldSelect
                            .nextAll()
                            .detach();

                        fieldSelect.detach();
                      

                        var itemContainer = $("<div class='query-item'></div>");

                        var fieldValue = $("<span class='bold-span'>" + field + "</span>");

                        var operationValue = $("<span class='light-span'>" + operation + "</span>");

                        var itemValue = valueEditor.createValue();


                        itemContainer.data('toQuery', function () {
                            return [field, operation, itemValue.data('toQuery')()];
                        });

                        var removeButton = $("<span class='close-button glyphicon glyphicon-remove'/></span>");

                        removeButton.click(function () {
                            itemContainer.detach();
                        });

                        fieldValue.appendTo(itemContainer);

                        operationValue.appendTo(itemContainer);

                        itemValue.appendTo(itemContainer);

                        removeButton.appendTo(itemContainer);

                        itemContainer.appendTo(root);

                        createButton.appendTo(root);

                        searchButton.appendTo(root);
                    });
                });
            });
        });

        createButton.appendTo(root);

        searchButton.appendTo(root);

        return this;
    };
}(jQuery));