/*
    Simon Sebright's Sudoku Player
    Copyright (C) 2007  Simon Sebright, www.simonsebright.com

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using SimonSebright.Sudoku;
using SimonSebright.Sudoku.Analyser;

namespace SimonSebright.SudokuUI
{
    public partial class WaitingDialog : Form
    {
        private readonly BackgroundWorker mBw = new BackgroundWorker();
        private Matrix mM;

        public WaitingDialog()
        {
            InitializeComponent();
        }

        private void WaitingDialog_Load(object sender, EventArgs e)
        {
            LabelMessage.Text = AppRes.CalculatingNewPuzzle;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Debug.Assert(!InvokeRequired);
            mBw.CancelAsync();
        }

        public static Matrix GenerateNewPuzzle()
        {
            var dlg = new WaitingDialog();
            return dlg.Go();
        }

        private void OnGeneratePuzzleComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.Assert(!InvokeRequired);
            mM = (Matrix) e.Result;
            Close();
        }

        private Matrix Go()
        {
            mBw.DoWork += Analyser.GenerateNewPuzzle;
            mBw.RunWorkerCompleted += OnGeneratePuzzleComplete;
            mBw.RunWorkerAsync();
            mBw.WorkerSupportsCancellation = true;
            ShowDialog();
            return mM;
        }

        private void WaitingDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}