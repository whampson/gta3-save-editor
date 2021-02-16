using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTA3SaveEditor.Core.Extensions;
using WpfEssentials;

namespace GTA3SaveEditor.Core.Game
{
    public class GTA3ScriptCommand : ObservableObject,
        IEquatable<GTA3ScriptCommand>
    {
        private Opcode m_op;
        private List<(ArgType, object)> m_args;
        private int m_offset;
        private bool m_notFlag;

        public Opcode Opcode
        {
            get { return m_op; }
            set { m_op = value; OnPropertyChanged(); }
        }

        public List<(ArgType, object)> Args
        {
            get { return m_args; }
            set { m_args = value; OnPropertyChanged(); }
        }

        public int Offset
        {
            get { return m_offset; }
            set { m_offset = value; OnPropertyChanged(); }
        }

        public bool NotFlag
        {
            get { return m_notFlag; }
            set { m_notFlag = value; }
        }

        public GTA3ScriptCommand(Opcode op)
            : this(op, null)
        { }

        public GTA3ScriptCommand(Opcode op, params (ArgType, object)[] args)
        {
            Opcode = op;
            Args = new List<(ArgType, object)>(args ?? new (ArgType, object)[0]);
        }

        public GTA3ScriptCommand(GTA3ScriptCommand other)
        {
            m_op = other.m_op;
            m_args = new List<(ArgType, object)>(other.m_args);
            m_notFlag = other.m_notFlag;
            m_offset = other.m_offset;
        }

        public ArgType GetArgType(int index)
        {
            if (index >= Args.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return Args[index].Item1;
        }

        public T GetArg<T>(int index)
        {
            if (index >= Args.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return (T) Convert.ChangeType(Args[index].Item2, typeof(T));
        }

        public void SetArgType(int index, ArgType type)
        {
            if (index >= Args.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var arg = Args[index];
            arg.Item1 = type;
            Args[index] = arg;
        }

        public void SetArg<T>(int index, T value)
        {
            if (index >= Args.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var arg = Args[index];
            arg.Item2 = value;
            Args[index] = arg;
        }

        public int GetLength()
        {
            return Serialize().Length;
        }

        public byte[] Serialize()
        {
            using MemoryStream m = new MemoryStream();
            using BinaryWriter w = new BinaryWriter(m);

            ushort op = (ushort) Opcode;
            if (NotFlag) op |= 0x8000;
            w.Write(op);

            foreach (var arg in Args)
            {
                var type = arg.Item1;
                var value = arg.Item2;

                if (type == ArgType.Text)
                {
                    w.Write(((string) value).GetBytes(GTA3ScriptParser.TextImmediateLength));
                    continue;
                }

                w.Write((byte) type);
                switch (type)
                {
                    case ArgType.Int8:
                        w.Write((byte) value);
                        break;
                    case ArgType.Int16:
                    case ArgType.LocalVar:
                    case ArgType.GlobalVar:
                        w.Write((short) value);
                        break;
                    case ArgType.Int32:
                        w.Write((int) value);
                        break;
                    case ArgType.Float:
                        w.Write(GTA3ScriptParser.EncodeFloat((float) value));
                        break;
                }
            }

            w.Flush();
            return m.ToArray();
        }

        public string Disassemble()
        {
            string disasm = "";

            //disasm += $"<{Offset}>: ";

            if (NotFlag) disasm += "NOT ";
            disasm += $"{Opcode} ";

            foreach (var arg in Args)
            {
                var type = arg.Item1;
                var value = arg.Item2;

                disasm += type switch
                {
                    ArgType.Float => $"{value:0.0###} ",
                    ArgType.GlobalVar => $"${value} ",
                    ArgType.LocalVar => $"@{value} ",
                    ArgType.Text => $"'{value}' ",
                    _ => $"{value} ",
                };
            }

            return disasm.Trim();
        }

        public override int GetHashCode()
        {
            return Serialize().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GTA3ScriptParser other))
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(GTA3ScriptCommand other)
        {
            return Serialize().SequenceEqual(other.Serialize());
        }

        public override string ToString()
        {
            return Disassemble();
        }
    }
}
