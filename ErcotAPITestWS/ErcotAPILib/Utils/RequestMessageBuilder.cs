using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErcotAPILib.ErcotNodalService;


namespace ErcotAPILib.Utils
{
    public class RequestMessageBuilder : ApplicationBase
    {
        /**************************************
         * Constants
         * ************************************/
        private const string SYSTEMSTATUS = "SystemStatus";
        private const string LMPS = "LMPs";


        /**************************************
         * Properties
         * ***********************************/
         public string UserID { get; set; }
         public string Source { get; set; }


        public RequestMessageBuilder():this(Properties.Settings.Default.Source, Properties.Settings.Default.ClientCertificateUserID)
        {
            
        }

        public RequestMessageBuilder(string source, string userID):base(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        {
            this.Source = source;
            this.UserID = userID;
            LogInfo("RequestMessageBuilder instance created...");
        }

        /// <summary>
        /// Builds the basic RequestMessage structure with populated ReplayDetection elements. 
        /// This method can be called outright and used internal to this class by specific market
        /// request builder methods.
        /// </summary>
        /// <returns></returns>
        public RequestMessage NewRequestMessageStructure(string source, string userID )
        {
            RequestMessage requestmsg = new RequestMessage();
            requestmsg.Header = new HeaderType();
            requestmsg.Header.ReplayDetection = new ReplayDetectionType();
            requestmsg.Header.ReplayDetection.Nonce = new EncodedString();
            requestmsg.Header.ReplayDetection.Nonce.Value = Guid.NewGuid().ToString("N");
            requestmsg.Header.ReplayDetection.Created = new AttributedDateTime();
            requestmsg.Header.ReplayDetection.Created.Value = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            requestmsg.Header.MessageID = ((int)((new Random()).NextDouble() * 1000000)).ToString();
            requestmsg.Header.Source = source;
            requestmsg.Header.UserID = userID;
            requestmsg.Request = new RequestType();
            requestmsg.Request.MarketType = new RequestTypeMarketType();
            
            requestmsg.Payload = new PayloadType();

            

            return requestmsg;
        }



        public RequestMessage GetSystemStatusRequestMessage()
        {
            RequestMessage requestmsg = this.NewRequestMessageStructure(this.Source, this.UserID);
            requestmsg.Header.Verb = HeaderTypeVerb.get;
            requestmsg.Header.Noun = SYSTEMSTATUS;

            return requestmsg;

        }



        public RequestMessage GetRtmLMPs(DateTime startTime, DateTime endTime)
        {
            RequestMessage requestmsg = this.NewRequestMessageStructure(this.Source, this.UserID);
            requestmsg.Header.Verb = HeaderTypeVerb.get;
            requestmsg.Header.Noun = LMPS;
            requestmsg.Request.StartTime = startTime;
            requestmsg.Request.StartTimeSpecified = true;
            requestmsg.Request.EndTime = endTime;
            requestmsg.Request.EndTimeSpecified = true;
            requestmsg.Request.MarketType = RequestTypeMarketType.RTM;
            requestmsg.Request.MarketTypeSpecified = true;
            return requestmsg;
        }



    }
}
