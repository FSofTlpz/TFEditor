using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TFEditor {
   static class Program {
      /// <summary>
      /// Der Haupteinstiegspunkt für die Anwendung.
      /// </summary>
      [STAThread]
      static void Main(string[] args) {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new FormMain(args));
      }
   }
}
