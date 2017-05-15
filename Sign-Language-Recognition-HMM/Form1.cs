using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace Sign_Language_Recognition_HMM
{

    public partial class Result : Form
    {
        private System.Timers.Timer myTimer;

        private int i = 0;

        public Result()
        {
            InitializeComponent();

            myTimer = new System.Timers.Timer(20);
            myTimer.Elapsed += myTimer_Elapsed;
            myTimer.AutoReset = true;
            myTimer.Enabled = true;
        }

        private void myTimer_Elapsed(object sender, EventArgs e)
        {
            txtShowResult.Text = Recognize.result;
        }



    }
}
