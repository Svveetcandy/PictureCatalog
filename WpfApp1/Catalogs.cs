using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Catalogs
    {
        public List<string> CatalogsName { get; set; }
        public string CatalogsLoction { get; set; } = "Picture Catalogs";
        public Catalogs()
        {
            string[] arr = Directory.GetFiles(CatalogsLoction);
            CatalogsName = new List<string>(arr);
        }
    }
}
