using K4os.Compression.LZ4.Internal;
using Nop.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{

    public class MessageReturn2
    {
        public const string SUCCESS_CODE = "00";
        public const string ERROR_CODE = "01";
        public const string EXISTS_CODE = "02";
        public const string NOT_FOUND_CODE = "04";
        public MessageReturn2() { }
        public MessageReturn2(string _code, string _msg)
        {
            Code = _code;
            Message = _msg;
        }

        public string Code { get; set; }
        public string Message { get; set; }
        public string IdRecord { get; set; }
        public bool IsSuccess { get; set; }
        /// <summary>
        /// Co the luu struct 1 object, hoac 1 list object
        /// </summary>
        public object ObjectInfo { get; set; }

        public static MessageReturn2 CreateSuccessMessage(string _msg = "Ok", object _objectInfo = null)
        {
            var msgret = new MessageReturn2(SUCCESS_CODE, _msg);
            msgret.ObjectInfo = _objectInfo;
            msgret.IsSuccess = true;
            return msgret;
        }
        public static MessageReturn2 CreateErrorMessage(string _msg, object _objectInfo = null)
        {
            var msgret = new MessageReturn2(ERROR_CODE, _msg);
            msgret.ObjectInfo = _objectInfo;
            msgret.IsSuccess = false;
            return msgret;
        }
        public static MessageReturn2 CreateExistsMessage(string _msg, object _objectInfo = null)
        {
            var msgret = new MessageReturn2(EXISTS_CODE, _msg);
            msgret.ObjectInfo = _objectInfo;
            msgret.IsSuccess = false;
            return msgret;
        }
        public static MessageReturn2 CreateNotFoundMessage(string _msg, object _objectInfo = null)
        {
            var msgret = new MessageReturn2(NOT_FOUND_CODE, _msg);
            msgret.ObjectInfo = _objectInfo;
            msgret.IsSuccess = false;
            return msgret;
        }
    }
}