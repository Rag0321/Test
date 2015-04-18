using System;
//using System.Linq;

///<summary>
///DHCPのパケット種別に関する列挙体
///</summary>
public enum DhcpKind
{
    Discover = 0,
    Offer,
    Request,
    Pack,
    Nack,
}
public class DhcpPacket
{
    private byte[] data;


    public byte OpeCode{ get{ return this.data[0]; } set{ this.data[0] = value; } }
    public UInt32 TransactionID{ get{ return (UInt32)this.GetData(4, (UInt32)sizeof(UInt32)); } set{ this.SetData(value, 4, (UInt32)sizeof(UInt32)); } }
    public UInt32 UserIPAddress{ get{ return (UInt32)this.GetData(16, (UInt32)sizeof(UInt32)); } set{ this.SetData(value, 16, (UInt32)sizeof(UInt32)); } }
    public UInt64 CLientMacAddress{ get{ return (UInt64)this.GetData(28, 6); } set{ this.SetData(value << 16, 28, (UInt32)sizeof(UInt32)); } }
//    public byte DhcpKind{ get{ return this.ByteToDhcpKind(this.data[]) } set{ this.data } }
    public ExtensionList Extension{ get; private set; }

    public DhcpPacket(){}
    public DhcpPacket(byte[] data){ this.data = data; this.Extension = ExtensionList.GetList(data); }

    public byte[] GetDhcpData()
    {
        this.Extension.MargePacket(ref this.data);
        return this.data;
    }

    protected UInt64 GetData(UInt32 offset, UInt32 length)
    {
        if(offset > this.data.Length || length > sizeof(UInt64))
        {
            return 0;
        }

        UInt64 ret = 0;
//        foreach(byte single in this.data.Skip(offset).Take(length))
        for(int index = 0; index < length; index++)
        {
            ret = (ret << 4) | data[offset + index];
        }

        return ret;
    }

    protected void SetData(UInt64 data, UInt32 offset, UInt32 length)
    {
        for(UInt32 index = offset + length - 1; index >= offset; index--)
        {
            this.data[index] = (byte)(data & 0xff);
            data >>= 8;
        }
    }
}





public class ExtensionList
{
    protected class ExtensionListTail : ExtensionList
    {
        public ExtensionList previous;

        public ExtensionListTail() : base(0, 0, null){ this.previous = null; ExtensionList.tail = this; }
        public ExtensionListTail(ExtensionList prev) : base(0, 0, null){ this.previous = prev; ExtensionList.tail = this; }

        public override ExtensionList SerchCode(byte code)
        {
            return null;
        }

        protected override void MargeExtension(ref byte[] data, UInt32 offset)
        {
            for(;offset < data.Length;offset++)
            {
                data[offset] = 0x00;
            }
        }
    }

    private const UInt32 ExtensionSize = 64;

    protected static ExtensionListTail tail;

    public static ExtensionList GetList(byte[] data)
    {
        ExtensionList instance = new ExtensionList(data, (UInt32)(data.Length - ExtensionList.ExtensionSize));
        return instance;
    }
    public static ExtensionList GetList()
    {
        ExtensionList instance = new ExtensionListTail();
        return instance;
    }


//    private UInt32 offset;  // offset of this extention
//    private byte code;      // extention code
//    private byte length;    // length of data of extension
    private byte[] val;     // extention's data

    private ExtensionList next; // refelence of next extension

    public byte Code{ get; set; }
    public byte Length{ get; private set; }
    public byte[] Val{ get{ return this.val; } set{ this.val = value; this.Length = (byte)this.val.Length; } }

    private ExtensionList(byte[] data, UInt32 offset)
    {
//        this.offset  = offset;
        this.Code = data[offset];
        this.Length = data[offset + 1];
        this.val = new byte[this.Length];
        for(byte index = 0; index < this.Length; index++)
        {
            this.val[index] = data[offset + 2 + index];
        }
        if(data[offset + 2 + this.Length] == 0)
        {
            this.next = new ExtensionListTail();
        }
        else
        {
            this.next = new ExtensionList(data, offset + 2 + this.Length);
        }
    }
    private ExtensionList(byte code, byte length, byte[] val)
    {
        this.Code = code;
        this.Length = length;
        this.val = val;
    }

    public virtual ExtensionList SerchCode(byte code)
    {
        if(code == this.Code)
        {
            return this;
        }
        else
        {
            return this.next.SerchCode(code);
        }
    }
    public void MargePacket(ref byte[] data)
    {
        this.MargeExtension(ref data, (UInt32)(data.Length - ExtensionList.ExtensionSize));
    }

    protected virtual void MargeExtension(ref byte[] data, UInt32 offset)
    {
        data[offset++] = this.Code;
        data[offset++] = this.Length;
        foreach(byte val in this.val)
        {
            data[offset++] = val;
        }
        this.next.MargeExtension(ref data, offset);
    }

    public void Add(byte code, byte[] val)
    {
        ExtensionList elem = new ExtensionList(code, (byte)val.Length, val);
        ExtensionList.tail.previous.next = elem;
        ExtensionList.tail.previous = elem;
    }
}