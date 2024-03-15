var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#dataTable").DataTable({
        "ajax": { url: "/admin/category/getall" },
        "columns": [
            { data: "name", "width": "40%" },
            { data: "displayOrder", "width": "20%" },
            {
                data: "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group text-end" role="group">
                            <a href="/Admin/Category/Edit?categoryId=${data}" 
                               class="btn btn-outline-primary mx-1" style="min-width: 120px; margin-left: 20px; border-radius: 20px">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick=Delete("/admin/category/delete/${data}") href="javascript:void(0);"
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

// Handle Delete
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#9195F6",
        cancelButtonColor: "#FF204E",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}