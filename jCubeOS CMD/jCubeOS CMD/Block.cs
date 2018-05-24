using System;
using System.Collections.Generic;
using System.Text;

namespace jCubeOS_CMD
{
    class Block
    {
        private int BlockSize { get; set; }
        private Cell[] Cells { get; set; }
        private bool Taken { get; set; }

        public Block(int blockSize = -1, int cellSize = -1, bool isTaken = false)
        {
            BlockSize = (((blockSize > 0) ? Utility.BLOCK_SIZE : BlockSize));
            Cells = new Cell[BlockSize];
            for (int i = 0; i < BlockSize; i++) Cells[i] = new Cell(cellSize);
            Taken = isTaken;
        }

        public Cell GetCell(int index) => Cells[index];

        public void SetValue(int index, char[] value) => GetCell(index).SetValue(value);

        public char[] GetValue(int index) => GetCell(index).GetValue();

        public bool IsTaken() => Taken;

        public void SetTaken(bool value) => Taken = value;

        public int GetBlockSize() => BlockSize;

        public int GetCellSize(int index) => GetCell(index).GetSize();
    }
}
