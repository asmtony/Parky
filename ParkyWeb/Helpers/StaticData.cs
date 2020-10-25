using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Helpers
{
    public static class StaticData
    {
        public static string APIBaseUrl = "https://localhost:44354/";
        public static string NationalParkApiPath = APIBaseUrl + "api/v1/nationalparks";
        public static string TrailAPIPAth = APIBaseUrl + "api/v1/trails";


    }
}
