using System;

public abstract class ViewBase
{
    protected void ShowTitle(string text)
    {
        Console.WriteLine("\n-*-*-*-*- {0} -*-*-*-*-\n", text);
    }
    protected void ShowListWithIndex(string[] list, bool hasExit)
    {
        for(int index = 0; index < list.Length; index++)
        {
            Console.WriteLine("{0}: {1}", index, list[index]);
        }
        if(hasExit)
        {
            Console.WriteLine("{0}: {1}", -1, "Exit");
        }
    }
    protected bool IsNumeric(string num)
    {
        int dummy;
        return Int32.TryParse(num, out dummy);
    }
    protected bool IsInsideRange(string numStr, int min, int max)
    {
        int num;
        bool ret = Int32.TryParse(numStr, out num);
        if(ret)
        {
            ret = (num >= min && num <= max);
        }
        return ret;
    }

    protected string InputString(string message)
    {
        string buf = "";
        do
        {
            Console.Write("{0} : ", message);
            buf = Console.ReadLine();
        } while(buf.Length < 1);
        return buf;
    }
    protected int InputNumberInsideRange(int min, int max)
    {
        string buf = "";
        while(!this.IsInsideRange(buf, min, max))
        {
            Console.Write("({0}..{1}) : ", min, max);
            buf = Console.ReadLine();
        }
        return Int32.Parse(buf);
    }
    protected bool InputYesNo()
    {
        string buf;
        do
        {
            Console.Write("(y/n) : ");
            buf = Console.ReadLine();
            if(buf == null || buf.Length < 1)
            {
                buf = "dummy";
            }
        } while(buf[0] != 'y' && buf[0] != 'n');
        return buf[0] == 'y';
    }
    protected bool InputYes()
    {
        string buf;
        Console.Write("(yes/[other]) : ");
        buf = Console.ReadLine();
        return buf == "yes";
    }
}