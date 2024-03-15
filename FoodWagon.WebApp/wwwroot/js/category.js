var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#dataTable").DataTable({
        "ajax": { url: "/admin/category/getall" },
        "columns": [
            { data: "name", "width": "30%" },
            { data: "displayOrder", "width": "30%" },
            {
                data: "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group text-end" role="group">
                            <a href="/Admin/Category/Edit?categoryId=${data}" 
                               class="btn btn-outline-primary mx-1" style="min-width: 120px; margin-left: 20px; border-radius: 20px">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a href="/Admin/Category/Delete?categoryId=${data}" 
                               class="btn btn-outline-danger mx-1" style="min-width: 120px; border-radius: 20px">
                                <i class="bi bi-trash"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "40%"
            }
        ]
    });
}