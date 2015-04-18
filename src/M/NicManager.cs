using System;
using System.Linq;
using System.Collections.Generic;
using System.Management;

public class NICManager
{
#region Singleton
    private static NICManager instance = null;
    public static NICManager Instance
    {
        get
        {
            if(NICManager.instance == null)
            {
                NICManager.instance = new NICManager();
            }
            return NICManager.instance;
        }
    }
#endregion

    private ManagementObjectSearcher searcher;
    private NIC[] nicList;

    private NICManager()
    {
        this.searcher = new ManagementObjectSearcher();
    }

    ///<summary>
    ///NICのリストを得る
    ///</summary>
    ///<returns>NICリスト</returns>
    public NIC[] GetNicList()
    {
        this.searcher.Query.QueryString =
            "SELECT * " +
            "FROM Win32_NetworkAdapterConfiguration " +
            "WHERE IPEnabled = TRUE";
        ManagementObjectCollection nicTable = this.searcher.Get();

        int index = 0;
        this.nicList = new NIC[nicTable.Count];
        foreach(ManagementObject nic in nicTable)
        {
            nicList[index++] = new NIC(nic);
        }
        return nicList;
    }
}