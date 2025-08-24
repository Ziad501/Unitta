function loadRadialBarChart(id, data) {
    var chartColors = chartColorsArray(id);
    var options = {
        fill: {
            colors: chartColors,
        },
        chart: {
            height: 90,
            width: 90,
            type: "radialBar",
            sparkline: {
                enabled: true
            },
            offsety: -10,
        },
        series: data.series,
        plotOptions: {
            radialBar: {
                hollow: {
                    margin: 15,
                    size: "55%"
                },
                dataLabels: {
                    showOn: "always",
                    name: {
                        offsetY: -10,
                        show: false,
                    },
                    value: {
                        color: chartColors,
                        fontSize: "15px",
                        show: true,
                        offsetY: 5,
                    }
                }
            }
        },
        stroke: {
            lineCap: "round",
        },
    };

    document.querySelector("#" + id).innerHTML = "";

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}
function chartColorsArray(id) {
    if (document.querySelector("#" + id) !== null) {
        var colors = document.querySelector("#" + id).getAttribute("data-colors");
        if (colors) {
            colors = JSON.parse(colors);
            return colors.map(function (value) {
                var newValue = value.replace(" ", "");
                if (newValue.indexOf(",") === -1) {
                    var color = getComputedStyle(document.documentElement).getPropertyValue(newValue);
                    if (color) {
                        return color;
                    } else {
                        return newValue;
                    }
                }
            });
        }
    }
}
