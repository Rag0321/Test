using System;

public class NicSelectView : ViewBase
{

    private NicController ctrl;
    private string[] nics;

    public NicSelectView(string[] nics, NicController ctrl)
    {
        this.ctrl = ctrl;
        this.SetNics(nics);
    }
    public void SetNics(string[] nics)
    {
        this.nics = nics;
    }


    public int SelectTargetNic()
    {
        bool doLoop = true;
        int targetIndex = 0;
        while(doLoop)
        {
            base.ShowTitle("Select Nic");
            base.ShowListWithIndex(this.nics, true);

            targetIndex = base.InputNumberInsideRange(-1, nics.Length - 1);
            if(targetIndex == -1)
            {
                doLoop = false;
            }
            else
            {
                doLoop = !GetOperate(targetIndex);
            }
        }

        return targetIndex;
    }


    private static readonly string[] OperateList = new string[]{ "Run DHCP on this NIC", "Configure infomation about this NIC" };
    private bool GetOperate(int nicIndex)
    {
        bool ret = false;
        bool doLoop = true;
        while(doLoop)
        {
            base.ShowTitle("Select operation on target NIC");
            Console.WriteLine(this.nics[nicIndex]);
            base.ShowListWithIndex(NicSelectView.OperateList, true);

            int operateIndex = base.InputNumberInsideRange(-1, NicSelectView.OperateList.Length - 1);
            switch(operateIndex)
            {
                case -1:
                    ret = false;
                    doLoop = false;
                    break;
                case 0:
                    ret = ReconfirmToRunDHCP(nicIndex);
                    doLoop = !ret;
                    break;
                case 1:
                    this.ConfigureNicInfo(nicIndex);
                    doLoop = true;
                    break;
            }
        }
        return ret;
    }


    private void ConfigureNicInfo(int nicIndex)
    {
        base.ShowTitle("Configure NIC Infomation");
        Console.WriteLine(">Is DHCP client function enable?");
        bool onDHCP = base.InputYesNo();
        string ip = "";
        if(!onDHCP)
        {
            Console.WriteLine(">Input IPAddress with subnet mask or CIDR");
            ip = base.InputString("(x.x.x.x/Subnet)");
        }

        Console.WriteLine(">Setting NIC infomation...");
        string message;
        string newNicText = this.ctrl.SetNicInfo(nicIndex, onDHCP, ip);
        if(newNicText != null)
        {
            message = ">Compleat!!!";
            this.nics[nicIndex] = newNicText;
        }
        else
        {
            message = ">Failed...";
        }
        Console.WriteLine(message);
        System.Threading.Thread.Sleep(500);
    }


    private bool ReconfirmToRunDHCP(int nicIndex)
    {
        bool ret;
        Console.WriteLine(">Could I run DHCP Server on this NIC?", this.nics[nicIndex]);
        ret = base.InputYesNo();
        if(ret && this.nics[nicIndex][1] == 'D')
        {
            Console.WriteLine(">***** Warning!!! *****\n>This NIC is Enabled DHCP client.\n>But, could you want to run DHCP Server on this NIC?");
            ret = base.InputYes();
        }

        string message;
        if(ret)
        {
            message = "I'll run DHCP Server on this NIC";
        }
        else
        {
            message = "I'll not run DHCP Server";
        }
        Console.WriteLine(message);
        System.Threading.Thread.Sleep(500);
        return ret;
    }
}