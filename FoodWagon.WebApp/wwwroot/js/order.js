﻿var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("pending")) {
        loadDataTable("pending");
    } else if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    } else if (url.includes("completed")) {
        loadDataTable("completed");
    } else if (url.includes("approved")) {
        loadDataTable("approved");
    } else {
        loadDataTable("all");
    }
});

function loadDataTable(status) {
    dataTable = $("#dataTable").DataTable({
        //"serverSide": true,
        "ajax": { url: "/admin/order/getall?status=" + status },
        "columns": [
            { data: "name", "width": "15%" },
            { data: "phoneNumber", "width": "15%" },
            { data: "orderStatus", "width": "10%" },
            { data: "orderTotal", "width": "10%" },
            { data: "streetAddress", "width": "20%" },
            { data: "city", "width": "10%" },
            { data: "orderDate", "width": "15%" },
            {
                data: "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-outline-primary mx-1" 
                              style="border-radius: 20px">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>
                    `
                },
                "width": "5%"
            }
        ]
    });
}