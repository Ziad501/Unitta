$(document).ready(function () {
    loadTotalClientPieChart();
});

function loadTotalClientPieChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetBookingPieChart",
        type: "GET",
        dataType: "json",
        success: function (data) {
            loadPieChart("ClientBookingCount", data);
            $(".chart-spinner").hide();
        }
    });
}
function loadPieChart(id, data) {
    var chartColors = chartColorsArray(id);
    options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            type: "pie",
            height: 300,
        },
        stroke: {
            show: false
        },
        legend: {
            position: "bottom",
            horizontalAlign: "center",
            labels: {
                colors: "#fff",
                useSeriesColors: true,
            }
        }
    }
    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}

