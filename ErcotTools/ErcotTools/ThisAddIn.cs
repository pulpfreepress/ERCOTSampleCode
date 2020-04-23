using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using ErcotTools.CustomTaskPane;

namespace ErcotTools
{
    public partial class ThisAddIn
    {

        private ErcotUserControl userControl;
        private Microsoft.Office.Tools.CustomTaskPane taskPane;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.Application.WorkbookActivate += AddErcotTaskPane;

        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            this.Application.WorkbookActivate -= AddErcotTaskPane;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion


        private void AddErcotTaskPane(object sheet)
        {
            userControl = new ErcotUserControl();
            userControl.Application = this.Application;

            taskPane = this.CustomTaskPanes.Add(userControl, "Ercot Controls");
            taskPane.Visible = true;
        }

    }
}
