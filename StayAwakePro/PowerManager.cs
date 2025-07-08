using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using StayAwakePro;

namespace StayAwakePro
{
    public class PowerManager
    {
        private readonly Action onSleep;
        private readonly Action onWake;

        public PowerManager(Action sleepCallback, Action wakeCallback)
        {
            onSleep = sleepCallback;
            onWake = wakeCallback;
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
            SystemEvents.SessionSwitch += OnSessionSwitch;
        }

        public void KeepAwake()
        {
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
        }

        public void PauseAwake()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend) onSleep();
            else if (e.Mode == PowerModes.Resume) onWake();
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock) onSleep();
            else if (e.Reason == SessionSwitchReason.SessionUnlock) onWake();
        }

        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(uint esFlags);

        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;
    }
}

