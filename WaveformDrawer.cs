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

        public WaveformDrawer(WaveForm wf, int sh, PictureBox pb)
        {
            myWaveForm = wf;
            myStreamHandle = sh;
            myPictureBox = pb;
        }

        public void startDrawing()
        {
            Thread t = new Thread(draw);
            t.Start();
        }

        private void draw()
        {


            while (Bass.BASS_ChannelGetPosition(myStreamHandle, BASSMode.BASS_POS_BYTES) < Bass.BASS_ChannelGetLength(myStreamHandle)
                   && curFrame <= endFrame
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
            curFrame = (int)(Math.Floor(curSecond * (myWaveForm.FrameResolution)));
            
            double secondoffset = myWaveForm.FrameResolution;
            double secondLeft = Bass.BASS_ChannelSeconds2Bytes(myStreamHandle, curSecond - secondoffset/2);
            double secondRight = Bass.BASS_ChannelSeconds2Bytes(myStreamHandle, curSecond + secondoffset/2);

            int frameLeft = (int)(Math.Floor(secondLeft / (myWaveForm.FrameResolution)));
            int frameRight = (int)(Math.Floor(secondRight / (myWaveForm.FrameResolution)));

            if (frameRight > endFrame)
            {
                startFrame = frameLeft;
                endFrame = endFrame + frameRight;
            }
            else endFrame = curFrame;
            //endFrame = (int)(Math.Floor(Bass.BASS_ChannelBytes2Seconds(myStreamHandle, Bass.BASS_ChannelGetLength(myStreamHandle)) / myWaveForm.FrameResolution));

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
