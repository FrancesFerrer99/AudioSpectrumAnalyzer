using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PP_m18
{
    class MusicFileManager
    {
        private string _fileName;
        public string fileName{ get { return _fileName; } }
        private FileStream _fileStream;
        public FileStream fileStream { get { return _fileStream; } }
        private OpenFileDialog openAudioFile;

        public MusicFileManager()
        {
            openAudioFile = new OpenFileDialog();
            this.openAudioFile.FileName = "openAudioFile";
            this.openAudioFile.Filter = "MP3 audio file (*.mp3)|*.mp3|Lossless audio file (*.wav)|*.wav";
            if (DialogResult.OK == openAudioFile.ShowDialog())
            {
                _fileName = openAudioFile.FileName;
                // apro il file
                _fileStream = File.OpenRead(fileName);
            }
            else throw new Exception("File non trovato!");
        }
    }
}
