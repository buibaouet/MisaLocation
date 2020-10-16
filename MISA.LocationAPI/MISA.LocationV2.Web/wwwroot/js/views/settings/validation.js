class Validation {
    constructor() {

    }

   /**
    * Phương thức validate ID mới được nhập vào
    * @param {string} type_param Loại thuộc tính cần validate (ID hoặc LocationID)
    * @param {string} id ID mới được nhập vào
    * @param {number} kind cấp của địa chỉ tại thời điểm nhập ID mới
    * @param {string} parentID mã cha của địa chỉ tại thời điểm nhập ID mới
    * @param {string} oldID giá trị ID hiện tại tại thời điểm nhập ID mới
    * created by nmthang (30/06/2020)
    * modified by bvbao (19/8/2020)
    */
    static validateLocationID(type_param, id, kind, parentID, oldID = "") {
        let validateObj = {
            idIsValid: true,
            msg: Common.object.idRequired
        };

        // kiem tra ID có trống không
        if (!id) {
            validateObj.idIsValid = false;
            return validateObj;
        }

        // kiểm tra của format của ID ([A-Z]{2}[0-9]{0, 10})
        validateObj.msg = Common.object.formatID;
        var regex = new RegExp("^[A-Z]{2}[0-9]{0,10}$");
        validateObj.idIsValid = regex.test(id);
        if (!validateObj.idIsValid) {
            return validateObj;
        }

        // kiem tra ID co chua ma cha khong
        validateObj.msg = Common.object.matchID;
        validateObj.idIsValid = (id.indexOf(parentID) == 0);
        if (!validateObj.idIsValid) {
            return validateObj;
        }

        // kiem tra do dai ID co phu hop voi kind khong
        validateObj.msg = Common.object.lengthID;
        validateObj.idIsValid = Validation.validateIDLengthByKind(type_param, id.length, kind);
        if (!validateObj.idIsValid) {
            return validateObj;
        }

        // kiem tra ID da ton tai trong database hay chua
        validateObj.msg = Common.object.idExist;
        validateObj.idIsValid = !Validation.checkDuplicateID(type_param, id, oldID);
        if (!validateObj.idIsValid) {
            return validateObj;
        }

        // vuot qua kiem tra
        validateObj.msg = "";
        return validateObj;
    }

    /**
     * Phương thức validate độ dài của ID theo cấp của địa chỉ
     * @param {number} idLen độ dài ID của địa chỉ
     * @param {number} kind cấp của địa chỉ
     * created by nmthang (30/06/2020)
     * modified by bvbao (19/8/2020)
     */
    static validateIDLengthByKind(type_param, idLen, kind) {
        if (type_param == "ID") {
            switch (kind) {
                case 0:
                    if (idLen == 2) {
                        return true;
                    }
                    break;
                case 1:
                    if (idLen == 4) {
                        return true;
                    }
                    break;
                case 2:
                    if (idLen == 7) {
                        return true;
                    }
                    break;
                case 3:
                    if (idLen == 12) {
                        return true;
                    }
                    break;
            }
        }
        else {
            switch (kind) {
                case 0:
                    if (idLen == 2) {
                        return true;
                    }
                    break;
                case 1:
                    if (idLen == 5) {
                        return true;
                    }
                    break;
                case 2:
                    if (idLen == 7) {
                        return true;
                    }
                    break;
                case 3:
                    if (idLen == 9) {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }

   /**
    * Phương thức kiểm tra ID đưa vào đã tồn tại trong database chưa
    * @param {string} id ID mới thay đổi cần kiểm tra
    * @param {string} oldID ID cũ trước khi thay đổi
    * modified by bvbao (19/8/2020)
    */
    static checkDuplicateID(type_param, id, oldID) {
        let url = '';
        let host = Common.defineDomainAPI().toString();
        if (type_param == "ID") {
            url = `${host}/settings/exist/${id}`;
        } else {
            url = `${host}/settings/exist/locationid/${id}`;
        }
        
        let duplicate = false;
        if (oldID != id) {
            $.ajax({
                method: "GET",
                url: url,
                headers: {
                    'apikey': Common.object.apikey,
                    'project': Common.object.project
                },
                async: false,
                success: function (res) {
                    // không return được ở đây
                    duplicate = res;
                },
                error: function () {
                    console.log(Common.object.ajaxFail);
                    // return false;
                }
            });
        }
        return duplicate;
    }
}