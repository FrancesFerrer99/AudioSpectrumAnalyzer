using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;

namespace PP_m18
{
    public partial class PlayFileForm : Form
    {
        MusicFileManager MFM = null;
        Player player = null;
        private WaveForm waveForm = null;
        WaveformDrawer wfdrawer = null;

        public PlayFileForm()
        {
            InitializeComponent();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                MFM = new MusicFileManager();
                buttonPlay.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            wfdrawer = null;
            try
            {
                if(player == null)
                    player = new Player(MFM.fileName, MFM.fileStream.Length, MFM.fileStream);
                //waveForm = new WaveForm(fileName, new WAVEFORMPROC(WaveChunkLoad), null);
                if(waveForm == null)
                    waveForm = new WaveForm();
                waveForm.FrameResolution = 0.01f;
                //waveForm.RenderStart(true, BASSFlag.BASS_DEFAULT);
                waveForm.RenderStart(player.streamHandle, true, false);
               // waveForm.CallbackFrequency = 5;
                //waveForm.RenderStartRecording(streamHandle, 5, 5);
                //myRecProc = new RECORDPROC(MyRecoring);
                //recHandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE, myRecProc, );
                wfdrawer = new WaveformDrawer(waveForm, player.streamHandle, pictureBoxWave);

                if (player.play())
                {
                    wfdrawer.startDrawing();
                    buttonPause.Enabled = true;
                    buttonStop.Enabled = true;
                    buttonPlay.Enabled = false;
                    buttonOpen.Enabled = false;
                }
                else
                {
                    buttonOpen.Enabled = true;
                    buttonPlay.Enabled = false;
                    buttonPause.Enabled = false;
                    buttonStop.Enabled = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void WaveChunkLoad(int framesDone, int framesTotal, TimeSpan elapsedTime, bool finished)
        {
            //throw new NotImplementedException();
            //MessageBox.Show(framesDone + Environment.NewLine + elapsedTime);
            //pictureBoxWave.BackgroundImage = waveForm.CreateBitmap(pictureBoxWave.Width, pictureBoxWave.Height, curFrame, framesTotal, true);
            //Invoke(new myDelegate(updateFrames));
        }
        
        private void buttonPause_Click(object sender, EventArgs e)
        {
            // metto in pausa la riproduzione
            if (player.pause())
            {
                buttonPause.Enabled = false;
                buttonPlay.Enabled = true;
            }
            
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            // stoppo la riproduzione
            if (player.stop())
            {
                buttonPlay.Enabled = true;
                buttonOpen.Enabled = true;
                buttonPause.Enabled = false;
                buttonStop.Enabled = false;
            }
        }

        private void PlayFileForm_Load(object sender, EventArgs e)
        {
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
            {
                MFM = null;
                player = null;
                
            }
        }

        private void PlayFileForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (player != null) player.clear();
        }
    }
}
