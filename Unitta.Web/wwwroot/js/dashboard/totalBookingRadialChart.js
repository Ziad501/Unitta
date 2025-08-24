$(document).ready(function () {
    loadTotalBookingRadialChart();
});

function loadTotalBookingRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetTotalBooking",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalBookingCount").innerHTML = data.totalCount;

            var sectionBooking = document.querySelector("#sectionBookingCount");
            sectionBooking.innerHTML = "";

            var sectionCurrentCount = document.createElement("span");
            if (data.isIncrease) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = `<i class="bi bi-arrow-up-right-circle me-1"></i> <span> ${data.countInCurrentMonth} </span>`;
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = `<i class="bi bi-arrow-down-right-circle me-1"></i> <span> ${data.countInCurrentMonth} </span>`;
            }

            sectionBooking.append(sectionCurrentCount);
            sectionBooking.append("since last month");

            loadRadialBarChart("totalBookingRadialChart", data);
            $(".chart-spinner").hide();
        }
    });
}

