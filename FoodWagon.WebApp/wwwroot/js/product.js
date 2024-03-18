var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": { url: '/Admin/Product/GetAll' },
        "columns": [
            {
                data: "productImages[0].imageUrl",
                "render": function (data) {
                    return `
                        <img src="${data}" style="width: 50px" />
                    `
                },
                "width": "15%"
            },
            { data: 'title', "width": "20%" },
            { data: 'price', "width": "10%" },
            { data: 'saleOff', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group text-end" role="group">
                            <a href="/Admin/Product/Edit?productId=${data}" 
                               class="btn btn-outline-primary mx-1" style="min-width: 120px; margin-left: 20px; border-radius: 20px">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick=Delete("/Admin/Product/Delete/${data}") href="javascript:void(0);"
                               class="btn btn-outline-danger mx-1" style="min-width: 120px; border-radius: 20px">
                                <i class="bi bi-trash"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "30%"
            },
        ],
    });
}

// Handle Delete
function Delete(url) {
    console.log(url);
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