﻿using Eto.Drawing;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private GroupBox groupBox_DOESettings, groupBox_tileSubSet;

    private TextBox textBox_useSpecificTiles;

    private Button btn_useCSViDRM, btn_useCSVQuilt;

    private RadioButton radio_useCSViDRM, radio_useCSVQuilt, radioButton_allTiles, radio_useSpecificTile, radio_useSpecificTiles;

    private NumericStepper num_colOffset, num_rowOffset, num_DOEColPitch, num_DOERowPitch, num_DOECols, num_DOERows,
        num_DOESCCol, num_DOESCRow;

    private Label lbl_colOffset, lbl_rowOffset, lbl_DOEColPitch, lbl_DOERowPitch, lbl_DOECols, lbl_DOERows,
        lbl_DOESCCol, lbl_DOESCRow;

    private TextArea textBox_userGuidanceDOE;

    private void twoD_DOEUISetup()
    {
        tabPage_2D_DOE_table.Rows.Add(new TableRow());
        TableCell tc0 = new();
        tabPage_2D_DOE_table.Rows[0].Cells.Add(tc0);
        Panel p = new();
        tc0.Control = p;

        TableLayout masterTable = new();
        p.Content = masterTable;

        masterTable.Rows.Add(new TableRow());

        TableCell left_tc = new();
        masterTable.Rows[0].Cells.Add(left_tc);
        TableCell right_tc = new();
        masterTable.Rows[0].Cells.Add(right_tc);
        masterTable.Rows[0].Cells.Add(new TableCell { Control = null });

        Panel leftPanel = new();
        TableLayout leftTable = new();
        leftPanel.Content = leftTable;
        left_tc.Control = leftPanel;
        leftTable.Rows.Add(new TableRow());
        TableCell ltc0 = new();
        leftTable.Rows[0].Cells.Add(ltc0);
        leftTable.Rows[0].Cells.Add(new TableCell { Control = null });
        twoD_DOEUISetup_settings(ltc0);

        leftTable.Rows.Add(new TableRow());
        TableCell ltc1 = new();
        leftTable.Rows[1].Cells.Add(ltc1);

        twoD_DOEUISetup_tileset(ltc1);

        leftTable.Rows.Add(new TableRow()); //padding

        masterTable.Rows.Add(new TableRow()); // padding

        textBox_userGuidanceDOE = new TextArea
        {
            Wrap = true,
            ReadOnly = true,
            Size = new Size(userGuidanceWidth, userGuidanceHeight),
            Text = "The DOE settings relate to geoCore input layers that have the DOE checkbox set.\r\n\r\n" +
                   "You can either manually set the DOE grid using the settings here, or load in a CSV file from iDRM that will restore the grid used in that tool.\r\n\r\n" +
                   "Notes:\r\n\r\n" +
                   "- The lower left corner of each extracted tile  bounding box is placed with respect to the origin.\r\n" +
                   "- Accurate definition of the DOE grid is important to ensure interaction with other layers is correctly assessed.\r\n" +
                   "- The DOE grid is common to all input layers.\r\n" +
                   "- The DOE is run sequentially from lower left to top right, with each tile extracted being evaluated for the full set of cases.\r\n" +
                   "- Output files are generated with row and column indices to show which tile they came from."
        };

        right_tc.Control = textBox_userGuidanceDOE;

        tabPage_2D_DOE_table.Rows.Add(new TableRow()); // padding.
    }

    private void twoD_DOEUISetup_settings(TableCell tc)
    {
        groupBox_DOESettings = new GroupBox();
        tc.Control = groupBox_DOESettings;

        TableLayout groupBox_DOESettings_table = new();
        groupBox_DOESettings.Content = groupBox_DOESettings_table;
        groupBox_DOESettings.Text = "Grid Layout Properties";

        groupBox_DOESettings_table.Rows.Add(new TableRow());

        lbl_rowOffset = new Label
        {
            Text = "Row Offset",
            ToolTip = "Vertical coordinate of lower left corner of lower left tile in DOE grid."
        };

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_rowOffset });

        num_rowOffset = new NumericStepper
        {
            Value = 0,
            Increment = 1,
            ToolTip = "Vertical coordinate of lower left corner of lower left tile in DOE grid."
        };
        setSize(num_rowOffset, 80);

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_rowOffset) });

        lbl_colOffset = new Label
        {
            Text = "Col Offset",
            ToolTip = "Horizontal coordinate of lower left corner of lower left tile in DOE grid."
        };

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_colOffset });

        num_colOffset = new NumericStepper
        {
            Value = 0,
            Increment = 1,
            ToolTip = "Horizontal coordinate of lower left corner of lower left tile in DOE grid."
        };
        setSize(num_colOffset, 80);

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_colOffset) });

        groupBox_DOESettings_table.Rows.Add(new TableRow());

        lbl_DOERows = new Label {Text = "Rows", ToolTip = "Number of rows in DOE grid."};

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOERows });

        num_DOERows = new NumericStepper
        {
            Value = 1, Increment = 1, MinValue = 1, ToolTip = "Number of rows in DOE grid."
        };
        setSize(num_DOERows, 80);

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOERows) });

        lbl_DOECols = new Label {Text = "Cols", ToolTip = "Number of columns in DOE grid."};

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOECols });

        num_DOECols = new NumericStepper
        {
            Value = 1, Increment = 1, MinValue = 1, ToolTip = "Number of columns in DOE grid."
        };
        setSize(num_DOECols, 80);

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOECols) });

        groupBox_DOESettings_table.Rows.Add(new TableRow());

        lbl_DOERowPitch = new Label {Text = "Row Pitch", ToolTip = "Row height in DOE grid."};

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOERowPitch });

        num_DOERowPitch = new NumericStepper
        {
            Value = 1, Increment = 1, MinValue = 1, ToolTip = "Row height in DOE grid."
        };
        setSize(num_DOERowPitch, 80);

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOERowPitch) });


        lbl_DOEColPitch = new Label {Text = "Col Pitch", ToolTip = "Column width in DOE grid."};

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOEColPitch });

        num_DOEColPitch = new NumericStepper
        {
            Value = 1, Increment = 1, MinValue = 1, ToolTip = "Column width in DOE grid."
        };
        setSize(num_DOEColPitch, 80);

        groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOEColPitch) });
    }

    private void twoD_DOEUISetup_tileset(TableCell tc)
    {
        groupBox_tileSubSet = new GroupBox();
        tc.Control = groupBox_tileSubSet;

        TableLayout masterTable = new();
        masterTable.Rows.Add(new TableRow());
        Panel p = new() {Content = masterTable};
        groupBox_tileSubSet.Content = p;

        TableLayout groupBox_tileSubSet_table = new();
        Panel p0 = new() {Content = groupBox_tileSubSet_table};
        masterTable.Rows[0].Cells.Add(new TableCell { Control = p0 });
        masterTable.Rows[0].Cells.Add(new TableCell { Control = null }); //padding
        groupBox_tileSubSet.Text = "Subset of Tiles";

        groupBox_tileSubSet_table.Rows.Add(new TableRow());

        TableLayout row0_tl = new();
        Panel row0 = new() {Content = row0_tl};
        row0_tl.Rows.Add(new TableRow());

        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row0 });
        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        radioButton_allTiles = new RadioButton
        {
            Text = "Use All Tiles",
            Checked = true,
            ToolTip = "Use all tiles in DOE grid.\nStart in lower left, end in upper right."
        };

        row0_tl.Rows[^1].Cells.Add(new TableCell { Control = radioButton_allTiles });
        row0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        groupBox_tileSubSet_table.Rows.Add(new TableRow());


        TableLayout row1_tl = new();
        Panel row1 = new() {Content = row1_tl};
        row1_tl.Rows.Add(new TableRow());

        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row1 });
        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        radio_useSpecificTile = new RadioButton(radioButton_allTiles)
        {
            Text = "Use Specific Tile", ToolTip = "Use single tile from DOE grid."
        };

        row1_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useSpecificTile });

        lbl_DOESCRow = new Label {Text = "Row", ToolTip = "Row index for tile.\n1 is lowest row."};

        row1_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOESCRow });

        num_DOESCRow = new NumericStepper
        {
            Value = 1, Increment = 1, MinValue = 1, ToolTip = "Row index for tile.\n1 is lowest row."
        };
        setSize(num_DOESCRow, 80);

        row1_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOESCRow) });

        lbl_DOESCCol = new Label {Text = "Col", ToolTip = "Column index for tile.\n1 is leftmost column."};

        row1_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOESCCol });

        num_DOESCCol = new NumericStepper
        {
            Value = 1, Increment = 1, MinValue = 1, ToolTip = "Column index for tile.\n1 is leftmost column."
        };
        setSize(num_DOESCCol, 80);

        row1_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOESCCol) });
        row1_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        groupBox_tileSubSet_table.Rows.Add(new TableRow());

        TableLayout row2_tl = new();
        row2_tl.Rows.Add(new TableRow());

        Panel row2 = new() {Content = row2_tl};

        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row2 });
        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        radio_useSpecificTiles = new RadioButton(radioButton_allTiles)
        {
            Text = "Use Specific Tiles (col,row;col,row;col;row)",
            ToolTip = "Use these tiles in grid.\nNote syntax.\n1:1 is the lower left tile."
        };

        row2_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useSpecificTiles });
        row2_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        row2_tl.Rows.Add(new TableRow());

        textBox_useSpecificTiles = new TextBox
        {
            Width = 200, ToolTip = "Use these tiles in grid.\nNote syntax.\n1:1 is the lower left tile."
        };

        row2_tl.Rows[^1].Cells.Add(new TableCell { Control = textBox_useSpecificTiles });
        row2_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        groupBox_tileSubSet_table.Rows.Add(new TableRow());

        TableLayout row3_tl = new();
        row3_tl.Rows.Add(new TableRow());

        Panel row3 = new() {Content = row3_tl};

        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row3 });

        radio_useCSViDRM = new RadioButton(radioButton_allTiles)
        {
            Text = "Use iDRM CSV", ToolTip = "Use iDRM CSV output to tag tiles for use in DOE."
        };

        row3_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useCSViDRM });

        btn_useCSViDRM = new Button {Text = "Choose File"};
        btn_useCSViDRM.Click += iDRMFileChooser_Handler;

        row3_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_useCSViDRM) });

        groupBox_tileSubSet_table.Rows.Add(new TableRow());

        TableLayout row4_tl = new();
        row4_tl.Rows.Add(new TableRow());

        Panel row4 = new() {Content = row4_tl};

        groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row4 });

        radio_useCSVQuilt = new RadioButton(radioButton_allTiles) {Text = "Use Quilt CSV"};

        row4_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useCSVQuilt });

        btn_useCSVQuilt = new Button {Text = "Choose File"};
        btn_useCSVQuilt.Click += QuiltFileChooser_Handler;

        row4_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_useCSVQuilt) });
    }
}