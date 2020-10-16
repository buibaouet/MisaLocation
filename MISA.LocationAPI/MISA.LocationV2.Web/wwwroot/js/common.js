$(document).ready(function () {
    var common = new Common();
});

class Common {
    /**
     *Các const messages
     */
    static object = {
        apikey: 'gwf6qezbungnbujwz5bvxn834rvm5kgy',
        project: 'rd-test',
        uploadFile: "*Please upload a file Excel!",
        excelFile: "*Please sellect file .xls or .xlsx!",
        uploadError: "*An error occurred while uploading Excel file*",
        dbSuccess: "Upload data to database successfully.",
        dbError: "Upload data to database failed.",
        nameRequired: 'Location name is required!',
        countryRequired: 'Please select a country first!',
        idRequired: "ID is required!",
        formatID: "Invalid format of ID",
        matchID: "ID does not match its ParentID!",
        lengthID: "Length of ID does not match its kind!",
        idExist: "ID already existed or something went wrong!",
        ajaxFail: "Ajax call failed!"
    };

    /**
     * Hàm get các địa chỉ con theo kind và parentId và đổ dữ liệu ra dropdown
     * @param {any} kind Loại địa chỉ
     * @param {any} $menu Vị trí cần đổ dữ liệu 
     * @param {any} parentID Mã cha cần get dữ liệu con
     * Created by nmthang
     * Modified by bvbao
     */
    static loadEachKind(kind, $menu, parentID = "") {
        let host = Common.defineDomainAPI().toString();
        let apiUrl =  `${host}/loc?kind=${kind}&parentID=${parentID}`;
        $.ajax({
            url: apiUrl,
            type: "GET",
            headers: {
                'apikey': Common.object.apikey,
                'project': Common.object.project
            },
            success: function (res) {
                if (!res.data) return;
                // các địa chỉ trả về được lưu trong trường data của response
                // sắp xếp các địa chỉ theo thứ tự từ điển
                let data = res.data.sort(function (listLocationSorted, listLocation) {
                    return listLocationSorted.LocationName.localeCompare(listLocation.LocationName);
                });

                //Lấy Việt Nam cho lên đầu list quốc gia
                if (kind == 0) {
                    let vietnam;
                    for (var i = data.length - 1; i >= 0; i--) {
                        if (data[i].ID == 'VN') {
                            vietnam = data[i];
                            data.splice(i, 1);
                            break;
                        }
                    }
                    data.unshift(vietnam);
                }

                $.each(data, function (index, loc) {
                    // tạo ra một thẻ <a> mới và thêm vào dropdown menu
                    let aEle = $('<a></a>');
                    aEle.addClass('dropdown-item').text(loc.LocationName).attr({
                        'loc-id': loc.ID,
                        'loc-location-id': loc.LocationID,
                        'title': loc.LocationName,
                        'loc-parent-id': parentID,
                        'loc-created-date': loc.CreatedDate,
                        'loc-modified-date': loc.ModifiedDate
                    });
                    $menu.eq(kind).append(aEle);
                });
            }
        });
    }

    /**
     * Phương thức tìm kiếm đơn giản địa chỉ bất kỳ trong một dropdown menu
     * @param {any} event sự kiện được gán cho input tìm kiếm
     */
    static searchByInput(event) {
        // phải xóa active của kết quả tìm kiếm liền trước trước khi sự kiện của hàm này xảy ra
        if (this.currentActiveItem != null) {
            this.currentActiveItem.removeClass("active");
        }

        // reset lại active position
        this.currentActivePos = -1;
        this.currentActiveItem = null;

        let $input = $(event.target);
        let locName = $input.val();
        let locItems = $input.parent().nextAll('a.dropdown-item');
        $.each(locItems, function (index, loc) {
            if (loc.innerText.toLowerCase().indexOf(locName.toLowerCase()) != -1) {
                $(loc).show();
            } else $(loc).hide();
        });

    }

    /**
     * Phương thức xử lý thao tác bàn phím với dropdown menu chứa các địa chỉ
     * @param {any} event
     * created by nmthang (22/06/2020)
     */
    static autocompleteKeyboard(event) {
        let $input = $(event.target);
        let $dropdownMenu = $input.closest('.dropdown-menu');
        let itemHeight = $dropdownMenu.children("a.dropdown-item").first().innerHeight();

        // những item đang được show
        let $shownItems = $input.parent().nextAll().filter('a.dropdown-item:not([style*="display: none"])');
        let len = $shownItems.length;

        // bấm nút mũi tên xuống
        if (event.keyCode == 40) {
            // tăng thứ tự ô đang focus
            this.currentActivePos++;
            // xóa focus của ô cũ
            $shownItems.eq(this.currentActivePos - 1).removeClass("active"); // nếu ở dòng này this.currentActivePos = 0 thì $shownItems.eq(this.currentActivePos - 1) là item đứng cuối
            // tính toán lại currentActivePos (nếu chạy đến cuối danh sách phải quay lên đầu)
            this.currentActivePos = (this.currentActivePos < len) ? this.currentActivePos : 0;
            // đặt active item hiện tại
            this.currentActiveItem = $shownItems.eq(this.currentActivePos);
            // thêm focus cho ô mới
            this.currentActiveItem.addClass("active");
        }

        // bấm nút mũi tên lên
        else if (event.keyCode == 38) {
            // giảm thú tự ô focus
            this.currentActivePos--;
            // xóa active của ô cũ
            $shownItems.eq(this.currentActivePos + 1).removeClass("active");
            // tính toán lại currentActivePos (nếu chạy lên đầu danh sách phải quay xuống cuối)
            this.currentActivePos = (this.currentActivePos > -1) ? this.currentActivePos : (len - 1);
            // đặt active item hiện tại
            this.currentActiveItem = $shownItems.eq(this.currentActivePos);
            // thêm focus cho ô mới
            this.currentActiveItem.addClass("active");
        }

        // bấm phím enter
        else if (event.keyCode == 13) {
            // kích hoạt sự kiện click của item
            if (this.currentActivePos > -1) {
                event.preventDefault();
                event.stopPropagation();
                $shownItems.eq(this.currentActivePos).click();
            }
        }

        // menu scroll khi di chuyển bằng mũi tên lên xuống
        $dropdownMenu.scrollTop((this.currentActivePos - 5) * itemHeight);
    }

    /**
     * Phương thức định nghĩa host domain cho các API tùy cho từng môi trường
     * Created by bvbao (25/9/2020)
     * */
    static defineDomainAPI() {
        var origin = window.location.origin;
        if (origin.includes("localhost")) {
            return "https://localhost:44399";
        }
        else if (origin.includes("test")) {
            return "https://test-rd.misa.com.vn/location";
        }
        else {
            return "http://10.0.6.58:31198";
        }
    }
}