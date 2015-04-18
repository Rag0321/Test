using System;

public class TitleView : ViewBase
{
    private const string    BorderSymbol = "*";
    private const int       RLSpace = 1;
    private const int       InfomationOffset = 3;
    private const int       InfomationLabelSize = 15;
    private const string    TitleName = "Right DHCP Server with NIC Configurer";
    private const int       TitleRLPadding = 10;
    private const string    Version = "0.0.0";
    private const string    Producted = "Kazu";

    private readonly int    Width = TitleName.Length + RLSpace * 2 + TitleRLPadding * 2;

    public void ShowFront()
    {
        string titleLine;
        titleLine  = this.MakePadding(BorderSymbol, TitleRLPadding);
        titleLine += this.MakePadding(" ", RLSpace);
        titleLine += TitleName;
        titleLine += this.MakePadding(" ", RLSpace);
        titleLine += this.MakePadding(BorderSymbol, TitleRLPadding);

        Console.WriteLine(titleLine);
        Console.WriteLine(this.MakeInfomation("Version", Version));
        Console.WriteLine(this.MakeInfomation("Producted", Producted));
        Console.WriteLine(this.MakePadding(BorderSymbol, Width));
    }

    private string MakePadding(string symbol, int length)
    {
        string ret = "";
        for(int index = length; index > 0; index--)
        {
            ret += symbol;
        }
        return ret;
    }

    private string MakeInfomation(string label, string message)
    {
        string ret;
        ret  = BorderSymbol;
        ret += MakePadding(" ", InfomationOffset);
        ret += label;
        ret += MakePadding(" ", InfomationLabelSize - label.Length);
        ret += ": ";
        ret += message;
        ret += MakePadding(" ", Width - ret.Length - 1);
        ret += BorderSymbol;
        return ret;
    }
}