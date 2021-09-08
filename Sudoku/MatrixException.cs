using System;

namespace SimonSebright.Sudoku
{
    public class MatrixException : ApplicationException
    {
        public MatrixException(string message) : base(message)
        {
        }
    }
}