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
            list = new List<int>();
            if(data.Length > 0) {
                int numberOfChannel = BitConverter.ToInt32(data, 0);
                for(int i=0; i<numberOfChannel; i++)
                {
                    int ch = BitConverter.ToInt32(data, 4*(i+1));
                    list.Add(ch);
                }
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            list = new List<int>();
            int len = 0;
            int numberOfChannels = 0;
            foreach(Channel ch in Channel._channel_list)
            {
                len += BitConverter.GetBytes(ch.id).Length;
                numberOfChannels++;
            }
            using(var ms = new MemoryStream(len + 4))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(numberOfChannels);
                foreach(Channel ch in Channel._channel_list)
                {
                    bw.Write(ch.id);
                }

                return ms.ToArray();
            }
        }

        public override string ToString()
        {
            String chain = String.Empty;

            foreach(int id in list)
            {
                chain += id;
            }

            return chain;
        }
    }
}