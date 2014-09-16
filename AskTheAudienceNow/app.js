/// <reference path="scripts/typings/signalr/signalr.d.ts" />
/// <reference path="scripts/typings/angularjs/angular.d.ts" />

var theApp = angular.module('theApp', []);

theApp.controller('roomController', function ($scope) {
    var context = $('#chart')[0].getContext('2d');
    var chart = new Chart(context);
    var chartInstance = null;
    var colorsOfChart = ['#00ff00', '#ff0000'];

    var hub = $.connection.hub.createHubProxy('DefaultHub');

    hub.on('UpdateTotaling', function (totaling) {
        var data = $.map(totaling, function (a, n) {
            return { label: a.label, value: a.value, color: colorsOfChart[n] };
        });
        data.reverse();
        if (chartInstance == null) {
            chartInstance = chart.Doughnut(data, { percentageInnerCutout: 70, animationEasing: 'easeOutQuart', animationSteps: 20 });
        } else {
            $.each(data, function (n, a) {
                chartInstance.segments[n].value = a.value;
            });
            chartInstance.update();
        }
    });

    hub.on('Reset', function () {
        chartInstance.clear();
        chartInstance = null;
    });

    $.connection.hub.start().then(function () {
        return hub.invoke('EnterRoom', window._app.roomNumber);
    }).then(function (data) {
        $scope.$apply(function () {
            $scope.options = data;
        });
    });

    $scope.postAnswer = function (option) {
        if (option.selected) {
            option.selected = false;
            hub.invoke('RevokeAnswer', window._app.roomNumber, option.text);
        } else {
            $.each($scope.options, function (_, opt) {
                opt.selected = false;
            });
            option.selected = true;
            hub.invoke('PostAnswer', window._app.roomNumber, option.text);
        }
    };

    $scope.reset = function () {
        hub.invoke('Reset', window._app.roomNumber);
    };
});
//# sourceMappingURL=app.js.map
