using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WpfEssentials;

namespace GTA3SaveEditor.Core.Game
{
    public class GTA3Script : ObservableObject,
        IEquatable<GTA3Script>
    {
        public const string DefaultName = "noname";

        private List<GTA3ScriptCommand> m_commands;
        private Dictionary<int, int> m_jumps;
        private int m_base;
        private int m_length;
        private string m_name;

        public List<GTA3ScriptCommand> Commands
        {
            get { return m_commands; }
            set { m_commands = value; OnPropertyChanged(); }
        }

        public Dictionary<int, int> Jumps
        {
            get { return m_jumps; }
            set { m_jumps = value; OnPropertyChanged(); }
        }

        public int Base
        {
            get { return m_base; }
            set { Relocate(value); OnPropertyChanged(); }
        }

        public int Length
        {
            get { return m_length; }
            set { m_length = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return m_name; }
            set { SetName(value); OnPropertyChanged(); }
        }

        private GTA3Script(List<GTA3ScriptCommand> commands, int baseAddr, string name)
        {
            Commands = new List<GTA3ScriptCommand>(commands);
            Jumps = new Dictionary<int, int>();
            m_base = baseAddr;
            m_name = name;
        }

        public GTA3Script(GTA3Script other)
        {
            m_base = other.m_base;
            m_name = other.m_name;
            m_length = other.m_length;
            m_jumps = new Dictionary<int, int>(other.m_jumps);
            m_commands = new List<GTA3ScriptCommand>();
            foreach (var cmd in other.m_commands)
                m_commands.Add(new GTA3ScriptCommand(cmd));
        }

        public static GTA3Script Load(string path)
        {
            return Load(path, 0);
        }

        public static GTA3Script Load(byte[] data)
        {
            return Load(data, 0);
        }

        public static GTA3Script Load(string path, int baseAddr)
        {
            return Load(File.ReadAllBytes(path), baseAddr);
        }

        public static GTA3Script Load(byte[] data, int baseAddr)
        {
            using var parser = new GTA3ScriptParser(data);
            var cmds = new List<GTA3ScriptCommand>();
            var jumps = new Dictionary<int, int>();

            GTA3ScriptCommand cmd;
            int offset = 0;

            string name = DefaultName;
            while ((cmd = parser.Next()) != null)
            {
                if (cmd.Opcode == Opcode.SCRIPT_NAME)
                    name = cmd.GetArg<string>(0);
                
                if (cmd.Opcode.IsJump())
                {
                    Debug.Assert(cmd.GetArgType(0) == ArgType.Int32, "Jump address is not encoded as a 32-bit integer.");

                    int addr = Math.Abs(cmd.GetArg<int>(0));
                    jumps.Add(offset, addr);
                }

                cmds.Add(cmd);
                offset += cmd.GetLength();
            }

            if (!parser.EndOfFile)
            {
                var op = (short) parser.LastOpcode;
                var opOffset = parser.CodeOffset - sizeof(short);

                throw new InvalidDataException($"Invalid opcode '{op:X4}' at offset {opOffset:X4}.");
            }

            return new GTA3Script(cmds, baseAddr, name) { Length = offset, Jumps = jumps };
        }

        public string SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = DefaultName;

            bool foundName = false;
            foreach (var cmd in Commands)
            {
                if (cmd.Opcode == Opcode.SCRIPT_NAME)
                {
                    foundName = true;
                    cmd.SetArg(0, name);
                    break;
                }
            }

            if (!foundName)
            {
                var cmd = new GTA3ScriptCommand(Opcode.SCRIPT_NAME, (ArgType.Text, name));
                Commands.Insert(0, cmd);

                var jumps = new Dictionary<int, int>();
                int len = cmd.GetLength();
                foreach (var jmp in Jumps)
                {
                    int newAddr = jmp.Key + len;
                    int newTgt = jmp.Value + len;
                    jumps[newAddr] = newTgt;
                }

                Jumps = jumps;
                Length += len;
            }

            m_name = name;
            return name;
        }

        public int GetLength()
        {
            return Length = Serialize().Length;
        }

        public void Relocate(int baseAddr)
        {
            int offset = 0;
            foreach (var cmd in Commands)
            {
                if (cmd.Opcode.IsJump())
                {
                    int diff = baseAddr - m_base;
                    int addr = Jumps[offset];
                    int newAddr = addr + diff;

                    cmd.SetArg(0, newAddr);     // TODO: assuming int32 operand
                    Jumps[offset] = newAddr;    // TODO: handle resize due to datatype change
                }
                offset += cmd.GetLength();
            }

            m_base = baseAddr;
        }

        public string Disassemble()
        {
            StringWriter w = new StringWriter();

            foreach (var cmd in Commands)
            {
                string disasm = "<";
                if (Base != 0) disasm += Base + "+";
                disasm += cmd.Offset + ">: ";
                disasm += cmd.Disassemble();
                w.WriteLine(disasm);
            }

            return w.ToString();
        }

        public byte[] Serialize()
        {
            MemoryStream m = new MemoryStream();
            BinaryWriter w = new BinaryWriter(m);

            foreach (var cmd in m_commands)
            {
                w.Write(cmd.Serialize());
            }

            w.Flush();
            return m.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GTA3Script other))
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            foreach (var cmd in Commands)
            {
                hash += 17 * cmd.GetHashCode();
            }

            return hash;
        }

        public bool Equals(GTA3Script other)
        {
            return Commands.SequenceEqual(other.Commands);
        }

        public override string ToString()
        {
            return $"{Name}, {Length}";
        }
    }
}
