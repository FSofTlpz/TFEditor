/*
Copyright (C) 2011 Frank Stinner

This program is free software; you can redistribute it and/or modify it 
under the terms of the GNU General Public License as published by the 
Free Software Foundation; either version 3 of the License, or (at your 
option) any later version. 

This program is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of 
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General 
Public License for more details. 

You should have received a copy of the GNU General Public License along 
with this program; if not, see <http://www.gnu.org/licenses/>. 


Dieses Programm ist freie Software. Sie können es unter den Bedingungen 
der GNU General Public License, wie von der Free Software Foundation 
veröffentlicht, weitergeben und/oder modifizieren, entweder gemäß 
Version 3 der Lizenz oder (nach Ihrer Option) jeder späteren Version. 

Die Veröffentlichung dieses Programms erfolgt in der Hoffnung, daß es 
Ihnen von Nutzen sein wird, aber OHNE IRGENDEINE GARANTIE, sogar ohne 
die implizite Garantie der MARKTREIFE oder der VERWENDBARKEIT FÜR EINEN 
BESTIMMTEN ZWECK. Details finden Sie in der GNU General Public License. 

Sie sollten ein Exemplar der GNU General Public License zusammen mit 
diesem Programm erhalten haben. Falls nicht, siehe 
<http://www.gnu.org/licenses/>. 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TFEditor {
   public partial class FormTyp : Form {

      uint _Input_Typ;
      uint _Input_Subtyp;

      public uint Input_Typ {
         get { return _Input_Typ; }
         set { _Input_Typ = value & 0x7ff; textBox1.Text = string.Format("{0:x}", _Input_Typ); }
      }
      public uint Input_Subtyp {
         get { return _Input_Subtyp; }
         set { _Input_Subtyp = value & 0x1f; textBox2.Text = string.Format("{0:x}", _Input_Subtyp); }
      }

      public string Input_Name {
         get { return textBox3.Text.Trim(); }
         set { textBox3.Text = value; }
      }

      public FormTyp() {
         InitializeComponent();
      }

      private void textBox1_TextChanged(object sender, EventArgs e) {
         TextBox tb = (TextBox)sender;
         try {
            Input_Typ = UInt32.Parse(tb.Text.Trim(), System.Globalization.NumberStyles.HexNumber) & 0x7ff;
         } catch (Exception) {
            Input_Typ = 0;
         }
         tb.Text = string.Format("{0:x}", Input_Typ);
      }

      private void textBox2_TextChanged(object sender, EventArgs e) {
         TextBox tb = (TextBox)sender;
         try {
            Input_Subtyp = UInt32.Parse(tb.Text.Trim(), System.Globalization.NumberStyles.HexNumber) & 0x7ff;
         } catch (Exception) {
            Input_Subtyp = 0;
         }
         tb.Text = string.Format("{0:x}", Input_Subtyp);
      }
   }
}
