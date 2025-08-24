$(document).ready(function () {
    loadTotalClientLineChart();
});

function loadTotalClientLineChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetMemberAndBookingPieChart",
        type: "GET",
        dataType: "json",
        success: function (data) {
            loadLineChart("ClientAndBookingCount", data);
            $(".chart-spinner").hide();
        }
    });
}
function loadLineChart(id, data) {
    var specificColors = ['#FFD700', '#1E90FF']; 

    var options = {
        colors: specificColors, // Use specific colors instead of chartColorsArray(id)
        series: data.series,
        chart: {
            type: "line",
            height: 350,
            toolbar: {
                show: true
            },
            theme: {
                mode: 'dark'
            }
        },
        xaxis: {
            categories: data.categories,
            labels: {
                style: {
                    colors: '#6495ED',
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: '#6495ED',
                }
            },
        },
        stroke: {
            show: true,
            curve: 'smooth',
            width: 3
        },
        markers: {
            size: 5,
        },
        tooltip: {
            theme: 'dark'
        },
        legend: {
            labels: {
                colors: '#ffffff', 
                useSeriesColors: true 
            }
        }
    };
    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}