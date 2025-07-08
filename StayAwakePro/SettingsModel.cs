using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayAwakePro
{
    public class SettingsModel
    {
        public bool StartOnBoot { get; set; }
        public bool StartMinimized { get; set; }
        public bool ShowTrayNotifications { get; set; }
        public bool Debug { get; set; }
    }
}

