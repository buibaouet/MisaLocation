$(document).ready(function () {
    var home = new Home();
});

class Home {
    constructor() {
        this.initEvent();
        this.allLocation = null;
    }

    initEvent() {
        $("#btnUpload").on("click", this.uploadFileExcel.bind(this));
        $("#btnDownloadJson").on("click", this. downloadFileJson.bind(this));
        $("#btnDownloadExcel").on("click", this. downloadFileExcel.bind(this));
        $("#btnUpToDB").on("click", this.updateDataToElasticsearch.bind(this));
        $(".check-file-upload").on("blur", this.inputFileOnBlur.bind(this));
        //Show tên file ra label khi upload file
        $(".check-file-upload").on("change", function () {
            let filename = $(this).val().split("\\").pop();
            if (filename == '') {
                $(this).next().text(Common.object.excelFile);
            }
            else {
                $(this).next().text(filename);
            }
            $(this).blur();
        });
        
        //show bảng danh sách các địa chỉ khác nhau
        $('#btn-different').on("click", function () {
            $('#table-different').show();
            $('#btn-different').addClass("active");
            $('#btn-duplication').removeClass("active");
            $('#table-duplication').hide();
        })
        //show bảng danh sách các địa chỉ trùng nhau
        $('#btn-duplication').on("click", function () {
            $('#table-different').hide();
            $('#table-duplication').show();
            $('#btn-different').removeClass("active");
            $('#btn-duplication').addClass("active");
        })
    }

    /**
     * Phương thức kiểm tra người dùng đã upload file chưa
     * @param {any} event
     * created by bvbao(26/8/2020)
     */
    inputFileOnBlur(event) {
        $(".upload-error").hide();
        var fileExtension = ['xls', 'xlsx'];
        let input = event.target;
        let $error = $(input).parent().next();
        
        if (!input.value) {
            $error.show().text(Common.object.uploadFile);
        }
        var extension = input.value.replace(/^.*\./, '');
        if ($.inArray(extension, fileExtension) == -1) {
            $error.show().text(Common.object.excelFile);
        }
        else {
            $error.text('').hide();
        }
    }
    /**
     * Kiểm tra extention của file có phù hợp không
     * @param {any} filename
     * Created by bvbao (20/7/2020)
     */
    checkFile(filename) {
        var fileExtension = ['xls', 'xlsx'];
        if (filename.length == 0) {
            alert(Common.object.uploadFile);
            return false;
        }
        else {
            var extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert(Common.object.excelFile);
                return false;
            }
        }
        return true;
    }

    /**
     * Upload file Excel và trả về danh sách trùng và khác
     * Created by bvbao (20/7/2020)
     * */
    uploadFileExcel() {
        let $saveBtn = $(event.target);
        let $modal = $saveBtn.closest('.card'); // modal hiện tại
        $modal.find('.check-file-upload').blur(); 
        let allAreValid = true;
        let $errors = $modal.find('.input-error'); // span thông báo lỗi
        $.each($errors, function (index, item) {
            if (item.innerText != '') {
                allAreValid = false;
            }
        });

        if (allAreValid) {
            var fileNew = $('#fileNewupload').val();
            var fileOld = $('#fileOldupload').val();
            self = this;

            if (this.checkFile(fileNew) && this.checkFile(fileOld)) {
                var fdata = new FormData();
                var fileNewUpload = $("#fileNewupload").get(0);
                fdata.append(fileNewUpload.files[0].name, fileNewUpload.files[0]);
                var fileOldUpload = $("#fileOldupload").get(0);
                fdata.append(fileOldUpload.files[0].name, fileOldUpload.files[0]);
                //Hiển thị gif load data, ẩn span báo lỗi
                $('#preload').show();
                $(".upload-error").hide();
                //vô hiệu hóa click button upload và download
                $("#btnUpload").prop("disabled", true);
                $("#btnDownload").prop("disabled", true);
                $("#btnUpToDB").prop("disabled", true);

                let host = Common.defineDomainAPI().toString();
                $.ajax({
                    type: "POST",
                    url: `${host}/synchronized/loc-synchronized`,
                    data: fdata,
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        //Ẩn gif load data
                        $('#preload').hide();
                        //Hiện button download và upload
                        $("#btnUpload").prop("disabled", false);
                        $("#btnDownload").prop("disabled", false);
                        $("#btnUpToDB").prop("disabled", false);
                        self.allLocation = response[2];

                        var DuplicationTable = $('#table-duplication .table').DataTable({
                            order: [[0, 'asc']],
                            data: response[0],
                            columns: [
                                { data: null }, // Ánh xạ các trường trong response trả về với các cột
                                { data: "newID" },
                                { data: "oldID" },
                                { data: "newLocName" },
                                { data: "newDistrict" },
                                { data: "newProvince" }
                            ],
                            "bDestroy": true
                        });
                        // đánh STT cho row
                        // tham khảo từ https://datatables.net/examples/api/counter_columns.html
                        DuplicationTable.on('order.dt search.dt', function () {
                            DuplicationTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                                cell.innerHTML = i + 1;
                            });
                        }).draw();

                        var DifferentTable = $('#table-different .table').DataTable({
                            order: [[0, 'asc']],
                            data: response[1],
                            columns: [
                                { data: null }, // Ánh xạ các trường trong response trả về với các cột
                                { data: "newID" },
                                { data: "oldID" },
                                { data: "oldLocName" },
                                { data: "oldDistrict" },
                                { data: "oldProvince" }
                            ],
                            "bDestroy": true
                        });

                        // đánh STT cho row
                        // tham khảo từ https://datatables.net/examples/api/counter_columns.html
                        DifferentTable.on('order.dt search.dt', function () {
                            DifferentTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                                cell.innerHTML = i + 1;
                            });
                        }).draw();
                    },
                    error: function (e) {
                        //ẩn gif load data và hiện thông báo lỗi
                        $('#preload').hide();
                        $(".upload-error").show().text(Common.object.uploadError);
                        //Hiện button download và upload
                        $("#btnUpload").prop("disabled", false);
                        $("#btnDownload").prop("disabled", false);
                        $("#btnUpToDB").prop("disabled", false);
                    }
                });
            }
        }
    }

    /**
     * Phương thức download dữ liệu địa chỉ thành file json
     * @param {any} event
     * created by bvbao (30/7/2020)
     */
     downloadFileJson(event) {
        let $saveBtn = $(event.target);
        let $modal = $saveBtn.closest('.card'); // modal hiện tại
        $(".upload-error").hide();

        let host = Common.defineDomainAPI().toString();
         if (this.allLocation) {
             var url = `${host}/synchronized/loc-downloadjson`;
            $.ajax({
                type: "GET",
                url: url,
                success: function (res) {
                    window.open(url, '_blank');
                },
                error: function (e) { debugger }
            })
        }
        else {
            this.showErrorUpload($modal);
        }
    }

    /**
     * Phương thức download dữ liệu địa chỉ thành file excel
     * @param {any} event
     * created by bvbao (7/10/2020)
     */
    downloadFileExcel(event) {
        let $saveBtn = $(event.target);
        let $modal = $saveBtn.closest('.card'); // modal hiện tại
        $(".upload-error").hide();

        let host = Common.defineDomainAPI().toString();
        if (this.allLocation) {
            var url = `${host}/synchronized/loc-downloadexcel`;
            $.ajax({
                type: "GET",
                url: url,
                success: function (res) {
                    window.open(url, '_blank');
                },
                error: function (e) { debugger }
            })
        }
        else {
            this.showErrorUpload($modal);
        }
    }

    /**
     * Phương thức đẩy dữ liệu đã đồng bộ ID lên database ES
     * Create by bvbao (3/8/2020)
     * */
    updateDataToElasticsearch(event) {
        let $saveBtn = $(event.target);
        let $modal = $saveBtn.closest('.card'); // modal hiện tại
        $('#preload').show();
        $(".upload-error").hide();

        let host = Common.defineDomainAPI().toString();
        if (this.allLocation) {
            $.ajax({
                type: "POST",
                url: `${host}/synchronized/loc-updatedb`,
                success: function (res) {
                    $('#preload').hide();
                    alert(Common.object.dbSuccess);
                },
                error: function (e) {
                    $('#preload').hide();
                    alert(Common.object.dbError);
                }
            })
        }
        else {
            $('#preload').hide();
            this.showErrorUpload($modal);
        }
    }

    /**
     * Phương thức show các lỗi khi chưa upload file mà thực hiện download và update3
     * @param {any} $modal modal upload file đang xét
     * Created by bvbao (7/10/2020)
     */
    showErrorUpload($modal) {
        $modal.find('.check-file-upload').blur();
        let allAreValid = true;
        let $errors = $modal.find('.input-error'); // span thông báo lỗi
        $.each($errors, function (index, item) {
            if (item.innerText != '') {
                allAreValid = false;
            }
        });

        if (allAreValid) {
            $(".upload-error").show().text(Common.object.uploadFile);
        }
    }
}