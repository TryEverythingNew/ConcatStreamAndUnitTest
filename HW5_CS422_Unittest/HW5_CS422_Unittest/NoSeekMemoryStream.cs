using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS422
{
    /// <summary>
    /// Represents a memory stream that does not support seeking, but otherwise has
    /// functionality identical to the MemoryStream class.
    /// </summary>
    public class NoSeekMemoryStream : MemoryStream
    {
        private byte[] mem;
        private long position;

        public NoSeekMemoryStream(byte[] buffer) // implement the first construction method, where directly copy buffer
        {
            mem = new byte[buffer.Length];
            buffer.CopyTo(mem, 0);
            position = 0;
            Capacity = mem.Length;
            SetLength(mem.Length);
        }
        public NoSeekMemoryStream(byte[] buffer, int offset, int count) // implement the second construction where use only part of buffer
        {
            //buffer.(mem, offset);
            mem = new byte[count];
            for(int i=0; i < count; i++)
            {
                mem[i] = buffer[i + offset];
            }
            position = 0;
            Capacity = count;
            SetLength(mem.Length);
        }

        public override int Read(byte[] buffer, int offset, int count)// for read, you read bytes into buffer until read enough bits or get to the end
        {
            for(int i = 0; i < count; i++)
            {
                if (position >= Length)
                    return i;
                buffer[i + offset] = mem[position++];
            }
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) // check expcetion condition 
                throw new ArgumentNullException();
            if (offset + count > buffer.Length)
                throw new ArgumentException();

            for(int i = 0; i < count; i++)
            {
                if (position >= Length) // when writting, if max length met, we need to enlarge our byte buffer
                {
                    byte[] memnew = new byte[Length * 2];
                    Capacity = (int) Length * 2;
                    SetLength(Length * 2);
                    mem.CopyTo(memnew, 0);  // copy the original buffer to new buffer
                    mem = memnew;
                }
                mem[position++] = buffer[i + offset];
            }
            //base.Write(buffer, offset, count);
        }

        public override long Position
        {
            get
            {
                return position;
            }

            set
            {
                throw new NotSupportedException(); // you are not allowed to set Position
            }
        }
        // Override necessary properties and methods to ensure that this stream functions
        // just like the MemoryStream class, but throws a NotSupportedException when seeking3
        // is attempted (you'll have to override more than just the Seek function!)
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            if (CanSeek)
            {
                long pos_origin = (long)loc;
                long start = 0;
                switch (pos_origin)
                {
                    case 0: start = 0; break;
                    case 1: start = Position; break;
                    case 2: start = Length; break;
                }
                position = start + offset;
                if(position >= Length)  // if we seek to some position larger than the buffer length, we enlarge the buffer
                {
                    byte[] memnew = new byte[position * 2];
                    Capacity = (int)position * 2;
                    SetLength(position * 2);
                    mem.CopyTo(memnew, 0);  // copy the original buffer to new buffer
                    mem = memnew;
                }
                return position;
            }
            else
                throw new NotSupportedException();
        }

    }
}
