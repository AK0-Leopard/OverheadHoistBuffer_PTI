using com.mirle.ibg3k0.sc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class ZoneInfo
    {
        public string ID;
        public int LOWER_LIMIT;
        public List<string> BORDER_SECTIONS;

    }
    public class ParkingZone
    {
        public string ParkingZoneID;                    //Parking Zone ID
        public List<string> ParkAddressIDs;             //這個Parking Zone內的停車位。List內的順序就是權重，第一位是入口，最後一位是出口
        public List<E_VH_TYPE> AllowedVehicleTypes;     //哪些Type的車可以停這裡?
        public int LowWaterlevel;                       //低水位
        public E_PARK_TYPE order;

        //public int Waterlevel = 0;

        //以下是可以由其他屬性推算的資料
        public int Capacity;// => ParkAddressIDs.Count();                  //停車區容量
        public string EntryAddress => ParkAddressIDs.FirstOrDefault();  //入口Address
        public string ExitAddress => ParkAddressIDs.LastOrDefault();    //出口Address

        //使用量計算(含在途量)
        //internal HashSet<AVEHICLE> parkedVehicles = GetParkedVehicles();
        //internal HashSet<ACMD> executingCommands = GetCommandsNotFinished();
        public int UsedCount { get; private set; } = 0;
        public int GetUsage(List<AVEHICLE> parkedVehicles, List<ACMD_OHTC> executingCommands)
        {
            int usedParkingSlot = 0;
            foreach (var vh in parkedVehicles)
            {
                if (ParkAddressIDs.Contains(vh.CUR_ADR_ID.Trim()) && AllowedVehicleTypes.Contains(vh.VEHICLE_TYPE))
                    usedParkingSlot++;
            }
            foreach (var cmd in executingCommands)
            {
                //if (ParkAddressIDs.Contains(cmd.DESTINATION.Trim()))
                if (ParkAddressIDs.Contains(SCUtility.Trim(cmd.DESTINATION_ADR, true)))
                    usedParkingSlot++;
            }
            UsedCount = usedParkingSlot;
            return usedParkingSlot;
        }
    }
}
