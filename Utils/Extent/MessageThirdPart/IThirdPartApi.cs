using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.ExtentHelper.MessageThirdPart
{
    public interface IThirdPartApi
    {
        string GetPhoneNumber(int _productId, int _provinceId, int _cityId, int _operatorId, string _token, string _phoneNumber, string _userName);

        string GetVCode(string _phoneNumber, string _userName, string _productId, string _token);

       

    }
}
