using Eto.Drawing;
using Eto.Forms;

namespace Variance
{
    public partial class MainForm
    {
        GroupBox groupBox_DOESettings, groupBox_tileSubSet;

        TextBox textBox_useSpecificTiles;

        Button btn_useCSViDRM, btn_useCSVQuilt;

        RadioButton radio_useCSViDRM, radio_useCSVQuilt, radioButton_allTiles, radio_useSpecificTile, radio_useSpecificTiles;

        NumericStepper num_colOffset, num_rowOffset, num_DOEColPitch, num_DOERowPitch, num_DOECols, num_DOERows,
        num_DOESCCol, num_DOESCRow;

        Label lbl_colOffset, lbl_rowOffset, lbl_DOEColPitch, lbl_DOERowPitch, lbl_DOECols, lbl_DOERows,
        lbl_DOESCCol, lbl_DOESCRow;

        TextArea textBox_userGuidanceDOE;

        void twoD_DOEUISetup()
        {
            tabPage_2D_DOE_table.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tabPage_2D_DOE_table.Rows[0].Cells.Add(tc0);
            Panel p = new Panel();
            tc0.Control = p;

            TableLayout masterTable = new TableLayout();
            p.Content = masterTable;

            masterTable.Rows.Add(new TableRow());

            TableCell left_tc = new TableCell();
            masterTable.Rows[0].Cells.Add(left_tc);
            TableCell right_tc = new TableCell();
            masterTable.Rows[0].Cells.Add(right_tc);
            masterTable.Rows[0].Cells.Add(new TableCell { Control = null });

            Panel leftPanel = new Panel();
            TableLayout leftTable = new TableLayout();
            leftPanel.Content = leftTable;
            left_tc.Control = leftPanel;
            leftTable.Rows.Add(new TableRow());
            TableCell ltc0 = new TableCell();
            leftTable.Rows[0].Cells.Add(ltc0);
            leftTable.Rows[0].Cells.Add(new TableCell { Control = null });
            twoD_DOEUISetup_settings(ltc0);

            leftTable.Rows.Add(new TableRow());
            TableCell ltc1 = new TableCell();
            leftTable.Rows[1].Cells.Add(ltc1);

            twoD_DOEUISetup_tileset(ltc1);

            leftTable.Rows.Add(new TableRow()); //padding

            masterTable.Rows.Add(new TableRow()); // padding

            textBox_userGuidanceDOE = new TextArea();
            textBox_userGuidanceDOE.Wrap = true;
            textBox_userGuidanceDOE.ReadOnly = true;
            textBox_userGuidanceDOE.Size = new Size(userGuidanceWidth, userGuidanceHeight);
            textBox_userGuidanceDOE.Text = "The DOE settings relate to geoCore input layers that have the DOE checkbox set.\r\n\r\n" +
                "You can either manually set the DOE grid using the settings here, or load in a CSV file from iDRM that will restore the grid used in that tool.\r\n\r\n" +
                "Notes:\r\n\r\n" +
                "- The lower left corner of each extracted tile  bounding box is placed with respect to the origin.\r\n" +
                "- Accurate definition of the DOE grid is important to ensure interaction with other layers is correctly assessed.\r\n" +
                "- The DOE grid is common to all input layers.\r\n" +
                "- The DOE is run sequentially from lower left to top right, with each tile extracted being evaluated for the full set of cases.\r\n" +
                "- Output files are generated with row and column indices to show which tile they came from.";

            right_tc.Control = textBox_userGuidanceDOE;

            tabPage_2D_DOE_table.Rows.Add(new TableRow()); // padding.
        }

        void twoD_DOEUISetup_settings(TableCell tc)
        {
            groupBox_DOESettings = new GroupBox();
            tc.Control = groupBox_DOESettings;

            TableLayout groupBox_DOESettings_table = new TableLayout();
            groupBox_DOESettings.Content = groupBox_DOESettings_table;
            groupBox_DOESettings.Text = "Grid Layout Properties";

            groupBox_DOESettings_table.Rows.Add(new TableRow());

            lbl_rowOffset = new Label();
            lbl_rowOffset.Text = "Row Offset";
            lbl_rowOffset.ToolTip = "Vertical coordinate of lower left corner of lower left tile in DOE grid.";

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_rowOffset });

            num_rowOffset = new NumericStepper();
            num_rowOffset.Value = 0;
            num_rowOffset.Increment = 1;
            num_rowOffset.ToolTip = "Vertical coordinate of lower left corner of lower left tile in DOE grid.";
            setSize(num_rowOffset, 80);

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_rowOffset) });

            lbl_colOffset = new Label();
            lbl_colOffset.Text = "Col Offset";
            lbl_colOffset.ToolTip = "Horizontal coordinate of lower left corner of lower left tile in DOE grid.";

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_colOffset });

            num_colOffset = new NumericStepper();
            num_colOffset.Value = 0;
            num_colOffset.Increment = 1;
            num_colOffset.ToolTip = "Horizontal coordinate of lower left corner of lower left tile in DOE grid.";
            setSize(num_colOffset, 80);

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_colOffset) });

            groupBox_DOESettings_table.Rows.Add(new TableRow());

            lbl_DOERows = new Label();
            lbl_DOERows.Text = "Rows";
            lbl_DOERows.ToolTip = "Number of rows in DOE grid.";

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOERows });

            num_DOERows = new NumericStepper();
            num_DOERows.Value = 1;
            num_DOERows.Increment = 1;
            num_DOERows.MinValue = 1;
            num_DOERows.ToolTip = "Number of rows in DOE grid.";
            setSize(num_DOERows, 80);

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOERows) });

            lbl_DOECols = new Label();
            lbl_DOECols.Text = "Cols";
            lbl_DOECols.ToolTip = "Number of columns in DOE grid.";

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOECols });

            num_DOECols = new NumericStepper();
            num_DOECols.Value = 1;
            num_DOECols.Increment = 1;
            num_DOECols.MinValue = 1;
            num_DOECols.ToolTip = "Number of columns in DOE grid.";
            setSize(num_DOECols, 80);

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOECols) });

            groupBox_DOESettings_table.Rows.Add(new TableRow());

            lbl_DOERowPitch = new Label();
            lbl_DOERowPitch.Text = "Row Pitch";
            lbl_DOERowPitch.ToolTip = "Row height in DOE grid.";

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOERowPitch });

            num_DOERowPitch = new NumericStepper();
            num_DOERowPitch.Value = 1;
            num_DOERowPitch.Increment = 1;
            num_DOERowPitch.MinValue = 1;
            num_DOERowPitch.ToolTip = "Row height in DOE grid.";
            setSize(num_DOERowPitch, 80);

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOERowPitch) });


            lbl_DOEColPitch = new Label();
            lbl_DOEColPitch.Text = "Col Pitch";
            lbl_DOEColPitch.ToolTip = "Column width in DOE grid.";

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOEColPitch });

            num_DOEColPitch = new NumericStepper();
            num_DOEColPitch.Value = 1;
            num_DOEColPitch.Increment = 1;
            num_DOEColPitch.MinValue = 1;
            num_DOEColPitch.ToolTip = "Column width in DOE grid.";
            setSize(num_DOEColPitch, 80);

            groupBox_DOESettings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOEColPitch) });
        }

        void twoD_DOEUISetup_tileset(TableCell tc)
        {
            groupBox_tileSubSet = new GroupBox();
            tc.Control = groupBox_tileSubSet;

            Panel p = new Panel();
            TableLayout masterTable = new TableLayout();
            masterTable.Rows.Add(new TableRow());
            p.Content = masterTable;
            groupBox_tileSubSet.Content = p;

            Panel p0 = new Panel();
            TableLayout groupBox_tileSubSet_table = new TableLayout();
            p0.Content = groupBox_tileSubSet_table;
            masterTable.Rows[0].Cells.Add(new TableCell { Control = p0 });
            masterTable.Rows[0].Cells.Add(new TableCell { Control = null }); //padding
            groupBox_tileSubSet.Text = "Subset of Tiles";

            groupBox_tileSubSet_table.Rows.Add(new TableRow());

            Panel row0 = new Panel();
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row0 });
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            TableLayout row0_tl = new TableLayout();
            row0.Content = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            radioButton_allTiles = new RadioButton();
            radioButton_allTiles.Text = "Use All Tiles";
            radioButton_allTiles.Checked = true;
            radioButton_allTiles.ToolTip = "Use all tiles in DOE grid.\nStart in lower left, end in upper right.";

            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = radioButton_allTiles });
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            groupBox_tileSubSet_table.Rows.Add(new TableRow());

            Panel row1 = new Panel();
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row1 });
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            TableLayout row1_tl = new TableLayout();
            row1.Content = row1_tl;
            row1_tl.Rows.Add(new TableRow());

            radio_useSpecificTile = new RadioButton(radioButton_allTiles);
            radio_useSpecificTile.Text = "Use Specific Tile";
            radio_useSpecificTile.ToolTip = "Use single tile from DOE grid.";

            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useSpecificTile });

            lbl_DOESCRow = new Label();
            lbl_DOESCRow.Text = "Row";
            lbl_DOESCRow.ToolTip = "Row index for tile.\n1 is lowest row.";

            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOESCRow });

            num_DOESCRow = new NumericStepper();
            num_DOESCRow.Value = 1;
            num_DOESCRow.Increment = 1;
            num_DOESCRow.MinValue = 1;
            num_DOESCRow.ToolTip = "Row index for tile.\n1 is lowest row.";
            setSize(num_DOESCRow, 80);

            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOESCRow) });

            lbl_DOESCCol = new Label();
            lbl_DOESCCol.Text = "Col";
            lbl_DOESCCol.ToolTip = "Column index for tile.\n1 is leftmost column.";

            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_DOESCCol });

            num_DOESCCol = new NumericStepper();
            num_DOESCCol.Value = 1;
            num_DOESCCol.Increment = 1;
            num_DOESCCol.MinValue = 1;
            num_DOESCCol.ToolTip = "Column index for tile.\n1 is leftmost column.";
            setSize(num_DOESCCol, 80);

            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_DOESCCol) });
            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            groupBox_tileSubSet_table.Rows.Add(new TableRow());

            Panel row2 = new Panel();
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row2 });
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            TableLayout row2_tl = new TableLayout();
            row2.Content = row2_tl;
            row2_tl.Rows.Add(new TableRow());

            radio_useSpecificTiles = new RadioButton(radioButton_allTiles);
            radio_useSpecificTiles.Text = "Use Specific Tiles (col,row;col,row;col;row)";
            radio_useSpecificTiles.ToolTip = "Use these tiles in grid.\nNote syntax.\n1:1 is the lower left tile.";

            row2_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useSpecificTiles });
            row2_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            row2_tl.Rows.Add(new TableRow());

            textBox_useSpecificTiles = new TextBox();
            textBox_useSpecificTiles.Width = 200;
            textBox_useSpecificTiles.ToolTip = "Use these tiles in grid.\nNote syntax.\n1:1 is the lower left tile.";

            row2_tl.Rows[^1].Cells.Add(new TableCell { Control = textBox_useSpecificTiles });
            row2_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            groupBox_tileSubSet_table.Rows.Add(new TableRow());

            Panel row3 = new Panel();
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row3 });

            TableLayout row3_tl = new TableLayout();
            row3.Content = row3_tl;
            row3_tl.Rows.Add(new TableRow());

            radio_useCSViDRM = new RadioButton(radioButton_allTiles);
            radio_useCSViDRM.Text = "Use iDRM CSV";
            radio_useCSViDRM.ToolTip = "Use iDRM CSV output to tag tiles for use in DOE.";

            row3_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useCSViDRM });

            btn_useCSViDRM = new Button();
            btn_useCSViDRM.Text = "Choose File";
            btn_useCSViDRM.Click += iDRMFileChooser_Handler;

            row3_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_useCSViDRM) });

            groupBox_tileSubSet_table.Rows.Add(new TableRow());

            Panel row4 = new Panel();
            groupBox_tileSubSet_table.Rows[^1].Cells.Add(new TableCell { Control = row4 });

            TableLayout row4_tl = new TableLayout();
            row4.Content = row4_tl;
            row4_tl.Rows.Add(new TableRow());

            radio_useCSVQuilt = new RadioButton(radioButton_allTiles);
            radio_useCSVQuilt.Text = "Use Quilt CSV";

            row4_tl.Rows[^1].Cells.Add(new TableCell { Control = radio_useCSVQuilt });

            btn_useCSVQuilt = new Button();
            btn_useCSVQuilt.Text = "Choose File";
            btn_useCSVQuilt.Click += QuiltFileChooser_Handler;

            row4_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_useCSVQuilt) });
        }
    }
}