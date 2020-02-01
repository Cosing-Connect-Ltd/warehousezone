using Ganedata.Warehouse.PropertiesSync.Data.Helpers;
using Ganedata.Warehouse.PropertiesSync.SyncLibrary;
using Ganedata.Warehouse.PropertiesSync.SyncLibrary.TCasApi;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Ganedata.Warehouse.PropertiesSyncServiceTester
{
    public partial class SyncTester : Form
    {
        private readonly SyncServiceFactory _factory;

        private static bool ExitOnSingleRun => ConfigurationManager.AppSettings["ExitOnSingleRun"] == "True";
        private Thread SyncTesterThread;

        public SyncTester()
        {
            _factory = new SyncServiceFactory();

            InitializeComponent();

            switch (GanedataGlobalConfigurations.WarehouseSyncSiteId)
            {
                case 1: rbSite1.Checked = true; break;
                case 2: rbSite2.Checked = true; break;
                case 3: rbSite3.Checked = true; break;
            }

            ExecuteSync(ExitOnSingleRun);
        }

        public void ExecuteSync(bool exitOnRun = false)
        {
            SyncTesterThread = new Thread(() => Task.Run(_factory.ImportDataFromSites));
            SyncTesterThread.Start();

        }

        private void btnExecuteSync_Click(object sender, EventArgs e)
        {
            ExecuteSync();
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            await _factory.ExportSyncedItemsOnly();
        }

        private async void btnExportLandlords_Click(object sender, EventArgs e)
        {
            await _factory.ExportLandlordsOnly();
        }

        private async void btnExportProperties_Click(object sender, EventArgs e)
        {
            await _factory.ExportPropertiesOnly();
        }

        private async void btnExportTenants_Click(object sender, EventArgs e)
        {
            await _factory.ExportTenantsOnly();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (SyncTesterThread != null && SyncTesterThread.IsAlive)
            {
                SyncTesterThread.Abort();
            }
            base.OnClosed(e);
        }

        private void btnTcas_Click(object sender, EventArgs e)
        {
            var client = new Api2SoapClient();
            var apartments = client.GetComplexArray("$PH7r%b0mT6fZ5#z1)H", Guid.NewGuid().ToString(), 1, 2017);
            MessageBox.Show(apartments.Item.Length.ToString());
        }

        private void btnExecuteSync_Click_1(object sender, EventArgs e)
        {

        }
    }
}
