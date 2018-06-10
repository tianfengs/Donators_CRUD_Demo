using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donators_CRUD_Demo
{
    class DonatorsProvinceViewModel
    {
        public string ProV { get; set; }
        public ICollection<Donator> DonatorList { get; set; }
    }
}
