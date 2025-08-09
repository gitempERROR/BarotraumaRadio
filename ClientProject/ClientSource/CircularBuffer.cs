using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarotraumaRadio.ClientSource
{
    public class CircularBuffer
    {
        private readonly byte[] _buffer;
        private int _head; 
        private int _tail;
        private int _count;
        private readonly object _lock = new();

        public CircularBuffer(int capacity)
        {
            _buffer = new byte[capacity];
        }

        public void Write(byte[] data)
        {
            lock (_lock)
            {
                foreach (byte b in data)
                {
                    _buffer[_head] = b;
                    _head = (_head + 1) % _buffer.Length;

                    if (_count < _buffer.Length)
                        _count++;
                    else
                        _tail = (_tail + 1) % _buffer.Length; 
                }
            }
        }

        public int Read(byte[] output, int offset, int count)
        {
            lock (_lock)
            {
                if (_count == 0) return 0;

                int bytesToRead = Math.Min(count, _count);
                for (int i = 0; i < bytesToRead; i++)
                {
                    output[offset + i] = _buffer[_tail];
                    _tail = (_tail + 1) % _buffer.Length;
                }
                _count -= bytesToRead;
                return bytesToRead;
            }
        }
    }
}
