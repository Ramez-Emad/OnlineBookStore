var dataTable;

$(document).ready(function () {
    const params = new URLSearchParams(window.location.search);
    const status = params.get("status")?.toLowerCase() ?? "all";

    const validStatuses = ["inprocess", "completed", "pending", "approved", "all"];
    const finalStatus = validStatuses.includes(status) ? status : "all";

    loadDataTable(finalStatus);
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/Order/GetAll?status=' + status },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "25%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/Details?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>`
                },
                "width": "10%"
            }
        ]
    });
}