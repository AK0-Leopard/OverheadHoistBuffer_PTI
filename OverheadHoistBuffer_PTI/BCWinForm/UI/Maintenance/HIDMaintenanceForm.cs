using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.ObjectRelay;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class HIDMaintenanceForm : Form
    {
        public BCMainForm MainForm { get; }
        public App.BCApplication BCApp;
        List<HIDObjToShow> HIDs = null;
        public HIDMaintenanceForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            BCApp = _mainForm.BCApp;
            dgv_trackData.AutoGenerateColumns = false;
            MainForm = _mainForm;
            initialHIDData();
        }

        private void initialHIDData()
        {
            var t = BCApp.SCApplication.EquipmentBLL.cache.loadHID();
            HIDs = t.Select(s => new HIDObjToShow(BCApp.SCApplication.VehicleBLL, s)).ToList();
            dgv_trackData.DataSource = HIDs;
        }

        private void HIDMaintenanceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
            MainForm.removeForm(nameof(HIDMaintenanceForm));
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            initialHIDData();
            dgv_trackData.Refresh();
        }



        private void TrackMaintenanceForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
