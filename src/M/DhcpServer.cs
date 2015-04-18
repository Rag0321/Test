using System;
using System.Net;
using System.Net.Sockets;

public class DhcpServer
{
    private const int DhcpPort = 67;

    private IPAddress   localIP;

    public static DhcpServer GetInstance(string localIPStr)
    {
        DhcpServer instace = new DhcpServer(localIPStr);
        if(instace.localIP == null)
        {
            instace = null;
        }
        return instace;
    }

    private DhcpServer(string localIPStr)
    {
        if(IPAddress.TryParse(localIPStr, out this.localIP))
        {
            // 処理なし
        }
        else
        {
            // 失敗
            this.localIP = null;
        }
    }

    public void Run()
    {
        IPEndPoint localEP = new IPEndPoint(this.localIP, DhcpServer.DhcpPort);
        using (UdpClient udp = new UdpClient(localEP))
        {
            while(true)
            {
                IPEndPoint remoteEP = null;

                Console.WriteLine("DHCP Discover waitting...");
                byte[] receiveDatas = udp.Receive(ref remoteEP);
//                this.ShowClientInfo(remoteEP, receiveDatas);
                Console.WriteLine("DHCP Discover Received!!");

                Console.WriteLine("DHCP Offer Sending...");
                receiveDatas = this.MakeOffer(receiveDatas);
                remoteEP.Address = IPAddress.Parse("255.255.255.255");
                System.Threading.Thread.Sleep(300);
                udp.Send(receiveDatas, receiveDatas.Length, remoteEP);
                Console.WriteLine("DHCP Offer Sended!!");
//                    ShowClientInfo(testEP, receiveDatas);

                Console.WriteLine("DHCP Request waitting...");
                receiveDatas = udp.Receive(ref remoteEP);
//                this.ShowClientInfo(remoteEP, receiveDatas);
                Console.WriteLine("DHCP Request Received!!");


                receiveDatas = this.MakePAck(receiveDatas);
                remoteEP.Address = IPAddress.Parse("255.255.255.255");
                System.Threading.Thread.Sleep(300);
                udp.Send(receiveDatas, receiveDatas.Length, remoteEP);


                break;
            }

            Console.WriteLine("終了");
        }
    }

    private byte[] MakeOffer(byte[] receiveDatas)
    {

        receiveDatas[0] = 2;

        receiveDatas[16] = 192;
        receiveDatas[17] = 168;
        receiveDatas[18] = 0;
        receiveDatas[19] = 1;

//        receiveDatas[20] = 192;
//        receiveDatas[21] = 169;
//        receiveDatas[22] = 0;
//        receiveDatas[23] = 254;

        int extensionOffset = 300 - 64 + 4;

        receiveDatas[extensionOffset++] = 0x35;
        receiveDatas[extensionOffset++] = 0x01;
        receiveDatas[extensionOffset++] = 0x02;

        receiveDatas[extensionOffset++] = 0x01;
        receiveDatas[extensionOffset++] = 0x04;
        receiveDatas[extensionOffset++] = 0xFF;
        receiveDatas[extensionOffset++] = 0xFF;
        receiveDatas[extensionOffset++] = 0xFF;
        receiveDatas[extensionOffset++] = 0x00;

        // ここ必須っぽい
        receiveDatas[extensionOffset++] = 0x36;
        receiveDatas[extensionOffset++] = 0x04;
        receiveDatas[extensionOffset++] = 192;
        receiveDatas[extensionOffset++] = 168;
        receiveDatas[extensionOffset++] = 0;
        receiveDatas[extensionOffset++] = 254;

        receiveDatas[extensionOffset++] = 0xFF;

        while(extensionOffset < receiveDatas.Length)
        {
            receiveDatas[extensionOffset++] = 0;
        }
        return receiveDatas;
    }

    private byte[] MakePAck(byte[] receiveDatas)
    {
        receiveDatas[0] = 2;

        receiveDatas[16] = 192;
        receiveDatas[17] = 168;
        receiveDatas[18] = 0;
        receiveDatas[19] = 1;

        int extensionOffset = 300 - 64 + 4;

        // dhcp ack
        receiveDatas[extensionOffset++] = 0x35;
        receiveDatas[extensionOffset++] = 0x01;
        receiveDatas[extensionOffset++] = 0x05;

        // subnet mask
        receiveDatas[extensionOffset++] = 0x01;
        receiveDatas[extensionOffset++] = 0x04;
        receiveDatas[extensionOffset++] = 0xFF;
        receiveDatas[extensionOffset++] = 0xFF;
        receiveDatas[extensionOffset++] = 0xFF;
        receiveDatas[extensionOffset++] = 0x00;

        // dhcp server identifier
        receiveDatas[extensionOffset++] = 0x36;
        receiveDatas[extensionOffset++] = 0x04;
        receiveDatas[extensionOffset++] = 192;
        receiveDatas[extensionOffset++] = 168;
        receiveDatas[extensionOffset++] = 0;
        receiveDatas[extensionOffset++] = 254;

        // end
        receiveDatas[extensionOffset++] = 0xFF;
        // pad 0
        while(extensionOffset < receiveDatas.Length)
        {
            receiveDatas[extensionOffset++] = 0;
        }
        return receiveDatas;
    }

    private void ShowClientInfo(IPEndPoint remoteEP, byte[] receiveDatas)
    {
        //受信したデータと送信者の情報を表示する
        Console.WriteLine("送信元アドレス:{0}/ポート番号:{1}", remoteEP.Address, remoteEP.Port);
        Console.WriteLine("受信データ:");

        int cnt = 0;
        foreach(byte data in receiveDatas)
        {
            Console.Write(data.ToString("X2"));
            cnt++;
            if(cnt == 8)
            {
                Console.Write("   ");
            }
            else if(cnt >= 16)
            {
                Console.Write("\n");
                cnt = 0;
            }
            else
            {
                Console.Write(" ");
            }
        }
        Console.Write("\n");
    }
}