using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WpfEssentials;

namespace GTA3SaveEditor.Core.Game
{
    public class GTA3ScriptParser : ObservableObject, IDisposable
    {
        public const string DefaultThreadName = "noname";
        public const int TextImmediateLength = 8;

        public static Dictionary<Opcode, int> OpcodeDefinitions = new Dictionary<Opcode, int>();

        private readonly MemoryStream m_code;
        private readonly BinaryReader m_codeReader;
        private bool m_disposed;

        private string m_threadName;
        private Opcode m_opcode;
        private bool m_eof;

        public int CodeLength
        {
            get { return (int) m_code.Length; }
        }

        public int CodeOffset
        {
            get { return (int) m_code.Position; }
            private set { m_code.Position = value; }
        }

        public string CurrentThreadName
        {
            get { return m_threadName; }
            private set { m_threadName = value; OnPropertyChanged(); }
        }

        public Opcode LastOpcode
        {
            get { return m_opcode; }
            private set { m_opcode = value; OnPropertyChanged(); }
        }

        public bool EndOfFile
        {
            get { return m_eof; }
            private set { m_eof = value; OnPropertyChanged(); }
        }

        public GTA3ScriptParser(byte[] code)
        {
            m_code = new MemoryStream(code);
            m_codeReader = new BinaryReader(m_code);
            Reset();
        }

        public void Reset()
        {
            CodeOffset = 0;
            LastOpcode = Opcode.NOP;
            CurrentThreadName = DefaultThreadName;
            EndOfFile = false;
        }

        public GTA3ScriptCommand Next()
        {
            if (EndOfFile)
            {
                return null;
            }

            bool notFlag = false;
            var args = new List<(ArgType, object)>();
            int offset = CodeOffset;

            try
            {
                if (!ParseOpcode(out int argCnt, out notFlag))
                {
                    return null;
                }

                while (args.Count < argCnt)
                {
                    var arg = ParseArgument();
                    args.Add(arg);
                }
            }
            catch (EndOfStreamException)
            {
                EndOfFile = true;
            }

            if (CodeOffset >= CodeLength)
            {
                EndOfFile = true;
            }

            return new GTA3ScriptCommand(LastOpcode, args.ToArray())
            {
                NotFlag = notFlag,
                Offset = offset
            };
        }

        private bool ParseOpcode(out int argCnt, out bool notFlag)
        {
            const short CleoEnd = 0x4156;

            argCnt = 0;
            notFlag = false;

            ushort op = ReadOpcode();
            if (op == CleoEnd)
            {
                // 'VA', beginning of 'VAR' section in CLEO script file built by SannyBuilder
                EndOfFile = true;
                return false;
            }

            notFlag = (op & 0x8000) == 0x8000;
            op = (ushort) (op & 0x7FFF);
            LastOpcode = (Opcode) op;

            if (!OpcodeDefinitions.TryGetValue(LastOpcode, out argCnt))
            {
                return false;
            }

            return true;
        }

        private (ArgType, object) ParseArgument()
        {
            ArgType type = ReadArgType();
            object value;

            switch (type)
            {
                case ArgType.Int8:
                    value = ReadInt8();
                    break;
                case ArgType.Int16:
                case ArgType.LocalVar:
                case ArgType.GlobalVar:
                    value = ReadInt16();
                    break;
                case ArgType.Int32:
                    value = ReadInt32();
                    break;
                case ArgType.Float:
                    value = DecodeFloat(ReadInt16());
                    break;
                default:
                    CodeOffset -= 1;
                    type = ArgType.Text;
                    value = ReadText();
                    break;
            }

            return (type, value);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_codeReader.Dispose();
                    m_code.Dispose();
                }
                m_disposed = true;
            }
        }

        public static short EncodeFloat(float f)
        {
            if (f == 0) return 0;

            f *= 16.0f;
            f += (f > 0) ? 0.5f : -0.5f;

            return (short) f;
        }

        public static float DecodeFloat(short s)
        {
            return s / 16.0f;
        }

        private byte ReadInt8() => m_codeReader.ReadByte();
        private short ReadInt16() => m_codeReader.ReadInt16();
        private int ReadInt32() => m_codeReader.ReadInt32();
        private ushort ReadOpcode() => m_codeReader.ReadUInt16();
        private ArgType ReadArgType() => (ArgType) ReadInt8();
        private string ReadText()
        {
            string s = "";
            int len = 0;
            bool foundNull = false;

            while (len < TextImmediateLength)
            {
                char c = (char) m_codeReader.ReadByte();
                if (c == '\0') foundNull = true;
                if (!foundNull) s += c;
                len++;
            }

            return s;
        }
    }

    public enum ArgType
    {
        Text = -1,  // immediate, type ID not encoded
        End,        // end of argument list (TODO: handle for START_NEW_SCRIPT)
        Int32,
        GlobalVar,
        LocalVar,
        Int8,
        Int16,
        Float,
    }
}
