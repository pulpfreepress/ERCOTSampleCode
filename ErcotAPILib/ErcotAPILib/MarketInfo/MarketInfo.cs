using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErcotAPILib.ErcotNodalService;
using ErcotAPILib.Utils;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;
using Microsoft.Web.Services3.Security;
using System.Net;
using System.Xml;

namespace ErcotAPILib.MarketInfo
{
    public class MarketInfo : ApplicationBase
    {


        /***********************************************************************************************************
         * Constants
         * *********************************************************************************************************/
        private readonly  string CLIENT_CERT_PATH = Environment.CurrentDirectory + @"\" +
                                                    Properties.Settings.Default.ClientCertificateDirectory + @"\" +
                                                    Properties.Settings.Default.ClientCertificateName;

        private readonly string CLIENT_CERT_PASSWORD = Properties.Settings.Default.ClientCertificatePassword;

        private readonly string CLIENT_CERT_USER_ID = Properties.Settings.Default.ClientCertificateUserID;

        private readonly string SOURCE = Properties.Settings.Default.Source;


        /*********************************************************************************************************
         * Fields
         * ******************************************************************************************************/
        private NodalService _ercotClient;
        private X509Certificate2 _clientCert;
        private X509Certificate2 _ercotMisApiCert;
        private SoapContext _context;
        private X509SecurityToken _token;
        private X509SecurityToken _token2;
        private List<Lmp> _lmpList;

        private enum MarketInfoCallType { LMPs };  // add more market call types in the future as required
        private MarketInfoCallType _marketCallType;

        public MarketInfo() : base(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        {

            LogDebug("Initializing Service Proxy");
            this.InitializeServiceProxy();

        }


        private void InitializeServiceProxy()
        {

            // Create NodalService proxy reference
            _ercotClient = new NodalService();

            // Load cert and sign SOAP message
            // clientCert = new X509Certificate2(@"C:\Projects\ErcotAPITestWS\ErcotAPITest\Cert\API_103_CERT.pfx", "a1e5i9");
            _clientCert = new X509Certificate2(CLIENT_CERT_PATH , CLIENT_CERT_PASSWORD);
            _ercotMisApiCert = new X509Certificate2(@"C:\Projects\ErcotAPITestWS\ErcotAPITest\Cert\misapi.cer");

            _ercotClient.ClientCertificates.Add(_clientCert);
            _ercotClient.ClientCertificates.Add(_ercotMisApiCert);

            _context = _ercotClient.RequestSoapContext;
            _token = new X509SecurityToken(_clientCert);
            _token2 = new X509SecurityToken(_ercotMisApiCert);
            //These are obsolete but necessary since they use old technology
            _context.Security.Tokens.Add(_token);
            _context.Security.Tokens.Add(_token2);
            _context.Security.Elements.Add(new MessageSignature(_token));


            //Set TLS1.2 protocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // Handle service reply with a callback method
            _ercotClient.MarketInfoCompleted += this.ErcotClient_MarketInfoCompleted;

        }


        private void ErcotClient_MarketInfoCompleted(object sender, MarketInfoCompletedEventArgs e)
        {
            LogDebug("MarketInfoAsync call completed...");
            
            try
            {
                LogDebug("Reading MarketInfo ReplyCode...");
                string reply = e.Result.Reply.ReplyCode;
                LogDebug("Ercot Get " + e.Result.Header.Noun + " Response: " + reply);
                LogDebug("Payload Format:" + e.Result.Payload.format);
                LogDebug("Processing MarketInfo response payload...");
               
                //If we get this far, reply must be valid object!
                if (e.Result.Payload.format.Equals("XML"))
                {

                    switch (_marketCallType)
                    {
                        case MarketInfoCallType.LMPs:
                            {
                                ProcessLMPsCallResults(e.Result.Payload);
                                break;
                            }
                        default: break;
                    }


                   
                }

            }
            catch (Exception)
            {

                LogError("Ercot Error Response: " + e.Error);

            }

        }

        public List<Lmp> GetRtmLmps()
        {
            LogDebug("Calling GetRtmLmps() method...");
            _marketCallType = MarketInfoCallType.LMPs;
            try
            {
                RequestMessageBuilder requestBuilder = new RequestMessageBuilder(SOURCE, CLIENT_CERT_USER_ID);
                RequestMessage requestmsg = requestBuilder.GetRtmLMPs(DateTime.Now.AddMinutes(-3.00), DateTime.Now);

                //This is where the rubber meets the road. Calling the service asynchronously with the help of a Task
                LogDebug("Attempting to call Ercot Get LMPs service...");
                //Task serviceTask = new Task(() => _ercotClient.MarketInfoAsync(requestmsg));
                // serviceTask.Start();
                // LogDebug("Ercot Get LMPs service task started...");
                ResponseMessage response = _ercotClient.MarketInfo(requestmsg);
                return ProcessLMPsCallResults(response.Payload);
               

            }
            catch (System.Web.Services.Protocols.SoapHeaderException she)
            {
                LogError(she);
                LogDebug(she);
            }

            catch (System.Reflection.TargetInvocationException te)
            {
                LogError(te);
                LogDebug(te);
            }

            catch (Exception ex)
            {
                LogError(ex);
                LogDebug(ex);
            }

            return _lmpList;
           
        }



        private List<Lmp> ProcessMarketInfoResponse(ResponseMessage response)
        {
            try
            {
                LogDebug("Reading MarketInfo ReplyCode...");
                // string reply = e.Result.Reply.ReplyCode;
                string reply = response.Reply.ReplyCode;
                LogDebug("Ercot Get " + response.Header.Noun + " Response: " + reply);
                LogDebug("Payload Format:" + response.Payload.format);
                LogDebug("Processing MarketInfo response payload...");

                //If we get this far, reply must be valid object!
                if (response.Payload.format.Equals("XML"))
                {

                    switch (_marketCallType)
                    {
                        case MarketInfoCallType.LMPs:
                            {
                                return ProcessLMPsCallResults(response.Payload);
                                
                            }
                        default: break;
                    }



                }

            }
            catch (Exception)
            {

                LogError("Ercot Error Response: " + response.Reply.Error);
            }

            return _lmpList;
        }


        private List<Lmp> ProcessLMPsCallResults(PayloadType payload)
        {
            _lmpList = new List<Lmp>();

            XmlElement element = (XmlElement)payload.Items[0];
            XmlNodeList LmpNodes = element.GetElementsByTagName("ns1:LMP");


            foreach (XmlNode xn in LmpNodes)
            {
                Lmp lmp = new Lmp(xn.ChildNodes[0].InnerText, xn.ChildNodes[1].InnerText, xn.ChildNodes[2].InnerText);
                _lmpList.Add(lmp);
            }

            return _lmpList;
        }

    }
}
