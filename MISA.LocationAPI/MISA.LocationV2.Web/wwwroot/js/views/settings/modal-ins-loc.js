$(document).ready(function () {
    var modalInsert = new ModalInsert();
});

class ModalInsert {
    constructor() {
        this.initEvents();
    }

    /**
     * Phương thức initEvents khởi tạo các sự kiện trong modal thêm mới
     * created by nmthang (09/06/2020)
     */
    initEvents() {
        $('#btn-ins-loc').on('click', function () {
            let $menus = $('#modal-ins-loc').find('.dropdown-menu');
            Common.loadEachKind(0, $menus);
        });

        $('#ins-kind').attr('kind', 0).text(Setting.kinds[0]);

        $('#ins-location-name').on('blur', this.insLocationNameOnBlur.bind(this));

        $('#ins-id').on('blur', this.insIdOnBlur.bind(this));

        $('#ins-location-id').on('blur', this.insLocationIdOnBlur.bind(this));
    }

    /**
     * Phương thức được kích hoạt khi xảy ra sự kiện blur cho input LocationName
     * @param {any} event sự kiện (blur) được gán cho input
     * created by nmthang (19/06/2020)
     */
    insLocationNameOnBlur(event) {
        let input = event.target;
        let $error = $(input).parent().next();
        if (!input.value) {
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
    insIdOnBlur(event) {
        let input = event.target;
        let $error = $(input).parent().next(); // thẻ span hiện thông báo lỗi

        // validate ID
        let kind = parseInt($('#ins-kind').attr('kind'));
        let parentID = $('#ins-parent-id').text();
        let validateObj = Validation.validateLocationID("ID", input.value, kind, parentID);
        if (!validateObj.idIsValid) {
            $error.show().text(validateObj.msg);
        }
        else {
            $error.text('').hide();
        }
    }

    /**
     *  Phương thức được kích hoạt khi xảy ra sự kiện blur cho input LocationID
     * @param {any} event sự kiện (blur) được gán cho input
     * created by bvbao (19/8/2020)
     */
    insLocationIdOnBlur(event) {
        let input = event.target;
        let $error = $(input).parent().next(); // thẻ span hiện thông báo lỗi

        // validate ID
        let kind = parseInt($('#ins-kind').attr('kind'));
        let parentID = $(input).parents("td").next().text();
        let validateObj = Validation.validateLocationID("LocationID", input.value, kind, parentID);
        if (!validateObj.idIsValid) {
            $error.show().text(validateObj.msg);
        }
        else {
            $error.text('').hide();
        }
    }

    /**
	 * Gọi API để thêm mới dữ liệu
	 * @param {any} data Dữ liệu về địa chỉ cần thêm vào database
	 * Created by: bvbao - 10/6/2020
	 */
    static submitInsert(data) {
        var result = '';
        let host = Common.defineDomainAPI().toString();
        let apiUrl = `${host}/settings/${data.ID}`;
        $.ajax({
            url: apiUrl,
            type: "POST",
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