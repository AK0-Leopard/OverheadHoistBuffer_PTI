﻿using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Configuration;
using com.mirle.ibg3k0.sc.Common;

namespace com.mirle.ibg3k0.sc.Data.TimerAction
{
    class LogFileHandlerTimer : ITimerAction
    {
        protected SCApplication scApp = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public LogFileHandlerTimer(string name, long intervalMilliSec)
            : base(name, intervalMilliSec)
        {
            _DefaultLogFilePath = getString("LogFilePath", @"D:\LogFiles\OHxC\PTI");
            _KeepLogDay = getInt("LogKeepData", 90);
        }
        private string getString(string key, string defaultValue)
        {
            string rtn = defaultValue;
            try
            {
                rtn = ConfigurationManager.AppSettings.Get(key);
                if (SCUtility.isEmpty(rtn))
                {
                    rtn = defaultValue;
                }
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }
        private int getInt(string key, int defaultValue)
        {
            int rtn = defaultValue;
            try
            {
                rtn = Convert.ToInt32(ConfigurationManager.AppSettings.Get(key));
            }
            catch (Exception e)
            {
                logger.Warn("Get Config error[key:{0}][Exception:{1}]", key, e);
            }
            return rtn;
        }
        private string _DefaultLogFilePath = @"D:\LogFiles\OHxC\PTI";
        private int _CompressLogDay = 1;
        private int _KeepLogDay = 90;
        private DateTime LastProcessDateTime = DateTime.MinValue;
        /// <summary>
        /// The synchronize point
        /// </summary>
        private long syncPoint = 0;
        /// <summary>
        /// Timer Action的執行動作
        /// </summary>
        /// <param name="obj">The object.</param>
        public override void doProcess(object obj)
        {
            if (System.Threading.Interlocked.Exchange(ref syncPoint, 1) == 0)
            {
                try
                {// 2021-09-31 06:00          2021-09-31 06:01 
                    if (LastProcessDateTime > DateTime.Now.AddDays(_CompressLogDay * -1))
                    {
                        return;
                    }
                    LastProcessDateTime = DateTime.Now;

                    var dirLogPath = new DirectoryInfo(_DefaultLogFilePath);
                    foreach (var directoryInfo in dirLogPath.GetDirectories())
                    {
                        var objDateTime = DateTime.Now;
                        if (!directoryInfo.Name.Contains("_")) return;
                        string log_data = directoryInfo.Name.Split('_').Last();
                        if (DateTime.TryParse(log_data, out objDateTime))
                        {
                            if (objDateTime <= DateTime.Now.AddDays(_CompressLogDay * -1))
                            {
                                string strZipName = directoryInfo.FullName + @".zip";
                                if (File.Exists(strZipName))
                                    File.Delete(strZipName);
                                ZipFile.CreateFromDirectory(directoryInfo.FullName, strZipName);
                                if (File.Exists(strZipName))
                                    directoryInfo.Delete(true);
                            }
                        }
                    }

                    var zip_file = dirLogPath.GetFiles().Where(f => f.Name.Contains(".zip")).ToList();
                    foreach (var file_info in zip_file)
                    {
                        var objDateTime = DateTime.Now;
                        if (!file_info.Name.Contains("_")) return;
                        string log_data = file_info.Name.Split('_').Last();
                        if (!file_info.Name.Contains(".")) return;
                        log_data = log_data.Split('.').First();
                        if (DateTime.TryParse(log_data, out objDateTime))
                        {
                            if (objDateTime <= DateTime.Now.AddDays(_KeepLogDay * -1))
                            {
                                string strZipName = file_info.FullName;
                                if (File.Exists(strZipName))
                                    File.Delete(strZipName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncPoint, 0);
                }
            }
        }

        public override void initStart()
        {
            scApp = SCApplication.getInstance();
        }
    }
}