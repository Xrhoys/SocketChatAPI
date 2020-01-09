using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Communication.Packets
{
    public class ServerAcknowledgementRequestChannelList: Packet, ICanSerialize
    {
        public List<int> list;

        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x1012);

            if(data.Length > 0 && data.Length%4 == 0) {
                for(int i=0; i<data.Length/4; i++)
                {
                    list.Add(BitConverter.ToInt32(data, 4*i));
                }
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            int len = 0;
            foreach(Channel ch in Channel._channel_list)
            {
                len += BitConverter.GetBytes(ch.id).Length;
            }
            using(var ms = new MemoryStream(len))
            using(var bw = new BinaryWriter(ms)) {
                foreach(Channel ch in Channel._channel_list)
                {
                    bw.Write(ch.id);
                }

                return ms.ToArray();
            }
        }
    }
}