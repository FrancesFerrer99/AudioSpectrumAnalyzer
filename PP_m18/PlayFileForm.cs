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
        private string fileName;
        private int streamHandle;
        private BASS_FILEPROCS soundStream;
        private FileStream fileStream;

        private int waveStreamHandle;
        private WaveForm waveForm;

        public PlayFileForm()
        {
            InitializeComponent();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            openAudioFile.FileName = String.Empty;
            if (DialogResult.OK == openAudioFile.ShowDialog(this))
            {
                if (File.Exists(openAudioFile.FileName))
                {
                    fileName = openAudioFile.FileName;
                    buttonPlay.Enabled = true;
                    buttonOpen.Enabled = true;
                    buttonPause.Enabled = false;
                    buttonStop.Enabled = false;
                }
                else
                {
                    fileName = String.Empty;
                    buttonOpen.Enabled = true;
                    buttonPlay.Enabled = false;
                    buttonPause.Enabled = false;
                    buttonStop.Enabled = false;
                }
            }
        }

        private RECORDPROC myRecProc;
        private int recHandle;

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            // svuoto sempre lo stream prima di crearne uno nuovo
            Bass.BASS_StreamFree(streamHandle);
            Bass.BASS_StreamFree(waveStreamHandle);
            // controllo che il file di cui creare lo stream esista
            if (fileName != String.Empty)
            {
                // apro il file
                fileStream = File.OpenRead(fileName);
                // creo un handle per lo streaming dal file
                //streamHandle = Bass.BASS_StreamCreateFileUser(BASSStreamSystem.STREAMFILE_NOBUFFER, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_MUSIC_PRESCAN, soundStream, IntPtr.Zero);
                streamHandle = Bass.BASS_StreamCreateFile(fileName, 0, fileStream.Length, BASSFlag.BASS_DEFAULT);

                //waveForm = new WaveForm(fileName, new WAVEFORMPROC(WaveChunkLoad), null);
                waveForm = new WaveForm("prova");
                waveForm.FrameResolution = 0.01f;
                //waveForm.RenderStart(true, BASSFlag.BASS_DEFAULT);
                waveForm.RenderStart(streamHandle, true, false);
                waveForm.CallbackFrequency = 5;

                //waveForm.RenderStartRecording(streamHandle, 5, 5);
                //myRecProc = new RECORDPROC(MyRecoring);
                //recHandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE, myRecProc, );


                if (streamHandle != 0 && Bass.BASS_ChannelPlay(streamHandle, false))
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
            MessageBox.Show("Ci sono");
            int endFrame = (int)(Math.Floor(Bass.BASS_ChannelBytes2Seconds(streamHandle, Bass.BASS_ChannelGetLength(streamHandle)) / waveForm.FrameResolution));

            while (Bass.BASS_ChannelGetPosition(streamHandle, BASSMode.BASS_POS_BYTES) < Bass.BASS_ChannelGetLength(streamHandle)
                && curFrame <= endFrame)
            {
                //MessageBox
                curFrame = (int)(Math.Floor(Bass.BASS_ChannelBytes2Seconds(streamHandle, Bass.BASS_ChannelGetPosition(streamHandle, BASSMode.BASS_POS_BYTES))) / waveForm.FrameResolution);
                if (curFrame > zoom) zoomEnd = curFrame;
                else zoomEnd = zoom;
                zoomStart = zoomEnd - zoom;
                DrawWave();
                Thread.Sleep(500);
            }

            MessageBox.Show("Luke Shaw");
        }


        private void buttonPause_Click(object sender, EventArgs e)
        {
            // metto in pausa la riproduzione
            if (streamHandle != 0 && Bass.BASS_ChannelPause(streamHandle))
            {
                buttonStop.Enabled = true;
                buttonPlay.Enabled = true;
                buttonOpen.Enabled = false;
                buttonPause.Enabled = false;
            }
            else
            {
                buttonPause.Enabled = true;
                buttonStop.Enabled = true;
                buttonOpen.Enabled = false;
                buttonPlay.Enabled = false;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            // stoppo la riproduzione
            if (streamHandle != 0 && Bass.BASS_ChannelStop(streamHandle))
            {
                //waveForm.RenderStop(); <-- da spostare!
                buttonPlay.Enabled = true;
                buttonOpen.Enabled = true;
                buttonPause.Enabled = false;
                buttonStop.Enabled = false;
            }
            else
            {
                buttonPause.Enabled = true;
                buttonStop.Enabled = true;
                buttonOpen.Enabled = false;
                buttonPlay.Enabled = false;
            }
        }

        private void PlayFileForm_Load(object sender, EventArgs e)
        {
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
            {
                fileName = String.Empty;
                streamHandle = 0;
                waveStreamHandle = 0;
                fileStream = null;
                waveForm = null;
                soundStream = new BASS_FILEPROCS(
                    new FILECLOSEPROC(closeFile),
                    new FILELENPROC(getFileLen),
                    new FILEREADPROC(readFile),
                    new FILESEEKPROC(setSeekCur)
                );
            }
        }

        private bool setSeekCur(long offset, IntPtr user)
        {
            if (fileStream == null) return false;
            try
            {
                fileStream.Seek(offset, SeekOrigin.Begin);
                return true;
            }
            catch (IOException ex)
            {
                return false;
            }
        }

        private long getFileLen(IntPtr user)
        {
            if (fileStream == null) return 0;
            return fileStream.Length;
        }

        private int readFile(IntPtr buffer, int length, IntPtr user)
        {
            if (fileStream == null) return 0;
            int bytesRead = 0;
            try
            {
                byte[] data = new byte[length];
                bytesRead = fileStream.Read(data, 0, length);
                Marshal.Copy(data, 0, buffer, bytesRead);
            }
            catch (IOException ex)
            {
                bytesRead = 0;
            }
            return bytesRead;
        }

        private void closeFile(IntPtr user)
        {
            if (fileStream == null) return;
            fileStream.Close();
        }
    }
}
