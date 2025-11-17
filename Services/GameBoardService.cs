using MAUI_Minesweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI_Minesweeper.Services
{
    public class GameBoardService
    {
        public static CellModel[,] GenerateBoard(int rows, int cols, int mineCount) 
        {
            var board = new CellModel[rows, cols];
            var random = new Random();

            // Initialize all cells
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    board[row, col] = new CellModel();
                }
            }

            // Place mines randomly
            int minesPlaced = 0;
            while (minesPlaced < mineCount)
            {
                int row = random.Next(rows);
                int col = random.Next(cols);

                // Replace all usages of board[row, col].IsMine with the property accessor
                if (!board[row, col].IsMine)
                {
                    board[row, col].IsMine = true;
                    minesPlaced++;
                }
            }

            // Calculate neighboring mine counts
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (board[row, col].IsMine)
                        continue;

                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                    {
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0)
                                continue;

                            int newRow = row + dr;
                            int newCol = col + dc;

                            // And in the neighboring mine count calculation:
                            // And in the mine check inside the nested loop:
                            if (newRow >= 0 && newRow < rows && 
                                newCol >= 0 && newCol < cols &&
                                board[newRow, newCol].IsMine)
                            {
                                count++;
                            }
                        }
                    }

                    board[row, col].NeighboringMines = count;
                }
            }

            return board;
        }

        public static void RevealCell(CellModel cell)
        {
            // Don't reveal if already revealed or flagged
            if (cell.IsRevealed || cell.IsFlagged)
                return;

            cell.IsRevealed = true;
        }

        public static void ToggleFlag(CellModel cell)
        {
            // Can only flag unrevealed cells
            if (cell.IsRevealed)
                return;

            cell.IsFlagged = !cell.IsFlagged;
        }
    }
}