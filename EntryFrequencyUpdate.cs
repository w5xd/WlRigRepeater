using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RigRepeater
{
    [Serializable()]
    class EntryFrequencyUpdate
    {
        public short NetLetter;
        public short LeftRight;
        public double TxFreq;
        public double RxFreq;
        public short Split;
        public short Mode;

        [NonSerialized()]
        public WriteLogClrTypes.ISingleEntry EntryWindow;

        [NonSerialized()]
        public EntryFrequencyUpdate LinkUpdatesFrom;

        [NonSerialized()]
        public int ListIndex = 0;

        public EntryFrequencyUpdate(short n, short lr, double tx, double rx, short split, short mode)
        {
            NetLetter = n;
            LeftRight = lr;
            TxFreq = tx;
            RxFreq = rx;
            Split = split;
            Mode = mode;
        }

        String LR()
        {
            switch (LeftRight)
            {
                case 0: return "L";
                case 1: return "R";
                case 2: return "3";
                case 3: return "4";
                default:
                return "?";
            }
        }

        public bool SameRig(EntryFrequencyUpdate other)
        {
            return other.NetLetter == NetLetter && other.LeftRight == LeftRight;
        }

        public bool Equals(EntryFrequencyUpdate other)
        {
            return TxFreq == other.TxFreq &&
                RxFreq == other.RxFreq &&
                Mode == other.Mode &&
                Split == other.Split &&
                NetLetter == other.NetLetter &&
                LeftRight == other.LeftRight;
        }

        public bool SameFrequency(EntryFrequencyUpdate other)
        {
            return TxFreq == other.TxFreq &&
                RxFreq == other.RxFreq &&
                Mode == other.Mode &&
                Split == other.Split;
        }

        public void UpdateFrom(EntryFrequencyUpdate other)
        {
            TxFreq = other.TxFreq;
            RxFreq = other.RxFreq;
            Mode = other.Mode;
            Split = other.Split;
        }

        public override string ToString()
        {
            byte[] byteArray = new byte[1];
            short nl = NetLetter;
            if (nl == 0)
            {
                nl = (short)System.Text.Encoding.UTF8.GetBytes("?")[0];
            }
            byteArray[0] = (byte)nl;
            String r = String.Format("{0} {1} {2,10:F} KHz", System.Text.Encoding.UTF8.GetString(byteArray),
                LR(), TxFreq);
            return r;
        }
    }
}
