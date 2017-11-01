using System;
using System.Drawing;
using System.Windows.Forms;

namespace TFEditor {
   public partial class FormColorPick : Form {

      public Color MyTrColor { get; set; }
      public Point MyLocation { get; set; }

      public FormColorPick() {
         InitializeComponent();
      }

      private void FormColorPick_Load(object sender, EventArgs e) {
         labelColor.BackColor = MyTrColor;
         Location = MyLocation;
      }
   }
}
