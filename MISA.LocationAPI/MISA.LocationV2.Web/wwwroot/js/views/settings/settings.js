$(document).ready(function () {
    var setting = new Setting();
});

class Setting {
    static defaultButtonTexts = ["Select a country", "Select a city/province", "Select a district", "Select a ward"];
    static kinds = ["Country", "City/Province", "District", "Ward"];
    static actions = ["INSERT", "UPDATE", "DELETE"];

    constructor() {
        this.currentActivePos = -1;
        this.currentActiveItem = null;
        this.initEvents();
    }

    /**
     * Phương thức khởi tạo các sự kiện
     * created by nmthang (18/05/2020)
     * modified by nmthang (29/06/2020)
     * */
    initEvents() {
        $("#btn-refrs").on("click", function () {
            $("#changelogs").empty();
            $('#noChange').show();
        });
        $(".hide-accept-change").on("click", function () {
            $("#acceptModalCenter").hide();
            let $modalshow = document.querySelectorAll(".modal.show") // lấy các modal đang show
            let $modal = new jQuery.fn.init($modalshow[0]); // modal hiện tại
            //loại bỏ opacity cho modal
            $modal.css('opacity', '');
        });

        $('.dropdown').on('shown.bs.dropdown', function () {
            $(this).find('.mini-search').focus();
        });

        $('.accept-change').on('click', this.saveData.bind(this));

        $('button.dropdown-toggle').on('click', this.resetActiveItem);
            
        $('.mini-search').on("input", Common.searchByInput.bind(this));

        $('.mini-search').keydown(Common.autocompleteKeyboard.bind(this));

        $('.modal').on('click', 'a.dropdown-item', this.menuItemOnClick.bind(this)); // Các dropdown item được sinh ra một cách dynamically ở thời điểm sau

        $('.btn-showmore').on('click', this.btnShowMoreOnClick.bind(this));

        $('.btn-hide').on('click', this.btnHideOnClick.bind(this));

        $('.btn-save').on('click', this.btnSaveOnClick.bind(this));

        $('.btn-discard').on('click', this.clearAll.bind(this));

    }

    resetActiveItem() {
        // reset lại Active Item
        if (this.currentActiveItem != null) {
            this.currentActiveItem.removeClass('active');
            this.currentActiveItem = null;
        }
        this.currentActivePos = -1;
    }

    /**
     * Phương thức xử lý khi click vào nút Show more để hiển thị thêm thông tin
     * @param {any} event sự kiện xảy ra
     * created by nmthang (27/06/2020)
     */
    btnShowMoreOnClick(event) {
        event.preventDefault();
        let $button = $(event.target);
        let $trShow = $button.closest('tr');
        $trShow.hide();
        $trShow.nextAll('tr.hidden-row').show();
    }

    /**
     * Phương thức xử lý khi click vào nút Hide để ẩn thông tin
     * @param {any} event sự kiện xảy ra
     * created by nmthang (27/06/2020)
     */
    btnHideOnClick(event) {
        event.preventDefault();
        let $button = $(event.target);
        let $trHide = $button.closest('tr');
        $trHide.hide();
        $trHide.prevAll('tr.hidden-row').hide();
        $('.btn-showmore').closest('tr').show();
    }

    /**
     * Phương thức xử lý khi nút save trong các dialog được click
     * gồm validate dữ liệu nhập vào và show dialog xác nhận thay đổi
     * @param {any} event sự kiện xảy ra
     * created by nmthang (27/06/2020)
     * modified by bvbao (26/8/2020)
     */
    btnSaveOnClick(event) {
        let $saveBtn = $(event.target);
        let $modal = $saveBtn.closest('.modal'); // modal hiện tại
        $modal.find('.checked-info').blur(); // những thông tin cần validate (ID và LocationName)
        let allAreValid = true;
        let $errors = $modal.find('.input-error'); // span thông báo lỗi
        $.each($errors, function (index, item) {
            if (item.innerText != '') {
                allAreValid = false;
            }
        });

        if (allAreValid) {
            $modal.css("opacity", 0.85);
            $('#acceptModalCenter').show();
        }
    }

    /**
     * Phương thức xử lý lưu dữ liệu khi xác nhận thay đổi trong dialog xác nhận
     * @param {any} 
     * Created by bvbao(26/8/2020)
     */
    saveData() {
        //ẩn modal xác nhận
        $('#acceptModalCenter').hide(); 
        let $modalshow = document.querySelectorAll(".modal.show") // lấy các modal đang show
        let $modal = new jQuery.fn.init($modalshow[0]); // modal hiện tại
        let idx = $('.modal').toArray().indexOf($modal[0]); // chỉ số của modal hiện tại trong các tất cả các modal hiện có
        let locationName = '';
        let ID = '';
        let locationID = '';
        let $ID = $modal.find('.id');
        let $locationID = $modal.find('.location-id');

        if ($ID.hasClass('id-val')) {
            ID = $ID.val();
            locationID = $locationID.val();
            locationName = $modal.find('.location-name').val();
        }
        // nếu ô chứa LocationID không phải input
        else if ($ID.hasClass('id-text')) {
            ID = $ID.text();
            locationID = $locationID.text();
            locationName = $modal.find('.location-name').text();
        }
        //format thời gian và chỉnh về GMT+7
        var tzoffset = (new Date()).getTimezoneOffset() * 60000;
        let timeshow = (new Date(Date.now() - tzoffset)).toISOString().replace('T', ' ').replace(/\..+/, '');

        let kind = $modal.find('.kind').attr('kind'); //lấy giá trị kind từ form
        let parentID = $modal.find('.parent-id').text(); //lấy giá trị parentID từ form
        //Khởi tạo FullAddress cho địa chỉ
        let fullAdd = locationName;
        for (let idx = kind - 1; idx >= 0; idx--) {
            fullAdd += (", " + $modal.find(`.btn-input:eq(${idx})`).text());
        }
        //Tạo input cho trường Suggestion
        var input = {
            input: this.makeSuggestionLocation(locationName)
        }

        // Khởi tạo một địa chỉ mới với tất cả các trường thuộc tính
        var location = {
            ID: ID,
            LocationName: locationName,
            Kind: kind,
            ParentID: parentID,
            CountryID: (ID.length >= 2) ? ID.substring(0, 2) : "",
            PID: (ID.length >= 4) ? ID.substring(0, 4) : "",
            DID: (ID.length >= 7) ? ID.substring(0, 7) : "",
            FullAddress: fullAdd,
            ZIPCode: "",
            CreatedDate: new Date().toISOString().replace(/\..+/, 'Z'),
            CreatedBy: "bvbao",
            ModifiedDate: new Date().toISOString().replace(/\..+/, 'Z'),
            ModifiedBy: "bvbao",
            UsedCount: 0,
            SortOrder: 0,
            LocationID: locationID,
            ProvinceID: (locationID.length >= 5) ? locationID.substring(0, 5) : "",
            DistrictID: (locationID.length >= 7) ? locationID.substring(0, 7) : "",
            AreaCode: "",
            PostalCode: (ID.length >= 2) ? ID.substring(2) : "",
            LocationCode: "",
            Suggestion: input
        };

        var data = {
            location,
            oldID: $modal.find(`.des-loc:eq(${kind})`).text() //lấy mã cũ để update dữ liệu theo mã cũ
        }

        let result = '';
        if (Setting.actions[idx] == "INSERT") {
            result = ModalInsert.submitInsert(data.location);
        }
        else if (Setting.actions[idx] == "UPDATE") {
            result = ModalUpdate.submitUpdate(data.location, data.oldID);
        }
        else {
            result = ModalDelete.submitDelete(data.location.ID);
        }
        //Đổ dữ liệu ra bảng hiển thị với kết quả của truy vấn
        let style = (result == "Success") ? 'text-success' : 'text-danger';
        let trHTML = $('<tr></tr>');
        let len = $('#changelogs > tr').length;
        let tdHTML = `<td>${len + 1}</td>
					<td>${ID}</td>
					<td>${locationID}</td>
					<td>${locationName}</td>
					<td>${kind}</td>
					<td>${parentID}</td>
					<td>${timeshow}</td>
					<td>${Setting.actions[idx]}</td>
                    <td class = '${style}'>${result}</td>`;

        trHTML.append(tdHTML);
        $('#changelogs').append(trHTML);
        $('#noChange').hide();

        // xử lý xong xuôi thì xóa hết thông tin hiển thị trên modal và đóng modal
        // lợi dụng luôn nút discard
        $modal.css('opacity', '');
        $modal.find('.btn-discard').click();
    }

    /**
     * Gọi API lấy tạo trường Suggestion cho địa danh
     * @param {any} name tên địa danh cần tạo suggestion
     * Created by bvbao (6/8/2020)
     */
    makeSuggestionLocation(name) {
        var input = [];
        let host = Common.defineDomainAPI().toString();
        $.ajax({
            type: "GET",
            url: `${host}/synchronized/loc-suggestion?locationName=${name}`,
            headers: {
                'apikey': Common.object.apikey,
                'project': Common.object.project
            },
            async: false,
            success: function (res) {
                input = res;
            },
            error: function (e) { debugger }
        })

        return input;
    }

    /**
     * Phương thức xử lý sự kiện một item được chọn
     * @param {any} event sự kiện xảy ra
     * created by nmthang (29/06/2020)
     * modified by bvbao (17/8/2020)
     */
    menuItemOnClick(event) {
        let $menuItem = $(event.target); // item được click
        let $modal = $menuItem.closest('.modal'); // modal hiện tại
        let modal_id = $modal.attr('id'); // lấy ra id của modal hiện tại
        let $menus = $modal.find('.dropdown-menu'); // tập các dropdown menu trong modal hiện tại
        let $inputs = $menus.prev('.btn-input'); // tập các input (là button)
        let $desLocs = $menus.closest('td').next('.des-loc'); // tập các description (ID) tương ứng của từng cấp địa chỉ úng với tên
        let length = $menus.length;
        let locID = $menuItem.attr('loc-id');
        let locLocationID = $menuItem.attr('loc-location-id');
        let idx = $menus.toArray().indexOf($menuItem.parent()[0]); // lấy ra vị trí của dropdown-menu đang hiện trong tất cả các dropdown-menu của modal hiện tại
        $modal.find('#parentLocationID').text(locLocationID);

        // điền dữ liệu vào các vùng hiển thị thông tin
        $inputs.eq(idx).text($menuItem.text());
        $desLocs.eq(idx).text(locID);
        let $ID = $modal.find('.id');
        let $locationID = $modal.find('.location-id');

        // nếu ô chứa LocationID có dạng input
        if ($ID.hasClass('id-val')) {
            $ID.val(locID);
            $locationID.val(locLocationID);
            if (modal_id == "modal-upd-loc") {
                $modal.find('.location-name').val($menuItem.text());
            }
        }
        // nếu ô chứa LocationID không phải input
        else if ($ID.hasClass('id-text')) {
            $ID.text(locID);
            $locationID.text(locLocationID);
            if (modal_id == "modal-del-loc") {
                $modal.find('.location-name').text($menuItem.text());
            }
        }
        
        $modal.find('.created-date').text($menuItem.attr('loc-created-date'));
        $modal.find('.modified-date').text($menuItem.attr('loc-modified-date'));

        if (modal_id === "modal-ins-loc") {
            $modal.find('.kind').attr('kind', idx + 1).text(Setting.kinds[idx + 1]);
            $modal.find('.parent-id').text(locID);
        }
        else {
            $modal.find('.kind').attr('kind', idx).text(Setting.kinds[idx]);
            $modal.find('.parent-id').text($menuItem.attr('loc-parent-id'));
        }

        // khi một cấp đang được select thì các tên địa chỉ, các description và các dropdown-menu của cấp dưới phải được xóa đi
        for (let i = idx + 1; i < length; i++) {
            $menus.eq(i).children('a.dropdown-item').remove();
            $inputs.eq(i).text(Setting.defaultButtonTexts[i]);
            $desLocs.eq(i).text('');
        }

        // load cấp ngay dưới nó lên
        if (idx < length - 1) {
            Common.loadEachKind(idx + 1, $menus, locID);
        }

        // reset lại ô tìm kiếm và menu
        $menuItem.parent().find('input.mini-search').val('');
        $menuItem.siblings('a.dropdown-item[style*="display: none"]').show();

        // reset lại Active Item
        this.resetActiveItem();
    }

    /**
     * Phương thức xóa hết tất cả các hiển thị trong một modal
     * @param {any} event sự kiện xảy ra
     * created by nmthang (29/06/2020)
     */
    clearAll(event) {
        let $modal = $(event.target).closest('.modal');
        let modal_id = $modal.attr('id');
        let $menus = $modal.find('.dropdown-menu');
        $menus.children('a.dropdown-item').remove();
        $menus.closest('td').next().text('');
        let $ID = $modal.find('.id');
        let $locationID = $modal.find('.location-id');
        $modal.find('#parentLocationID').text('');
        // nếu ô chứa LocationID có dạng input
        if ($ID.hasClass('id-val')) {
            $ID.val('');
            $locationID.val('');
            $modal.find('.location-name').val('');
        }
        // nếu ô chứa LocationID không phải input
        else if ($ID.hasClass('id-text')) {
            $ID.text('');
            $locationID.text('');
            $modal.find('.location-name').text('');
        }

        if (modal_id === "modal-ins-loc") {
            $modal.find('.kind').attr('kind', 0).text(Setting.kinds[0]);
        }
        else {
            $modal.find('.kind').attr('kind', -1).text('');
        }

        // xóa hết các lỗi khi validate nếu có
        $modal.find('.input-error').text('').hide();

        $modal.find('.additional-info').text('');
        let $inputs = $menus.prev('.btn-input');
        for (let i = 0; i < $menus.length; i++) {
            $inputs.eq(i).text(Setting.defaultButtonTexts[i]);
        }

        // reset Active Item
        this.resetActiveItem();

        // ẩn modal
        $modal.modal("hide");
    }
}