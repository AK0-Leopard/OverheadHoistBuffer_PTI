﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 組件的一般資訊是由下列的屬性集控制。
// 變更這些屬性的值即可修改組件的相關
// 資訊。
[assembly: AssemblyTitle("ScriptControl")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ScriptControl")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 將 ComVisible 設定為 false 會使得這個組件中的型別
// 對 COM 元件而言為不可見。如果您需要從 COM 存取這個組件中
// 的型別，請在該型別上將 ComVisible 屬性設定為 true。
[assembly: ComVisible(false)]

// 下列 GUID 為專案公開 (Expose) 至 COM 時所要使用的 typelib ID
[assembly: Guid("3d2cfe14-a57f-447f-8474-024dcf26db43")]

// 組件的版本資訊是由下列四項值構成:
//
//      主要版本
//      次要版本 
//      組建編號
//      修訂編號
//
// 您可以指定所有的值，也可以依照以下的方式，使用 '*' 將組建和修訂編號
// 指定為預設值:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.6.5")]
[assembly: AssemblyFileVersion("1.6.5")]

//版號 + Tab + 日期 + Tab + 修改人 + Tab + 敘述

//1.6.5 2020/11/05  KevinWei+ 1. Bug fix:在143、134、136多加入判斷，如果目前是斷線狀態，卻收到連線成功，則將其改回連線狀態，避免瞬斷問題。
//                            2. 加入避免在OHT一連上線後，且同步動作尚未完成時，馬上下命令給OHT因而造成OHT、OHBC狀態不同步的問題。
//                            3. 加入修改143狀態詢問time out後，會跳例外導致連線後的on line初始化流程中斷的問題
//1.6.4 2020/10/22  JasonWu+ 1. 修改空盒要求與 1 in 1 out 非 swap 的預設flag 由 false 改為 true
//                           2. 修改自動轉in 被modechangable觸發後，判斷時要確認非為mismatch 才可轉向。
//1.6.3 2020/10/14  JasonWu+ 修改emergency 狀況下 對於實盒可以轉向
//1.6.2 2020/10/14  JasonWu+ 新增判斷在實盒且inmode 時需要拒絕AGVC命令，避免回答OK後，只有一個空盒給AGVC，造成兩邊皆無命令知情型。
//1.6.1 2020/10/12  JasonWu+ 新增3 port swap 控制模式，及自動觸發流程，並修改1 in 1 out 流程中對於2實盒之判定。
//1.6.0 2020/10/11  JasonWu+ 合併內外版本，新增swap功能，增加新UI選項，開關多出功能，及藉由AGVC觸發調整mode changable自動waitin 功能。
//                  JasonWu- 拿掉單取單放 強制2 in 動作，避免出現先2in 右被觸發後，In 轉out 之情形，造成該CST 路徑問題。
//1.5.1 2020/09/30  SCC+     增加命令狀態為 Canceling、Aborting 進行強制結束功能
//1.5.0 2020/09/28  JasonWu+ 修改PortValueDefMapAction 中對於LoadPostion1 的觸發，要同時判斷是否為Auto mode。
//1.4.9 2020/09/23  JasonWu+ 1.修改判定 port 是否可用之判定邏輯，拿掉 port ready 及 mismatch 訊號。 
//                           2.修改判定是否可轉向，進一步確認是否為實盒及out mode 避免出貨命令被取走。
//                           3.修改原判定退補空盒命令產生時間，由 10 -> 3.5 (因為AGVC 每3秒觸發一次)
//1.4.7 2020/09/13  SCC+     API CheckIsSourceFromAGVStation 內，增加Log
//1.4.6 2020/09/10  SCC*     修正OHT AlarmType
//1.4.5 2020/09/10  SCC*     GetAGV_OutModeInServicePortName 取得可用AGVPort，跳出迴圈
