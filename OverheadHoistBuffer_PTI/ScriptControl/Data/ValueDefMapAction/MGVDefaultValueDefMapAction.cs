//*********************************************************************************
//      DefaultValueDefMapAction.cs
//*********************************************************************************
// File Name: MGVDefaultValueDefMapAction.cs
// Description: Port Scenario 
//
//(c) Copyright 2013, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.PLC_Functions.MGV;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class MGVDefaultValueDefMapAction : IValueDefMapAction
    {
        protected AEQPT eqpt = null;
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;
        private BCFApplication bcfApp = null;

        protected String[] recipeIDNodes = null;

        public MGVDefaultValueDefMapAction()
            : base()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }

        public string getIdentityKey()
        {
            return this.GetType().Name;
        }

        public void setContext(BaseEQObject baseEQ)
        {
            this.eqpt = baseEQ as AEQPT;
        }

        public void unRegisterEvent()
        {
        }

        public void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:

                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        /// <summary>
        /// Does the initialize.
        /// </summary>
        public void doInit()
        {
            try
            {
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_RUN", out ValueRead vr))
                {
                    vr.afterValueChange += (_sender, _e) => MGV_Status_Change_RUN(_sender, _e);
                }
                if (bcfApp.tryGetReadValueEventstring(eqpt.EqptObjectCate, eqpt.EQPT_ID, "MGV_TO_OHxC_DOWN", out ValueRead vr2))
                {
                    vr2.afterValueChange += (_sender, _e) => MGV_Status_Change_DOWN(_sender, _e);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        private void MGV_Status_Change_RUN(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<OHxCToMGV_Status>(eqpt.EQPT_ID) as OHxCToMGV_Status;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMGV_Status>(function);
            }
        }

        private void MGV_Status_Change_DOWN(object sender, ValueChangedEventArgs e)
        {
            var function = scApp.getFunBaseObj<OHxCToMGV_Status>(eqpt.EQPT_ID) as OHxCToMGV_Status;
            try
            {
                //1.建立各個Function物件
                function.Read(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.read log
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<OHxCToMGV_Status>(function);
            }
        }

        public void OHxCToMGVAlive(UInt16 index)
        {
            var function = scApp.getFunBaseObj<MGVToOHxC_Alive>(eqpt.EQPT_ID) as MGVToOHxC_Alive;
            try
            {
                //1.建立各個Function物件
                function.AliveIndex = index;
                function.Write(bcfApp, eqpt.EqptObjectCate, eqpt.EQPT_ID);
                //2.write log
                NLog.LogManager.GetCurrentClassLogger().Info(function.ToString());

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
            finally
            {
                scApp.putFunBaseObj<MGVToOHxC_Alive>(function);
            }
        }
    }
}
