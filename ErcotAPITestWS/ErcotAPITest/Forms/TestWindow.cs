using System;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
//using ErcotAPITest.ErcotNodalService;
//using ErcotAPITest.ErcotNodalService;
using ErcotAPILib.ErcotNodalService;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;
using Microsoft.Web.Services3.Security;
using ErcotAPILib.Utils;
using ErcotAPILib.MarketInfo;
using System.Xml;
using System.Timers;

namespace ErcotAPITest.Forms
{
    public class TestWindow : Form
    {

        private Button _button1;
        private FlowLayoutPanel _flowLayoutPanel;
        private MarketInfo _marketInfo;
        private System.Timers.Timer _lmpQueryTimer;

       

        public TestWindow()
        {
            _button1 = new Button();
            _button1.Text = "Test";
            _button1.Click += ButtonOneClickHandler;

            _flowLayoutPanel = new FlowLayoutPanel();
            _flowLayoutPanel.Controls.Add(_button1);
           
            this.Controls.Add(_flowLayoutPanel);

            _marketInfo = new MarketInfo();

           
        }

        

        /// <summary>
        /// This EventHandler method is assigned to the _button1.Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOneClickHandler(object sender, EventArgs e)
        {


            this.StartLMPQueries(10000);


        }



      



        /// <summary>
        /// This callback method validates the Ercot cert.  It's not necessary for this example but it came in handy during troubleshooting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
    
        private bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                Console.WriteLine("Cert looks good!");
                return true;
            }

            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'", cert.Subject, error.ToString());
            return false;
        }



        private void StartLMPQueries(int timerIntervalMilliseconds)
        {
            _lmpQueryTimer = new System.Timers.Timer(timerIntervalMilliseconds);
            _lmpQueryTimer.Elapsed += QueryLmpService;
            _lmpQueryTimer.AutoReset = true;
            
            _lmpQueryTimer.Start();

        }


        private void QueryLmpService(object sender, EventArgs e)
        {
            if ((DateTime.Now.Minute % 5) == 0)
            {

                List<Lmp> lmpList = _marketInfo.GetRtmLmps();
                
                if (lmpList != null)
                {
                    foreach (Lmp lmp in lmpList)
                    {
                        Console.WriteLine(lmp.ToString());
                    }
                }
            }
            else
            {
                StaticLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Service call too early. Minutes must be on the 5s.");
            }
        }

}

  
}
