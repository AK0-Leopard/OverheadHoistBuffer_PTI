//*********************************************************************************
//      PTI_TransferService.cs
//*********************************************************************************
// File Name: PTI_TransferService.cs
// Description: 控制
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag          Description
// ------------- -------------  -------------  ------       -----------------------------
// 2020/11/09    JasonWu        N/A            A0.01        新建PTI_TransferService
//**********************************************************************************
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Service
{
    public class PTI_TransferService : TransferService
    {
        #region 屬性
        #region 系統
        public Logger PTI_TransferServiceLogger = NLog.LogManager.GetLogger("PTI_TransferServiceLogger");

        #endregion
        #endregion

    }
}
