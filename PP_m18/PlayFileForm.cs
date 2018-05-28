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
        private int waveStreamHandle;
        private WaveForm waveForm;

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

        private RECORDPROC myRecProc;
        private int recHandle;

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            try
            {
                player = new Player(MFM.fileName, MFM.fileStream.Length, MFM.fileStream);

                //waveForm = new WaveForm(fileName, new WAVEFORMPROC(WaveChunkLoad), null);
                waveForm = new WaveForm("prova");
                waveForm.FrameResolution = 0.01f;
                //waveForm.RenderStart(true, BASSFlag.BASS_DEFAULT);
                waveForm.RenderStart(player.streamHandle, true, false);
                waveForm.CallbackFrequency = 5;

                //waveForm.RenderStartRecording(streamHandle, 5, 5);
                //myRecProc = new RECORDPROC(MyRecoring);
                //recHandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE, myRecProc, );


                if (player.play())
                {

                    Thread t = new Thread(updateFrames);
                    t.Start();

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

        private int curFrame;
        private int zoom = 100; // = 5sec., since our resolution is 0.01sec.
        private int zoomEnd;
        private int zoomStart;
        private void DrawWave()
        {
            //time -> frame ([FrameResolution] frame : 1 secondo)
            //frame -> picture box
            //frame -> pixel
            //framemax = ratioconversione * maxpixel
            //rconversione = frame / pixel (i frame in un pixel)

            //int curFrame = (int)(Math.Floor(Bass.BASS_ChannelBytes2Seconds(streamHandle, Bass.BASS_ChannelGetPosition(streamHandle, BASSMode.BASS_POS_BYTES))) / waveForm.FrameResolution);

            //int zoom  = 100; // = 5sec., since our resolution is 0.01sec.
            //int zoomEnd;
            //int zoomStart;
            //WaveWriter ww = new WaveWriter("wave", );

            pictureBoxWave.BackgroundImage = waveForm.CreateBitmap(200, 200, zoomStart, zoomEnd, true);
        }

        public delegate void myDelegate();
        private void updateFrames()
        {
            int endFrame = (int)(Math.Floor(Bass.BASS_ChannelBytes2Seconds(player.streamHandle, Bass.BASS_ChannelGetLength(player.streamHandle)) / waveForm.FrameResolution));

            while (Bass.BASS_ChannelGetPosition(player.streamHandle, BASSMode.BASS_POS_BYTES) < Bass.BASS_ChannelGetLength(player.streamHandle)
                && curFrame <= endFrame)
            {
                curFrame = (int)(Math.Floor(Bass.BASS_ChannelBytes2Seconds(player.streamHandle, Bass.BASS_ChannelGetPosition(player.streamHandle, BASSMode.BASS_POS_BYTES))) / waveForm.FrameResolution);
                if (curFrame > zoom) zoomEnd = curFrame;
                else zoomEnd = zoom;
                zoomStart = zoomEnd - zoom;
                DrawWave();
                Thread.Sleep(500);
            }
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
    }
}
