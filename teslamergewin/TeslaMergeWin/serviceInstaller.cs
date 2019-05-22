using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace TeslaMergeWin
{
    [RunInstaller(true)]
    public partial class serviceInstaller : System.Configuration.Install.Installer
    {
        public serviceInstaller()
        {
            InitializeComponent();
        }
    }
}
