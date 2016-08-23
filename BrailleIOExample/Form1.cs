using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BraillIOExample
{
    public partial class Form1 : Form
    {
        BrailleIOExample ex;
        public Form1()
        {
            InitializeComponent();
            ex = new BrailleIOExample();
            timer_screenCaptureUpdate.Start();
        }

        private void timer_screenCaptureUpdate_Tick(object sender, EventArgs e)
        {
            if (ex != null) ex.updateScreenshotInCenterVr();
        }
    }
}
