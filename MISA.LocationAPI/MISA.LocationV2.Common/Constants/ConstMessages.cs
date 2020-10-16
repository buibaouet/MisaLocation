using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.LocationAPI.Constants
{
    /// <summary>
    /// Static class Constants chứa các hằng hay được sử dụng trong chương trình
    /// Created by nmthang - 09/06/2020
    /// </summary>
    public static class ConstMessages
    {
        public const int InvalidKeyCode = 1000;
        public const string InvalidKeyMsg = "Project or apikey is invalid";

        public const int InvalidInputDataCode = 1001;
        public const string InvalidInputDataMsg = "Input data is invalid!";

        public const int QueryFailedCode = 1002;
        public const string QueryFailedMsg = "Query failed to database!";

        public const int UnidentifiedErrorCode = 1003;
        public const string UnidentifiedErrorMsg = "Unidentified error!";

        public const int DataNotFoundCode = 1004;
        public const string DataNotFoundMsg = "Data not found!";

        public const int LongChangeLogsCode = 1005;
        public const string LongChangeLogsMsg = "Change logs is too long, please use api sync_all";
        
        public const string InsertSuccess = "Insert location to the database successfully!";
        public const string UpdateSuccess = "Update location to the database successfully!";
        public const string DeleteSuccess = "Delete location successfully!";
    }

}
