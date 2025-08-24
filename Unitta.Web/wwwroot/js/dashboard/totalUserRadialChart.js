$(document).ready(function () {
    loadTotalUserRadialChart();
});

function loadTotalUserRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetTotalUsers",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalUserCount").innerHTML = data.totalCount;

            var sectionUser = document.querySelector("#sectionUserCount");
            sectionUser.innerHTML = "";

            var sectionCurrentCount = document.createElement("span");
            if (data.isIncrease) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = `<i class="bi bi-arrow-up-right-circle me-1"></i> <span> ${data.countInCurrentMonth} </span>`;
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = `<i class="bi bi-arrow-down-right-circle me-1"></i> <span> ${data.countInCurrentMonth} </span>`;
            }

            sectionUser.append(sectionCurrentCount);
            sectionUser.append("since last month");

            loadRadialBarChart("totalUserRadialChart", data);
            $(".chart-spinner").hide();
        }
    });
}

