using DSO_Utilities.Clicker;
using DSO_Utilities.Config;
using DSO_Utilities.Hotkeys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSO_Utilities
{
    public partial class MainForm : Form
    {
        private ClickerManager clickers;
        private HotkeyManager hotkeys;
        private NumericUpDown sleepInput;
        private Label leftKeyLabel;
        private Label rightKeyLabel;
        private ConfigData config;

        private GlobalHotkey leftHotkeyReg;
        private GlobalHotkey rightHotkeyReg;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            leftHotkeyReg = new GlobalHotkey(this.Handle, 1, hotkeys.LeftHotkey);
            rightHotkeyReg = new GlobalHotkey(this.Handle, 2, hotkeys.RightHotkey);

            leftHotkeyReg.Pressed += ToggleLeftClicker;
            rightHotkeyReg.Pressed += ToggleRightClicker;

        }

        protected override void WndProc(ref Message m)
        {
            leftHotkeyReg?.ProcessMessage(m);
            rightHotkeyReg?.ProcessMessage(m);
            base.WndProc(ref m);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            leftHotkeyReg.Dispose();
            rightHotkeyReg.Dispose();
            base.OnFormClosed(e);
        }

        public MainForm()
        {
            InitializeComponent();

            this.Text = "DSO Utilities";
            this.Width = 350;
            this.Height = 250;
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;

            clickers = new ClickerManager();
            hotkeys = new HotkeyManager();

            config = ConfigManager.Load();
            hotkeys.SetHotkeys(config.LeftHotkey, config.RightHotkey);
        
            // UI controls
            Label sleepLbl = new Label() { Text = "Sleep (ms):", Left = 20, Top = 20, AutoSize = true };
            sleepInput = new NumericUpDown() { Left = 100, Top = 18, Width = 80, Minimum = 1, Maximum = 10000, Value = 100 };

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
