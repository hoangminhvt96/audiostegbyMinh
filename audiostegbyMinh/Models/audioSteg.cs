using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace audiostegbyMinh.Models
{
    public class audioSteg
    {
        private Functions file;

        public audioSteg(Functions file)
        {
            this.file = file;
        }

        //Watermark message
        public void waterMessage(string message)
        {
            //save streams to cache
            List<short> leftStream = file.getLeftStream();
            List<short> rightStream = file.getRightStream();

            //Start watermark
            //change mess to bytes
            byte[] bufferMessage = System.Text.Encoding.Unicode.GetBytes(message);
            short tempBit;
            int bufferIndex = 0; //setup buffer
            int bufferLength = bufferMessage.Length; //take message length
            int channelLength = leftStream.Count; //take length of audio length, left = right
            //Lấy phạm vi khối lưu trữ dựa trên độ dài của luồng âm thanh và luồng mess
            int storageBlock = (int)Math.Ceiling((double)bufferLength / (channelLength * 2));

            //Lưu trữ thông tin độ dài tin nhắn trong phần tử đầu tiên của luồng trái và phải
            leftStream[0] = (short)(bufferLength / 32767);
            rightStream[0] = (short)(bufferLength % 32767);
            for(int i = 1; i < leftStream.Count; i++)
            {
                if(i < leftStream.Count)
                {
                    //Điều kiện để nhắm mục tiêu các phần tử từ vị trí cuối cùng của mỗi khối âm thanh 8 bit (được tính toán dựa trên storageBlock)
                    if (bufferIndex < bufferLength && i % 8 > 7 - storageBlock && i % 8 <= 7)
                    {
                        tempBit = (short)bufferMessage[bufferIndex++];
                        leftStream.Insert(i, tempBit);
                    }
                }
                if(i < rightStream.Count)
                {
                    if(bufferIndex < bufferLength && i % 8 > 7 - storageBlock && i % 8 <=7)
                    {
                        tempBit = (short)bufferMessage[bufferIndex++];
                        rightStream.Insert(i, tempBit);
                    }
                }
            }
            file.updateStream(leftStream, rightStream);
        }

        //Check watermark message
        public string extractMessage()
        {
            //Lưu luồng kênh âm thanh bộ nhớ cache cục bộ từ đối tượng wav
            List<short> leftStream = file.getLeftStream();
            List<short> rightStream = file.getRightStream();

            //Extract
            int bufferIndex = 0;
            int messageLengthQuotient = leftStream[0];
            int messageLengthRemainder = rightStream[0];
            int channelLength = leftStream.Count;

            int bufferLength = 32767 * messageLengthQuotient + messageLengthRemainder;
            int storageBlock = (int)Math.Ceiling((double)bufferLength / (channelLength * 2));

            byte[] bufferMessage = new byte[bufferLength];
            for(int i = 1; i < leftStream.Count; i++)
            {
                if(bufferIndex < bufferLength && i % 8 > 7 - storageBlock && i % 8 <= 7)
                {
                    bufferMessage[bufferIndex++] = (byte)leftStream[i];
                    if (bufferIndex < bufferLength)
                        bufferMessage[bufferIndex++] = (byte)rightStream[i];
                }
            }
            return System.Text.Encoding.UTF8.GetString(bufferMessage);
        }
    }
}