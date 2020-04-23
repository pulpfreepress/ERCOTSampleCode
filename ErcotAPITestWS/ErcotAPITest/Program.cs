using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErcotAPITest.ErcotNodalService;
//using ErcotAPITest.ErcotNodalService;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Messaging;
using ErcotAPITest.Forms;
using System.Windows.Forms;





namespace ErcotAPITest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Application.Run(new TestWindow());
        }
    }
}
