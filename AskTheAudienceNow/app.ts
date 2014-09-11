/// <reference path="scripts/typings/signalr/signalr.d.ts" />
/// <reference path="scripts/typings/angularjs/angular.d.ts" />
interface Window {
    _app: { roomNumber: number };
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
        console.dir(totaling);
        var data = <any[]>$.map(totaling, (a, n) => {
            return { label: a.label, value: a.value, color: colorsOfChart[n] };
        });
        data.reverse();
        if (chartInstance == null) {
            console.dir(data);
            chartInstance = chart.Doughnut(data, { percentageInnerCutout: 70, animationEasing: 'easeOutQuart', animationSteps: 50 });
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
        .done(() => {
            hub.invoke('EnterRoom', window._app.roomNumber);
        });

    $scope.postAnswer = (answer: string) => {
        hub.invoke('PostAnswer', window._app.roomNumber, answer);
    };

    $scope.reset = () => {
        hub.invoke('Reset', window._app.roomNumber);
    };
});