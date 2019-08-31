﻿using System;
using System.Numerics;
using Org.BouncyCastle.Utilities.Encoders;

namespace BlockM3.AEternity.SDK.Sophia.Types
{
    public class BytesType : SophiaType
    {
        private readonly int? _length;


        public BytesType(string mapname, int? length) : base(mapname)
        {
            _length = length;
        }

        public override string SophiaBaseName => _length.HasValue ? "bytes(" + _length + ")" : "bytes";
        public override string SophiaName => SophiaBaseName;

        public override string Serialize(object o, Type t)
        {
            if (o is byte[] data)
            {
                if (data.Length != 64)
                    throw new ArgumentException($"Invalid signature, signature length should be 32 bytes, current length is {data.Length}");
                return "#" + Hex.ToHexString(data);
            }

            return base.Serialize(o, t);
        }

        public override object Deserialize(string value, Type t)
        {
            if (typeof(byte[]) == t)
            {
                if (!value.StartsWith("#"))
                    throw new ArgumentException($"Invalid byte array, return sophia type should start with #");
                byte[] d = Hex.Decode(value.Substring(1));
                if (d.Length != 32)
                    throw new ArgumentException($"Invalid signature, signature length should be 32 bytes, current length is {d.Length}");
                return d;
            }

            return base.Deserialize(value, t);
        }

        public override object FromBigInteger(BigInteger b)
        {
            return BigIntegerToByteArray(b);
        }
    }
}