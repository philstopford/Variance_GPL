using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer GeoCore
        GroupBox gB_layer_lithography;
        GroupBox gB_layer_geoCore;
        Button btn_chooseFile_geoCore, btn_globalApply_geoCore;
        DropDown comboBox_lDList_geoCore, comboBox_structureList_geoCore, comboBox_polyFill_geoCore, comboBox_tipLocations_geoCore;
        Label lbl_lD_geoCore, lbl_cell_geoCore, lbl_tipLocations_geoCore;
        TextBox textBox_fileLocation_geoCore;
        CheckBox cB_DOE_geoCore, cB_geoCore_shapeEngine, cB_geoCore_shapeEngine_perPoly, cB_geoCore_layoutReference;

        void twoD_LayerUISetup_geoCore()
        {
            Application.Instance.Invoke(() =>
            {
                gB_layer_geoCore = new GroupBox();
                TableLayout geoCore_table = new TableLayout();
                gB_layer_geoCore.Content = geoCore_table;
                gB_layer_geoCore.Text = "Layout Controls";

                geocore_table(geoCore_table);

                layerShapeProperties_tcPanel.Content = gB_layer_geoCore;

                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            });
        }

        void geocore_table(TableLayout geoCore_table)
        {
            // Outer table, row 1
            TableRow geocore_tr0 = new TableRow();
            geoCore_table.Rows.Add(geocore_tr0);
            geocore_tr0.Cells.Add(new TableCell());

            geocore_row0(geocore_tr0.Cells[0]);

            TableRow geocore_tr1 = new TableRow();
            geoCore_table.Rows.Add(geocore_tr1);
            geocore_tr1.Cells.Add(new TableCell());

            geocore_row1(geocore_tr1.Cells[0]);
        }

        void geocore_row0(TableCell tc)
        {
            TableLayout tl = new TableLayout();
            tc.Control = tl;

            tl.Rows.Add(new TableRow());

            TableCell tr0_0 = new TableCell();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(tr0_0);

            geocore_row0_0(tr0_0);

            TableCell tr0_1 = new TableCell();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(tr0_1);

            geocore_row0_1(tr0_1);
        }

        void geocore_row0_0(TableCell tc)
        {
            TableLayout tl = new TableLayout();

            tc.Control = tl;

            tl.Rows.Add(new TableRow());

            TableLayout tl0 = new TableLayout();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = tl0 });
            tl0.Rows.Add(new TableRow());

            btn_chooseFile_geoCore = new Button();
            btn_chooseFile_geoCore.Text = "Choose File";
            btn_chooseFile_geoCore.ToolTip = "Browse for GDS or Oasis file";
            tl0.Rows[tl0.Rows.Count - 1].Cells.Add(new TableCell() { Control = btn_chooseFile_geoCore });

            cB_DOE_geoCore = new CheckBox();
            cB_DOE_geoCore.Text = "DOE";
            cB_DOE_geoCore.Width = 60;
            cB_DOE_geoCore.ToolTip = "This is a DOE layout, to be used with tile extraction.";
            tl0.Rows[tl0.Rows.Count - 1].Cells.Add(new TableCell() { Control = cB_DOE_geoCore });

            cB_geoCore_shapeEngine = new CheckBox();
            cB_geoCore_shapeEngine.Text = "Contour";
            cB_geoCore_shapeEngine.Width = 75;
            cB_geoCore_shapeEngine.ToolTip = "Use layout polygons as inputs for contour generation";
            tl0.Rows[tl0.Rows.Count - 1].Cells.Add(new TableCell() { Control = cB_geoCore_shapeEngine });

            cB_geoCore_shapeEngine_perPoly = new CheckBox();
            cB_geoCore_shapeEngine_perPoly.Text = "Per-Poly";
            cB_geoCore_shapeEngine_perPoly.Width = 75;
            cB_geoCore_shapeEngine_perPoly.ToolTip = "Rotation per-polyon";
            tl0.Rows[tl0.Rows.Count - 1].Cells.Add(new TableCell() { Control = cB_geoCore_shapeEngine_perPoly });

            tl.Rows.Add(new TableRow());

            TableLayout tl1 = new TableLayout();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = tl1 });
            tl1.Rows.Add(new TableRow());

            btn_globalApply_geoCore = new Button();
            btn_globalApply_geoCore.Text = "Apply To All";
            btn_globalApply_geoCore.ToolTip = "Use this geoCore input in all layers of the simulation.";
            tl1.Rows[tl1.Rows.Count - 1].Cells.Add(new TableCell() { Control = btn_globalApply_geoCore });

            cB_geoCore_layoutReference = new CheckBox();
            cB_geoCore_layoutReference.Text = "Reference";
            cB_geoCore_layoutReference.Width = 75;
            cB_geoCore_layoutReference.ToolTip = "Reference the layout file";
            tl1.Rows[tl1.Rows.Count - 1].Cells.Add(new TableCell() { Control = cB_geoCore_layoutReference });

            tl1.Rows[tl1.Rows.Count - 1].Cells.Add(new TableCell() { ScaleWidth = true });

            tl.Rows.Add(new TableRow());

            TableLayout tl2 = new TableLayout();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = tl2 });
            tl2.Rows.Add(new TableRow());

            comboBox_polyFill_geoCore = new DropDown();
            comboBox_polyFill_geoCore.Width = 88;
            comboBox_polyFill_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.polyFillList);
            tl2.Rows[tl2.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_polyFill_geoCore) });

            tl2.Rows[tl2.Rows.Count - 1].Cells.Add(new TableCell() { ScaleWidth = true });
        }

        void geocore_row0_1(TableCell tc)
        {
            TableLayout tl = new TableLayout();
            tc.Control = tl;

            tl.Rows.Add(new TableRow());

            lbl_cell_geoCore = new Label();
            lbl_cell_geoCore.Text = "Cell";
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_cell_geoCore });

            comboBox_structureList_geoCore = new DropDown();
            comboBox_structureList_geoCore.Enabled = false;
            comboBox_structureList_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.geoCoreStructureList_exp);
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_structureList_geoCore) });

            tl.Rows.Add(new TableRow());

            lbl_lD_geoCore = new Label();
            lbl_lD_geoCore.Text = "L/D";
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lD_geoCore });

            comboBox_lDList_geoCore = new DropDown();
            comboBox_lDList_geoCore.Enabled = false;
            comboBox_lDList_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.geoCoreLDList_exp);
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_lDList_geoCore) });
        }

        void geocore_row1(TableCell tc)
        {
            TableLayout tl = new TableLayout();
            tc.Control = tl;
                
            tl.Rows.Add(new TableRow());

            lbl_tipLocations_geoCore = new Label();
            lbl_tipLocations_geoCore.Text = "Tip Locs";
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_tipLocations_geoCore });

            comboBox_tipLocations_geoCore = new DropDown();
            comboBox_tipLocations_geoCore.Enabled = false;
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = comboBox_tipLocations_geoCore });
            comboBox_tipLocations_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

            textBox_fileLocation_geoCore = new TextBox();
            textBox_fileLocation_geoCore.ReadOnly = true;
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = textBox_fileLocation_geoCore, ScaleWidth = true });
        }
    }
}
