/// <reference path="scripts/typings/signalr/signalr.d.ts" />
/// <reference path="scripts/typings/angularjs/angular.d.ts" />

var RoomController = (function () {
    function RoomController($scope) {
        var _this = this;
        this.$scope = $scope;
        var context = $('#chart')[0].getContext('2d');
        var chart = new Chart(context);
        var chartInstance = null;
        var colorsOfChart = ['#00ff00', '#ff0000'];

        this.hub = $.connection.hub.createHubProxy('DefaultHub');

        this.hub.on('UpdateTotaling', function (totaling) {
            $scope.$apply(function () {
                totaling.forEach(function (a) {
                    var option = $scope.options.filter(function (opt) {
                        return opt.text == a.label;
                    })[0];
                    option.count = a.value;
                });
            });
            var data = totaling.map(function (a, n) {
                return { label: a.label, value: a.value, color: colorsOfChart[n] };
            });
            data.reverse();
            if (chartInstance == null) {
                chartInstance = chart.Doughnut(data, { percentageInnerCutout: 70, animationEasing: 'easeOutQuart', animationSteps: 20 });
            } else {
                data.forEach(function (a, n) {
                    return chartInstance.segments[n].value = a.value;
                });
                chartInstance.update();
            }
        });

        this.hub.on('Reset', function () {
            $scope.$apply(function () {
                if (chartInstance != null) {
                    chartInstance.destroy();
                    chartInstance = null;
                }
                $scope.options.forEach(function (o) {
                    o.count = 0;
                    o.selected = false;
                });
            });
        });

        $.connection.hub.start().then(function () {
            return _this.hub.invoke('EnterRoom', window._app.roomNumber);
        }).then(function (data) {
            $scope.$apply(function () {
                $scope.options = data;
            });
        });
    }
    RoomController.prototype.postAnswer = function (option) {
        if (option.selected) {
            option.selected = false;
            this.hub.invoke('RevokeAnswer', window._app.roomNumber, option.text);
        } else {
            this.$scope.options.forEach(function (opt) {
                return opt.selected = false;
            });
            option.selected = true;
            this.hub.invoke('PostAnswer', window._app.roomNumber, option.text);
        }
    };

    RoomController.prototype.reset = function () {
        this.hub.invoke('Reset', window._app.roomNumber);
    };
    return RoomController;
})();

var theApp = angular.module('theApp', []);
theApp.controller('roomController', RoomController);
//# sourceMappingURL=app.js.map
