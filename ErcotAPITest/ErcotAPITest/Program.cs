using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ErcotAPITest.ErcotNodalService;
using ErcotAPITest.ErcotNodalService;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.Services3;





namespace ErcotAPITest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Attempting to call Ercot Get SystemStatus service...");

            try
            {
                OperationsClient ercotClient = new OperationsClient();

                
                /***********************************************/

                X509Certificate2 cert = new X509Certificate2(@"C:\Users\swodog\Desktop\Projects\ErcotAPITest\ErcotAPITest\Cert\API_103_CERT.pfx", "a1e5i9" );

                ercotClient.ClientCredentials.ClientCertificate.SetCertificate(cert.Subject, StoreLocation.CurrentUser, StoreName.My);

               



                /***********************************************/




                MarketTransactionsRequest requestmsg = new MarketTransactionsRequest();
                requestmsg.RequestMessage = new RequestMessage();
                requestmsg.RequestMessage.Header = new HeaderType();
                requestmsg.RequestMessage.Header.Verb = HeaderTypeVerb.get;
                requestmsg.RequestMessage.Header.Noun = "SystemStatus";
                requestmsg.RequestMessage.Header.Source = "IMRE";
                requestmsg.RequestMessage.Header.UserID = "API_103";
                
               

            
              

                MarketTransactionsResponse response = ercotClient.MarketInfo(requestmsg);

            }
            catch(Exception e)
            {

                Console.WriteLine(e);
                Console.ReadLine();
            }

        }
    }
}
