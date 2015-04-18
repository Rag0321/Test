using System.Linq;

public class NicController
{
    private NIC[] nicList;
    private int nicIndex;

    public void Run()
    {
        this.SetNic();
    }
    public void SetNic()
    {
        this.nicList = NICManager.Instance.GetNicList();
        var nsv = new NicSelectView(nicList.Select(nic => nic.Text).ToArray(), this);
        this.nicIndex = nsv.SelectTargetNic();
    }

    public string SetNicInfo(int nicIndex, bool doDHCP, string ip)
    {
        bool res;
        if(doDHCP)
        {
            res = this.nicList[nicIndex].EnableDHCP();
        }
        else
        {
            res = this.nicList[nicIndex].SetStaticIPAddress(ip);
        }

        string ret;
        if(res)
        {
            ret = this.nicList[nicIndex].Text;
        }
        else
        {
            ret = null;
        }
        return ret;
    }
}