/// <reference path="scripts/typings/signalr/signalr.d.ts" />
/// <reference path="scripts/typings/angularjs/angular.d.ts" />
interface Window {
    _app: { roomNumber: number };
}

interface Option {
    text: string;
    selected: boolean;
    count: number;
}

declare class Chart {
    constructor(contet: CanvasRenderingContext2D);
    Doughnut(data: any[], options: any): any;
}

interface IScope extends ng.IScope {
    options: Option[];
}

class RoomController {
    hub: HubProxy;
    $scope: IScope;

    constructor($scope: IScope) {
        this.$scope = $scope;
        this.$scope.options = [];
        var context = (<HTMLCanvasElement>$('#chart')[0]).getContext('2d');
        var chart = new Chart(context);
        var chartInstance = null;
        var colorsOfChart = ['#00ff00', '#ff0000'];

        this.hub = $.connection.hub.createHubProxy('DefaultHub');

        this.hub.on('UpdateTotaling', (totaling: any[]) => {
            $scope.$apply(() => {
                totaling.forEach(a => {
                    var option = $scope.options.filter(opt => opt.text == a.label)[0];
                    option.count = a.value;
                });
            });
            var data = totaling.map((a, n) => {
                return { label: a.label, value: a.value, color: colorsOfChart[n] };
            });
            data.reverse();
            if (chartInstance == null) {
                chartInstance = chart.Doughnut(data, { percentageInnerCutout: 70, animationEasing: 'easeOutQuart', animationSteps: 20 });
            }
            else {
                data.forEach((a, n) => chartInstance.segments[n].value = a.value);
                chartInstance.update();
            }
        });

        this.hub.on('Reset', () => {
            $scope.$apply(() => {
                if (chartInstance != null) {
                    chartInstance.destroy();
                    chartInstance = null;
                }
                $scope.options.forEach(o => {
                    o.count = 0;
                    o.selected = false;
                });
            });
        });

        $.connection.hub
            .start()
            .then(() => this.hub.invoke('EnterRoom', window._app.roomNumber))
            .then(data => {
                $scope.$apply(() => {
                    $scope.options = <Option[]><any>data;
                });
            });
    }

    public postAnswer(option: Option) {
        if (option.selected) {
            option.selected = false;
            this.hub.invoke('RevokeAnswer', window._app.roomNumber, option.text);
        }
        else {
            this.$scope.options.forEach(opt => opt.selected = false);
            option.selected = true;
            this.hub.invoke('PostAnswer', window._app.roomNumber, option.text);
        }
    }

    public reset() {
        this.hub.invoke('Reset', window._app.roomNumber);
    }
}

var theApp = angular.module('theApp', []);
theApp.controller('roomController', RoomController);