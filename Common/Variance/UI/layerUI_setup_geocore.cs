using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    // 2D Layer GeoCore
    private GroupBox gB_layer_lithography;
    private GroupBox gB_layer_geoCore;
    private Button btn_chooseFile_geoCore, btn_globalApply_geoCore;
    private DropDown comboBox_lDList_geoCore, comboBox_structureList_geoCore, comboBox_polyFill_geoCore, comboBox_tipLocations_geoCore;
    private Label lbl_lD_geoCore, lbl_cell_geoCore, lbl_tipLocations_geoCore;
    private TextBox textBox_fileLocation_geoCore;
    private CheckBox cB_DOE_geoCore, cB_geoCore_shapeEngine, cB_geoCore_shapeEngine_perPoly, cB_geoCore_layoutReference;

    private NumericStepper num_geoCore_rayExtension;
    private Label lbl_geoCore_rayExtension;

    private void twoD_LayerUISetup_geoCore()
    {
        Application.Instance.Invoke(() =>
        {
            TableLayout geoCore_table = new();
            gB_layer_geoCore = new GroupBox {Content = geoCore_table, Text = "Layout Controls"};

            geocore_table(geoCore_table);

            layerShapeProperties_tcPanel.Content = gB_layer_geoCore;

            setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
        });
    }

    private void geocore_table(TableLayout geoCore_table)
    {
        // Outer table, row 1
        TableRow geocore_tr0 = new();
        geoCore_table.Rows.Add(geocore_tr0);
        geocore_tr0.Cells.Add(new TableCell());

        geocore_row0(geocore_tr0.Cells[0]);

        TableRow geocore_tr1 = new();
        geoCore_table.Rows.Add(geocore_tr1);
        geocore_tr1.Cells.Add(new TableCell());

        geocore_row1(geocore_tr1.Cells[0]);
    }

    private void geocore_row0(TableCell tc)
    {
        TableLayout tl = new();
        tc.Control = tl;

        tl.Rows.Add(new TableRow());

        TableCell tr0_0 = new();
        tl.Rows[^1].Cells.Add(tr0_0);

        geocore_row0_0(tr0_0);

        TableCell tr0_1 = new();
        tl.Rows[^1].Cells.Add(tr0_1);

        geocore_row0_1(tr0_1);
    }

    private void geocore_row0_0(TableCell tc)
    {
        TableLayout tl = new();

        tc.Control = tl;

        tl.Rows.Add(new TableRow());

        TableLayout tl0 = new();
        tl.Rows[^1].Cells.Add(new TableCell { Control = tl0 });
        tl0.Rows.Add(new TableRow());

        btn_chooseFile_geoCore = new Button {Text = "Choose File", ToolTip = "Browse for GDS or Oasis file"};
        tl0.Rows[^1].Cells.Add(new TableCell { Control = btn_chooseFile_geoCore });

        cB_DOE_geoCore = new CheckBox
        {
            Text = "DOE", Width = 60, ToolTip = "This is a DOE layout, to be used with tile extraction."
        };
        tl0.Rows[^1].Cells.Add(new TableCell { Control = cB_DOE_geoCore });

        cB_geoCore_shapeEngine = new CheckBox
        {
            Text = "Contour", Width = 75, ToolTip = "Use layout polygons as inputs for contour generation"
        };
        tl0.Rows[^1].Cells.Add(new TableCell { Control = cB_geoCore_shapeEngine });

        cB_geoCore_shapeEngine_perPoly = new CheckBox
        {
            Text = "Per-Poly", Width = 75, ToolTip = "Rotation per-polyon"
        };
        tl0.Rows[^1].Cells.Add(new TableCell { Control = cB_geoCore_shapeEngine_perPoly });

        tl.Rows.Add(new TableRow());

        TableLayout tl1 = new();
        tl.Rows[^1].Cells.Add(new TableCell { Control = tl1 });
        tl1.Rows.Add(new TableRow());

        btn_globalApply_geoCore = new Button
        {
            Text = "Apply To All", ToolTip = "Use this geoCore input in all layers of the simulation."
        };
        tl1.Rows[^1].Cells.Add(new TableCell { Control = btn_globalApply_geoCore });

        cB_geoCore_layoutReference = new CheckBox
        {
            Text = "Reference", Width = 75, ToolTip = "Reference the layout file"
        };
        tl1.Rows[^1].Cells.Add(new TableCell { Control = cB_geoCore_layoutReference });

        tl1.Rows[^1].Cells.Add(new TableCell { ScaleWidth = true });

        tl.Rows.Add(new TableRow());

        TableLayout tl2 = new();
        tl.Rows[^1].Cells.Add(new TableCell { Control = tl2 });
        tl2.Rows.Add(new TableRow());

        comboBox_polyFill_geoCore = new DropDown {Width = 88};
        comboBox_polyFill_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.polyFillList);
        tl2.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_polyFill_geoCore) });

        tl2.Rows[^1].Cells.Add(new TableCell { ScaleWidth = true });

        lbl_geoCore_rayExtension = new Label {Text = "Extension", ToolTip = "Extension factor for keyhole raycast."};
        tl2.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(lbl_geoCore_rayExtension) });

        num_geoCore_rayExtension = new NumericStepper
        {
            MinValue = 1.0, Increment = 0.1, DecimalPlaces = 2, ToolTip = "Line end extension."
        };
        setSize(num_geoCore_rayExtension, 55);
        tl2.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_geoCore_rayExtension) });

    }

    private void geocore_row0_1(TableCell tc)
    {
        TableLayout tl = new();
        tc.Control = tl;

        tl.Rows.Add(new TableRow());

        lbl_cell_geoCore = new Label {Text = "Cell"};
        tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_cell_geoCore });

        comboBox_structureList_geoCore = new DropDown {Enabled = false};
        comboBox_structureList_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.geoCoreStructureList_exp);
        tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_structureList_geoCore) });

        tl.Rows.Add(new TableRow());

        lbl_lD_geoCore = new Label {Text = "L/D"};
        tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lD_geoCore });

        comboBox_lDList_geoCore = new DropDown {Enabled = false};
        comboBox_lDList_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.geoCoreLDList_exp);
        tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_lDList_geoCore) });
    }

    private void geocore_row1(TableCell tc)
    {
        TableLayout tl = new();
        tc.Control = tl;
                
        tl.Rows.Add(new TableRow());

        lbl_tipLocations_geoCore = new Label {Text = "Tip Locs"};
        tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_tipLocations_geoCore });

        comboBox_tipLocations_geoCore = new DropDown {Enabled = false};
        tl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_tipLocations_geoCore });
        comboBox_tipLocations_geoCore.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

        textBox_fileLocation_geoCore = new TextBox {ReadOnly = true};
        tl.Rows[^1].Cells.Add(new TableCell { Control = textBox_fileLocation_geoCore, ScaleWidth = true });
    }
}