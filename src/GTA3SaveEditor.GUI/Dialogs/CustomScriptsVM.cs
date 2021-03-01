using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Extensions;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Util;
using GTASaveData;
using GTASaveData.GTA3;
using WHampson.ToolUI;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Dialogs
{
    public class CustomScriptsVM : WindowVMBase
    {
        const string FileFilter = "Custom Scripts (*.cs, *.cm)|*.cs;*.cm|All Files (*.*)|*.*";

        const string Tag = "CSC\0";
        const int MaxNumEntries = 64;                   // arbitrary limit, should be enough...
        const int TableAddress = 0x4800;                // semi-arbitrary, chosen to allocate some extra global variable space (close to 300 new globals)
        const int TableSize = 8 + (MaxNumEntries * 4);  // ('CSC\0' + num_active_entries [int32]) + (MaxNumEntries * (entry_address [int16] + entry_size [int16]))
        const int TotalSpace = 17260;                   // start of object defs in MAIN.SCM (PC). TODO: could possibly overwrite :MAIN code for extra space

        private ObservableCollection<CustomScript> m_customScripts;
        private CustomScript m_selectedScript;
        private bool m_commit;
        private int m_availableSpace;

        public ObservableCollection<CustomScript> CustomScripts
        {
            get { return m_customScripts; }
            set { m_customScripts = value; OnPropertyChanged(); }
        }

        public CustomScript SelectedScript
        {
            get { return m_selectedScript; }
            set { m_selectedScript = value; OnPropertyChanged(); }
        }

        public bool ShouldCommit
        {
            get { return m_commit; }
            set { m_commit = value; OnPropertyChanged(); }
        }

        public int AvailableSpace
        {
            get { return m_availableSpace; }
            set { m_availableSpace = value; OnPropertyChanged(); }
        }

        public override void Load()
        {
            base.Load();
            Enumerate();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Commit();
        }

        public void Enumerate()
        {
            var customScripts = new List<CustomScript>();
            var scriptSpace = TheSave.Scripts.ScriptSpace.ToArray();
            var threads = TheSave.Scripts.Threads.ToArray();
            using MemoryStream m = new MemoryStream(scriptSpace);
            using BinaryReader r = new BinaryReader(m);

            // Table format:
            // 0000: 'CSC\0'
            // 0004: count
            // 0008: entries[count]
            //   0000: address
            //   0002: length

            AvailableSpace = TotalSpace;

            int tableAddr = scriptSpace.IndexOfSequence(Tag.GetBytes()).FirstOrDefault();
            if (tableAddr == 0)
                goto AssignScripts;

            AvailableSpace -= TableSize;
            m.Position = tableAddr + 4;
            int count = r.ReadInt32();

            long mark;
            for (int i = 0; i < count; i++)
            {
                int addr = r.ReadInt16();
                int length = r.ReadInt16();
                AvailableSpace -= length;

                var thread = threads.Where(x => x.IP >= addr && x.IP < addr + length).FirstOrDefault();

                mark = m.Position;
                m.Position = addr;

                var code = GTA3Script.Load(r.ReadBytes(length), addr);
                m.Position = mark;

                Log.Info($"Found custom script '{code.Name}' at address {addr}:\n" + code.Disassemble());
                customScripts.Add(new CustomScript(code, thread, addr, thread != null));
            }

        AssignScripts:
            CustomScripts = new ObservableCollection<CustomScript>(customScripts);
        }

        public void Commit()
        {
            if (!m_commit)
                return;

            using DataBuffer w = new DataBuffer();

            var threads = TheSave.Scripts.Threads;
            var scriptSpace = TheSave.Scripts.ScriptSpace.ToArray();
            w.Write(scriptSpace);

            int scriptSpaceSize;
            int tableAddr = scriptSpace.IndexOfSequence(Tag.GetBytes()).FirstOrDefault();

            if (tableAddr == 0)
                tableAddr = TableAddress;
            
            w.Seek(tableAddr);

            if (CustomScripts.Count == 0)
            {
                scriptSpaceSize = w.Position;
                goto WriteScriptSpace;
            }

            w.Write(Tag.GetBytes());
            w.Write(CustomScripts.Count);

            int addr = TableAddress + TableSize;
            for (int i = 0; i < MaxNumEntries; i++)
            {
                if (i >= CustomScripts.Count)
                {
                    w.Write((short) 0);
                    w.Write((short) 0);
                    continue;
                }

                var script = CustomScripts[i];
                script.Code.Relocate(addr);
                script.Thread.IP = addr;

                byte[] code = CustomScripts[i].Code.Serialize();
                int len = code.Length;

                w.Write((short) addr);
                w.Write((short) len);
                w.Mark();

                w.Seek(addr);
                w.Write(code);
                w.Seek(w.MarkedPosition);

                addr = DataBuffer.Align4(addr + len);
            }

            w.Seek(w.Length);
            w.Align4();
            scriptSpaceSize = w.Length;

        WriteScriptSpace:
            w.Seek(3);
            w.Write(scriptSpaceSize);

            w.Seek(scriptSpaceSize);
            TheSave.Scripts.ScriptSpace = w.GetBytes();
        }

        private CustomScript LoadScript(string path)
        {
            try
            {
                GTA3Script sc = GTA3Script.Load(path);
                Log.Info($"Loaded custom script '{sc.Name}':\n" + sc.Disassemble());

                //if (CustomScripts.Select(x => x.Code.Equals(sc)).FirstOrDefault())
                //{
                //    ShowError("This script is already present in the save file.");
                //    return null;
                //}

                return new CustomScript(sc);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                ShowException(ex);
                return null;
            }
        }
        public ICommand AddScriptCommand => new RelayCommand
        (
            () => ShowFileDialog(new FileDialogEventArgs(FileDialogType.OpenFileDialog, (r, e) =>
            {
                if (r != true)
                    return;

                if (CustomScripts.Count == MaxNumEntries)
                {
                    ShowError($"Only {MaxNumEntries} scripts can be added to a save file at this time.");
                    return;
                }

                var script = LoadScript(e.FileName);
                if (script == null)
                    return;

                AvailableSpace -= script.Code.Length;
                if (CustomScripts.Count == 0)
                    AvailableSpace -= TableSize;

                if (AvailableSpace < 0)
                    ShowWarning($"The available custom script space has been exceeded.\n\nThe game may fail to load this save file.");

                CustomScripts.Add(script);
                TheSave.Scripts.Threads.Add(script.Thread);
                script.Enabled = true;
                ShouldCommit = true;
            })
            { 
                Filter = FileFilter,
            })
        );

        public ICommand ExtractScriptCommand => new RelayCommand
        (
            () => ShowFileDialog(new FileDialogEventArgs(FileDialogType.SaveFileDialog, (r, e) =>
            {
                if (r != true)
                    return;

                GTA3Script copy = new GTA3Script(SelectedScript.Code);
                copy.Relocate(0);
                File.WriteAllBytes(e.FileName, copy.Serialize());
            })
            {
                FileName = $"{SelectedScript.Code.Name}.cs",
                Filter = FileFilter,
                OverwritePrompt = true,
            }),
            () => SelectedScript != null
        );

        public ICommand DeleteScriptCommand => new RelayCommand
        (
            () =>
            {
                AvailableSpace += SelectedScript.Code.Length;

                SelectedScript.Enabled = false;
                TheSave.Scripts.Threads.Remove(SelectedScript.Thread);
                CustomScripts.Remove(SelectedScript);
                ShouldCommit = true;

                if (CustomScripts.Count == 0)
                    AvailableSpace = TotalSpace;
            },
            () => SelectedScript != null
        );

        public ICommand EnableScriptCommand => new RelayCommand
        (
            () =>
            {
                var threads = TheSave.Scripts.Threads;
                var sc = SelectedScript;

                if (sc.Enabled && !threads.Contains(sc.Thread))
                {
                    threads.Add(sc.Thread);
                }
                if (!sc.Enabled && threads.Contains(sc.Thread))
                {
                    threads.Remove(sc.Thread);
                }
            },
            () => SelectedScript != null
        );
    }
}
