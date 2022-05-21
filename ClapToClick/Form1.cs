using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClapToClick
{
    public partial class Form1 : Form
    {

        [DllImport("user32")]
        private static extern void mouse_event(int flag, int deltaX, int deltaY, int data, int extraInformation);

        WaveInEvent waveIn = new WaveInEvent();

        public Form1()
        {
            InitializeComponent();
            waveIn.DataAvailable += (s, args) =>
            {
                float max = 0;
                // interpret as 16 bit audio
                for (int index = 0; index < args.BytesRecorded; index += 2)
                {
                    short sample = (short)((args.Buffer[index + 1] << 8) |
                                            args.Buffer[index + 0]);
                    // to floating point
                    var sample32 = sample / 32768f;
                    // absolute value 
                    if (sample32 < 0) sample32 = -sample32;
                    // is this the max value?
                    if (sample32 > max) max = sample32;
                }
                label1.Invoke(new MethodInvoker(() =>
                {
                    label1.Text = max + "";
                }));
                if(max >= 0.1f)
                {
                    mouse_event(0x0002 | 0x0004, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            waveIn.StartRecording();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            waveIn.StopRecording();
        }
    }
}
