//định nghĩa host domain cho các API tùy cho từng môi trường
let host = Common.defineDomainAPI().toString();

/***
 *Call api check dữ liệu thay đổi từ file 
 * created by nmthang
 * modified by bvbao (30/9/2020)
 */
var myDropzone = new Dropzone("#dropzoneForm", {
    url: `${host}/changes`,
    method: "POST",
    headers: {
        'apikey': Common.object.apikey,
        'project': Common.object.project
    }
});

//Khắc phục lỗi "Dropzone already attached."
Dropzone.autoDiscover = false;

/***
 * Định nghĩa các thuộc tính cho form upload file
 * created by nmthang
 * modified by bvbao (30/9/2020)
 */
Dropzone.options.dropzoneForm = {
    paramName: "file",
    maxFilesize: 16,
    maxFiles: 1,
    acceptedFiles: "application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    autoProcessQueue: false,
    dictDefaultMessage: Common.object.excelFile,
    addRemoveLinks: true,
    init: function () {
        // xóa file sau khi upload
        this.on("complete", function (file) {
            this.removeFile(file);
        });
    }
};

/***
 * Show dữ liệu các địa chỉ thay đổi đã check được khi nhấn button
 * created by nmthang
 * modified by bvbao (30/9/2020)
 */
$("#btn-upload-check").on("click", function () {
    $.ajax({
        url: `${host}/changes`,
        type: 'GET',
        headers: {
            'apikey': Common.object.apikey,
            'project': Common.object.project
        },
        success: function (res) {
            var dataTable = $('.dataTables-example').DataTable({
                order: [[1, 'asc']],
                data: res,
                columns: [
                    { data: null }, // Ánh xạ các trường trong response trả về với các cột
                    { data: "newProvince" },
                    { data: "newDistrict" },
                    { data: "newID" },
                    { data: "newLocName" },
                    { data: "oldLocName" },
                    { data: "oldID" },
                    { data: "oldDistrict" },
                    { data: "oldProvince" },
                    { data: "action" }
                ],
                pageLength: 10,
                responsive: true,
                dom: '<"html5buttons"B>lTfgitp',
                buttons: [
                    { extend: 'copy' },
                    { extend: 'csv' },
                    { extend: 'excel', title: 'ExampleFile' },
                    { extend: 'pdf', title: 'ExampleFile' },

                    {
                        extend: 'print',
                        customize: function (win) {
                            $(win.document.body).addClass('white-bg');
                            $(win.document.body).css('font-size', '10px');

                            $(win.document.body).find('table')
                                .addClass('compact')
                                .css('font-size', 'inherit');
                        }
                    }
                ],
                bDestroy: true   // tránh lỗi DataTable can not reinitialise
            });

            // đánh STT cho row
            // tham khảo từ https://datatables.net/examples/api/counter_columns.html
            dataTable.on('order.dt search.dt', function () {
                dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                    cell.innerHTML = i + 1;
                });
            }).draw();
        }
    });
})