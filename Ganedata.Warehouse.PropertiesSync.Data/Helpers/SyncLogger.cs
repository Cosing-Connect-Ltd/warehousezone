using System;
using System.Diagnostics;
using System.IO;

namespace Ganedata.Warehouse.PropertiesSync.Data.Helpers
{
    public static class SyncLogger
    {
        public static void WriteLog(Exception ex)
        {
            WriteLog(ex.Source.Trim() + "; " + ex.Message.Trim());
        }

        public static void WriteLog(string message)
        {
            try
            {
                var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\" + DateTime.UtcNow.ToString("ddMMMyyyy") + ".txt", true);
                var msg = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") + ": " + message;
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                Console.WriteLine(msg);
            }
            catch (Exception ex)
            {
                var err = "Ganedata Sync Service - Writing logs :" + ex.Source;
                EventLog.WriteEntry(err, ex.Message);
                Console.WriteLine(err);
            }
        }
    }
}