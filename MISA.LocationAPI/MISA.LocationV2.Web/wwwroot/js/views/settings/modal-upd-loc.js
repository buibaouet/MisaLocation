$(document).ready(function () {
    var modalUpdate = new ModalUpdate();
});

class ModalUpdate {
    constructor() {
        this.initEvents();
    }

    /**
     * Phương thức initEvents khởi tạo các sự kiện trong modal chỉnh sửa
     * created by nmthang (10/06/2020)
     */
    initEvents() {
        $('#btn-upd-loc').on('click', function () {
            $('#modal-upd-loc .hidden-row').hide();
            $('#modal-upd-loc .showmore-row').show();
            let $menus = $('#modal-upd-loc').find('.dropdown-menu');
            Common.loadEachKind(0, $menus);
        });

        $('#upd-kind').attr('kind', -1).text('');

        $('#upd-location-name').on('blur', this.updLocationNameOnBlur.bind(this));

        $('#upd-id').on('blur', this.updIdOnBlur.bind(this));

        $('#upd-location-id').on('blur', this.updLocationIdOnBlur.bind(this));
    }

    /**
    * Phương thức được kích hoạt khi xảy ra sự kiện blur cho input LocationName
    * @param {any} event sự kiện (blur) được gán cho input
    * created by nmthang (19/06/2020)
    */
    updLocationNameOnBlur(event) {
        let input = event.target;
        let $error = $(input).parent().next();
        // kiểm tra nếu chưa chọn quốc gia trước thì báo lỗi khi nhập LocationName
        if ($('#upd-btn-country').text() == Setting.defaultButtonTexts[0]) {
            $error.show().text(Common.object.countryRequired);
        }
        // nếu chọn rồi mà chưa nhập LocationName
        else if (!input.value) {
            $error.show().text(Common.object.nameRequired);
        }
        else {
            $error.text('').hide();
        }
    }

    /**
     * Phương thức được kích hoạt khi xảy ra sự kiện blur cho input ID
     * @param {any} event sự kiện (blur) được gán cho input
     * created by nmthang (19/06/2020)
     * modified by nmthang (30/06/2020)
     */
    updIdOnBlur(event) {
        let input = event.target;
        let $error = $(input).parent().next(); // thẻ span hiện thông báo lỗi

        if ($('#upd-btn-country').text() == Setting.defaultButtonTexts[0]) {
            $error.show().text(Common.object.countryRequired);
        }
        else {
            // validate ID
            let kind = parseInt($('#upd-kind').attr('kind'));
            let parentID = $('#upd-parent-id').text();
            let oldID = $('#modal-upd-loc .des-loc').eq(kind).text();
            let validateObj = Validation.validateLocationID("ID", input.value, kind, parentID, oldID);
            if (!validateObj.idIsValid) {
                $error.show().text(validateObj.msg);
            }
            else {
                $error.text('').hide();
            }
        }
    }

    /**
     * Phương thức được kích hoạt khi xảy ra sự kiện blur cho input LocationID
     * @param {any} event sự kiện (blur) được gán cho input
     * created by bvbao (19/8/2020)
     */
    updLocationIdOnBlur(event) {
        let input = event.target;
        let $error = $(input).parent().next(); // thẻ span hiện thông báo lỗi

        if ($('#upd-btn-country').text() == Setting.defaultButtonTexts[0]) {
            $error.show().text(Common.object.countryRequired);
        }
        else {
            // validate ID
            let kind = parseInt($('#upd-kind').attr('kind'));
            let oldLocationID = $(input).parents("td").next().text();
            let parentID = (kind == 0) ? "" :
                (kind == 1) ? oldLocationID.substring(0, 2) :
                    (kind == 2) ? oldLocationID.substring(0, 5) : oldLocationID.substring(0, 7);
 
            let validateObj = Validation.validateLocationID("LocationID", input.value, kind, parentID, oldLocationID);
            if (!validateObj.idIsValid) {
                $error.show().text(validateObj.msg);
            }
            else {
                $error.text('').hide();
            }
        }
    }

    /**
     * Gọi API để cập nhật dữ liệu
     * @param {any} data
     * Created by: bvbao - 10/6/2020
     */
    static submitUpdate(data, oldId) {
        let host = Common.defineDomainAPI().toString();
        var result = '';
        let apiUrl = `${host}/settings/${oldId}`;
        $.ajax({
            url: apiUrl,
            type: "PUT",
            headers: {
                'apikey': Common.object.apikey,
                'project': Common.object.project
            },
            data: JSON.stringify(data),
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
