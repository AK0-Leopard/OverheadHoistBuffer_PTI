using com.mirle.ibg3k0.sc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI.Test
{
    public partial class ModifyPortStationData : Form
    {
        private App.BCApplication BCApp;

        public ModifyPortStationData()
        {
            InitializeComponent();
        }

        public ModifyPortStationData(App.BCApplication bcApp) : this()
        {
            BCApp = bcApp;
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            refreshUI();
        }

        private void refreshUI()
        {
            dgvAPORTSTATIONData.DataSource = BCApp.SCApplication.PortStationBLL.OperateDB.loadAll();
            dgvAPORTSTATIONData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvAPORTSTATIONData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void ModifyPortStationData_Load(object sender, EventArgs e)
        {

        }

        private void btnToLoadReq_Click(object sender, EventArgs e)
        {
            var targetRow = dgvAPORTSTATIONData.SelectedRows;
            if (targetRow != null)
            {
                var targetItem = targetRow[0].DataBoundItem as APORTSTATION;
                BCApp.SCApplication.PortStationService.doUpdateEqPortRequestStatus(targetItem.PORT_ID.Trim(), E_EQREQUEST_STATUS.LoadRequest);
            }
            refreshUI();
        }

        private void btnToUnloadReq_Click(object sender, EventArgs e)
        {
            var targetRow = dgvAPORTSTATIONData.SelectedRows;
            if (targetRow != null)
            {
                var targetItem = targetRow[0].DataBoundItem as APORTSTATION;
                BCApp.SCApplication.PortStationService.doUpdateEqPortRequestStatus(targetItem.PORT_ID.Trim(), E_EQREQUEST_STATUS.UnloadRequest);
            }
            refreshUI();
        }

        private void btnToNoReq_Click(object sender, EventArgs e)
        {
            var targetRow = dgvAPORTSTATIONData.SelectedRows;
            if (targetRow != null)
            {
                var targetItem = targetRow[0].DataBoundItem as APORTSTATION;
                BCApp.SCApplication.PortStationService.doUpdateEqPortRequestStatus(targetItem.PORT_ID.Trim(), E_EQREQUEST_STATUS.NoRequest);
            }
            refreshUI();
        }

        private void btnToInServ_Click(object sender, EventArgs e)
        {
            var targetRow = dgvAPORTSTATIONData.SelectedRows;
            if (targetRow != null)
            {
                var targetItem = targetRow[0].DataBoundItem as APORTSTATION;
                BCApp.SCApplication.PortStationService.doUpdatePortStationServiceStatus(targetItem.PORT_ID.Trim(), E_PORT_STATUS.InService);
            }
            refreshUI();
        }

        private void btnToOutServ_Click(object sender, EventArgs e)
        {
            var targetRow = dgvAPORTSTATIONData.SelectedRows;
            if (targetRow != null)
            {
                var targetItem = targetRow[0].DataBoundItem as APORTSTATION;
                BCApp.SCApplication.PortStationService.doUpdatePortStationServiceStatus(targetItem.PORT_ID.Trim(), E_PORT_STATUS.OutOfService);
            }
            refreshUI();
        }

        private void btnToErrOn_Click(object sender, EventArgs e)
        {
            var targetRow = dgvAPORTSTATIONData.SelectedRows;
            if (targetRow != null)
            {
                var targetItem = targetRow[0].DataBoundItem as APORTSTATION;
                BCApp.SCApplication.PortStationService.doUpdateEqPortErrorStatus(targetItem.PORT_ID.Trim(), true);
            }
            refreshUI();
        }

        private void btnToErrOff_Click(object sender, EventArgs e)
        {
            var targetRow = dgvAPORTSTATIONData.SelectedRows;
            if (targetRow != null)
            {
                var targetItem = targetRow[0].DataBoundItem as APORTSTATION;
                BCApp.SCApplication.PortStationService.doUpdateEqPortErrorStatus(targetItem.PORT_ID.Trim(), false);
            }
            refreshUI();
        }
    }
}
