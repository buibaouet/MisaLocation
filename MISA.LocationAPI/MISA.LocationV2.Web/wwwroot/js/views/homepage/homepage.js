$(document).ready(function () {
    var home = new Home();
});

class Home {
    static defaultButtonTexts = ["Select a country", "Select a city/province", "Select a district", "Select a ward"];
    constructor() {
        this.currentActivePos = -1;
        this.currentActiveItem = null;
        this.initEvents();
        Common.loadEachKind(0, $('.loclist'));
    }

    /**
     * Phương thức khởi tạo các sự kiện gán cho các thành phần
     */
    initEvents() {
        $('.loclist').on('click', 'a', this.menuItemOnClick.bind(this));

        $('.dropdown').on('shown.bs.dropdown', function () {
            $(this).find('.mini-search').focus();
        });

        $('.mini-search').on("input", Common.searchByInput.bind(this));

        $('.mini-search').keydown(Common.autocompleteKeyboard.bind(this));
    }

    /**
     * Phương thức được kích hoạt khi một item trong menu địa chỉ được click
     * @param {any} event sự kiện click được gán cho item
     * created by nmthang (15/06/2020)
     */
    menuItemOnClick(event) {
        let $menuItem = $(event.target);
        let $loclist = $('.loclist'); // tập các dropdown-menu chứa danh sách địa chỉ
        let $locs = $('.loc'); // tập các input
        let length = $loclist.length;
        let locID = $menuItem.attr('loc-id');
        let idx = $menuItem.parent().index('.loclist'); // lấy ra vị trí của dropdown-menu đang hiện trong tất cả các dropdown-menu
        $locs.eq(idx).text($menuItem.text());

        // khi một cấp đang được select thì các hiển thị cấp dưới sẽ phải xóa đi
        for (let i = idx + 1; i < length; i++) {
            $loclist.eq(i).children('a.dropdown-item').remove();
            $locs.eq(i).text(Home.defaultButtonTexts[i]);
        }

        // reset lại ô tìm kiếm và toàn bộ menu
        $menuItem.parent().find('input.mini-search').val('');
        $menuItem.siblings('a.dropdown-item[style*="display: none"]').show();

        if (idx < length - 1) {
            Common.loadEachKind(idx + 1, $('.loclist'), locID);
        }
        if (this.currentActiveItem != null) {
            this.currentActiveItem.removeClass('active');
            this.currentActiveItem = null;
        }
        this.currentActivePos = -1;
    }
}