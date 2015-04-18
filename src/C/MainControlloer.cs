using System;

public static class MainController
{
    [STAThread]
    public static void Main()
    {
        var tv = new TitleView();
        tv.ShowFront();

        var nicC = new NicController();
        nicC.Run();

        var ev = new EndView();
        ev.ShowEnd();
    }
}