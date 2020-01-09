namespace Communication
{
    public enum PacketType: ushort
    {
        //Client
        ClientSendMessage = 0x0001,
        ClientRequestChannelList = 0x0002,
        ClientRequestChat = 0x0003,
        ClientAccountLogin = 0x0004,
        ClientAccountRegister = 0x0005,
        ClientAccountLogout = 0x0006,
        ClientConnection = 0x0007,
        ClientDisconnection = 0x0008,
        ClientRequestChannelAccess = 0x0009,
        ClientRequestCreateChannel = 0x0010,
        ClientRequestChannelInfo = 0x0011,
        //Server
        ServerSendMessage = 0x1001,
        ServerSystemError = 0x1002,
        ServerAcknowledgementRequestChat = 0x1003,
        ServerAcknowledgementLogin = 0x1004,
        ServerAcknowledgementRegister = 0x1005,
        ServerAcknowledgementLogout = 0x1006,
        ServerAcknowledgementConnection = 0x1007,
        ServerAcknowledgementDisconnection = 0x1008,
        ServerAcknowledgementRequestChannelAccess= 0x1009,
        ServerAcknowledgementCreateChannel = 0x1010,
        ServerAcknowledgementChannelInfo = 0x1011,
        ServerAcknowledgementRequestChannelList = 0x1012,
    }
}