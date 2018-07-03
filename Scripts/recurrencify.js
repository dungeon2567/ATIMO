(function ($) {
    $.fn.recurrencify = function (acceptAction) {
        var root = this;

        root.children()
            .detach();

        var createAcceptButton = function () {
            var acceptButton = $("<button type='button' class='btn btn-default  btn-lg' disabled><span class='glyphicon glyphicon-ok'/></button>");

            acceptButton.click(function () {
                var options = {

                };

                var weeknumber = null;

                var unmatcher = function (item) {
                    return unmatcher;
                };

                function parseDate(input) {
                    var parts = input.split('/');

                    return new Date(parts[2], parts[1] - 1, parts[0]); // Note: months are 0-based
                }

                var dropdownMatcher = function (item) {
                    switch ($(item).find("option:selected").text()) {
                        case "dia(s)":
                            options.freq = RRule.DAILY;
                            break;
                        case "semana(s)":
                            options.freq = RRule.WEEKLY;
                            break;
                        case "mes(es)":
                            options.freq = RRule.MONTHLY;
                            break;
                        case "ano(s)":
                            options.freq = YEARLY;
                            break;

                        case "até":
                            return function (item) {
                                options.until = parseDate($(item).val());

                                return unmatcher;
                            };
                            break;

                        case "no(a)":
                            break;

                        case "primeiro(a)":
                            weeknumber = 1;
                            break;
                        case "segundo(a)":
                            weeknumber = 2;
                            break;
                        case "terceiro(a)":
                            weeknumber = 3;
                            break;
                        case "quarto(a)":
                            weeknumber = 4;
                            break;
                        case "ultimo(a)":
                            weeknumber = -1;
                            break;

                        case "domingo":
                            options.byweekday = RRule.SU;

                            if (weeknumber != null) {
                                options.byweekday = options.byweekday.nth(weeknumber);
                            }
                            break;
                        case "segunda-feira":
                            options.byweekday = RRule.MO;

                            if (weeknumber != null) {
                                options.byweekday = options.byweekday.nth(weeknumber);
                            }
                            break;
                        case "terça-feira":
                            options.byweekday = RRule.TU;

                            if (weeknumber != null) {
                                options.byweekday = options.byweekday.nth(weeknumber);
                            }
                            break;
                        case "quarta-feira":
                            options.byweekday = RRule.WE;

                            if (weeknumber != null) {
                                options.byweekday = options.byweekday.nth(weeknumber);
                            }
                            break;
                        case "quinta-feira":
                            options.byweekday = RRule.TH;

                            if (weeknumber != null) {
                                options.byweekday = options.byweekday.nth(weeknumber);
                            }
                            break;
                        case "sexta-feira":
                            options.byweekday = RRule.FR;

                            if (weeknumber != null) {
                                options.byweekday = options.byweekday.nth(weeknumber);
                            }
                            break;
                        case "sábado":
                            options.byweekday = RRule.SA;

                            if (weeknumber != null) {
                                options.byweekday = options.byweekday.nth(weeknumber);
                            }
                            break;

                        case "repetindo":
                            return function (item) {
                                options.count = parseInt($(item).val());

                                return unmatcher;
                            };
                            break;
                    }

                    return dropdownMatcher;
                };

                var action = function (item) {
                    return function (item) {
                        options.dtstart = parseDate($(item).val());
                        return function (item) {
                            return function (item) {
                                options.interval = parseInt($(item).val());

                                return dropdownMatcher;
                            };
                        }
                    }
                }

                root.children().each(function () {
                    action = action(this);
                });

                var rrule = new RRule(options);

                acceptAction(rrule.all());
            });

            return acceptButton;
        };

        function createContinuationSelect(continuations) {
            var continuationSelect = $("<select class='form-control input-lg'><option selected style='display: none';></option>" + Object.keys(continuations)
                .reduce(function (previous, current) {
                    return previous + "<option value='" + current + "'>" + current + "</option>";
                }, "") + "</select>");

            continuationSelect.change(function () {
                continuationSelect.nextAll()
                    .detach();

                continuations[continuationSelect.val()]().appendTo(root);
            });

            return continuationSelect;
        }

        var untilFactory = function () {
            var continuationInput = $("<input type='text' style='width: auto;' class='form-control input-lg'>");

            var acceptButton = createAcceptButton();

            continuationInput.datepicker({
                maxViewMode: 0,
                todayBtn: "linked",
                language: "pt-BR",
                autoclose: true,
                toggleActive: true
            }).on('changeDate clearDate', function () {
                if (continuationInput.datepicker('getDate')) {
                    acceptButton.attr('disabled', false);
                }
                else {
                    acceptButton.attr('disabled', true);
                }
            });

            return continuationInput.add(acceptButton);
        };

        var weekdayFactory = function () {
            var weekdays = [
                "domingo",
                "segunda-feira",
                "terça-feira",
                "quarta-feira",
                "quinta-feira",
                "sexta-feira",
                "sábado",
            ];

            var weekdaySelect = $("<select class='form-control input-lg'><option selected style='display: none';></option>" + weekdays.reduce(function (previous, current) {
                return previous + "<option value='" + current + "'>" + current + "</option>";
            }, "") + "</select>");

            weekdaySelect.change(function () {
                weekdaySelect
               .nextAll()
               .detach();

                var continuations = {
                    "até": untilFactory,
                    "repetindo": repeatFactory,
                };

                var continuationSelect = createContinuationSelect(continuations);

                continuationSelect.appendTo(root);


                return continuationSelect;
            });

            return weekdaySelect;
        };

        var weeknumberFactory = function () {
            var weeknumbers = [
                "primeiro(a)",
                "segundo(a)",
                "terceiro(a)",
                "quarto(a)",
                "ultimo(a)",
            ];

            var weeknumberSelect = $("<select class='form-control input-lg'><option selected style='display: none';></option>" + weeknumbers.reduce(function (previous, current) {
                return previous + "<option value='" + current + "'>" + current + "</option>";
            }, "") + "</select>");

            weeknumberSelect.change(function () {
                weeknumberSelect
               .nextAll()
               .detach();

                var weekdaySelect = weekdayFactory();

                weekdaySelect.appendTo(root);
            });

            return weeknumberSelect;
        };

        var repeatFactory = function () {
            var continuationInput = $("<input type='number' style='width: auto;' min='1' max='100' class='form-control input-lg'>");

            var acceptButton = createAcceptButton();

            continuationInput.on('input', function () {
                if (continuationInput.val()) {
                    acceptButton.attr('disabled', false);
                }
                else {
                    acceptButton.attr('disabled', true);
                }
            });

            var times = $("<span>vezes</span>");

            return continuationInput.add(times).add(acceptButton);
        }

        var frequencies = {
            "dia(s)": function () {
                var continuations = {
                    "até": untilFactory,
                    "repetindo": repeatFactory,
                };

                var continuationSelect = $("<select class='form-control input-lg'><option selected style='display: none';></option>" +
                    Object.keys(continuations).reduce(function (previous, current) {
                        return previous + "<option value='" + current + "'>" + current + "</option>";
                    }, "") + "</select>");

                continuationSelect.change(function () {
                    continuationSelect.nextAll()
                        .detach();

                    continuations[continuationSelect.val()]().appendTo(root);
                });

                return continuationSelect;

            },

            "semana(s)": function () {
                var continuations = {
                    "no(a)": weekdayFactory,

                    "até": untilFactory,

                    "repetindo": repeatFactory,
                };

                var continuationSelect = $("<select class='form-control input-lg'><option selected style='display: none';></option>" + Object.keys(continuations)
                    .reduce(function (previous, current) {
                        return previous + "<option value='" + current + "'>" + current + "</option>";
                    }, "") + "</select>");

                continuationSelect.change(function () {
                    continuationSelect.nextAll()
                        .detach();

                    continuations[continuationSelect.val()]().appendTo(root);
                });

                return continuationSelect;
            },

            "mes(es)": function () {
                var continuations = {
                    "no(a)": weeknumberFactory,

                    "até": untilFactory,

                    "repetindo": repeatFactory,
                };

                return createContinuationSelect(continuations);
            },

            "ano(s)": function () {
                var continuations = {
                    "até": untilFactory,

                    "repetindo": repeatFactory,
                };

                return createContinuationSelect(continuations);
            },
        };

        root.addClass('form-inline');

        var from = $("<span>de</span>");

        var fromInput = $("<input type='text' style='width: auto;' class='form-control input-lg'>");

        fromInput.datepicker({
            maxViewMode: 0,
            todayBtn: "linked",
            language: "pt-BR",
            autoclose: true,
            toggleActive: true
        }).on('changeDate clearDate', function () {
            if (fromInput.datepicker('getDate')) {
                var every = $("<span>cada</span>");

                var interval = $("<input type='number' min='1' max='999' class='form-control input-lg' style='width: auto;'/>");

                var frequencySelect = $("<select class='form-control input-lg'><option selected style='display: none';></option>" + Object.keys(frequencies)
                    .reduce(function (previous, current) {
                        return previous + "<option value='" + current + "'>" + current + "</option>";
                    }, "") + "</select>");

                frequencySelect.change(function () {
                    frequencySelect.nextAll()
                        .detach();

                    frequencies[frequencySelect.val()]().appendTo(root);
                });

                every.appendTo(root);

                interval.appendTo(root);

                frequencySelect.appendTo(root);
            }
            else {
                fromInput
                    .nextAll()
                    .detach();
            }
        });

        from.appendTo(root);

        fromInput.appendTo(root);

        return this;
    }
}(jQuery));