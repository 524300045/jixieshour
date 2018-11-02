using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace StriveEngine.BinaryDemoCore
{
    public static class SerializeHelper
    {
        #region SerializeObject
        public static byte[] SerializeObject(object obj) //obj 可以是数组
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();//此种情况下,mem_stream的缓冲区大小是可变的

            formatter.Serialize(memoryStream, obj);

            byte[] buff = memoryStream.ToArray();
            memoryStream.Close();

            return buff;
        }

        public static void SerializeObject(object obj, ref byte[] buff, int offset) //obj 可以是数组
        {
            byte[] rude_buff = SerializeHelper.SerializeObject(obj);
            for (int i = 0; i < rude_buff.Length; i++)
            {
                buff[offset + i] = rude_buff[i];
            }
        }
        #endregion

        #region DeserializeBytes
        public static object DeserializeBytes(byte[] buff, int index, int count)
        {
            IFormatter formatter = new BinaryFormatter();

            MemoryStream stream = new MemoryStream(buff, index, count);
            object obj = formatter.Deserialize(stream);
            stream.Close();

            return obj;
        }
        #endregion 
    }
}
