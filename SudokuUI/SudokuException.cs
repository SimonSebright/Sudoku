using System;

namespace SimonSebright.SudokuUI
{
    internal class SudokuException : ApplicationException
    {
        public SudokuException(string message) : base(message)
        {
        }
    }
}