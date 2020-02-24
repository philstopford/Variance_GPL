using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer GeoCore
        GroupBox groupBox_layer_lithography_exp;
        GroupBox groupBox_layer_geoCore_exp;
        Button button_layer_chooseFile_geoCore_exp, button_layer_globalApply_geoCore_exp;
        DropDown comboBox_layerLDList_geoCore_exp, comboBox_layerStructureList_geoCore_exp, comboBox_layerPolyFill_geoCore_exp, comboBox_layerTipLocations_geoCore_exp;
        Label lbl_layerLD_geoCore_exp, lbl_layerCell_geoCore_exp, lbl_layerTipLocations_geoCore_exp;
        TextBox textBox_layer_FileLocation_geoCore_exp;
        CheckBox checkBox_DOELayer_geoCore_exp, checkBox_layer_geoCore_shapeEngine_exp, checkBox_layer_geoCore_shapeEngine_perPoly_exp, checkBox_layer_geoCore_layoutReference_exp;

        void twoD_LayerUISetup_geoCore_exp()
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_layer_geoCore_exp = new GroupBox();
                TableLayout groupBox_layer_geoCore_table = new TableLayout();
                groupBox_layer_geoCore_exp.Content = groupBox_layer_geoCore_table;
                groupBox_layer_geoCore_exp.Text = "Layout Controls";

                geocore_table(groupBox_layer_geoCore_table);

                layerShapeProperties_tcPanel.Content = groupBox_layer_geoCore_exp;

                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            });
        }

        void geocore_table(TableLayout groupBox_layer_geoCore_table)
        {
            // Outer table, row 1
            TableRow geocore_tr0 = new TableRow();
            groupBox_layer_geoCore_table.Rows.Add(geocore_tr0);
            geocore_tr0.Cells.Add(new TableCell());

            geocore_row0(geocore_tr0.Cells[0]);

            TableRow geocore_tr1 = new TableRow();
            groupBox_layer_geoCore_table.Rows.Add(geocore_tr1);
            geocore_tr1.Cells.Add(new TableCell());

            geocore_row1(geocore_tr1.Cells[0]);
        }

        void geocore_row0(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout tl = new TableLayout();
            p.Content = tl;

            tl.Rows.Add(new TableRow());

            TableCell tr0_0 = new TableCell();
            tl.Rows[0].Cells.Add(tr0_0);

            geocore_row0_0(tr0_0);

            TableCell tr0_1 = new TableCell();
            tl.Rows[0].Cells.Add(tr0_1);

            geocore_row0_1(tr0_1);
        }

        void geocore_row0_0(TableCell tc)
        {
            TableLayout tl = new TableLayout();
            Panel p = new Panel();
            p.Content = tl;

            tc.Control = p;

            TableRow tr0 = new TableRow();
            tl.Rows.Add(tr0);

            TableLayout tr0_tl = new TableLayout();
            tr0.Cells.Add(new TableCell() { Control = tr0_tl });
            tr0_tl.Rows.Add(new TableRow());

            button_layer_chooseFile_geoCore_exp = new Button();
            button_layer_chooseFile_geoCore_exp.Text = "Choose File";
            button_layer_chooseFile_geoCore_exp.ToolTip = "Browse for GDS or Oasis file";
            tr0_tl.Rows[0].Cells.Add(new TableCell() { Control = button_layer_chooseFile_geoCore_exp });

            checkBox_DOELayer_geoCore_exp = new CheckBox();
            checkBox_DOELayer_geoCore_exp.Text = "DOE";
            checkBox_DOELayer_geoCore_exp.Width = 60;
            checkBox_DOELayer_geoCore_exp.ToolTip = "This is a DOE layout, to be used with tile extraction.";
            tr0_tl.Rows[0].Cells.Add(new TableCell() { Control = checkBox_DOELayer_geoCore_exp });

            checkBox_layer_geoCore_shapeEngine_exp = new CheckBox();
            checkBox_layer_geoCore_shapeEngine_exp.Text = "Contour";
            checkBox_layer_geoCore_shapeEngine_exp.Width = 75;
            checkBox_layer_geoCore_shapeEngine_exp.ToolTip = "Use layout polygons as inputs for contour generation";
            tr0_tl.Rows[0].Cells.Add(new TableCell() { Control = checkBox_layer_geoCore_shapeEngine_exp });

            checkBox_layer_geoCore_shapeEngine_perPoly_exp = new CheckBox();
            checkBox_layer_geoCore_shapeEngine_perPoly_exp.Text = "Per-Poly";
            checkBox_layer_geoCore_shapeEngine_perPoly_exp.Width = 75;
            checkBox_layer_geoCore_shapeEngine_perPoly_exp.ToolTip = "Rotation per-polyon";
            tr0_tl.Rows[0].Cells.Add(new TableCell() { Control = checkBox_layer_geoCore_shapeEngine_perPoly_exp });

            TableRow tr1 = new TableRow();
            tl.Rows.Add(tr1);

            TableLayout tr1_tl = new TableLayout();
            tr1.Cells.Add(new TableCell() { Control = tr1_tl });
            tr1_tl.Rows.Add(new TableRow());

            button_layer_globalApply_geoCore_exp = new Button();
            button_layer_globalApply_geoCore_exp.Text = "Apply To All";
            button_layer_globalApply_geoCore_exp.ToolTip = "Use this geoCore input in all layers of the simulation.";
            tr1_tl.Rows[0].Cells.Add(new TableCell() { Control = button_layer_globalApply_geoCore_exp });

            checkBox_layer_geoCore_layoutReference_exp = new CheckBox();
            checkBox_layer_geoCore_layoutReference_exp.Text = "Reference";
            checkBox_layer_geoCore_layoutReference_exp.Width = 75;
            checkBox_layer_geoCore_layoutReference_exp.ToolTip = "Reference the layout file";
            tr1_tl.Rows[0].Cells.Add(new TableCell() { Control = checkBox_layer_geoCore_layoutReference_exp });

            tr1_tl.Rows[0].Cells.Add(new TableCell() { ScaleWidth = true });

            TableRow tr2 = new TableRow();
            tl.Rows.Add(tr2);

            TableLayout tr2_tl = new TableLayout();
            tr2.Cells.Add(new TableCell() { Control = tr2_tl });
            tr2_tl.Rows.Add(new TableRow());

            comboBox_layerPolyFill_geoCore_exp = new DropDown();
            comboBox_layerPolyFill_geoCore_exp.Width = 88;
            comboBox_layerPolyFill_geoCore_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.polyFillList);
            tr2_tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerPolyFill_geoCore_exp) });

            tr2_tl.Rows[0].Cells.Add(new TableCell() { ScaleWidth = true });
        }

        void geocore_row0_1(TableCell tc)
        {
            Panel p = new Panel();

            TableLayout tl = new TableLayout();
            p.Content = tl;

            tc.Control = p;

            TableRow tr0 = new TableRow();
            tl.Rows.Add(tr0);

            lbl_layerCell_geoCore_exp = new Label();
            lbl_layerCell_geoCore_exp.Text = "Cell";
            tr0.Cells.Add(new TableCell() { Control = lbl_layerCell_geoCore_exp });

            comboBox_layerStructureList_geoCore_exp = new DropDown();
            comboBox_layerStructureList_geoCore_exp.Enabled = false;
            comboBox_layerStructureList_geoCore_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.geoCoreStructureList_exp);
            tr0.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerStructureList_geoCore_exp) });

            TableRow tr1 = new TableRow();
            tl.Rows.Add(tr1);

            lbl_layerLD_geoCore_exp = new Label();
            lbl_layerLD_geoCore_exp.Text = "L/D";
            tr1.Cells.Add(new TableCell() { Control = lbl_layerLD_geoCore_exp });

            comboBox_layerLDList_geoCore_exp = new DropDown();
            comboBox_layerLDList_geoCore_exp.Enabled = false;
            comboBox_layerLDList_geoCore_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.geoCoreLDList_exp);
            tr1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerLDList_geoCore_exp) });
        }

        void geocore_row1(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout tl = new TableLayout();
            p.Content = tl;

            tl.Rows.Add(new TableRow());

            lbl_layerTipLocations_geoCore_exp = new Label();
            lbl_layerTipLocations_geoCore_exp.Text = "Tip Locs";
            tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layerTipLocations_geoCore_exp });

            comboBox_layerTipLocations_geoCore_exp = new DropDown();
            comboBox_layerTipLocations_geoCore_exp.Enabled = false;
            tl.Rows[0].Cells.Add(new TableCell() { Control = comboBox_layerTipLocations_geoCore_exp });
            comboBox_layerTipLocations_geoCore_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

            textBox_layer_FileLocation_geoCore_exp = new TextBox();
            textBox_layer_FileLocation_geoCore_exp.ReadOnly = true;
            tl.Rows[0].Cells.Add(new TableCell() { Control = textBox_layer_FileLocation_geoCore_exp, ScaleWidth = true });
        }
    }
}
