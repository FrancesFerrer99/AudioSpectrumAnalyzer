using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP_m18
{

    public class Note
    {
        private string _nameIt;
        private string _nameEn;
        private float _frequency;

        public Note(string nameIt, string nameEn, float frequency)
        {
            _nameIt = nameIt;
            _nameEn = nameEn;
            _frequency = frequency;

        }

        public string NameIt { get => _nameIt; set => _nameIt = value; }
        public string NameEn { get => _nameEn; set => _nameEn = value; }
        public float Frequency { get => _frequency; set => _frequency = value; }
    }
}
