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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SimonSebright.Sudoku;
using SimonSebright.Sudoku.Analyser;

namespace SimonSebright.SudokuUI
{
    public partial class WaitingDialog : Form
    {
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
            System.Diagnostics.Debug.Assert(!this.InvokeRequired);
            m_bw.CancelAsync();
        }

        public static Matrix GenerateNewPuzzle()
        {
            WaitingDialog dlg = new WaitingDialog();
            return dlg.Go();
        }

        private void OnGeneratePuzzleComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Diagnostics.Debug.Assert(!this.InvokeRequired);
            m_m = (Matrix)e.Result;
            Close();
        }

        private Matrix Go()
        {
            m_bw.DoWork += Analyser.GenerateNewPuzzle;
            m_bw.RunWorkerCompleted += this.OnGeneratePuzzleComplete;
            m_bw.RunWorkerAsync();
            m_bw.WorkerSupportsCancellation = true;
            ShowDialog();
            return m_m;
        }

        Matrix m_m;
        BackgroundWorker m_bw = new BackgroundWorker();

        private void WaitingDialog_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

    }
}