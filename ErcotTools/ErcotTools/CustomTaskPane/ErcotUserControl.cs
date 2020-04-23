using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using ErcotAPILib.MarketInfo;
using ErcotAPILib.Utils;




namespace ErcotTools.CustomTaskPane
{
    public partial class ErcotUserControl : UserControl
    {

        /*********************************************************
         * Fields
         * ******************************************************/
        private System.Timers.Timer _lmpQueryTimer;
        private MarketInfo _marketInfo;
        private bool _querySuccess = false;




        public Excel.Application Application
        {
            get;
            set;
        }

        public ErcotUserControl()
        {
            InitializeComponent();
            tableLayoutPanel1.SetColumnSpan(button1, 2);
            _marketInfo = new MarketInfo();
            button1.BackColor = Color.IndianRed;
        }

        private void button1_Click(object sender, EventArgs e)
        {


            // this.Application.ActiveWorkbook.Worksheets[1].Range["A2"].Formula = response.Header.Verb;
            this.StartLMPQueries(10000);



        }

        private void StartLMPQueries(int timerIntervalMilliseconds)
        {
            _lmpQueryTimer = new System.Timers.Timer(timerIntervalMilliseconds);
            _lmpQueryTimer.Elapsed += QueryLmpService;
            _lmpQueryTimer.AutoReset = true;

            _lmpQueryTimer.Start();
            button1.BackColor = Color.Green;

        }


        private void QueryLmpService(object sender, EventArgs e)
        {
            if ((DateTime.Now.Minute % 5) == 0)
            {
                if (!_querySuccess)
                {
                    List<Lmp> lmpList = _marketInfo.GetRtmLmps();
                    Dictionary<string, Lmp> lmpDictionary = new Dictionary<string, Lmp>();

                    if (lmpList != null)
                    {
                       
                        foreach (Lmp lmp in lmpList)
                        {
                            lmpDictionary.Add(lmp.Bus, lmp);
                        }
                    }

                    if (lmpDictionary.Count > 0)
                    {
                        this.Application.ActiveWorkbook.Worksheets[1].Range["B2"].Formula = lmpDictionary["HB_HOUSTON"].Bus;
                        this.Application.ActiveWorkbook.Worksheets[1].Range["C2"].Formula = lmpDictionary["HB_HOUSTON"].ReportRunTime;
                        this.Application.ActiveWorkbook.Worksheets[1].Range["D2"].Formula = lmpDictionary["HB_HOUSTON"].Price;
                        /*********************/
                        this.Application.ActiveWorkbook.Worksheets[1].Range["B3"].Formula = lmpDictionary["HB_NORTH"].Bus;
                        this.Application.ActiveWorkbook.Worksheets[1].Range["C3"].Formula = lmpDictionary["HB_NORTH"].ReportRunTime;
                        this.Application.ActiveWorkbook.Worksheets[1].Range["D3"].Formula = lmpDictionary["HB_NORTH"].Price;
                        _querySuccess = true;
                    }
                    else
                    {
                        StaticLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "No results from service.");
                        _querySuccess = false;
                    }

                }
                else
                {
                    _querySuccess = false;
                    StaticLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "No change since last update.");
                }
            }
            else
            {
                _querySuccess = false;
                StaticLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Service call too early. Minutes must be on the 5s.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_lmpQueryTimer != null)
            {
                _lmpQueryTimer.Stop();
                button1.BackColor = Color.IndianRed;
            }
        
        }
    }
}
