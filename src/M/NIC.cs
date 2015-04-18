using System;
using System.Management;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;


public class NIC
{

    private ManagementObjectSearcher searcher = new ManagementObjectSearcher();

    private ManagementObject nic;
    private IPInformation backUp;
    private bool isChanged;

    public NIC(ManagementObject nic)
    {
        this.nic = nic;
        if((bool)nic["DHCPEnabled"])
        {
            this.backUp = null;
        }
        else
        {
            this.backUp = IPInformation.GetInstance(nic);
        }
        this.isChanged = false;
    }


#region Roll back functions
    private bool Exe_RollBack()
    {
        // 公開用のIPアドレス関係メソッドは失敗した場合にロールバックする為循環参照となってこもるので、ロールバック処理の無い実行用メソッドを直接利用する
        if(this.backUp == null)
        {
            this.isChanged = this.Exe_EnableDHCP() != 0;
        }
        else
        {
            this.isChanged = this.Exe_SetStaticIPAddress(this.backUp) != 0;
        }
        return !this.isChanged;
    }
    public bool RollBack()
    {
        bool ret = this.isChanged;
        if(ret)
        {
            ret = this.Exe_RollBack();
        }
        return ret;
    }
#endregion

#region Set static IPAddress functions
    private UInt32 Exe_SetStaticIPAddress(IPInformation info)
    {
        object[] args = new object[]{ new string[]{ info.IPAddressString }, new string[]{ info.SubnetString } };
        UInt32 ret = (UInt32)this.nic.InvokeMethod("EnableStatic", args);
        this.Renew();
        return ret;
    }
    public bool SetStaticIPAddress(IPInformation info)
    {
        bool ret;
        if(info != null)
        {
            ret = this.Exe_SetStaticIPAddress(info) == 0;
            if(ret)
            {
                this.isChanged = true;
            }
            else
            {
                this.RollBack();
            }
        }
        else
        {
            ret = false;
        }
        return ret;
    }
    public bool SetStaticIPAddress(string ip, string subnet)
    {
        return this.SetStaticIPAddress(IPInformation.GetInstance(ip, subnet));
    }
    public bool SetStaticIPAddress(string ip_subnet)
    {
        return this.SetStaticIPAddress(IPInformation.GetInstance(ip_subnet));
    }
#endregion

#region Enable DHCP functions
    private UInt32 Exe_EnableDHCP()
    {
        UInt32 ret = (UInt32)this.nic.InvokeMethod("EnableDHCP", null);
        this.Renew();
        return ret;
    }
    public bool EnableDHCP()
    {
        bool ret = (bool)this.nic["DHCPEnabled"];
        if(!ret)
        {
            ret = this.Exe_EnableDHCP() == 0;
            if(ret)
            {
                this.isChanged = true;
            }
            else
            {
                this.RollBack();
            }
        }
        return ret;
    }
#endregion

    public void Renew()
    {
        this.searcher.Query.QueryString =
            "SELECT * " +
            "FROM Win32_NetworkAdapterConfiguration "+
            "WHERE Index = " + (UInt32)this.nic["Index"];
        // 最初のアイテムを代入
        foreach(ManagementObject temp in this.searcher.Get())
        {
            this.nic = temp;
            break;
        }
    }

    public string Text
    {
        get
        {
            string text = (bool)this.nic["DHCPEnabled"] ? "[D]" : "[S]";
            text += (string)this.nic["Caption"];
            text += "(" + IPInformation.GetInstance(nic).IPAddress_CIDR + ")";
            return text;
        }
    }
    public override string ToString()
    {
        return this.Text;
    }
}