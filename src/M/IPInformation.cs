using System;
using System.Linq;
using System.Management;


///<summary>
///IPアドレスとサブネットマスクを同時に扱うためのクラス
///</summary>
public class IPInformation
{
    private static readonly char[] Dot = new char[]{ '.' };
    private static readonly char[] Slash = new char[]{ '/' };

#region GetInstances
    public static IPInformation GetInstance(byte[] ip, byte[] subnet)
    {
        IPInformation instance;
        if(ip == null || subnet == null)                { instance = null; }
        if(ip.Length == 4 && subnet.Length == 4)        { instance = new IPInformation(ip, subnet); }
        else if(ip.Length == 4 && subnet.Length == 1)   { instance = IPInformation.GetInstance(ip, subnet[0]); }
        else                                            { instance = null; }
        return instance;
    }
    public static IPInformation GetInstance(byte[] ip, int cidr)
    {
        IPInformation instance;
        if(cidr >= 0 && cidr <= 32)
        {
            UInt32 mask = 0xFFFFFFFF << (32 - cidr);
            byte[] subnet = new byte[4];
            for(int index = subnet.Length - 1; index >= 0 ; index--)
            {
                subnet[index] = (byte)(mask & 0xFF);
                mask >>= 8;
            }
            instance = IPInformation.GetInstance(ip, subnet);
        }
        else
        {
            instance = null;
        }
        return instance;
    }
    public static IPInformation GetInstance(string ip, string subnet)
    {
        return IPInformation.GetInstance(IPInformation.ToIPBytes(ip), IPInformation.ToIPBytes(subnet));
    }
    public static IPInformation GetInstance(string ip, int cidr)
    {
        return IPInformation.GetInstance(IPInformation.ToIPBytes(ip), cidr);
    }
    public static IPInformation GetInstance(string ip_subnet)
    {
        IPInformation instance;
        var ipDatas = ip_subnet.Split(IPInformation.Slash);
        if(ipDatas.Length == 2)
        {
            instance = IPInformation.GetInstance(ipDatas[0], ipDatas[1]);
        }
        else
        {
            instance = null;
        }
        return instance;
    }
    public static IPInformation GetInstance(ManagementObject nic)
    {
        return IPInformation.GetInstance(((string[])nic["IPAddress"])[0], ((string[])nic["IPSubnet"])[0]);
    }
#endregion

#region IP utiles
    private static byte[] ToIPBytes(string ip)
    {
        byte[] ret;
        if(IPInformation.IsIPFormat(ip))
        {
            ret = ip.Split(IPInformation.Dot)
                    .Select(oct => Byte.Parse(oct))
                    .ToArray();
        }
        else
        {
            byte cidr;
            if(Byte.TryParse(ip, out cidr))
            {
                ret = new byte[]{cidr};
            }
            else
            {
                ret = null;
            }
        }
        return ret;
    }
    private static string ToIPString(byte[] ip)
    {
        return ip.Select(oct => oct.ToString())
                 .Aggregate((ipStr, oct) => ipStr += "." + oct);
    }

    private static byte[] ToSubnetBytes(int cidr)
    {
        byte[] subnet;
        if(cidr >= 0 && cidr <= 32)
        {
            subnet = new byte[4];
            UInt32 mask = 0xFFFFFFFF << (32 - cidr);
            for(int index = subnet.Length - 1; index >= 0 ; index--)
            {
                subnet[index] = (byte)(mask & 0xFF);
                mask >>= 8;
            }
        }
        else
        {
            subnet = null;
        }
        return subnet;
    }
    private static int ToCIDR(byte[] subnet)
    {
        UInt32 temp = subnet.Select(oct => (UInt32)oct)
                            .Aggregate((ipNum, oct) => (ipNum << 8) + oct);
        int cidr = 32;
        while((temp & 0x01) == 0 && cidr > 0)
        {
            cidr--;
            temp >>= 1;
        }
        return cidr;
    }
    private static string ToCIDR(string subnet)
    {
        int cidr;
        if(IPInformation.IsIPFormat(subnet))
        {
            cidr = IPInformation.ToCIDR(subnet.Split(IPInformation.Dot).Select(oct => Byte.Parse(oct)).ToArray());
        }
        else
        {
            cidr = -1;
        }
        return cidr.ToString();
    }

    private static bool IsIPFormat(string ip)
    {
        byte dummy;
        bool ret;
        string[] divIp = ip.Split(IPInformation.Dot);
        if(divIp.Length == 4)
        {
            ret = divIp.All(oct => Byte.TryParse(oct, out dummy));
        }
        else
        {
            ret = false;
        }
        return ret;
    }
#endregion

    private IPInformation(byte[] ip, byte[] subnet)
    {
        this.IPAddressBytes = ip;
        this.SubnetBytes = subnet;
    }

    public byte[] IPAddressBytes{ get; set; }
    public byte[] SubnetBytes{ get; set; }

    public string IPAddressString
    {
        get{ return IPInformation.ToIPString(this.IPAddressBytes); }
        set{ this.IPAddressBytes = IPInformation.ToIPBytes(value); }
    }
    public string SubnetString
    {
        get{ return IPInformation.ToIPString(this.SubnetBytes); }
        set{ this.SubnetBytes = IPInformation.ToIPBytes(value); }
    }

    public int CIDR
    {
        get{ return IPInformation.ToCIDR(this.IPAddressBytes); }
        set{ this.SubnetBytes = IPInformation.ToSubnetBytes(value); }
    }

    public string IPAddress_Subnet
    {
        get{ return this.IPAddressString + "/" + this.SubnetString; }
    }
    public string IPAddress_CIDR
    {
        get{ return this.IPAddressString + "/" + this.CIDR.ToString(); }
    }

    public override string ToString()
    {
        return this.IPAddress_CIDR;
    }
}