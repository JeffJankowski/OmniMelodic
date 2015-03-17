using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniMelodic
{
    class Grid
    {
        private const int DEF_ROWS =    10;

        OmniNote[][] grid;

        //use default size grid
        public Grid() : this(DEF_ROWS) { }
        
        public Grid(int rows)
        {
            NumRows = rows;
            
            grid = new OmniNote[DEF_ROWS][];
            for (int i = 0; i < rows; i++)
                grid[i] = new OmniNote[rows];

        }

        public int NumRows { get; set; }

        public OmniNote AddNote(int i, int j, double amp, Note n, Scale s)
        {
            return grid[i][j] = new OmniNote(amp, n, s);
        }

        public OmniNote GetNote(int i, int j)
        {
            return grid[i][j];
        }

        public void RemoveNote(int i, int j)
        {
            grid[i][j] = null;
        }

        public bool IsEmpty(int i, int j)
        {
            return grid[i][j] == null;
        }

        public void Clear()
        {
            for (int i = 0; i < NumRows; i++)
                for (int j = 0; j < NumRows; j++)
                    grid[i][j] = null;
        }
    }
}
