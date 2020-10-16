using Microsoft.AspNetCore.Mvc;
using MISA.LocationAPI.Models;
using MISA.LocationV2.Entities;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.LocationV2.BL.Services
{
    /// <summary>
    /// Interface định ra các cài đặt chính cho các service thực hiện
    /// </summary>
    public interface ILocationService
    {
        public void ReadLocationChanges(ISheet sheet, List<LocationChangesModel> listParam, int colCount);
        public IEnumerable<LocationChangesModel> GetLocationChanges();
        public bool IsChangedRow(IRow row, int comparePos, int colCount);
        public bool IsExistedID(string id);
        public Task<bool> IsExistedLocationIDAsync(string locationId);
    }
}
