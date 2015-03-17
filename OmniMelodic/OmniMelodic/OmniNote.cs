using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniMelodic
{
    //C-Major 7th
    public enum Note { C, E, G, B };
    public enum Scale { LOW, MID, HIGH };

    class OmniNote
    {
        //loudness between 0-1
        public double Amplitude { get; set; }
        //note from Cmaj7 scale
        public Note Tone { get; set; }
        //LOW, MIDDLE, HIGH octaves
        public Scale Octave { get; set; }

        //How many vectors are currently on the note
        public int activeVectors { get; set; }

        public bool[] connections;

        public OmniNote(double amp, Note tone, Scale oct)
        {
            Amplitude = amp;
            Tone = tone;
            Octave = oct;
            activeVectors = 0;
            connections = new bool[8];
        }
    }
}
