using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS422
{
    class Program
    {
        static void Main(string[] args)
        {
            // test two memory streams.
            byte[] data1 = new byte[5]{ 1, 2, 3 , 4, 5 };
            byte[] data2 = new byte[5] { 11, 12, 13, 14, 15 };
            MemoryStream mem1 = new MemoryStream();
            mem1.Write(data1, 0, data1.Length);
            MemoryStream mem2 = new MemoryStream();
            mem2.Write(data2,0,data2.Length);
            ConcatStream cs1 = new ConcatStream(mem1, mem2);
            byte[] test1 = new byte[1024];
            Random a = new Random();
            int offset = 0;
            int count = 4;// you can set this value of count from 4 to any integer for unit testing
            int read = cs1.Read(test1, offset, 10);
            //cs1.Seek(0, SeekOrigin.Begin);
            cs1.Position = 0;
            read = cs1.Read(test1, offset, 10);
            cs1.Write(data2, 0, data2.Length);
            cs1.Position = 1;
            read = cs1.Read(test1, offset, 10);

            Console.WriteLine("expected value is start with offset of " + offset + " and read count is " + count);
            for (int i = 0; i < read; i++)
            {
                Console.Write(test1[i+offset].ToString() + " ");
            }
            Console.WriteLine();

            offset = a.Next(5);
            count = 6; // you can set this value of count from 6 to any integer for unit testing
            read = cs1.Read(test1, offset, count);

            Console.WriteLine("expected value is start with offset of " + offset + " and read count is " + count);
            for (int i = 0; i < read; i++)
            {
                Console.Write(test1[i + offset].ToString() + " ");
            }
            Console.WriteLine();

            // unit test for reading concatenated stream with memory stream as the first stream and noseekmem one as the second
            byte[] data3 = new byte[10] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30};
            NoSeekMemoryStream nmem1 = new NoSeekMemoryStream(data3);
            ConcatStream cs2 = new ConcatStream(mem1, nmem1);
            offset = a.Next(5);
            count = 6;
            read = cs2.Read(test1, offset, count); 

            Console.WriteLine("expected value is start with offset of " + offset + " and read count is " + count);
            for (int i = 0; i < read; i++)
            {
                Console.Write(test1[i + offset].ToString() + " ");
            }
            Console.WriteLine();

            NoSeekMemoryStream nmem2 = new NoSeekMemoryStream(data3, offset, 10-offset);
            ConcatStream cs3 = new ConcatStream(mem1, nmem2, 20);
            offset = a.Next(5);
            count = 6;
            read = cs3.Read(test1, offset, count); 

            Console.WriteLine("expected value is start with offset of " + offset + " and read count is " + count);
            for (int i = 0; i < read; i++)
            {
                Console.Write(test1[i + offset].ToString() + " ");
            }
            Console.WriteLine();

            // test the read / write functionality
            NoSeekMemoryStream nmem3 = new NoSeekMemoryStream(data3, offset, 10 - offset);
            cs3 = new ConcatStream(mem1, nmem3, 20);
            cs3.Write(data3, 0, 10);
            read = cs3.Read(test1, offset, count);
            cs3.Write(data3, 0, 10);
            read = cs3.Read(test1, offset, count);
            cs3.Position = 0; // expecting exception
            read = cs3.Read(test1, offset, count);
            // test query the length property
            long l = cs1.Length;
            l = cs2.Length;
            l = cs3.Length; // because nonmemorystream is similar to memorystream in length property, we expect that we can query the length

            // check the seek function
            cs1.Seek(1, SeekOrigin.Begin);  // expect working
            cs2.Seek(1, SeekOrigin.Current); // expect exception due to nonmemorystream
            cs3.Seek(1, SeekOrigin.End);    // expect exception due to nomemorystream
        }
    }
}
