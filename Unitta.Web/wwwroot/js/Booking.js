var dataTable;
$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadDataTable(status);
});

function loadDataTable(status) {
    let url = "/Booking/GetAllBookings";
    if (status && status.trim() !== "") {
        url += "?status=" + encodeURIComponent(status);
    }

    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": url
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "10%" },
            { "data": "phone", "width": "15%" },
            { "data": "email", "width": "20%" },
            { "data": "status", "width": "5%" },
            { "data": "checkInDate", "width": "15%" },
            { "data": "nights", "width": "5%" },
            {
                "data": "totalCost",
                "render": function (data) {
                    return '$' + parseFloat(data).toFixed(2);
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group">
                                <a href="/Booking/Details/${data}" class="btn btn-outline-primary mx-2">
                                    <i class="bi bi-pencil-square"></i> Details
                                </a>
                            </div>`;
                }
            }
        ]
    });
}