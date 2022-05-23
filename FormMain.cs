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
using GarminCore;
using GarminCore.Files;
using GarminCore.Files.Typ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace TFEditor {
   public partial class FormMain : Form {

      string sProgname = "";

      StdFile_TYP tf = null;
      string sTypfilename;
      string sStartTypfilename = null;
      bool bTFChanged = false;
      bool bPoiWithGarminColor = false;

      /// <summary>
      /// Anzeigeform der Elelementlisten beim Progstart
      /// </summary>
      View ViewStatus = View.Details;

      /// <summary>
      /// bevorzugte, d.h. angezeigte Sprache
      /// </summary>
      Text.LanguageCode PreferredLanguage = GarminCore.Files.Typ.Text.LanguageCode.german;

      /// <summary>
      /// Breite des kleinen Listenbildes in den Listviews
      /// </summary>
      int iListViewSmallImageWidth = 32;
      /// <summary>
      /// Höhe des kleinen Listenbildes in den Listviews
      /// </summary>
      int iListViewSmallImageHeight = 32;
      /// <summary>
      /// Breite des großen Listenbildes in den Listviews
      /// </summary>
      int iListViewLargeImageWidth = 64;
      /// <summary>
      /// Höhe des großen Listenbildes in den Listviews
      /// </summary>
      int iListViewLargeImageHeight = 64;
      /// <summary>
      /// Kachelbreite in den Listviews
      /// </summary>
      int iListViewTileWidth = 150;
      /// <summary>
      /// Kachelhöhe in den Listviews
      /// </summary>
      int iListViewTileHeight = 70;


      // Unklar ist, ob Linien generell nur max. 32 Pixel dick sein dürfen (siehe Bitmap). Prinzipiell
      // könnte eine Linienbreite bis 255 gespeichert werden.
      /// <summary>
      /// max. zulässige Linienbreite
      /// </summary>
      int iMaxLineWidth = 32;

      Color colBitmapBackcolor = Color.Transparent;


      /// <summary>
      /// alle Sprachen für MultiText
      /// </summary>
      Text.LanguageCode[] Language4Text;
      /// <summary>
      /// Liste der Codepages
      /// </summary>
      EncodingInfo[] encinfo = Encoding.GetEncodings();
      /// <summary>
      /// gemeinsamer Eventhandler für alle Items des Untermenüs für Sprachen
      /// </summary>
      EventHandler evPreferredLanguage_Clicked;

      /// <summary>
      /// Zuordnung Polygontyp - Bezeichnug
      /// </summary>
      SortedList<Polygone.ColorType, string> AreaTypName = new SortedList<Polygone.ColorType, string>();
      /// <summary>
      /// Zuordnung Linientyp - Bezeichnug
      /// </summary>
      SortedList<Polyline.PolylineType, string> LineTypName = new SortedList<Polyline.PolylineType, string>();
      /// <summary>
      /// Zuordnung POI-Typ - Bezeichnug
      /// </summary>
      SortedList<BitmapColorMode, string> PointTypName = new SortedList<BitmapColorMode, string>();
      /// <summary>
      /// Zuordnung Fontdata - Bezeichnug
      /// </summary>
      SortedList<GraphicElement.Fontdata, string> FontdataName = new SortedList<GraphicElement.Fontdata, string>();
      /// <summary>
      /// Zuordnung CustomColours - Bezeichnug
      /// </summary>
      SortedList<GraphicElement.FontColours, string> CustomColoursName = new SortedList<GraphicElement.FontColours, string>();


      #region Definition der Verknüpfung einiger Listen mit Typefile-Eigenschaften

      Polygone.ColorType[] data4ListboxAreaTyp = new Polygone.ColorType[] {
         Polygone.ColorType.Day1,
         Polygone.ColorType.Day1_Night1,
         Polygone.ColorType.BM_Day1,
         Polygone.ColorType.BM_Day2,
         Polygone.ColorType.BM_Day2_Night2,
         Polygone.ColorType.BM_Day1_Night2,
         Polygone.ColorType.BM_Day2_Night1,
         Polygone.ColorType.BM_Day1_Night1
      };

      Polyline.PolylineType[] data4ListboxLineTyp = new Polyline.PolylineType[] {
         Polyline.PolylineType.Day2,
         Polyline.PolylineType.Day1_Night2,
         Polyline.PolylineType.Day2_Night2,
         Polyline.PolylineType.NoBorder_Day1,
         Polyline.PolylineType.NoBorder_Day1_Night1,
         Polyline.PolylineType.NoBorder_Day2_Night1
      };

      GraphicElement.Fontdata[] data4ListboxFont = new GraphicElement.Fontdata[] {
         GraphicElement.Fontdata.Default,
         GraphicElement.Fontdata.Normal,
         GraphicElement.Fontdata.Small,
         GraphicElement.Fontdata.Large,
         GraphicElement.Fontdata.Nolabel
      };

      GraphicElement.FontColours[] data4ListboxCustomcolor = new GraphicElement.FontColours[] {
         GraphicElement.FontColours.No,
         GraphicElement.FontColours.Day,
         GraphicElement.FontColours.Night,
         GraphicElement.FontColours.DayAndNight
      };

      #endregion

      List<string> LastFiles = new List<string>();


      public FormMain(string[] args) {
         InitializeComponent();

         AreaTypName.Add(Polygone.ColorType.Day1, "1 Tagesfarbe");
         AreaTypName.Add(Polygone.ColorType.BM_Day1, "Bild mit 1 Tagesfarbe");
         AreaTypName.Add(Polygone.ColorType.BM_Day2, "Bild mit 2 Tagesfarben");
         AreaTypName.Add(Polygone.ColorType.Day1_Night1, "1 Tagesfarbe, 1 Nachtfarbe");
         AreaTypName.Add(Polygone.ColorType.BM_Day2_Night2, "Bild mit 2 Tages- und Bild mit 2 Nachtfarben");
         AreaTypName.Add(Polygone.ColorType.BM_Day1_Night2, "Bild mit 1 Tages- und Bild mit 2 Nachtfarben");
         AreaTypName.Add(Polygone.ColorType.BM_Day2_Night1, "Bild mit 2 Tages- und Bild mit 1 Nachtfarbe");
         AreaTypName.Add(Polygone.ColorType.BM_Day1_Night1, "Bild mit 1 Tages- und Bild mit 1 Nachtfarbe");

         LineTypName.Add(Polyline.PolylineType.Day2, "2 Tagesfarben");
         LineTypName.Add(Polyline.PolylineType.NoBorder_Day1, "1 Tagesfarbe, ohne Rand");
         LineTypName.Add(Polyline.PolylineType.Day1_Night2, "1 Tagesfarbe, 2 Nachtfarben");
         LineTypName.Add(Polyline.PolylineType.Day2_Night2, "2 Tagesfarben, 2 Nachtfarben");
         LineTypName.Add(Polyline.PolylineType.NoBorder_Day1_Night1, "1 Tagesfarbe, 1 Nachtfarben, ohne Rand");
         LineTypName.Add(Polyline.PolylineType.NoBorder_Day2_Night1, "2 Tagesfarben, 1 Nachtfarbe, ohne Rand");

         PointTypName.Add(BitmapColorMode.POI_SIMPLE, "ohne Transparenz");
         PointTypName.Add(BitmapColorMode.POI_TR, "eine (Voll-)transparente Farbe");
         PointTypName.Add(BitmapColorMode.POI_ALPHA, "Farben mit individueller Transparenz");

         FontdataName.Add(GraphicElement.Fontdata.Default, "Standard");
         FontdataName.Add(GraphicElement.Fontdata.Normal, "normal");
         FontdataName.Add(GraphicElement.Fontdata.Small, "klein");
         FontdataName.Add(GraphicElement.Fontdata.Large, "groß");
         FontdataName.Add(GraphicElement.Fontdata.Nolabel, "ohne Label");

         CustomColoursName.Add(GraphicElement.FontColours.No, "keine");
         CustomColoursName.Add(GraphicElement.FontColours.Day, "Tag");
         CustomColoursName.Add(GraphicElement.FontColours.Night, "Nacht");
         CustomColoursName.Add(GraphicElement.FontColours.DayAndNight, "Tag und Nacht");

         sProgname = Title();
         if (args.Length > 0 && args[0].Length > 0)
            sStartTypfilename = args[0];
      }

      private void FormMain_Shown(object sender, EventArgs e) {

         this.Text = sProgname;

         for (int i = 0; i < data4ListboxAreaTyp.Length; i++)
            listBoxAreaTyp.Items.Add(AreaTypName[data4ListboxAreaTyp[i]]);
         for (int i = 0; i < data4ListboxLineTyp.Length; i++)
            listBoxLineTyp.Items.Add(LineTypName[data4ListboxLineTyp[i]]);

         for (int i = 0; i < data4ListboxFont.Length; i++)
            listBoxFont.Items.Add(FontdataName[data4ListboxFont[i]]);
         for (int i = 0; i < data4ListboxCustomcolor.Length; i++)
            listBoxCustomColor.Items.Add(CustomColoursName[data4ListboxCustomcolor[i]]);

         listViewArea.LargeImageList = new ImageList();
         listViewArea.SmallImageList = new ImageList();
         listViewArea.LargeImageList.ImageSize = new System.Drawing.Size(iListViewLargeImageWidth, iListViewLargeImageHeight);
         listViewArea.SmallImageList.ImageSize = new System.Drawing.Size(iListViewSmallImageWidth, iListViewSmallImageHeight);
         listViewArea.TileSize = new System.Drawing.Size(iListViewTileWidth, iListViewTileHeight);
         listViewArea.ListViewItemSorter = new ListViewColumnSorter();
         listViewArea.Columns[1].Width = 100;

         listViewLine.LargeImageList = new ImageList();
         listViewLine.SmallImageList = new ImageList();
         listViewLine.LargeImageList.ImageSize = listViewArea.LargeImageList.ImageSize;
         listViewLine.SmallImageList.ImageSize = listViewArea.SmallImageList.ImageSize;
         listViewLine.TileSize = listViewArea.TileSize;
         listViewLine.ListViewItemSorter = new ListViewColumnSorter();
         listViewPoint.Columns[1].Width = listViewArea.Columns[1].Width;

         listViewPoint.LargeImageList = new ImageList();
         listViewPoint.SmallImageList = new ImageList();
         listViewPoint.LargeImageList.ImageSize = listViewArea.LargeImageList.ImageSize;
         listViewPoint.SmallImageList.ImageSize = listViewArea.SmallImageList.ImageSize;
         listViewPoint.TileSize = listViewArea.TileSize;
         listViewPoint.ListViewItemSorter = new ListViewColumnSorter();
         listViewPoint.Columns[1].Width = listViewArea.Columns[1].Width;

         // max. 16 benutzerdefinierte Farben setzen (ohne Transparenz!!)
         colorDialog1.CustomColors = new int[] {
            //     BBGGRR
               0x000000ff,
               0x000054ff,
               0x0000a8ff,
               0x0000ffff,

               0x0000ff00,
               0x0054ff00,
               0x00a8ff00,
               0x00ffff00,

               0x0000ff00,
               0x0000ff54,
               0x0000ffa8,
               0x0000ffff,

               0x00ff0000,
               0x00ff5400,
               0x00ffa800,
               0x00ffff00
            };

         Array a = Enum.GetValues(typeof(Text.LanguageCode));
         Language4Text = new Text.LanguageCode[a.GetLength(0)];
         a.CopyTo(Language4Text, 0);
         Array.Sort(Language4Text);
         foreach (Text.LanguageCode lang in Language4Text)
            dataGridViewMultiText.Rows.Add(new string[] { lang.ToString(), "" });
         foreach (EncodingInfo info in encinfo)
            toolStripComboBoxCodepage.Items.Add(info.CodePage.ToString() + " - " + info.DisplayName);
         // Untermenü mit allen Sprachen erzeugen
         evPreferredLanguage_Clicked = new EventHandler(PreferredLanguage_Clicked);
         foreach (Text.LanguageCode lang in Language4Text) {
            ToolStripMenuItem it = (ToolStripMenuItem)ToolStripMenuItemExtraLanguage.DropDownItems.Add(lang.ToString(), null, evPreferredLanguage_Clicked);
            it.Tag = lang;       // Sprache für die dieses Item zuständig ist
            if (lang == PreferredLanguage)
               it.Checked = true;
         }
         ToolStripMenuItemExtraGarmincolor.Checked = bPoiWithGarminColor;

         tf = new StdFile_TYP();

         ClearAll();
         SetTypefileStatus(false);

#if DEBUG

         //LoadTypefile("..\\Typefiles\\M0000001.TYP");
         //LoadTypefile("..\\Typefiles\\M05678.typ");
         //LoadTypefile("..\\Typefiles\\topod_org.typ");
         //LoadTypefile("..\\Typefiles\\Sachsen6.TYP");
         //LoadTypefile("..\\Typefiles\\Transalpin_v2.TYP");
         //LoadTypefile("..\\Typefiles\\teddy.typ");
         //LoadTypefile("..\\Typefiles\\italien.typ");
         LoadTypefile("..\\..\\fsoft3.TYP");

#endif

         if (sStartTypfilename != null && sStartTypfilename.Length > 0)
            LoadTypefile(sStartTypfilename);
      }

      private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
         if (bTFChanged)
            if (MessageBox.Show("Wollen sie das Programm beenden ohne zu speichern?", "ACHTUNG",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
               e.Cancel = true;
      }

      void ClearAll() {
         listViewArea.Items.Clear();
         listViewLine.Items.Clear();
         listViewPoint.Items.Clear();
         DiableDataControls4Polygones();
         DiableDataControls4Polylines();
         DiableDataControls4Points();
         DiableDataControls4Text();
      }

      private void LoadTypefile(string file) {
         if (bTFChanged)
            if (MessageBox.Show("Wollen sie die aktuellen Daten erst speichern?", "ACHTUNG",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
               return;
         try {
            ShowAllData4GraphicElement(null);
            SetTypefileStatus(false);
            using (BinaryReaderWriter br = new BinaryReaderWriter(file, true)) {
               tf = new StdFile_TYP(br, true);
               if (tf.RelaxedModeErrors.Length > 0)
                  MessageBox.Show(tf.RelaxedModeErrors, "Fehler beim Einlesen des Typfiles", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            sTypfilename = file;
            this.Text = sProgname + " - " + sTypfilename;
            toolStripTextBoxProductID.Text = tf.ProductID.ToString();
            toolStripTextBoxFamilyID.Text = tf.FamilyID.ToString();
            toolStripComboBoxCodepage.SelectedIndex = -1;
            for (int i = 0; i < encinfo.Length; i++)
               if (encinfo[i].CodePage == tf.Codepage) {
                  toolStripComboBoxCodepage.SelectedIndex = i;
                  break;
               }
            FillListviews();
            SetListviewStatus(ViewStatus);
            SetTypefileStatus(false);

            AddLastfile(file);

         } catch (Exception ex) {
            MessageBox.Show("Fehler beim Öffnen der Datei: " + openFileDialog1.FileName + System.Environment.NewLine +
                            System.Environment.NewLine +
                            ex.Message,
                            "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            tf = null;
         }
      }

      /// <summary>
      /// die Listviews für Polygone, Linien und Punkte füllen
      /// </summary>
      private void FillListviews() {
         // Listview für Flächen füllen
         ListView lv = listViewArea;
         lv.SuspendLayout();
         lv.LargeImageList.Images.Clear();
         lv.SmallImageList.Images.Clear();
         lv.Items.Clear();
         for (int i = 0; i < tf.PolygonCount; i++)
            lv.Items.Add(MakeListViewItem4GraphicElement(tf.GetPolygone(i)));
         lv.ResumeLayout();

         // Listview für Linien füllen
         lv = listViewLine;
         lv.SuspendLayout();
         lv.LargeImageList.Images.Clear();
         lv.SmallImageList.Images.Clear();
         lv.Items.Clear();
         for (int i = 0; i < tf.PolylineCount; i++)
            lv.Items.Add(MakeListViewItem4GraphicElement(tf.GetPolyline(i)));
         lv.ResumeLayout();

         // Listview für Punkte füllen
         lv = listViewPoint;
         lv.SuspendLayout();
         lv.LargeImageList.Images.Clear();
         lv.SmallImageList.Images.Clear();
         lv.Items.Clear();
         for (int i = 0; i < tf.PoiCount; i++)
            lv.Items.Add(MakeListViewItem4GraphicElement(tf.GetPoi(i)));
         lv.ResumeLayout();

      }

      #region Reaktion auf Menüauswahl

      private void ToolStripMenuItemNew_Click(object sender, EventArgs e) {
         tf = new StdFile_TYP();
         sTypfilename = "";
         this.Text = sProgname;
         ClearAll();
         FillListviews();
         SetListviewStatus(ViewStatus);
         SetTypefileStatus(false);
      }

      private void ToolStripMenuItemOpen_Click(object sender, EventArgs e) {
         if (openFileDialog1.ShowDialog() == DialogResult.OK)
            LoadTypefile(openFileDialog1.FileName);
      }

      private void ToolStripMenuItemSave_Click(object sender, EventArgs e) {
         if (sTypfilename == null || sTypfilename.Length == 0)
            ToolStripMenuItemSave2_Click(sender, e);
         else
            try {
               using (BinaryReaderWriter bw = new BinaryReaderWriter(sTypfilename, false, true)) {
                  tf.Write(bw);
                  SetTypefileStatus(false);
               }
            } catch (Exception ex) {
               MessageBox.Show("Fehler beim Speichern der Datei: " + saveFileDialog1.FileName + System.Environment.NewLine +
                               System.Environment.NewLine +
                               ex.Message,
                               "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
      }

      private void ToolStripMenuItemSave2_Click(object sender, EventArgs e) {
         if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
            try {
               using (BinaryReaderWriter bw = new BinaryReaderWriter(saveFileDialog1.FileName, false, true)) {
                  tf.Write(bw);
                  SetTypefileStatus(false);
               }
               sTypfilename = saveFileDialog1.FileName;
               this.Text = sProgname + " - " + sTypfilename;
               AddLastfile(saveFileDialog1.FileName);
            } catch (Exception ex) {
               MessageBox.Show("Fehler beim Speichern der Datei: " + saveFileDialog1.FileName + System.Environment.NewLine +
                               System.Environment.NewLine +
                               ex.Message,
                               "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
      }

      private void ToolStripMenuItemClose_Click(object sender, EventArgs e) {
         Close();
      }

      /// <summary>
      /// alle Bitmaps der Elementliste werden gespeichert
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void ToolStripMenuItemSavePictures_Click(object sender, EventArgs e) {
         if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
            string sDirectory = folderBrowserDialog1.SelectedPath;
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            string sExtension = "png";
            System.Drawing.Imaging.ImageFormat ifmt = System.Drawing.Imaging.ImageFormat.Png;
            if (mi == ToolStripMenuItemSavePng) {
               sExtension = "png";
               ifmt = System.Drawing.Imaging.ImageFormat.Png;
            } else if (mi == ToolStripMenuItemSaveGif) {
               sExtension = "gif";
               ifmt = System.Drawing.Imaging.ImageFormat.Gif;
            } else if (mi == ToolStripMenuItemSaveBmp) {
               sExtension = "bmp";
               ifmt = System.Drawing.Imaging.ImageFormat.Bmp;
            } else if (mi == ToolStripMenuItemSaveTif) {
               sExtension = "tif";
               ifmt = System.Drawing.Imaging.ImageFormat.Tiff;
            }
            ListView lv = GetListView4ToolStripMenuItem(sender);
            if (lv != null) {
               string sPrefix = "";
               if (lv == listViewArea) sPrefix = "F";
               else if (lv == listViewLine) sPrefix = "L";
               else if (lv == listViewPoint) sPrefix = "P";
               Cursor = Cursors.WaitCursor;
               try {
                  for (int i = 0; i < lv.Items.Count; i++) {
                     ListViewItem lvi = lv.Items[i];
                     GraphicElement ge = (GraphicElement)lvi.Tag;
                     string sFilename = string.Format("{0}_0x{1:x3}{2:x2}_T.{3}", sPrefix, ge.Type, ge.Subtype, sExtension);
                     Bitmap bm = ge.AsBitmap(true, true);
                     bm.Save(Path.Combine(sDirectory, sFilename), ifmt);
                     if (ge.WithNightBitmap) {
                        sFilename = string.Format("{0}_0x{1:x3}{2:x2}_N.{3}", sPrefix, ge.Type, ge.Subtype, sExtension);
                        bm = ge.AsBitmap(false, true);
                        bm.Save(Path.Combine(sDirectory, sFilename), ifmt);
                     }
                  }
               } catch (Exception ex) {
                  MessageBox.Show("Fehler beim Speichern: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                  Cursor = DefaultCursor;
               }
               Cursor = DefaultCursor;
            }
         }
      }

      private void ToolStripMenuItemExtraGarmincolor_Click(object sender, EventArgs e) {
         bPoiWithGarminColor = !bPoiWithGarminColor;
         ((ToolStripMenuItem)sender).Checked = bPoiWithGarminColor;
      }

      /// <summary>
      /// eine neue Sprache wurde im Menü eingestellt
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      public void PreferredLanguage_Clicked(object sender, EventArgs e) {
         ToolStripMenuItem it = (ToolStripMenuItem)sender;
         if (!it.Checked) {
            // alle bisher markierten löschen
            foreach (ToolStripItem item in ToolStripMenuItemExtraLanguage.DropDownItems)
               ((ToolStripMenuItem)item).Checked = false;
            it.Checked = true;
            PreferredLanguage = (Text.LanguageCode)it.Tag;
            FillListviews();
         }
      }

      private void ToolStripMenuItemInfo_Click(object sender, EventArgs e) {
         FormInfo dlg = new FormInfo();
         StringBuilder sb = new StringBuilder();
         sb.AppendLine(sProgname);
         sb.AppendLine();
         sb.Append("mit Garmin-Interface-DLL: ");
         sb.AppendLine(Garmin.DllTitle());
         dlg.MyInfoText = sb.ToString();
         dlg.ShowDialog();
      }

      #region Art der Ansicht der Listviews einstellen

      private void ToolStripMenuItemTile_Click(object sender, EventArgs e) {
         SetListviewStatus(View.Tile);
      }

      private void ToolStripMenuItemLarge_Click(object sender, EventArgs e) {
         SetListviewStatus(View.LargeIcon);
      }

      private void ToolStripMenuItemSmall_Click(object sender, EventArgs e) {
         SetListviewStatus(View.SmallIcon);
      }

      private void ToolStripMenuItemList_Click(object sender, EventArgs e) {
         SetListviewStatus(View.List);
      }

      private void ToolStripMenuItemDetail_Click(object sender, EventArgs e) {
         SetListviewStatus(View.Details);
      }

      private void SetListviewStatus(View status) {
         ViewStatus = status;
         ToolStripMenuItemTile.Checked =
         ToolStripMenuItemLarge.Checked =
         ToolStripMenuItemSmall.Checked =
         ToolStripMenuItemList.Checked =
         ToolStripMenuItemDetail.Checked = false;
         switch (ViewStatus) {
            case View.Tile: ToolStripMenuItemTile.Checked = true; break;
            case View.LargeIcon: ToolStripMenuItemLarge.Checked = true; break;
            case View.SmallIcon: ToolStripMenuItemSmall.Checked = true; break;
            case View.List: ToolStripMenuItemList.Checked = true; break;
            case View.Details: ToolStripMenuItemDetail.Checked = true; break;
         }
         listViewArea.View =
         listViewLine.View =
         listViewPoint.View = ViewStatus;
         if (ViewStatus == View.Details) {
            listViewArea.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewLine.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewPoint.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
         } else {
            //listViewArea.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            //listViewLine.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            //listViewPoint.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
         }
      }

      #endregion

      #endregion

      #region Reaktion auf Toolbar

      private void toolStripButtonSave_Click(object sender, EventArgs e) {
         ToolStripMenuItemSave_Click(null, null);    // nur weiterleiten
      }

      private void toolStripButtonOpen_Click(object sender, EventArgs e) {
         ToolStripMenuItemOpen_Click(null, null);
      }

      private void toolStripTextBoxProductID_TextChanged(object sender, EventArgs e) {
         ToolStripTextBox tb = (ToolStripTextBox)sender;
         SetTypefileStatus(true);
         if (tb.Text.Length == 0) tf.ProductID = 0;
         else
            try {
               tf.ProductID = Convert.ToUInt16(tb.Text);
            } catch (Exception) {
               MessageBox.Show("Dieser Wert kann nicht gesetzt werden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Stop);
               tf.ProductID = 0;
            }
      }

      /// <summary>
      /// die FamilyID wurde geändert
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void toolStripTextBoxFamilyID_TextChanged(object sender, EventArgs e) {
         ToolStripTextBox tb = (ToolStripTextBox)sender;
         SetTypefileStatus(true);
         if (tb.Text.Length == 0) tf.FamilyID = 0;
         else
            try {
               tf.FamilyID = Convert.ToUInt16(tb.Text);
            } catch (Exception) {
               MessageBox.Show("Dieser Wert kann nicht gesetzt werden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Stop);
               tf.FamilyID = 0;
            }
      }

      /// <summary>
      /// die Codepage wurde geändert
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void toolStripComboBoxCodepage_SelectedIndexChanged(object sender, EventArgs e) {
         ToolStripComboBox cb = (ToolStripComboBox)sender;
         if (cb.SelectedIndex >= 0) {
            tf.Codepage = (ushort)encinfo[cb.SelectedIndex].CodePage;
         }
      }

      private void toolStripTextBoxProductID_KeyPress(object sender, KeyPressEventArgs e) {
         if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))   // wenn keine Ziffer
            e.Handled = true;
      }

      private void toolStripTextBoxFamilyID_KeyPress(object sender, KeyPressEventArgs e) {
         if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))   // wenn keine Ziffer
            e.Handled = true;
      }

      #endregion

      #region Kontextmenü "Graphicelement"

      /// <summary>
      /// fügt ein neues GraphicElement in das Typfile und in die richtige Liste ein
      /// </summary>
      /// <param name="lv"></param>
      /// <param name="bAsCopy">Kopie des akt. Elements erzeugen</param>
      /// <param name="bChange">Typ des akt. Elements ändern (hat Vorrang vor bAsCopy)</param>
      private void InsertNewGraphicElement(ListView lv, bool bAsCopy, bool bChange) {
         ListViewItem lvi = lv.SelectedItems.Count > 0 ? lv.SelectedItems[0] : null;
         GraphicElement ge = lvi != null ? (GraphicElement)lvi.Tag : null;
         uint typ = 0, subtyp = 0;
         if (ge != null) {
            typ = ge.Type;
            subtyp = ge.Subtype;
         } else
            bAsCopy = false;

         FormTyp dlg = new FormTyp();
         dlg.Input_Typ = typ;
         dlg.Input_Subtyp = subtyp;
         dlg.Input_Name = ge != null ? ge.Text.Get(PreferredLanguage) : "";
         if (dlg.ShowDialog() == DialogResult.OK) {
            try {
               MultiText mt = ge != null ? new MultiText(ge.Text) : new MultiText();
               mt.Set(PreferredLanguage, dlg.Input_Name);

               if (!bChange) {         // es geht um ein neues Element
                  GraphicElement ge_new = null;
                  if (lv == listViewArea) {
                     if (bAsCopy)
                        ge_new = ((Polygone)ge).GetCopy(dlg.Input_Typ, dlg.Input_Subtyp);
                     else
                        ge_new = new Polygone(dlg.Input_Typ, dlg.Input_Subtyp);
                  }
                  if (lv == listViewLine) {
                     if (bAsCopy)
                        ge_new = ((Polyline)ge).GetCopy(dlg.Input_Typ, dlg.Input_Subtyp);
                     else
                        ge_new = new Polyline(dlg.Input_Typ, dlg.Input_Subtyp);
                  }
                  if (lv == listViewPoint) {
                     if (bAsCopy)
                        ge_new = ((POI)ge).GetCopy(dlg.Input_Typ, dlg.Input_Subtyp);
                     else
                        ge_new = new POI(dlg.Input_Typ, dlg.Input_Subtyp);
                  }
                  if (ge_new != null) {
                     if (!tf.Insert(ge_new))
                        throw new Exception("Das Element '" + ElementTyp2Text(ge_new) + "' existiert schon.");
                     ge_new.SetText(mt);
                     ge = ge_new;
                  }
               } else {             // es geht um eine Typänderung
                  if (!tf.ChangeTyp(ge, dlg.Input_Typ, dlg.Input_Subtyp))
                     throw new Exception("Das Element '" + ElementTyp2Text(ge) + "' existiert schon.");
                  ge.SetText(mt);            // Text könnte auch geändert sein
                  RemoveListItem(lvi);       // altes Element aus der Liste entfernen
               }
               lvi = MakeListViewItem4GraphicElement(ge);
               lv.Items.Add(lvi);
               lvi.Selected = true;
               SetTypefileStatus(true);

               if (lv.View != View.SmallIcon &&
                   lv.View != View.LargeIcon &&
                   lv.View != View.Tile)
                  lv.TopItem = lvi;
            } catch (Exception ex) {
               MessageBox.Show("Fehler beim Einfügen eines neuen Elementes: " + ex.Message, "FEHLER", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
      }

      private void ToolStripMenuItemElementNew_Click(object sender, EventArgs e) {
         ListView lv = GetListView4ToolStripMenuItem(sender);
         if (lv != null)
            InsertNewGraphicElement(lv, false, false);
      }

      private void ToolStripMenuItemElementCopyTo_Click(object sender, EventArgs e) {
         ListView lv = GetListView4ToolStripMenuItem(sender);
         if (lv != null)
            InsertNewGraphicElement(lv, true, false);
      }

      private void ToolStripMenuItemElementDelete_Click(object sender, EventArgs e) {
         ListView lv = GetListView4ToolStripMenuItem(sender);
         if (lv != null) {
            ListViewItem lvi = lv.SelectedItems[0];
            GraphicElement ge = (GraphicElement)lvi.Tag;
            if (ge != null) {
               if (MessageBox.Show(string.Format("Wollen sie das Element [{0}] wirklich löschen?", ElementTyp2Text(ge)),
                                   "Achtung",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question) == DialogResult.Yes) {
                  uint typ = ge.Type;
                  uint subtyp = ge.Subtype;
                  if (ge is Polygone) tf.RemovePolygone(typ, subtyp);
                  if (ge is Polyline) tf.RemovePolyline(typ, subtyp);
                  if (ge is POI) tf.RemovePoi(typ, subtyp);

                  int idx = lvi.Index;
                  RemoveListItem(lvi);

                  if (idx < lv.Items.Count - 1)
                     lv.Items[idx].Selected = true;
                  else
                     if (lv.Items.Count > 0)
                     lv.Items[lv.Items.Count - 1].Selected = true;

                  SetTypefileStatus(true);
               }
            }
         }
      }

      private void ToolStripMenuItemElementTypchange_Click(object sender, EventArgs e) {
         ListView lv = GetListView4ToolStripMenuItem(sender);
         if (lv != null)
            InsertNewGraphicElement(lv, false, true);
      }

      private void contextMenuStripElement_Opening(object sender, CancelEventArgs e) {
         ContextMenuStrip cm = (ContextMenuStrip)sender;
         if (cm.SourceControl != null && cm.SourceControl is ListView) {
            cm.Tag = cm.SourceControl;
            ListView lv = (ListView)cm.SourceControl;
            ToolStripMenuItemElementDelete.Enabled =
            ToolStripMenuItemElementCopyTo.Enabled =
            ToolStripMenuItemElementSaveAllPictures.Enabled = lv.Items.Count > 0;
         }
      }

      #endregion

      #region spezielle Funktionen für Polygone

      #region Kontextmenü Polygone: Farben/Bitmap kopieren bzw. einfügen

      /// <summary>
      /// Farbe 1 sollneu gesetzt werden
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void ToolStripMenuAreaColor1_Click(object sender, EventArgs e) {
         SetGraphicElementColor(true, pictureBoxAreaDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
      }

      /// <summary>
      /// Farbe 2 sollneu gesetzt werden
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void ToolStripMenuAreaColor2_Click(object sender, EventArgs e) {
         SetGraphicElementColor(false, pictureBoxAreaDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
      }

      private void ToolStripMenuItemAreaColor1Transparent_Click(object sender, EventArgs e) {
         PictureBox pb = GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender);
         if (pb != null) {
            if (pb == pictureBoxAreaDay ||
                pb == pictureBoxAreaNight) {
               GraphicElement ge = GetSelectedAndActiveGraphicElement();
               if (ge != null) {
                  bool bIsDayColor = pictureBoxAreaDay == pb;
                  if (bIsDayColor)
                     ge.DayColor1 = Color.Transparent;
                  else
                     ge.NightColor1 = Color.Transparent;
                  ShowSelectedGraphicElementWithNewProps();
               }
            }
         }
      }

      private void ToolStripMenuAreaColorSwap_Click(object sender, EventArgs e) {
         Polygone p = GetSelectedPolygone();
         if (p != null) {
            p.SwapColors(pictureBoxAreaDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
            ShowSelectedGraphicElementWithNewProps();
         }
      }

      private void ToolStripMenuAreaCopy_Click(object sender, EventArgs e) {
         SelectedGraphicElement2Clipboard(pictureBoxAreaDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
      }

      private void ToolStripMenuAreaInsert_Click(object sender, EventArgs e) {
         if (Clipboard.ContainsImage()) {
            bool bDayPictureBox = pictureBoxAreaDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender);
            Polygone p = GetSelectedPolygone();
            Bitmap bm = new Bitmap(Clipboard.GetImage());
            if (bm.Width != 32 || bm.Height != 32) {
               if (MessageBox.Show("Das Bild hat nicht die Größe 32 x 32. Soll es angepasst werden?",
                                   "Achtung",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question) == DialogResult.No)
                  return;
               bm = new Bitmap(bm, new Size(32, 32));
            }
            try {
               if (p != null) {
                  p.SetBitmaps(p.Colortype, bDayPictureBox ? bm : p.AsBitmap(true, true), bDayPictureBox ? p.AsBitmap(false, true) : bm);
                  ShowSelectedGraphicElementWithNewProps();
               }
            } catch (Exception ex) {
               MessageBox.Show("Fehler beim Einfügen der Grafik: " + ex.Message, "FEHLER", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
         }
      }

      private void contextMenuStripArea_Opening(object sender, CancelEventArgs e) {
         ContextMenuStrip cm = (ContextMenuStrip)sender;
         if (cm.SourceControl != null && cm.SourceControl is PictureBox) {
            bool bDayPictureBox = pictureBoxAreaDay == GetPictureBox4ContextMenu((ContextMenuStrip)sender);
            foreach (ToolStripItem item in cm.Items)
               item.Enabled = false;

            if (listViewArea.SelectedItems.Count > 0) {
               ListViewItem lvi = listViewArea.SelectedItems[0];
               Polygone p = (Polygone)lvi.Tag;

               Bitmap bm = new Bitmap(10, 10);
               Graphics canvas = Graphics.FromImage(bm);
               canvas.Clear(bDayPictureBox ? p.DayColor1 : p.NightColor1);
               canvas.Flush();
               ToolStripMenuItemAreaColor1.Image = bm;

               bm = new Bitmap(10, 10);
               canvas = Graphics.FromImage(bm);
               canvas.Clear(bDayPictureBox ? p.DayColor2 : p.NightColor2);
               canvas.Flush();
               ToolStripMenuItemAreaColor2.Image = bm;

               switch (p.Colortype) {
                  case Polygone.ColorType.Day1:
                     if (bDayPictureBox) {
                        ToolStripMenuItemAreaColor1.Enabled =
                        ToolStripMenuItemAreaCopy.Enabled = true;
                        ToolStripMenuItemAreaColor1Transparent.Enabled = true;
                     }
                     break;

                  case Polygone.ColorType.Day1_Night1:
                     ToolStripMenuItemAreaColor1.Enabled =
                     ToolStripMenuItemAreaCopy.Enabled = true;
                     ToolStripMenuItemAreaColor1Transparent.Enabled = true;
                     break;

                  case Polygone.ColorType.BM_Day1:
                     if (bDayPictureBox) {
                        ToolStripMenuItemAreaColor1.Enabled =
                        ToolStripMenuItemAreaColorSwap.Enabled =
                        ToolStripMenuItemAreaCopy.Enabled = true;
                        ToolStripMenuItemAreaInsert.Enabled = Clipboard.ContainsImage();
                        ToolStripMenuItemAreaColor1Transparent.Enabled = true;
                     }
                     break;

                  case Polygone.ColorType.BM_Day1_Night2:
                     ToolStripMenuItemAreaColor1.Enabled =
                     ToolStripMenuItemAreaColorSwap.Enabled =
                     ToolStripMenuItemAreaCopy.Enabled = true;
                     ToolStripMenuItemAreaInsert.Enabled = Clipboard.ContainsImage();
                     if (bDayPictureBox)
                        ToolStripMenuItemAreaColor1Transparent.Enabled = true;
                     else
                        ToolStripMenuItemAreaColor2.Enabled = true;
                     break;

                  case Polygone.ColorType.BM_Day1_Night1:
                     ToolStripMenuItemAreaColor1.Enabled =
                     ToolStripMenuItemAreaColorSwap.Enabled =
                     ToolStripMenuItemAreaCopy.Enabled = true;
                     ToolStripMenuItemAreaInsert.Enabled = Clipboard.ContainsImage();
                     ToolStripMenuItemAreaColor1Transparent.Enabled = true;
                     break;

                  case Polygone.ColorType.BM_Day2_Night1:
                     ToolStripMenuItemAreaColor1.Enabled =
                     ToolStripMenuItemAreaColorSwap.Enabled =
                     ToolStripMenuItemAreaCopy.Enabled = true;
                     ToolStripMenuItemAreaInsert.Enabled = Clipboard.ContainsImage();
                     if (bDayPictureBox)
                        ToolStripMenuItemAreaColor2.Enabled = true;
                     else
                        ToolStripMenuItemAreaColor1Transparent.Enabled = true;
                     break;

                  case Polygone.ColorType.BM_Day2:
                     if (bDayPictureBox) {
                        ToolStripMenuItemAreaColor1.Enabled =
                        ToolStripMenuItemAreaColor2.Enabled =
                        ToolStripMenuItemAreaColorSwap.Enabled =
                        ToolStripMenuItemAreaCopy.Enabled = true;
                        ToolStripMenuItemAreaInsert.Enabled = Clipboard.ContainsImage();
                     }
                     break;

                  case Polygone.ColorType.BM_Day2_Night2:
                     ToolStripMenuItemAreaColor1.Enabled =
                     ToolStripMenuItemAreaColor2.Enabled =
                     ToolStripMenuItemAreaColorSwap.Enabled =
                     ToolStripMenuItemAreaCopy.Enabled = true;
                     ToolStripMenuItemAreaInsert.Enabled = Clipboard.ContainsImage();
                     break;
               }
            }
         }

      }

      #endregion

      private void listViewArea_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
         Polygone p = e.Item.Tag != null ? (Polygone)e.Item.Tag : null;
         ShowAllData4GraphicElement(e.IsSelected ? p : null);
      }

      private void listBoxAreaTyp_SelectedIndexChanged(object sender, EventArgs e) {
         Polygone p = GetSelectedPolygone();
         if (p != null && p.Colortype != data4ListboxAreaTyp[listBoxAreaTyp.SelectedIndex]) {
            p.Colortype = data4ListboxAreaTyp[listBoxAreaTyp.SelectedIndex];
            ShowSelectedGraphicElementWithNewProps();
         }
      }

      private void numericUpDownAreaDraworder_ValueChanged(object sender, EventArgs e) {
         Polygone p = GetSelectedPolygone();
         if (p != null && p.Draworder != numericUpDownAreaDraworder.Value) {
            p.Draworder = (uint)numericUpDownAreaDraworder.Value;
            ShowSelectedGraphicElementWithNewProps();
         }
      }

      /// <summary>
      /// liefert das 1. selektierte Polygon oder null
      /// </summary>
      /// <returns></returns>
      private Polygone GetSelectedPolygone() {
         return listViewArea.SelectedItems.Count > 0 ? (Polygone)listViewArea.SelectedItems[0].Tag : null;
      }

      /// <summary>
      /// die Fläche wird in der angegebenen PicturBox dargestellt
      /// </summary>
      /// <param name="pb"></param>
      /// <param name="org"></param>
      private void ShowAreaInPicturebox(PictureBox pb, Bitmap org) {
         if (org != null && org.Height > 0) {
            pb.Image = org;
         } else
            pb.Image = null;
      }

      #endregion

      #region spezielle Funktionen für Linien

      #region Kontextmenü Linien: Farben/Bitmap kopieren bzw. einfügen

      private void ToolStripMenuItemLineColor1_Click(object sender, EventArgs e) {
         SetGraphicElementColor(true, pictureBoxLineDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
      }

      private void ToolStripMenuItemLineColor2_Click(object sender, EventArgs e) {
         SetGraphicElementColor(false, pictureBoxLineDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
      }

      private void ToolStripMenuItemLineColor1Transparent_Click(object sender, EventArgs e) {
         PictureBox pb = GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender);
         if (pb != null) {
            if (pb == pictureBoxLineDay ||
                pb == pictureBoxLineNight) {
               GraphicElement ge = GetSelectedAndActiveGraphicElement();
               if (ge != null) {
                  bool bIsDayColor = pictureBoxLineDay == pb;
                  if (bIsDayColor)
                     ge.DayColor1 = Color.Transparent;
                  else
                     ge.NightColor1 = Color.Transparent;
                  ShowSelectedGraphicElementWithNewProps();
               }
            }
         }
      }

      private void ToolStripMenuItemLineColorSwap_Click(object sender, EventArgs e) {
         Polyline p = GetSelectedPolyline();
         if (p != null) {
            p.SwapColors(pictureBoxLineDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
            ShowSelectedGraphicElementWithNewProps();
         }
      }

      private void ToolStripMenuItemLineCopy_Click(object sender, EventArgs e) {
         SelectedGraphicElement2Clipboard(pictureBoxLineDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
      }

      private void ToolStripMenuItemLineInsert_Click(object sender, EventArgs e) {
         if (Clipboard.ContainsImage()) {
            Polyline p = GetSelectedPolyline();
            Bitmap bm = new Bitmap(Clipboard.GetImage());
            if (bm.Width != 32 || bm.Height >= 32) {
               if (MessageBox.Show("Das Bild hat nicht die Größe 32 x n (n<32). Soll es angepasst werden?",
                                   "Achtung",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question) == DialogResult.No)
                  return;
               bm = new Bitmap(bm, new Size(32, bm.Height >= 32 ? 31 : bm.Height));
            }
            try {
               if (p != null) {
                  p.SetBitmap(p.Polylinetype, bm);
                  ShowSelectedGraphicElementWithNewProps();
               }
            } catch (Exception ex) {
               MessageBox.Show("Fehler beim Einfügen der Grafik: " + ex.Message, "FEHLER", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
         }
      }

      private void contextMenuStripLine_Opening(object sender, CancelEventArgs e) {
         ContextMenuStrip cm = (ContextMenuStrip)sender;
         if (cm.SourceControl != null && cm.SourceControl is PictureBox) {
            bool bDayPictureBox = pictureBoxLineDay == GetPictureBox4ContextMenu(cm);
            foreach (ToolStripItem item in cm.Items)
               item.Enabled = false;

            if (listViewLine.SelectedItems.Count > 0) {
               ListViewItem lvi = listViewLine.SelectedItems[0];
               Polyline p = (Polyline)lvi.Tag;

               Bitmap bm = new Bitmap(10, 10);
               Graphics canvas = Graphics.FromImage(bm);
               canvas.Clear(bDayPictureBox ? p.DayColor1 : p.NightColor1);
               canvas.Flush();
               ToolStripMenuItemLineColor1.Image = bm;

               bm = new Bitmap(10, 10);
               canvas = Graphics.FromImage(bm);
               canvas.Clear(bDayPictureBox ? p.DayColor2 : p.NightColor2);
               canvas.Flush();
               ToolStripMenuItemLineColor2.Image = bm;

               if (bDayPictureBox) {
                  switch (p.Polylinetype) {
                     case Polyline.PolylineType.Day2:
                     case Polyline.PolylineType.Day2_Night2:
                     case Polyline.PolylineType.NoBorder_Day2_Night1:
                        ToolStripMenuItemLineColor1.Enabled =
                        ToolStripMenuItemLineColor2.Enabled =
                        ToolStripMenuItemLineColorSwap.Enabled =
                        ToolStripMenuItemLineCopy.Enabled =
                        ToolStripMenuItemLineInsert.Enabled = true;
                        break;
                     case Polyline.PolylineType.Day1_Night2:
                     case Polyline.PolylineType.NoBorder_Day1:
                     case Polyline.PolylineType.NoBorder_Day1_Night1:
                        ToolStripMenuItemLineColor1.Enabled =
                        ToolStripMenuItemLineCopy.Enabled =
                        ToolStripMenuItemLineInsert.Enabled = true;
                        if (p.WithDayBitmap)
                           ToolStripMenuItemLineColorSwap.Enabled = true;
                        ToolStripMenuItemLineColor1Transparent.Enabled = true;
                        break;
                  }
               } else
                  switch (p.Polylinetype) {
                     case Polyline.PolylineType.Day2:
                        break;
                     case Polyline.PolylineType.NoBorder_Day2_Night1:
                     case Polyline.PolylineType.NoBorder_Day1_Night1:
                        ToolStripMenuItemLineColor1.Enabled =
                        ToolStripMenuItemLineCopy.Enabled =
                        ToolStripMenuItemLineInsert.Enabled = true;
                        if (p.WithDayBitmap)
                           ToolStripMenuItemLineColorSwap.Enabled = true;
                        ToolStripMenuItemLineColor1Transparent.Enabled = true;
                        break;
                     case Polyline.PolylineType.Day2_Night2:
                     case Polyline.PolylineType.Day1_Night2:
                        ToolStripMenuItemLineColor1.Enabled =
                        ToolStripMenuItemLineColor2.Enabled =
                        ToolStripMenuItemLineColorSwap.Enabled =
                        ToolStripMenuItemLineCopy.Enabled =
                        ToolStripMenuItemLineInsert.Enabled = true;
                        break;
                  }
            }
         }
      }

      #endregion

      /// <summary>
      /// liefert das 1. selektierte Polyline oder null
      /// </summary>
      /// <returns></returns>
      private Polyline GetSelectedPolyline() {
         return listViewLine.SelectedItems.Count > 0 ? (Polyline)listViewLine.SelectedItems[0].Tag : null;
      }

      /// <summary>
      /// eine andere Linie wurde ausgewählt
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void listViewLine_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
         Polyline p = e.Item.Tag != null ? (Polyline)e.Item.Tag : null;
         ShowAllData4GraphicElement(e.IsSelected ? p : null);
      }

      /// <summary>
      /// die Linienbreite wurde verändert
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void numericUpDownLineHeight_ValueChanged(object sender, EventArgs e) {
         if (IsControlValueSettingsValidatedOn((Control)sender)) {
            if (numericUpDownLineBorder.Enabled) {          // ev. verändern
               int maxborder = (int)(numericUpDownLineHeight.Value / 2);
               if (maxborder % 2 == 0 && maxborder > 0) maxborder--;
               numericUpDownLineBorder.Tag = true;
               if (numericUpDownLineBorder.Value > maxborder)
                  numericUpDownLineBorder.Value = maxborder;
               numericUpDownLineBorder.Maximum = maxborder;
               numericUpDownLineBorder.Tag = null;
            }
            Polyline p = GetSelectedAndActiveGraphicElement() as Polyline;
            if (p != null) {
               p.SetWidthAndColors(p.Polylinetype,
                                   (uint)(numericUpDownLineHeight.Value - 2 * numericUpDownLineBorder.Value),
                                   (uint)numericUpDownLineBorder.Value);
               ShowSelectedGraphicElementWithNewProps();
            }
         }
      }

      /// <summary>
      /// die Randbreite einer Linie wurde verändert
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void numericUpDownLineBorder_ValueChanged(object sender, EventArgs e) {
         if (IsControlValueSettingsValidatedOn((Control)sender)) {
            Polyline p = GetSelectedAndActiveGraphicElement() as Polyline;
            if (p != null) {
               p.SetWidthAndColors(p.Polylinetype,
                                   (uint)(Math.Max(1, numericUpDownLineHeight.Value - 2 * numericUpDownLineBorder.Value)),
                                   (uint)numericUpDownLineBorder.Value);
               ShowSelectedGraphicElementWithNewProps();
            }
         }
      }

      /// <summary>
      /// die TextRotation-Eigenschaft wurde geändert
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void checkBoxLineTextRotation_CheckedChanged(object sender, EventArgs e) {
         if (IsControlValueSettingsValidatedOn((Control)sender)) {
            Polyline p = GetSelectedAndActiveGraphicElement() as Polyline;
            if (p != null)
               p.WithTextRotation = ((CheckBox)sender).Checked;
            ShowSelectedGraphicElementWithNewProps();
         }
      }

      /// <summary>
      /// der Linientyp wird geändert
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void listBoxLineTyp_SelectedIndexChanged(object sender, EventArgs e) {
         if (IsControlValueSettingsValidatedOn((Control)sender)) {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0) {
               Polyline p = GetSelectedPolyline();
               if (p != null) {
                  p.Polylinetype = data4ListboxLineTyp[idx];
                  ShowSelectedGraphicElementWithNewProps();
               }
            }
         }
      }

      private void radioButtonLineSolidColor_CheckedChanged(object sender, EventArgs e) {
         if (IsControlValueSettingsValidatedOn((Control)sender)) {
            if (((RadioButton)sender).Checked) {
               numericUpDownLineHeight.Enabled =
               numericUpDownLineBorder.Enabled = true;
               Polyline p = GetSelectedAndActiveGraphicElement() as Polyline;
               if (p != null)
                  p.SetWidthAndColors(Polyline.PolylineType.Day2, p.BitmapHeight, 0);
               ShowSelectedGraphicElementWithNewProps();
            }
         }
      }

      private void radioButtonLineBitmap_CheckedChanged(object sender, EventArgs e) {
         if (IsControlValueSettingsValidatedOn((Control)sender)) {
            if (((RadioButton)sender).Checked) {
               numericUpDownLineHeight.Enabled =
               numericUpDownLineBorder.Enabled = false;
               Polyline p = GetSelectedAndActiveGraphicElement() as Polyline;
               if (p != null) {
                  Bitmap bmday = new Bitmap(32, (int)(p.InnerWidth + 2 * p.BorderWidth));
                  Graphics canvas = Graphics.FromImage(bmday);
                  canvas.Clear(p.DayColor2);
                  if (p.BorderWidth > 0) {
                     canvas.FillRectangle(new SolidBrush(p.DayColor1), 0, 0, bmday.Width, p.BorderWidth);
                     canvas.FillRectangle(new SolidBrush(p.DayColor1), 0, p.BorderWidth + p.InnerWidth, bmday.Width, p.BorderWidth);
                  }
                  canvas.Flush();
                  Bitmap bmdnight = null;
                  if (p.Polylinetype == Polyline.PolylineType.Day1_Night2 ||
                      p.Polylinetype == Polyline.PolylineType.Day2_Night2) {
                     bmdnight = new Bitmap(bmday.Width, bmday.Height);
                     canvas = Graphics.FromImage(bmdnight);
                     canvas.Clear(p.NightColor2);
                     if (p.BorderWidth > 0) {
                        canvas.FillRectangle(new SolidBrush(p.NightColor1), 0, 0, bmdnight.Width, p.BorderWidth);
                        canvas.FillRectangle(new SolidBrush(p.NightColor1), 0, p.BorderWidth + p.InnerWidth, bmdnight.Width, p.BorderWidth);
                     }
                     canvas.Flush();
                  }
                  p.SetBitmap(Polyline.PolylineType.Day2, bmday);
               }
               ShowSelectedGraphicElementWithNewProps();
            }
         }
      }

      /// <summary>
      /// passt die PictureBox in der Größe an und füllt sie mit dem Bitmap für die Liniendarstellung
      /// </summary>
      /// <param name="pb"></param>
      /// <param name="org"></param>
      private void ShowLineInPicturebox(PictureBox pb, Bitmap org) {
         if (org != null && org.Height > 0) {
            pb.ClientSize = new Size(pb.ClientSize.Width, (pb.ClientSize.Width * org.Height) / org.Width);

            Bitmap bm = new Bitmap(pb.ClientSize.Width, pb.ClientSize.Height);
            Graphics canvas = Graphics.FromImage(bm);
            canvas.DrawImage(org, new Rectangle(0, 0, bm.Width, bm.Height));
            canvas.Flush();
            pb.Image = bm;
         } else {
            pb.ClientSize = new Size(pb.ClientSize.Width, 0);
            pb.Image = null;
         }
      }

      /// <summary>
      /// passt die PictureBox in der Größe an und füllt sie mit der Liniendarstellung
      /// </summary>
      /// <param name="pb"></param>
      /// <param name="col1"></param>
      /// <param name="innerwidth"></param>
      /// <param name="col2"></param>
      /// <param name="borderwidth"></param>
      private void ShowLineInPicturebox(PictureBox pb, Color col1, int innerwidth, Color col2, int borderwidth) {
         int width = pb.ClientSize.Width * (innerwidth + 2 * borderwidth) / 32;  // Linienbreite auf 32 bezogen (analog Bitmapbreite)
         if (width > 0) {
            pb.ClientSize = new Size(pb.ClientSize.Width, width);

            Bitmap bm = new Bitmap(pb.ClientSize.Width, pb.ClientSize.Height);
            Graphics canvas = Graphics.FromImage(bm);
            if (borderwidth > 0)
               canvas.FillRectangle(new SolidBrush(col2), 0, 0, bm.Width, bm.Height);
            canvas.FillRectangle(new SolidBrush(col1), 0, (bm.Width * borderwidth) / 32, bm.Width, (bm.Width * innerwidth) / 32);
            canvas.Flush();
            pb.Image = bm;
         } else {
            pb.ClientSize = new Size(pb.ClientSize.Width, 0);
            pb.Image = null;
         }
      }

      /// <summary>
      /// Anzeige der Linie als Bilder
      /// </summary>
      /// <param name="p"></param>
      private void ShowLineInPictureboxes(Polyline p) {
         if (p != null)
            switch (p.Polylinetype) {
               case Polyline.PolylineType.Day2:
                  if (p.WithDayBitmap) {
                     ShowLineInPicturebox(pictureBoxLineDay, p.AsBitmap(true, true));
                     ShowLineInPicturebox(pictureBoxLineNight, null);
                  } else {
                     ShowLineInPicturebox(pictureBoxLineDay, p.DayColor1, (int)p.InnerWidth, p.DayColor2, (int)p.BorderWidth);
                     ShowLineInPicturebox(pictureBoxLineNight, null);
                  }
                  break;
               case Polyline.PolylineType.Day1_Night2:
               case Polyline.PolylineType.Day2_Night2:
                  if (p.WithDayBitmap) {
                     ShowLineInPicturebox(pictureBoxLineDay, p.AsBitmap(true, true));
                     ShowLineInPicturebox(pictureBoxLineNight, p.AsBitmap(false, true));
                  } else {
                     ShowLineInPicturebox(pictureBoxLineDay, p.DayColor1, (int)p.InnerWidth, p.DayColor2, (int)p.BorderWidth);
                     ShowLineInPicturebox(pictureBoxLineNight, p.NightColor1, (int)p.InnerWidth, p.NightColor2, (int)p.BorderWidth);
                  }
                  break;
               case Polyline.PolylineType.NoBorder_Day1:
                  numericUpDownLineBorder.Enabled = false;
                  if (p.WithDayBitmap) {
                     ShowLineInPicturebox(pictureBoxLineDay, p.AsBitmap(true, true));
                     ShowLineInPicturebox(pictureBoxLineNight, null);
                  } else {
                     ShowLineInPicturebox(pictureBoxLineDay, p.DayColor1, (int)p.InnerWidth, p.DayColor2, (int)p.BorderWidth);
                     ShowLineInPicturebox(pictureBoxLineNight, null);
                  }
                  break;
               case Polyline.PolylineType.NoBorder_Day1_Night1:
               case Polyline.PolylineType.NoBorder_Day2_Night1:
                  numericUpDownLineBorder.Enabled = false;
                  if (p.WithDayBitmap) {
                     ShowLineInPicturebox(pictureBoxLineDay, p.AsBitmap(true, true));
                     ShowLineInPicturebox(pictureBoxLineNight, p.AsBitmap(false, true));
                  } else {
                     ShowLineInPicturebox(pictureBoxLineDay, p.DayColor1, (int)p.InnerWidth, p.DayColor2, (int)p.BorderWidth);
                     ShowLineInPicturebox(pictureBoxLineNight, p.NightColor1, (int)p.InnerWidth, p.NightColor2, (int)p.BorderWidth);
                  }
                  break;
            }
      }

      #endregion

      #region spezielle Funktionen für Punkte

      #region Kontextmenü Punkt: Bitmap kopieren bzw. einfügen

      private void ToolStripMenuItemPointCopy_Click(object sender, EventArgs e) {
         SelectedGraphicElement2Clipboard(pictureBoxPointDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender));
      }

      private void ToolStripMenuItemPointInsert_Click(object sender, EventArgs e) {
         if (Clipboard.ContainsImage()) {
            try {
               Bitmap bm = new Bitmap(Clipboard.GetImage());
               if (bm != null)
                  ToolStripMenuItemPointInsert_Core(bm, (ToolStripMenuItem)sender);
            } catch (Exception ex) {
               MessageBox.Show("Fehler beim Einfügen der Grafik aus der Zwischenablage: "
                  + ex.Message, "FEHLER", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
         }
      }

      private void ToolStripMenuItemPointLoad_Click(object sender, EventArgs e) {
         if (openFileDialogPicture.ShowDialog() == DialogResult.OK) {
            try {
               Bitmap bm = new Bitmap(openFileDialogPicture.FileName);
               if (bm != null)
                  ToolStripMenuItemPointInsert_Core(bm, (ToolStripMenuItem)sender);
            } catch (Exception ex) {
               MessageBox.Show("Fehler beim Einfügen der Grafik aus der Datei '" + openFileDialogPicture.FileName + "': " +
                  ex.Message, "FEHLER", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
         }
      }

      private void ToolStripMenuItemPointInsert_Core(Bitmap bm, ToolStripMenuItem sender) {
         bool bDayPictureBox = pictureBoxPointDay == GetPictureBox4ToolStripMenuItem((ToolStripMenuItem)sender);
         POI p = GetSelectedPoi();
         if (p != null) {
            if (bm != null)
               try {
                  p.SetBitmaps(bDayPictureBox ? bm : p.AsBitmap(true),
                               bDayPictureBox ? (p.WithNightBitmap ? p.AsBitmap(false) : null) : bm,
                               bPoiWithGarminColor);
                  ShowSelectedGraphicElementWithNewProps();
               } catch (Exception ex) {
                  MessageBox.Show("Fehler beim Einfügen der Grafik: " + ex.Message, "FEHLER", MessageBoxButtons.OK, MessageBoxIcon.Stop);
               }
         }
      }

      bool bWait4TranspClick = false;

      private void ToolStripMenuItemPointSetTransp_Click(object sender, EventArgs e) {
         bWait4TranspClick = true;
         pictureBoxPointDay.Cursor = Cursors.Cross;
         Cursor.Clip = pictureBoxPointDay.RectangleToScreen(pictureBoxPointDay.ClientRectangle);
      }

      private void pictureBoxPoint_Click(object sender, EventArgs e) {
         PictureBox pb = (PictureBox)sender;
         if (bWait4TranspClick) {
            bWait4TranspClick = false;
            pb.Cursor = DefaultCursor;
            Cursor.Clip = Screen.PrimaryScreen.Bounds;
            Point mp = pb.PointToClient(MousePosition);
            if (pb.Image != null) {
               FormColorPick dlg = new FormColorPick();
               Bitmap bm = pb.Image as Bitmap;
               dlg.MyTrColor = bm.GetPixel(Math.Min(Math.Max(0, mp.X), bm.Width - 1), Math.Min(Math.Max(0, mp.Y), bm.Height - 1));
               // Das Formular dicht neben die PictureBox setzen damit der Weg für die Maus kurz ist.
               dlg.MyLocation = new Point(pb.RectangleToScreen(pb.ClientRectangle).Right + 5,
                                          pb.RectangleToScreen(pb.ClientRectangle).Top + 5);
               if (dlg.ShowDialog() == DialogResult.OK) {
                  bool b4Day = pb == pictureBoxPointDay;
                  GraphicElement ge = GetSelectedAndActiveGraphicElement();
                  if (ge != null) {
                     POI p = (POI)ge;
                     Bitmap bmp = p.AsBitmap(b4Day);
                     if (bmp != null) {
                        Color org = dlg.MyTrColor;
                        for (int x = 0; x < bmp.Width; x++)
                           for (int y = 0; y < bmp.Height; y++)
                              if (bmp.GetPixel(x, y) == org)
                                 bmp.SetPixel(x, y, Color.Transparent);
                        p.SetBitmaps(b4Day ? bmp : p.AsBitmap(true),
                                     b4Day ? (p.WithNightBitmap ? p.AsBitmap(false) : null) : bmp,
                                     bPoiWithGarminColor);
                        ShowSelectedGraphicElementWithNewProps();
                     }
                  }
               }
            }
         }
      }
      private BitmapColorMode BestBitmapColorMode(BitmapColorMode bcm1, BitmapColorMode bcm2) {
         // Im Prinzip MathMax(bcm1, bcm2), aber um keine Voraussetzung über die enum-Def. zu verwenden:
         if (bcm1 == bcm2) return bcm1;
         if (bcm1 == BitmapColorMode.unknown || bcm2 == BitmapColorMode.unknown)
            return BitmapColorMode.unknown;
         switch (bcm1) {
            case BitmapColorMode.POI_SIMPLE:
               return bcm2;
            case BitmapColorMode.POI_TR:
               return bcm2 == BitmapColorMode.POI_SIMPLE ? BitmapColorMode.POI_TR : BitmapColorMode.POI_ALPHA;
            case BitmapColorMode.POI_ALPHA:
               return bcm1;
         }
         return BitmapColorMode.POI_ALPHA;
      }

      private void contextMenuStripPoint_Opening(object sender, CancelEventArgs e) {
         ContextMenuStrip cm = (ContextMenuStrip)sender;
         if (cm.SourceControl != null && cm.SourceControl is PictureBox) {
            bool bDayPictureBox = pictureBoxPointDay == GetPictureBox4ContextMenu((ContextMenuStrip)sender);
            if (bDayPictureBox)
               ToolStripMenuItemPointCopy.Enabled = true;
            else {
               if (listViewPoint.SelectedItems.Count > 0) {
                  POI p = (POI)listViewPoint.SelectedItems[0].Tag;
                  ToolStripMenuItemPointCopy.Enabled = p.WithNightBitmap;
               }
            }
            ToolStripMenuItemPointInsert.Enabled = Clipboard.ContainsImage();
         }
      }

      #endregion

      /// <summary>
      /// ein anderer Punkt wurde ausgewählt
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void listViewPoint_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
         POI p = e.Item.Tag != null ? (POI)e.Item.Tag : null;
         ShowAllData4GraphicElement(e.IsSelected ? p : null);
      }

      /// <summary>
      /// Vergrößerung für die Punktdarstellung in der PictureBox
      /// </summary>
      int iScale4Point = 5;

      /// <summary>
      /// der Punkt wird in der angegebenen PicturBox dargestellt
      /// </summary>
      /// <param name="pb"></param>
      /// <param name="org"></param>
      private void ShowPointInPicturebox(PictureBox pb, Bitmap org) {
         if (org != null && org.Height > 0) {
            pb.ClientSize = new System.Drawing.Size(org.Width * iScale4Point, org.Height * iScale4Point);
            pb.BorderStyle = BorderStyle.FixedSingle; // None;
            pb.Image = StretchBitmap(org, iScale4Point);
         } else {
            pb.Size = new System.Drawing.Size(50, 50);
            pb.BorderStyle = BorderStyle.FixedSingle;
            pb.Image = pb.ErrorImage;
         }
      }

      /// <summary>
      /// liefert den 1. selektierte Punkt oder null
      /// </summary>
      /// <returns></returns>
      private POI GetSelectedPoi() {
         return listViewPoint.SelectedItems.Count > 0 ? (POI)listViewPoint.SelectedItems[0].Tag : null;
      }

      #endregion

      #region Funktionen für die Textdarstellung

      /// <summary>
      /// ein anderer Fonttyp wurde ausgewählt
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void listBoxFont_SelectedIndexChanged(object sender, EventArgs e) {
         if (IsControlValueSettingsValidatedOn((Control)sender)) {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0) {
               GraphicElement ge = GetSelectedAndActiveGraphicElement();
               ge.FontType = data4ListboxFont[idx];
               SetTypefileStatus(true);
            }
         }
      }

      /// <summary>
      /// Anzeige der Texteigenschaften oder bei null Anzeige auf Dummywerte setzen
      /// </summary>
      /// <param name="ge"></param>
      private void ShowTextProperties(GraphicElement ge) {
         ControlValueSettingsValidated(splitContainerMain.Panel2, false);
         if (ge != null) {
            ShowMultitextInGridview(ge.Text, dataGridViewMultiText);
            for (int i = 0; i < data4ListboxFont.Length; i++)
               if (data4ListboxFont[i] == ge.FontType) {
                  listBoxFont.SelectedIndex = i;
                  break;
               }
            for (int i = 0; i < data4ListboxCustomcolor.Length; i++)
               if (data4ListboxCustomcolor[i] == ge.FontColType) {
                  listBoxCustomColor.SelectedIndex = i;
                  break;
               }
            switch (ge.FontColType) {
               case GraphicElement.FontColours.Day:
                  pictureBoxCustomColor1.BackColor = ge.FontColor1;
                  pictureBoxCustomColor2.BackColor = Color.Transparent;
                  break;
               case GraphicElement.FontColours.DayAndNight:
                  pictureBoxCustomColor1.BackColor = ge.FontColor1;
                  pictureBoxCustomColor2.BackColor = ge.FontColor2;
                  break;
               case GraphicElement.FontColours.Night:
                  pictureBoxCustomColor1.BackColor = Color.Transparent;
                  pictureBoxCustomColor2.BackColor = ge.FontColor2;
                  break;
               case GraphicElement.FontColours.No:
                  pictureBoxCustomColor1.BackColor = Color.Transparent;
                  pictureBoxCustomColor2.BackColor = Color.Transparent;
                  break;
            }
            dataGridViewMultiText.Enabled =
            listBoxFont.Enabled =
            listBoxCustomColor.Enabled = true;
            pictureBoxCustomColor1.Enabled =
            pictureBoxCustomColor2.Enabled = ge.FontColType != GraphicElement.FontColours.No;
         } else {
            ShowMultitextInGridview(null, dataGridViewMultiText);
            int idx = 0;
            for (int i = 0; i < data4ListboxFont.Length; i++)
               if (data4ListboxFont[i] == GraphicElement.Fontdata.Default) {
                  idx = i;
                  break;
               }
            listBoxFont.SelectedIndex = idx;
            idx = 0;
            for (int i = 0; i < data4ListboxCustomcolor.Length; i++)
               if (data4ListboxCustomcolor[i] == GraphicElement.FontColours.No) {
                  idx = i;
                  break;
               }
            listBoxCustomColor.SelectedIndex = idx;
            pictureBoxCustomColor1.BackColor = Color.Transparent;
            pictureBoxCustomColor2.BackColor = Color.Transparent;
            dataGridViewMultiText.Enabled =
            listBoxFont.Enabled =
            listBoxCustomColor.Enabled =
            pictureBoxCustomColor1.Enabled =
            pictureBoxCustomColor2.Enabled = false;
         }
         ControlValueSettingsValidated(splitContainerMain.Panel2, true);
      }

      /// <summary>
      /// das Editieren eines Textes wurde beendet
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGridViewMultiText_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
         DataGridView dgv = (DataGridView)sender;
         if (IsControlValueSettingsValidatedOn((Control)dgv))
            if (e.ColumnIndex == 1) {
               GraphicElement ge = GetSelectedAndActiveGraphicElement();
               MultiText mt = new MultiText();
               try {
                  for (int i = 0; i < Language4Text.GetLength(0); i++) {
                     object o = dgv.Rows[i].Cells[1].Value;
                     string txt = o != null ? o.ToString().Trim() : "";
                     if (txt.Length > 0)
                        mt.Set(Language4Text[i], txt);
                  }
               } catch (Exception ex) {
                  MessageBox.Show("Fehler beim Ändern des Textes: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }
               ge.SetText(mt);
               ShowSelectedGraphicElementWithNewProps();
            }
      }

      #region Behandlung der Fontfarbe (Customcolor)

      private void listBoxCustomColor_SelectedIndexChanged(object sender, EventArgs e) {
         int idx = ((ListBox)sender).SelectedIndex;
         if (idx >= 0) {
            switch (data4ListboxCustomcolor[idx]) {
               case GraphicElement.FontColours.No:
                  pictureBoxCustomColor1.Enabled =
                  pictureBoxCustomColor2.Enabled = false;
                  break;
               case GraphicElement.FontColours.Day:
                  pictureBoxCustomColor1.Enabled = true;
                  pictureBoxCustomColor2.Enabled = false;
                  break;
               case GraphicElement.FontColours.Night:
                  pictureBoxCustomColor1.Enabled = false;
                  pictureBoxCustomColor2.Enabled = true;
                  break;
               case GraphicElement.FontColours.DayAndNight:
                  pictureBoxCustomColor1.Enabled =
                  pictureBoxCustomColor2.Enabled = true;
                  break;
            }
            GraphicElement ge = GetSelectedAndActiveGraphicElement();
            if (ge != null) {
               ge.FontColType = data4ListboxCustomcolor[idx];
               SetTypefileStatus(true);
               ShowTextProperties(ge);
            }
         }
      }

      /// <summary>
      /// ev. CustomColor1 verändern
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void pictureBoxCustomColor1_MouseClick(object sender, MouseEventArgs e) {
         if ((e.Button & MouseButtons.Right) > 0) {
            int idx = listBoxCustomColor.SelectedIndex;
            if (idx >= 0 &&
                (data4ListboxCustomcolor[idx] == GraphicElement.FontColours.Day ||
                 data4ListboxCustomcolor[idx] == GraphicElement.FontColours.DayAndNight)) {
               GraphicElement ge = GetSelectedAndActiveGraphicElement();
               if (ge != null) {
                  Color col = ge.FontColor1;
                  if (ChooseColor(ref col)) {
                     ge.FontColor1 = ((PictureBox)sender).BackColor = col;
                     SetTypefileStatus(true);
                  }
               }
            }
         }
      }

      /// <summary>
      /// ev. CustomColor2 verändern
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void pictureBoxCustomColor2_MouseClick(object sender, MouseEventArgs e) {
         if ((e.Button & MouseButtons.Right) > 0) {
            int idx = listBoxCustomColor.SelectedIndex;
            if (idx >= 0 &&
                (data4ListboxCustomcolor[idx] == GraphicElement.FontColours.Night ||
                 data4ListboxCustomcolor[idx] == GraphicElement.FontColours.DayAndNight)) {
               GraphicElement ge = GetSelectedAndActiveGraphicElement();
               if (ge != null) {
                  Color col = ge.FontColor2;
                  if (ChooseColor(ref col)) {
                     ge.FontColor2 = ((PictureBox)sender).BackColor = col;
                     SetTypefileStatus(true);
                  }
               }
            }
         }
      }

      #endregion

      /// <summary>
      /// zeigt den Multitext im Gridview an
      /// </summary>
      /// <param name="mt">wenn null, dann alles löschen</param>
      /// <param name="dgv"></param>
      private void ShowMultitextInGridview(MultiText mt, DataGridView dgv) {
         ControlValueSettingsValidated(dgv, false);
         for (int i = 0; i < Language4Text.GetLength(0); i++)
            dgv.Rows[i].Cells[1].Value = mt != null ? mt.Get(Language4Text[i]) : "";
         ControlValueSettingsValidated(dgv);
      }

      #endregion

      #region allgemeine Funktionen

      /// <summary>
      /// Status: Ist das Typefile geändert, aber noch nicht gespeichert?
      /// </summary>
      /// <param name="bChanged"></param>
      private void SetTypefileStatus(bool bChanged) {
         bTFChanged = bChanged;
         toolStripButtonSave.Enabled =
         ToolStripMenuItemSave.Enabled =
         ToolStripMenuItemSave2.Enabled = tf != null && bTFChanged;
      }

      /// <summary>
      /// entfernt ein Item aus der Liste (und die dazugehörigen Bilder)
      /// </summary>
      /// <param name="lvi"></param>
      private void RemoveListItem(ListViewItem lvi) {
         ListView lv = lvi.ListView;
         lv.SuspendLayout();
         int idx = lvi.ImageIndex;
         lv.Items.Remove(lvi);      // altes Element aus der Liste entfernen
         // ERST die notwendigen Änderungen im Bildindex ausführen und DANACH die Bilder entfernen
         // (sonst wird in den Items die auf das letzte Bild zeigen sofort der Index angepasst, da
         //  er nach dem Bildlöschen zu groß ist)
         for (int i = 0; i < lv.Items.Count; i++)        // Bildindex korrigieren
            if (lv.Items[i].ImageIndex >= idx)
               lv.Items[i].ImageIndex--;
         lv.SmallImageList.Images.RemoveAt(idx);
         lv.LargeImageList.Images.RemoveAt(idx);
         lv.ResumeLayout();
      }

      /// <summary>
      /// erzeugt ein passendes ListViewItem für das GraphicElement
      /// </summary>
      /// <param name="ge"></param>
      /// <returns></returns>
      private ListViewItem MakeListViewItem4GraphicElement(GraphicElement ge) {
         ListViewItem lvi = null;

         if (ge is Polygone) {

            Polygone p = (Polygone)ge;
            Bitmap bmorg = p.AsBitmap(true, true);
            Bitmap bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                           listViewLine.SmallImageList.ImageSize.Width, listViewLine.SmallImageList.ImageSize.Height);
            listViewArea.SmallImageList.Images.Add(bm);
            bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                           listViewLine.LargeImageList.ImageSize.Width, listViewLine.LargeImageList.ImageSize.Height);
            listViewArea.LargeImageList.Images.Add(bm);
            string[] dat = new string[] {
                     ElementTyp2Text(p),
                     p.Text.Get(PreferredLanguage),
                     p.Draworder.ToString(),
                     AreaTypName[p.Colortype]
                  };
            lvi = new ListViewItem(dat, listViewArea.LargeImageList.Images.Count - 1);
            lvi.Tag = p;
            lvi.ToolTipText = string.Join(" - ", dat);

         } else if (ge is Polyline) {

            Polyline p = (Polyline)ge;
            Bitmap bmorg = p.AsBitmap(true, true);
            Bitmap bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                           listViewLine.SmallImageList.ImageSize.Width, listViewLine.SmallImageList.ImageSize.Height);
            listViewLine.SmallImageList.Images.Add(bm);
            bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                           listViewLine.LargeImageList.ImageSize.Width, listViewLine.LargeImageList.ImageSize.Height);
            listViewLine.LargeImageList.Images.Add(bm);
            string[] dat = new string[] {
                     ElementTyp2Text(p),
                     p.Text.Get(PreferredLanguage),
                     LineTypName[p.Polylinetype],
                     p.Height.ToString()
                  };
            lvi = new ListViewItem(dat, listViewLine.LargeImageList.Images.Count - 1);
            lvi.Tag = p;
            lvi.ToolTipText = string.Join(" - ", dat);

         } else if (ge is POI) {

            POI p = (POI)ge;
            Bitmap bmorg = p.AsBitmap(true);
            Bitmap bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                           listViewPoint.SmallImageList.ImageSize.Width, listViewPoint.SmallImageList.ImageSize.Height);
            listViewPoint.SmallImageList.Images.Add(bm);
            bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                           listViewPoint.LargeImageList.ImageSize.Width, listViewPoint.LargeImageList.ImageSize.Height);
            listViewPoint.LargeImageList.Images.Add(bm);
            string[] dat = new string[] {
                     ElementTyp2Text(p),
                     p.Text.Get(PreferredLanguage),
                     string.Format("{0} x {1}",p.Width,p.Height)
                  };
            lvi = new ListViewItem(dat, listViewPoint.LargeImageList.Images.Count - 1);
            lvi.Tag = p;
            lvi.ToolTipText = string.Join(" - ", dat);

         }
         return lvi;
      }

      /// <summary>
      /// ein Bitmap bestmöglich in die vorgegebene Größe einpassen
      /// </summary>
      /// <param name="org"></param>
      /// <param name="backcol"></param>
      /// <param name="destwidth">Zielgröße (Max.)</param>
      /// <param name="destheigth">Zielgröße (Max.)</param>
      /// <returns></returns>
      private Bitmap GetImage4Imagelist(Bitmap org, Color backcol, int destwidth, int destheigth) {
         // Ein 32x32-Bitmap soll optimal eingepasst werden, d.h. daraus ergibt sich der Standardvergrößerung. 
         // Größere Bitmaps werden verkleinert.
         float dScale = 1.0f;
         if (org.Height > destheigth || org.Width > destwidth)    // Bitmap ist zu groß
            dScale = Math.Min((float)destheigth / org.Height, (float)destwidth / org.Width);
         else
            dScale = Math.Min(destheigth / 32f, destwidth / 32f);
         Bitmap bm = new Bitmap(destwidth, destheigth);
         Graphics canvas = Graphics.FromImage(bm);
         SetCanvasProps(canvas);
         canvas.Clear(backcol);
         if (dScale == 1.0)
            canvas.DrawImageUnscaled(org, 0, (destheigth - org.Height) / 2);
         else {
            float h = org.Height * dScale;
            float w = org.Width * dScale;
            canvas.DrawImage(org, (destwidth - w) / 2, (destheigth - h) / 2, w, h);
         }
         canvas.Flush();
         return bm;
      }

      private Bitmap StretchBitmap(Bitmap org, int iScale) {
         Bitmap bm = new Bitmap(org.Width * iScale, org.Height * iScale);
         Graphics canvas = Graphics.FromImage(bm);
         SetCanvasProps(canvas);
         canvas.DrawImage(org, 0, 0, bm.Width, bm.Height);
         return bm;
      }

      /// <summary>
      /// Einstellung, damit das Skalieren sauber funkt.
      /// </summary>
      /// <param name="canvas"></param>
      private void SetCanvasProps(Graphics canvas) {
         canvas.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
         canvas.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
         canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
         canvas.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
      }

      /// <summary>
      /// neue Sortierung der eines Listviews
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void listView_ColumnClick(object sender, ColumnClickEventArgs e) {
         ListView lv = (ListView)sender;
         lv.ListViewItemSorter = mylvsorter;
         if (e.Column != mylvsorter.SortColumn) {
            mylvsorter.SortColumn = e.Column;
            mylvsorter.Order = SortOrder.Ascending;
         } else {
            mylvsorter.Order = mylvsorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
         }
         lv.Sort();
      }

      MyColumnSorter mylvsorter = new MyColumnSorter();

      /// <summary>
      /// spezielle Sortierklasse für das Listview um die Spalte 2 als Zahl sortieren zu können
      /// </summary>
      public class MyColumnSorter : IComparer {

         public int SortColumn;
         public SortOrder Order;

         public MyColumnSorter(int column = 0, SortOrder order = SortOrder.Ascending) {
            SortColumn = column;
            Order = order;
         }

         /// <summary>
         /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
         /// </summary>
         /// <param name="x"></param>
         /// <param name="y"></param>
         /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
         public int Compare(object x, object y) {
            int result = 0;

            string sTextX = (x as ListViewItem).SubItems[SortColumn].Text;
            string sTextY = (y as ListViewItem).SubItems[SortColumn].Text;

            if (SortColumn != 2) {
               result = string.Compare(sTextX, sTextY);
            } else {
               // Spalte 2 wird als Zahl interpretiert und sortiert
               int iX = Convert.ToInt32(sTextX);
               int iY = Convert.ToInt32(sTextY);

               result = iX > iY ? 1 : iX < iY ? -1 : 0;
            }

            if (Order == SortOrder.Descending)     // invertieren
               result = -result;

            return result;
         }
      }


      /// <summary>
      /// liefert den Typ als standardisierten Text
      /// </summary>
      /// <param name="ge"></param>
      /// <returns></returns>
      private string ElementTyp2Text(GraphicElement ge) {
         return ge.Subtype == 0 ? string.Format("0x{0:x3}", ge.Type) : string.Format("0x{0:x3}, 0x{1:x2}", ge.Type, ge.Subtype);
      }

      /// <summary>
      /// liefert die PictureBox auf die sich die Menüauswahl bezieht
      /// </summary>
      /// <param name="mi"></param>
      /// <returns></returns>
      private PictureBox GetPictureBox4ToolStripMenuItem(ToolStripMenuItem mi) {
         ContextMenuStrip cm = (ContextMenuStrip)mi.Owner;
         return (cm.SourceControl != null && cm.SourceControl is PictureBox) ? (PictureBox)cm.SourceControl : null;
      }
      /// <summary>
      /// liefert die PictureBox auf die sich das Kontextmenü bezieht
      /// </summary>
      /// <param name="cm"></param>
      /// <returns></returns>
      private PictureBox GetPictureBox4ContextMenu(ContextMenuStrip cm) {
         return (cm.SourceControl != null && cm.SourceControl is PictureBox) ? (PictureBox)cm.SourceControl : null;
      }

      /// <summary>
      /// allgemeine Funktion zur Auswahl einer Farbe
      /// </summary>
      /// <param name="col"></param>
      /// <returns></returns>
      private bool ChooseColor(ref Color col) {
         bool ret = false;
         colorDialog1.Color = col;
         if (colorDialog1.ShowDialog() == DialogResult.OK) {
            col = colorDialog1.Color;
            ret = true;
         }
         return ret;
      }

      /// <summary>
      /// setzt Farbe 1 oder 2 für Tag oder Nacht neu
      /// </summary>
      /// <param name="bIsColor1"></param>
      /// <param name="bIsDayColor"></param>
      private void SetGraphicElementColor(bool bIsColor1, bool bIsDayColor) {
         GraphicElement ge = GetSelectedAndActiveGraphicElement();
         Color col = bIsColor1 ? (bIsDayColor ? ge.DayColor1 : ge.NightColor1) :
                                 (bIsDayColor ? ge.DayColor2 : ge.NightColor2);
         if (ChooseColor(ref col)) {
            if (bIsDayColor) {
               if (bIsColor1) ge.DayColor1 = col;
               else ge.DayColor2 = col;
            } else {
               if (bIsColor1) ge.NightColor1 = col;
               else ge.NightColor2 = col;
            }
            ShowSelectedGraphicElementWithNewProps();
         }
      }

      /// <summary>
      /// liefert den GraphicElement-Typ der aktiven Tabpage
      /// </summary>
      /// <returns></returns>
      private Type GetType4ActiveTabpage() {
         TabPage tb = tabControl1.TabPages[tabControl1.SelectedIndex];     // aktive Tabpage
         if (tb == tabPageArea)
            return typeof(Polygone);
         if (tb == tabPageLine)
            return typeof(Polyline);
         if (tb == tabPagePoint)
            return typeof(POI);
         return null;
      }

      /// <summary>
      /// ein anderer Tab wird aktiv
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
         GraphicElement ge = GetSelectedAndActiveGraphicElement();
         ShowAllData4GraphicElement(ge);
      }

      /// <summary>
      /// Soll die Änderung von Werten ev. zeitweilig NICHT ausgewertet werden? (für Änderung durch Prog)
      /// </summary>
      /// <param name="ctrl"></param>
      /// <param name="bStatusOn"></param>
      /// <returns></returns>
      private bool ControlValueSettingsValidated(Control ctrl, bool bStatusOn) {
         bool old = ctrl.Tag == null ? false : true;
         object tag = null;
         if (!bStatusOn) tag = 1;
         ControlValueSettingsValidated(ctrl, tag);
         return old;
      }
      private void ControlValueSettingsValidated(Control ctrl, object tag) {
         ctrl.Tag = tag;
         foreach (Control item in ctrl.Controls)      // auch für alle untergeordneten Controls setzen
            ControlValueSettingsValidated(item, tag);
      }
      private bool ControlValueSettingsValidated(Control ctrl) {
         return ControlValueSettingsValidated(ctrl, true);
      }
      /// <summary>
      /// Ist die Auswertung der Werte sinnvoll?
      /// </summary>
      /// <param name="ctrl"></param>
      /// <returns></returns>
      private bool IsControlValueSettingsValidatedOn(Control ctrl) {
         return ctrl.Tag == null;
      }

      /// <summary>
      /// fügt entsprechend des akt. GraphicElement ein Bild in die Zwischenablage ein
      /// </summary>
      /// <param name="b4Day"></param>
      private void SelectedGraphicElement2Clipboard(bool b4Day) {
         GraphicElement ge = GetSelectedAndActiveGraphicElement();
         if (ge != null)
            Clipboard.SetImage(ge.AsBitmap(b4Day, true));
      }

      /// <summary>
      /// liefert das akt. selektierte Element in der aktiven Tabpage oder null
      /// </summary>
      /// <returns></returns>
      private GraphicElement GetSelectedAndActiveGraphicElement() {
         Type t = GetType4ActiveTabpage();
         if (t == typeof(Polygone))
            return GetSelectedPolygone();
         if (t == typeof(Polyline))
            return GetSelectedPolyline();
         if (t == typeof(POI))
            return GetSelectedPoi();
         return null;
      }

      /// <summary>
      /// zeigt das ausgewählte Element mit seinen neuen Eigenschaften an (auch in der Liste)
      /// </summary>
      /// <param name="lvi"></param>
      /// <param name="p"></param>
      private void ShowSelectedGraphicElementWithNewProps() {
         ListView lv = null;
         GraphicElement ge = null;
         Bitmap bmorg = null;
         Type t = GetType4ActiveTabpage();
         if (t == typeof(Polygone))
            lv = listViewArea;
         if (t == typeof(Polyline))
            lv = listViewLine;
         if (t == typeof(POI))
            lv = listViewPoint;
         if (lv.SelectedItems.Count == 0)
            return;
         ListViewItem lvi = lv.SelectedItems[0];
         int pictidx = lvi.ImageIndex;
         int idx = lvi.Index;
         if (t == typeof(Polygone))
            bmorg = ((Polygone)lvi.Tag).AsBitmap(true, true);
         else if (t == typeof(Polyline))
            bmorg = ((Polyline)lvi.Tag).AsBitmap(true, true);
         else if (t == typeof(POI))
            bmorg = ((POI)lvi.Tag).AsBitmap(true);
         Bitmap bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                        lv.SmallImageList.ImageSize.Width, lv.SmallImageList.ImageSize.Height);
         lv.SmallImageList.Images[pictidx] = bm;
         bm = GetImage4Imagelist(bmorg, colBitmapBackcolor,
                        lv.LargeImageList.ImageSize.Width, lv.LargeImageList.ImageSize.Height);
         lv.LargeImageList.Images[pictidx] = bm;

         ge = (GraphicElement)lvi.Tag;
         string sType = ElementTyp2Text(ge);
         string sName = ge.Text.Get(PreferredLanguage);
         string[] dat = null;
         if (t == typeof(Polygone))
            dat = new string[] { sType, sName,
                                 ((Polygone)ge).Draworder.ToString(),
                                 AreaTypName[((Polygone)ge).Colortype]
            };
         else if (t == typeof(Polyline))
            dat = new string[] { sType, sName,
                                 LineTypName[((Polyline)ge).Polylinetype],
                                 ((Polyline)ge).Height.ToString()
            };
         else
            dat = new string[] { sType, sName,
                                 string.Format("{0} x {1}",((POI)ge).Width,((POI)ge).Height)
            };
         for (int i = 2; i < dat.Length; i++)
            lvi.SubItems[i].Text = dat[i];
         lvi.SubItems[1].Text = dat[1];
         lvi.ToolTipText = string.Join(" - ", dat);
         lvi.Tag = ge;
         lvi.ListView.RedrawItems(idx, idx, false);

         ShowAllData4GraphicElement(ge);
         SetTypefileStatus(true);
      }

      /// <summary>
      /// zeigt alle Daten für dieses GraphicElement an oder bei null Anzeige auf Dummywerte setzen
      /// </summary>
      /// <param name="ge"></param>
      private void ShowAllData4GraphicElement(GraphicElement ge) {
         Type tp_typ = (ge == null) ?         // dann fkt. "is" natürlich nicht
                           tp_typ = GetType4ActiveTabpage() :
                           tp_typ = ge.GetType();
         if (tp_typ == typeof(Polygone)) {
            #region Datenanzeige für Polygone
            Polygone p = ge as Polygone;
            numericUpDownAreaDraworder.Enabled =
            pictureBoxAreaDay.Enabled =
            pictureBoxAreaNight.Enabled =
            listBoxAreaTyp.Enabled = p != null;
            if (p != null) {
               numericUpDownAreaDraworder.Value = p.Draworder;
               ShowAreaInPicturebox(pictureBoxAreaDay, p.AsBitmap(true, true));
               pictureBoxAreaNight.Image = null;
               switch (p.Colortype) {
                  case Polygone.ColorType.Day1_Night1:
                  case Polygone.ColorType.BM_Day1_Night1:
                  case Polygone.ColorType.BM_Day1_Night2:
                  case Polygone.ColorType.BM_Day2_Night1:
                  case Polygone.ColorType.BM_Day2_Night2:
                     ShowAreaInPicturebox(pictureBoxAreaNight, p.AsBitmap(false, true));
                     break;
               }
               for (int i = 0; i < data4ListboxAreaTyp.Length; i++)
                  if (data4ListboxAreaTyp[i] == p.Colortype) {
                     listBoxAreaTyp.SelectedIndex = i;
                     break;
                  }
            } else {       // Dummywerte und alles disabled
               DiableDataControls4Polygones();
            }
            #endregion
         } else if (tp_typ == typeof(Polyline)) {
            #region Datenanzeige für Polyline
            Polyline p = ge as Polyline;
            pictureBoxLineDay.Enabled =
            pictureBoxLineNight.Enabled =
            groupBoxLine1.Enabled =
            listBoxLineTyp.Enabled =
            checkBoxLineTextRotation.Enabled = p != null;
            ControlValueSettingsValidated(splitContainerLine.Panel2, false);


            if (p != null) {
               if (p.WithDayBitmap) {     // max. 32 Pixel hoch
                  radioButtonLineBitmap.Checked = true;
                  numericUpDownLineHeight.Enabled =
                  numericUpDownLineBorder.Enabled = false;
                  labelLineWidth.Text = "Liniendicke: " + p.BitmapHeight.ToString();
               } else {
                  // Es scheint auch atypische Linien zu geben, z.B. vom Typ Day2, aber nur mit Breite 1
                  // so daß kein Rand gezeichnet werden kann.
                  uint iBorder = p.BorderWidth;
                  switch (p.Polylinetype) {
                     case Polyline.PolylineType.Day2:
                     case Polyline.PolylineType.Day1_Night2:
                     case Polyline.PolylineType.Day2_Night2:
                        break;
                     case Polyline.PolylineType.NoBorder_Day1:
                     case Polyline.PolylineType.NoBorder_Day1_Night1:
                     case Polyline.PolylineType.NoBorder_Day2_Night1:
                        numericUpDownLineBorder.Enabled = false;
                        iBorder = 0;
                        break;
                  }
                  radioButtonLineSolidColor.Checked =
                  numericUpDownLineHeight.Enabled =
                  numericUpDownLineBorder.Enabled = true;
                  numericUpDownLineBorder.Maximum = iMaxLineWidth;
                  numericUpDownLineBorder.Value = iBorder;
                  numericUpDownLineHeight.Maximum = iMaxLineWidth;
                  numericUpDownLineHeight.Value = p.Height;
               }
               labelLineWidth.Text = "Liniendicke: " + p.Height.ToString();
               ShowLineInPictureboxes(p);
               checkBoxLineTextRotation.Checked = p.WithTextRotation;
               for (int i = 0; i < data4ListboxLineTyp.Length; i++)
                  if (data4ListboxLineTyp[i] == p.Polylinetype) {
                     listBoxLineTyp.SelectedIndex = i;
                     break;
                  }
            } else {       // Dummywerte und alles disabled
               DiableDataControls4Polylines();
            }
            ControlValueSettingsValidated(splitContainerLine.Panel2);
            #endregion
         } else if (tp_typ == typeof(POI)) {
            #region Datenanzeige für POI
            POI p = ge as POI;
            pictureBoxPointDay.Enabled =
            pictureBoxPointNight.Enabled = p != null;
            if (p != null) {
               Bitmap bm = p.AsBitmap(true);
               LabelPointDay.Text = string.Format("Tag: {0} x {1}, {2}", bm.Width, bm.Height, PointTypName[p.ColormodeDay]);
               ShowPointInPicturebox(pictureBoxPointDay, bm);
               bm = p.AsBitmap(false);
               LabelPointNigth.Location = new Point(LabelPointNigth.Location.X, pictureBoxPointDay.Bottom + 25);
               pictureBoxPointNight.Location = new Point(pictureBoxPointNight.Location.X, LabelPointNigth.Bottom + 18);
               if (p.WithNightBitmap) {
                  // neue Position festlegen
                  LabelPointNigth.Text += string.Format("Nacht: {0} x {1}, {2}", bm.Width, bm.Height, PointTypName[p.ColormodeNight]);
               } else
                  LabelPointNigth.Text = "Nacht:";
               ShowPointInPicturebox(pictureBoxPointNight, p.WithNightBitmap ? bm : null);
            } else {       // Dummywerte
               DiableDataControls4Points();
            }
            #endregion
         }
         ShowTextProperties(ge);
      }

      /// <summary>
      /// liefert das Listview zu dem das ToolStripMenuItem im Moment gehört
      /// </summary>
      /// <param name="sender"></param>
      /// <returns></returns>
      private ListView GetListView4ToolStripMenuItem(object sender) {
         ListView lv = null;
         ToolStripMenuItem mi = (ToolStripMenuItem)sender;
         while (mi != null && mi.OwnerItem != null)
            mi = (ToolStripMenuItem)mi.OwnerItem;
         if (mi != null && mi.Owner != null)
            lv = (ListView)((ContextMenuStrip)mi.Owner).Tag;
         return lv;
      }

      void DiableDataControls4Polygones() {
         pictureBoxAreaDay.Image = null;
         pictureBoxAreaNight.Image = null;
         for (int i = 0; i < data4ListboxAreaTyp.Length; i++)
            if (data4ListboxAreaTyp[i] == Polygone.ColorType.Day1) {
               listBoxAreaTyp.SelectedIndex = i;
               break;
            }

         numericUpDownAreaDraworder.Enabled =
         pictureBoxAreaDay.Enabled =
         pictureBoxAreaNight.Enabled =
         listBoxAreaTyp.Enabled = false;

         numericUpDownAreaDraworder.Value = 1;
      }

      void DiableDataControls4Polylines() {
         labelLineWidth.Text = "Liniendicke: -";
         pictureBoxLineDay.Image = null;
         pictureBoxLineNight.Image = null;
         radioButtonLineSolidColor.Checked = true;
         numericUpDownLineHeight.Value = numericUpDownLineHeight.Minimum;
         numericUpDownLineBorder.Value = numericUpDownLineBorder.Minimum;
         checkBoxLineTextRotation.Checked = false;
         groupBoxLine1.Enabled = false;
         for (int i = 0; i < data4ListboxLineTyp.Length; i++)
            if (data4ListboxLineTyp[i] == Polyline.PolylineType.Day2) {
               listBoxLineTyp.SelectedIndex = i;
               break;
            }

         pictureBoxLineDay.Enabled =
         pictureBoxLineNight.Enabled =
         groupBoxLine1.Enabled =
         listBoxLineTyp.Enabled =
         checkBoxLineTextRotation.Enabled = false;
      }

      void DiableDataControls4Points() {
         LabelPointDay.Text = LabelPointNigth.Text = "";
         ShowPointInPicturebox(pictureBoxPointDay, null);
         ShowPointInPicturebox(pictureBoxPointNight, null);

         pictureBoxPointDay.Enabled =
         pictureBoxPointNight.Enabled = false;
         LabelPointDay.Text = LabelPointNigth.Text = "";
         ShowPointInPicturebox(pictureBoxPointDay, null);
         ShowPointInPicturebox(pictureBoxPointNight, null);
      }

      void DiableDataControls4Text() {
         ShowMultitextInGridview(null, dataGridViewMultiText);
         dataGridViewMultiText.Enabled = false;
         listBoxFont.Enabled = false;
         listBoxCustomColor.Enabled = false;
         pictureBoxCustomColor1.BackColor = Color.Black;
         pictureBoxCustomColor1.Enabled = false;
         pictureBoxCustomColor2.BackColor = Color.Black;
         pictureBoxCustomColor2.Enabled = false;
      }

      /// <summary>
      /// liefert den Titel des Programms
      /// </summary>
      /// <param name="a"></param>
      /// <returns></returns>
      public string Title() {
         Assembly a = Assembly.GetExecutingAssembly();
         string sTitle = "";
         object[] attributes = a.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
         if (attributes.Length > 0) {
            AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
            if (titleAttribute.Title != "")
               sTitle = titleAttribute.Title;
         }
         if (sTitle.Length == 0)         // Notlösung
            sTitle = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
         //string sVersion = a.GetName().Version.ToString();
         string sInfoVersion = "";
         attributes = a.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
         if (attributes.Length > 0)
            sInfoVersion = ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
         return sTitle + ", " + sInfoVersion;
      }

      #region DragAndDrop

      protected override void OnDragEnter(DragEventArgs e) {
         base.OnDragEnter(e);
         if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
         else
            e.Effect = DragDropEffects.None;
      }

      protected override void OnDragDrop(DragEventArgs e) {
         base.OnDragDrop(e);
         if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadTypefile(files[0]);
         }
      }

      #endregion


      #endregion

      void AddLastfile(string file) {
         string fileupper = file.ToUpper();
         for (int i = 0; i < LastFiles.Count; i++) {
            if (LastFiles[i].ToUpper() == fileupper) {
               LastFiles.RemoveAt(i);
               break;
            }
         }
         LastFiles.Add(file);
         while (LastFiles.Count > 15) // nicht mehr als 15 Dateien
            LastFiles.RemoveAt(0);
      }

      private void menuStripMain_MenuActivate(object sender, EventArgs e) {
         ToolStripMenuItem mi = (sender as MenuStrip).Items[0] as ToolStripMenuItem;   // mi für 1. Spalte
         for (int i = 0; i < mi.DropDownItems.Count; i++) {
            ToolStripMenuItem mir = mi.DropDownItems[i] as ToolStripMenuItem;
            if (mir == ToolStripMenuItemLastFiles) {
               ToolStripDropDown dd = mir.DropDown;
               dd.Items.Clear();
               for (int j = 0; j < LastFiles.Count; j++) {
                  dd.Items.Add("&" + (j + 1).ToString() + ". " + LastFiles[j], null, new EventHandler(ToolStripMenuItem_recentused));
                  dd.Items[dd.Items.Count - 1].Tag = LastFiles[j];
               }
               ToolStripMenuItemLastFiles.Enabled = dd.Items.Count > 0;
               break;
            }
         }
      }

      void ToolStripMenuItem_recentused(object sender, EventArgs e) {
         LoadTypefile((sender as ToolStripMenuItem).Tag as string);
      }

   }

}
