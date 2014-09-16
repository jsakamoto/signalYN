/// <reference path="scripts/typings/signalr/signalr.d.ts" />
/// <reference path="scripts/typings/angularjs/angular.d.ts" />
interface Window {
    _app: { roomNumber: number };
}

interface Option {
    text: string;
    selected: boolean;
}

declare class Chart {
    constructor(contet: CanvasRenderingContext2D);
    Doughnut(data: any[], options: any): any;
}

var theApp = angular.module('theApp', []);

theApp.controller('roomController', ($scope: any) => {
    var context = (<HTMLCanvasElement>$('#chart')[0]).getContext('2d');
    var chart = new Chart(context);
    var chartInstance = null;
    var colorsOfChart = ['#00ff00', '#ff0000'];

    var hub = $.connection.hub.createHubProxy('DefaultHub');

    hub.on('UpdateTotaling', (totaling) => {
        var data = <any[]>$.map(totaling, (a, n) => {
            return { label: a.label, value: a.value, color: colorsOfChart[n] };
        });
        data.reverse();
        if (chartInstance == null) {
            chartInstance = chart.Doughnut(data, { percentageInnerCutout: 70, animationEasing: 'easeOutQuart', animationSteps: 20 });
        }
        else {
            $.each(data, (n, a) => {
                chartInstance.segments[n].value = a.value;
            });
            chartInstance.update();
        }
    });

    hub.on('Reset', () => {
        chartInstance.clear();
        chartInstance = null;
    });

    $.connection.hub
        .start()
        .then(() => hub.invoke('EnterRoom', window._app.roomNumber))
        .then(data => {
            $scope.$apply(() => {
                $scope.options = data;
            });
        });

    $scope.postAnswer = (option: Option) => {
        if (option.selected) {
            option.selected = false;
            hub.invoke('RevokeAnswer', window._app.roomNumber, option.text);
        }
        else {
            $.each($scope.options, (_, opt: Option) => { opt.selected = false; });
            option.selected = true;
            hub.invoke('PostAnswer', window._app.roomNumber, option.text);
        }
    };

    $scope.reset = () => {
        hub.invoke('Reset', window._app.roomNumber);
    };
});