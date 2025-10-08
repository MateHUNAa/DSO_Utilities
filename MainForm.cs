using DSO_Utilities.Clicker;
using DSO_Utilities.Config;
using DSO_Utilities.Hotkeys;
using DSO_Utilities.Revive;
using DSO_Utilities.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DSO_Utilities
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        private ClickerManager clickers;
        private HotkeyManager hotkeys;
        private NumericUpDown sleepInput;
        private Label leftKeyLabel;
        private Label rightKeyLabel;
        private ConfigData config;
        private ReviveMacroManager reviveMacros;

        private readonly Dictionary<string, Label> labels = new Dictionary<string, Label>();

        private GlobalHotkey leftHotkeyReg;
        private GlobalHotkey rightHotkeyReg;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            leftHotkeyReg = new GlobalHotkey(this.Handle, 1, hotkeys.LeftHotkey);
            rightHotkeyReg = new GlobalHotkey(this.Handle, 2, hotkeys.RightHotkey);

            leftHotkeyReg.Pressed += ToggleLeftClicker;
            rightHotkeyReg.Pressed += ToggleRightClicker;

            reviveMacros = new ReviveMacroManager(this.Handle, config, OnRevivePositionSaved);
        }
        private void OnRevivePositionSaved(string slot, Point pos)
        {
            ToastNotifier.Show(this, $"{slot} position saved ({pos.X}, {pos.Y})");
        }
        protected override void WndProc(ref Message m)
        {
            leftHotkeyReg?.ProcessMessage(m);
            rightHotkeyReg?.ProcessMessage(m);
            reviveMacros?.ProcessMessage(m);
            base.WndProc(ref m);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            leftHotkeyReg?.Dispose();
            rightHotkeyReg?.Dispose();
            reviveMacros?.Dispose();
            base.OnFormClosed(e);
        }

        public MainForm()
        {
            InitializeComponent();
            Instance = this;
            
            this.Text = "DSO Utilities";
            this.Width = 550;
            this.Height = 500;
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;

            clickers = new ClickerManager();
            hotkeys = new HotkeyManager();

            config = ConfigManager.Load();
            hotkeys.SetHotkeys(config.LeftHotkey, config.RightHotkey);
        
            // UI controls
            Label sleepLbl = new Label() { Text = "Sleep (ms):", Left = 20, Top = 20, AutoSize = true };
            sleepInput = new NumericUpDown() { Left = 100, Top = 18, Width = 80, Minimum = 1, Maximum = 10000, Value = 100 };

            sleepInput.ValueChanged += (s,e) =>
            {
               config.SleepTime = (int)sleepInput.Value;
                ConfigManager.Save(config);
            };

            Button setLeftKeyBtn = new Button() { Text = "Set Left Key", Left = 20, Top = 60, Width = 100 };
            leftKeyLabel = new Label() { Text = $"Current: {hotkeys.LeftHotkey}", Left = 140, Top = 65, AutoSize = true };
            setLeftKeyBtn.Click += (s, e) =>
            {
                hotkeys.StartWaitingLeft();
                leftKeyLabel.Text = "Press a key...";
            };

            Button setRightKeyBtn = new Button() { Text = "Set Right Key", Left = 20, Top = 100, Width = 100 };
            rightKeyLabel = new Label() { Text = $"Current: {hotkeys.RightHotkey}", Left = 140, Top = 105, AutoSize = true };
            setRightKeyBtn.Click += (s, e) =>
            {
                hotkeys.StartWaitingRight();
                rightKeyLabel.Text = "Press a key...";
            };

            Controls.Add(sleepLbl);
            Controls.Add(sleepInput);
            Controls.Add(setLeftKeyBtn);
            Controls.Add(leftKeyLabel);
            Controls.Add(setRightKeyBtn);
            Controls.Add(rightKeyLabel);

            // ===========
            // Revive Mapping Controls
            // ===========

            GroupBox reviveGroup = new GroupBox()
            {
                Text = "Revive Macro Bindings",
                Left = 20,
                Top = 150,
                Width = 500,
                Height = 220,
            };

            Controls.Add(reviveGroup);

            int startY = 30;
            for (int i =1;i<=5;i++)
            {
                string slot = $"Slot{i}";
                string keyName = config.ReviveHotkeys.ContainsKey(slot)
                    ? config.ReviveHotkeys[slot]
                    : $"F{i}";

                Label slotLbl = new Label()
                {
                    Text= $"Slot {i}",
                    Left = 15,
                    Top = startY + (i-1) * 35+5,
                    AutoSize=true
                };

                Label keyLbl = new Label()
                {
                    Text = $"Key: {keyName}",
                    Left = 80,
                    Top = startY + (i - 1) * 35 + 5,
                    Width = 120
                };

                Button setKeyBtn = new Button()
                {
                    Text = "Change Key",
                    Left= 380,
                    Top = startY + (i-1)*35,
                    Width=90
                };

                setKeyBtn.Click += (s, e) =>
                {
                    keyLbl.Text = "Press key...";
                    KeyEventHandler tempHandler = null;

                    tempHandler = (sender, args) =>
                    {
                        this.KeyDown -= tempHandler;

                        config.ReviveHotkeys[slot] = args.KeyCode.ToString();
                        ConfigManager.Save(config);

                        keyLbl.Text = $"Key: {args.KeyCode}";
                        reviveMacros.UpdateHotkey(slot, args.KeyCode);

                        ToastNotifier.Show(this, $"Bound {slot} to {args.KeyCode}");
                    };

                    this.KeyDown += tempHandler;
                };

                reviveGroup.Controls.Add(slotLbl);
                reviveGroup.Controls.Add(keyLbl);
                reviveGroup.Controls.Add(setKeyBtn);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (hotkeys.HandleKeyDown(e.KeyCode))
            {
                // Update labels if we were waiting for a key
                leftKeyLabel.Text = $"Current: {hotkeys.LeftHotkey}";
                rightKeyLabel.Text = $"Current: {hotkeys.RightHotkey}";
                return;
            }

            // Left click toggle
            if (e.KeyCode == hotkeys.LeftHotkey)
            {
                if (clickers.LeftClicker.IsRunning)
                    clickers.LeftClicker.Stop();
                else
                    clickers.LeftClicker.Start((int)sleepInput.Value);
            }

            // RCLick

            if (e.KeyCode == hotkeys.RightHotkey)
            {
                if (clickers.RightClicker.IsRunning)
                    clickers.RightClicker.Stop();
                else
                    clickers.RightClicker.Start((int)sleepInput.Value);
            }
        }
        private void ToggleLeftClicker()
        {
            if (clickers.LeftClicker.IsRunning) clickers.LeftClicker.Stop();
            else clickers.LeftClicker.Start((int)sleepInput.Value);
        }
        private void ToggleRightClicker()
        {
            if (clickers.RightClicker.IsRunning) clickers.RightClicker.Stop();
            else clickers.RightClicker.Start((int)sleepInput.Value);
        }
    }
}
;