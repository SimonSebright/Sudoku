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
using System.Reflection;

namespace SimonSebright.SudokuUI
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private string GetCopyrightAttribute(Assembly a )
        {
            const bool ignored = false;
            return GetCopyrightAttribute(a.GetCustomAttributes(ignored));
        }

        private string GetCopyrightAttribute(Object[] attributes)
        {
            foreach (Object attribute in attributes)
            {
                AssemblyCopyrightAttribute ac = attribute as AssemblyCopyrightAttribute;

                if ( ac != null )
                {
                    return ac.Copyright;
                }
            }

            throw new ApplicationException("Attribute not found: AssemblyCopyright");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.Links[0].Visited = true;
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            string version = a.GetName().Version.ToString();
            LabelVersion.Text = string.Format(AppRes.VersionFormat, version);

            LabelCopyright.Text = GetCopyrightAttribute(a);
        }
    }
}