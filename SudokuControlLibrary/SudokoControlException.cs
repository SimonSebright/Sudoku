using System;

namespace SimonSebright.SudokuControlLibrary
{
    public class SudokoControlException : ApplicationException
    {
        public SudokoControlException(string message) : base(message)
        {
        }
    }
}