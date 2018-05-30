using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using System.Windows.Forms;

namespace PP_m18
{
    class WaveformDrawer
    {
        private int startFrame = -1;
        private int curFrame =-1;
        private int endFrame = 200;
        private int myStreamHandle;
        private WaveForm myWaveForm;
        private PictureBox myPictureBox;
        private int framesPictureBox;

        public WaveformDrawer(WaveForm wf, int sh, PictureBox pb)
        {
            myWaveForm = wf;
            myStreamHandle = sh;
            myPictureBox = pb;
            framesPictureBox = (int)myWaveForm.FrameResolution;
        }


        public void startDrawing()
        {
            Thread t = new Thread(draw);
            t.Start();
        }

        private void draw()
        {


            while (Bass.BASS_ChannelGetPosition(myStreamHandle, BASSMode.BASS_POS_BYTES) < Bass.BASS_ChannelGetLength(myStreamHandle)
                   && Bass.BASS_ChannelIsActive(myStreamHandle) == BASSActive.BASS_ACTIVE_PLAYING
                   )
            {
                updateFrames();
                drawWave();
                Thread.Sleep(50);
            }
        }


        private void updateFrames()
        {

            double curSecond = Bass.BASS_ChannelBytes2Seconds(myStreamHandle, Bass.BASS_ChannelGetPosition(myStreamHandle, BASSMode.BASS_POS_BYTES));
            curFrame = myWaveForm.Position2Frames(Bass.BASS_ChannelGetPosition(myStreamHandle));
            //MessageBox.Show(curSecond + " " + curFrame);
            /*
            double secondOffset = myWaveForm.FrameResolution*5;
            double secondLeft = curSecond - secondOffset;
            double secondRight = curSecond + secondOffset;*/
            int framesOffset = 200;
            if (curFrame > framesOffset)
            {
                endFrame = curFrame;
            }
            else endFrame = framesOffset;
            startFrame = endFrame-framesOffset;

        }

        public void drawWave()
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

            myPictureBox.BackgroundImage = myWaveForm.CreateBitmap(myPictureBox.Width, myPictureBox.Height, startFrame, endFrame, true);
        }

    }
}
