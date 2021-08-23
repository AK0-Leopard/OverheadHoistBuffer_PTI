using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction.Interface
{
    public interface ICommonPortInfoValueDefMapAction : IValueDefMapAction
    {
        object GetPortState();
        Task ChangeToInModeAsync(bool isOn);
        Task ChangeToOutModeAsync(bool isOn);
        Task ResetAlarmAsync();
        Task StopBuzzerAsync();
        Task SetRunAsync();
        Task SetStopAsync();
        Task SetCommandingAsync(bool setOn);
        Task SetControllerErrorIndexAsync(int newIndex);
    }
}