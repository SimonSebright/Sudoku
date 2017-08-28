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

namespace SimonSebright.SudokuControlLibrary
{
    partial class SudokuControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemGameMode = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemDesign = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemValues = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemGameMode,
            this.MenuItemValues});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(131, 48);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // MenuItemGameMode
            // 
            this.MenuItemGameMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemDesign,
            this.MenuItemPlay});
            this.MenuItemGameMode.Name = "MenuItemGameMode";
            this.MenuItemGameMode.Size = new System.Drawing.Size(130, 22);
            this.MenuItemGameMode.Text = "Game mode";
            // 
            // MenuItemDesign
            // 
            this.MenuItemDesign.Name = "MenuItemDesign";
            this.MenuItemDesign.Size = new System.Drawing.Size(106, 22);
            this.MenuItemDesign.Tag = "Design";
            this.MenuItemDesign.Text = "Design";
            // 
            // MenuItemPlay
            // 
            this.MenuItemPlay.Name = "MenuItemPlay";
            this.MenuItemPlay.Size = new System.Drawing.Size(106, 22);
            this.MenuItemPlay.Tag = "Play";
            this.MenuItemPlay.Text = "Play";
            // 
            // MenuItemValues
            // 
            this.MenuItemValues.Name = "MenuItemValues";
            this.MenuItemValues.Size = new System.Drawing.Size(130, 22);
            this.MenuItemValues.Text = "Value";
            // 
            // SudokuControl
            // 
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SudokuControl_MouseDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SudokuControl_MouseClick);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SudokuControl_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SudokuControl_KeyDown);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MenuItemGameMode;
        private System.Windows.Forms.ToolStripMenuItem MenuItemDesign;
        private System.Windows.Forms.ToolStripMenuItem MenuItemPlay;
        private System.Windows.Forms.ToolStripMenuItem MenuItemValues;
    }
}
