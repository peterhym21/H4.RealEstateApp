using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Models
{
    //[AddINotifyPropertyChangedInterface]
    public class LoginResult
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string Refreshtoken { get; set; }

    }
}
