$(document).ready(function () {
    loadTotalRevenueRadialChart();
});

function loadTotalRevenueRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetTotalRevenue",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalRevenueCount").innerHTML = data.totalCount;

            var sectionRevenue = document.querySelector("#sectionRevenueCount");
            sectionRevenue.innerHTML = "";

            var sectionCurrentCount = document.createElement("span");
            if (data.isIncrease) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = `<i class="bi bi-arrow-up-right-circle me-1"></i> <span> ${data.countInCurrentMonth} </span>`;
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = `<i class="bi bi-arrow-down-right-circle me-1"></i> <span> ${data.countInCurrentMonth} </span>`;
            }

            sectionRevenue.append(sectionCurrentCount);
            sectionRevenue.append("since last month");

            loadRadialBarChart("totalRevenueRadialChart", data);
            $(".chart-spinner").hide();
        }
    });
}

