﻿using System;
using System.Linq;

namespace ColorRunServer.Network
{
    [System.Diagnostics.DebuggerDisplay("{Value}")]
    internal class Packet : IDisposable
    {
        private string[] _params;
        int position;
        public Client Sender { get; private set; }

#if DEBUG
        public string Value { get { return (string)this; } }
#endif

        public static implicit operator Packet(string pack)
        {
            return new Packet(pack, null, true);
        }

        public static explicit operator string(Packet us)
        {
            return System.String.Join("|", us._params);
        }

        public Packet(string[] _params)
        {
            this._params = _params;
            position = -1;
        }

        public Packet(string packet, Client sender = null, bool skipHeaders = true)
        {
            _params = packet.Split('|');
            position = skipHeaders ? 1 : -1;
            this.Sender = sender;
        }

        public int Header
        {
            get { return int.Parse(this._params[0]); }
        }


        public int ReadInt()
        {
            return int.Parse(_params[++position]);
        }

        public byte ReadByte()
        {
            return byte.Parse(_params[++position]);
        }

        public sbyte ReadSByte()
        {
            return sbyte.Parse(_params[++position]);
        }

        public short ReadShort()
        {
            return short.Parse(_params[++position]);
        }

        public uint ReadUInt32()
        {
            return uint.Parse(_params[++position]);
        }

        public ulong ReadULong()
        {
            return ulong.Parse(_params[++position]);
        }

        public ushort ReadUShort()
        {
            return ushort.Parse(_params[++position]);
        }

        public string ReadString()
        {
            return _params[++position];
        }
		
        public void Seek(int count)
        {
            this.position += count;
        }

        public bool ReadBoolean()
        {
            var nxt = _params[++position];
            return nxt.ToLower().Equals("true") || nxt.Equals("1");
        }

        public object[] ReadAllLeft ()
        {
            return this._params.Skip(position + 1).ToArray();
        }

        public bool MoreToRead
        {
            get { return (_params.Length > (position + 1)); }
        }

        public void Dispose()
        {
            this._params = null;
        }
    }
}