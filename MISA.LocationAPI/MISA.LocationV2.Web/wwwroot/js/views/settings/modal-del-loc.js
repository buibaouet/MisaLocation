$(document).ready(function () {
    var modalDelete = new ModalDelete();
});

class ModalDelete {
    constructor() {
        this.initEvents();
    }

    /**
     * Phương thức initEvents khởi tạo các sự kiện trong modal xóa
     * created by nmthang (10/06/2020)
     * modified by bvbao (6/7/2020)
     */
    initEvents() {
        $('#btn-del-loc').on('click', function () {
            $('#modal-del-loc .hidden-row').hide();
            $('#modal-del-loc .showmore-row').show();
            let $menus = $('#modal-del-loc').find('.dropdown-menu');
            Common.loadEachKind(0, $menus);
        });

        $('#del-kind').attr('kind', -1).text('');
        $('#del-btn-country').on('blur', this.delLocationOnBlur.bind(this));
        //Khi chọn một địa chỉ trong dropdown thì ẩn span đi
        $('.dropdown-menu').click(function () {
            $(this).parent().next().text('').hide();
        });
    }

    /**
     * Phương thức kiểm tra đã chọn địa chỉ nào để xóa hay chưa
     * @param {any} event
     * created by bvbao (6/7/2020)
     */

    delLocationOnBlur(event) {
        let input = event.target;
        let $error = $(input).parent().next();

        // kiểm tra nếu chưa chọn địa chỉ nào
        if ($('#del-btn-country').text() == Setting.defaultButtonTexts[0]) {
            $error.show().text(Common.object.nameRequired);
        }
        else {
            $error.text('').hide();
        }
    }

    /**
     * Gọi API để xóa dữ liệu
     * @param {any} data
     * Created by: bvbao - 10/6/2020
     */
    static submitDelete(id) {
        var result = '';
        let host = Common.defineDomainAPI().toString();
        let apiUrl = `${host}/settings/${id}`;
        $.ajax({
            url: apiUrl,
            type: "DELETE",
            data: '',
            headers: {
                'apikey': Common.object.apikey,
                'project': Common.object.project
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
        }).done(function (res) {
            if (res.code == 200) {
                result = 'Success';
            }
            else {
                result = 'Error';
            }
        }).fail(function (res) {
            result = 'Error';
        })
        return result;
    }
}