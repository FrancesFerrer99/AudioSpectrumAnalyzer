using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace PP_m18
{
    class Player
    {
        private int _streamHandle;
        public int streamHandle { get { return _streamHandle; } }
        private BASS_FILEPROCS soundStream;
        private FileStream fileStream;

        public Player(String fileName, long fileLenght, FileStream fileStream)
        {
            // creo un handle per lo streaming dal file
            //streamHandle = Bass.BASS_StreamCreateFileUser(BASSStreamSystem.STREAMFILE_NOBUFFER, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_MUSIC_PRESCAN, soundStream, IntPtr.Zero);
            _streamHandle = Bass.BASS_StreamCreateFile(fileName, 0, fileLenght, BASSFlag.BASS_DEFAULT);
            this.fileStream = fileStream;
            soundStream = new BASS_FILEPROCS(
                    new FILECLOSEPROC(closeFile),
                    new FILELENPROC(getFileLen),
                    new FILEREADPROC(readFile),
                    new FILESEEKPROC(setSeekCur)
                );
        }
        
        public bool play()
        {
            return Bass.BASS_ChannelPlay(streamHandle, false);
        }

        public bool stop()
        {
            return Bass.BASS_ChannelStop(streamHandle);
        }

        public bool pause()
        {
            return Bass.BASS_ChannelPause(streamHandle);
        }
        public void clear()
        {
            Bass.BASS_StreamFree(streamHandle);
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
