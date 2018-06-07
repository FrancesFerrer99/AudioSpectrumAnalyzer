namespace PP_m18
{
    public class Notes
    {
        public readonly string[] notesIT = { "Do", "Do#", "Re", "Re#", "Mi", "Fa", "Fa#", "Sol", "Sol#", "La", "La#", "Si" };
        public  readonly string[] notesEN = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "B#" };
        private readonly float fixedFrequency = 440; //frequency of A4 note
        private readonly int fixedIndex = 10; //A position in the array
        public float[] frequencies = new float[12];
        /*Use to check
         * { 262.626f, 277.183f,293.665f,
            311.127f, 329.628f,349.228f,
            369.994f, 391.995f, 415.305f,
                440f,   466.164f, 493.883f
        };*/

        private static Notes _instance = null;
        private static Note[] notes = new Note[12];

        public static Notes Instance
        {
            get
            {
                if (_instance == null) _instance = new Notes();
                return _instance;
            }
        }

        private Notes()
        {
            for (int i = 0; i < notesIT.Length; i++)
            {
                /*
                 * 
                fn = f0 * (a)^n
                f0: frequency for tuning
                a: 2^(1/12)
                n: number half steps between the fixed note and the note we want
                n=log_a_(fn/f0)*/
                frequencies[i] = fixedFrequency * (float)System.Math.Pow((System.Math.Pow(2, 1 / 12)), i + 1 - fixedIndex);
                notes[i] = new Note(notesIT[i], notesEN[i], frequencies[i]);
            }
        }

        public Note noteFromFrequency(float frequency)
        {
            double halfSteps = System.Math.Log(frequency / fixedFrequency, System.Math.Pow(2.0, 1.0 / 12.0));
            int dist = (int)System.Math.Round(halfSteps)-1;
            if (dist < 0 && System.Math.Abs(dist) <= fixedIndex)
            {
                return notes[fixedIndex + dist];
            }
            else
            {
                return notes[(fixedIndex + System.Math.Abs(dist)) % 12];
            }
        }


    }
}