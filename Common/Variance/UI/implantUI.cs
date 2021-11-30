using System;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private NumericStepper num_implantResistCD, num_implantResistCDVar,
        num_implantResistHeight, num_implantResistHeightVar,
        num_implantResistTopCRR, num_implantResistTopCRRVar,
        num_implantTiltAngle, num_implantTiltAngleVar,
        num_implantTwistAngle, num_implantTwistAngleVar,
        num_implantCornerSegments, num_implantNumOfCases;

    private TextBox textBox_implantShadowNom, textBox_implantShadowMin, textBox_implantShadowMax;

    private Label lbl_implantResistCD, lbl_implantResistHeight, lbl_implantResistTopCRR,
        lbl_implantTiltAngle,
        lbl_implantTwistAngle, lbl_implantShadowNom, lbl_implantShadowMin, lbl_implantShadowMax,
        lbl_implantCornerSegments, lbl_implantNumOfCases,
        lbl_implantNom, lbl_implantVar, lbl_implantRNG;

    private GroupBox groupBox_implant;

    private CheckBox checkBox_CSV_implant, checkBox_external_implant;

    private DropDown comboBox_implantRNG, comboBox_externalTypes_implant;

    private void implantTabSetup()
    {
        groupBox_implant = new GroupBox {Text = "Implant Conditions"};
        tabPage_implant_table.Rows.Add(new TableRow());
        TableCell tc0 = new() {Control = groupBox_implant};
        tabPage_implant_table.Rows[0].Cells.Add(tc0);
        tabPage_implant_table.Rows[0].Cells.Add(new TableCell { Control = null });
        tabPage_implant_table.Rows.Add(new TableRow());

        TableLayout groupBox_implant_table = new();
        groupBox_implant.Content = groupBox_implant_table;

        groupBox_implant_table.Rows.Add(new TableRow());
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = null });

        TableLayout row0ptl = new();
        row0ptl.Rows.Add(new TableRow());

        Panel row0p = new() {Content = row0ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row0p });

        lbl_implantNom = new Label {Text = "Nominal"};
        row0ptl.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantNom });

        lbl_implantVar = new Label {Text = "3-sigma"};
        row0ptl.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantVar });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantResistCD = new Label {Text = "Developed resist CD"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantResistCD });

        TableLayout row1ptl = new();
        row1ptl.Rows.Add(new TableRow());

        Panel row1p = new() {Content = row1ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row1p });

        num_implantResistCD = new NumericStepper {Value = 1, DecimalPlaces = 2, MinValue = 1, Increment = 0.1};
        num_implantResistCD.LostFocus += doImplantShadowing;
        setSize(num_implantResistCD, 50);
        row1ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistCD) });

        num_implantResistCDVar = new NumericStepper {Value = 0, DecimalPlaces = 2, MinValue = 0, Increment = 0.1};
        num_implantResistCDVar.LostFocus += doImplantShadowing;
        setSize(num_implantResistCDVar, 50);
        row1ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistCDVar) });

        row1ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantResistHeight = new Label {Text = "Developed resist height"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantResistHeight });

        TableLayout row2ptl = new();
        row2ptl.Rows.Add(new TableRow());

        Panel row2p = new() {Content = row2ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row2p });

        num_implantResistHeight = new NumericStepper {Value = 1, DecimalPlaces = 2, MinValue = 1, Increment = 0.1};
        num_implantResistHeight.LostFocus += doImplantShadowing;
        setSize(num_implantResistHeight, 50);
        row2ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistHeight) });

        num_implantResistHeightVar = new NumericStepper
        {
            Value = 0, DecimalPlaces = 2, MinValue = 0, Increment = 0.1
        };
        num_implantResistHeightVar.LostFocus += doImplantShadowing;
        setSize(num_implantResistHeightVar, 50);
        row2ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistHeightVar) });

        row2ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantResistTopCRR = new Label {Text = "Resist Top Corner Rounding"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantResistTopCRR });

        TableLayout row3ptl = new();
        row3ptl.Rows.Add(new TableRow());

        Panel row3p = new() {Content = row3ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row3p });

        num_implantResistTopCRR = new NumericStepper {Value = 0, DecimalPlaces = 2, MinValue = 0, Increment = 0.1};
        num_implantResistTopCRR.LostFocus += doImplantShadowing;
        setSize(num_implantResistTopCRR, 50);
        row3ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistTopCRR) });

        num_implantResistTopCRRVar = new NumericStepper {Value = 0, DecimalPlaces = 2, MinValue = 0};
        num_implantResistTopCRRVar.LostFocus += doImplantShadowing;
        setSize(num_implantResistTopCRRVar, 50);
        row3ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistTopCRRVar) });

        row3ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantTiltAngle = new Label {Text = "Implant Tilt Angle (deg)"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantTiltAngle });

        TableLayout row4ptl = new();
        row4ptl.Rows.Add(new TableRow());

        Panel row4p = new() {Content = row4ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row4p });

        num_implantTiltAngle = new NumericStepper {Value = 0, DecimalPlaces = 2, MinValue = 0, MaxValue = 90};
        num_implantTiltAngle.LostFocus += doImplantShadowing;
        setSize(num_implantTiltAngle, 50);
        row4ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTiltAngle) });

        num_implantTiltAngleVar = new NumericStepper {Value = 0, DecimalPlaces = 2, MinValue = 0, MaxValue = 90};
        num_implantTiltAngleVar.LostFocus += doImplantShadowing;
        setSize(num_implantTiltAngleVar, 50);
        row4ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTiltAngleVar) });

        row4ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantTwistAngle = new Label {Text = "Implant Twist Angle (deg)"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantTwistAngle });

        TableLayout row5ptl = new();
        row5ptl.Rows.Add(new TableRow());

        Panel row5p = new() {Content = row5ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row5p });

        num_implantTwistAngle = new NumericStepper {Value = 0, DecimalPlaces = 2, MinValue = 0};
        num_implantTwistAngle.LostFocus += doImplantShadowing;
        setSize(num_implantTwistAngle, 50);
        row5ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTwistAngle) });

        num_implantTwistAngleVar = new NumericStepper {Value = 0, DecimalPlaces = 2, MinValue = 0};
        num_implantTwistAngleVar.LostFocus += doImplantShadowing;
        setSize(num_implantTwistAngleVar, 50);
        row5ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTwistAngleVar) });

        row5ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantCornerSegments = new Label {Text = "Corner Segments"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantCornerSegments });

        TableLayout row6ptl = new();
        row6ptl.Rows.Add(new TableRow());

        Panel row6p = new() {Content = row6ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row6p });

        num_implantCornerSegments = new NumericStepper {Value = 90, MinValue = 2, Increment = 1};
        num_implantCornerSegments.LostFocus += doImplantShadowing;
        setSize(num_implantCornerSegments, 80);
        row6ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantCornerSegments) });

        row6ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantNumOfCases = new Label {Text = "Number of Cases"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantNumOfCases });

        TableLayout row7ptl = new();
        row7ptl.Rows.Add(new TableRow());

        Panel row7p = new() {Content = row7ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row7p });

        num_implantNumOfCases = new NumericStepper {Value = 25000, DecimalPlaces = 0, MinValue = 1};
        num_implantNumOfCases.LostFocus += doImplantShadowing;
        setSize(num_implantNumOfCases, 80);

        row7ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantNumOfCases) });

        row7ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantRNG = new Label {Text = "RNG"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantRNG });

        TableLayout row8ptl = new();
        row8ptl.Rows.Add(new TableRow());

        Panel row8p = new() {Content = row8ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row8p });

        comboBox_implantRNG = new DropDown {DataContext = DataContext};
        comboBox_implantRNG.SelectedIndexChanged += doImplantShadowing;
        setSize(comboBox_implantRNG, 175);
        comboBox_implantRNG.BindDataContext(c => c.DataStore, (UIStringLists m) => m.rngTypeList);

        row8ptl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_implantRNG });

        row8ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = null });

        TableLayout row9ptl = new();
        row9ptl.Rows.Add(new TableRow());

        Panel row9p = new() {Content = row9ptl};

        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row9p });

        checkBox_CSV_implant = new CheckBox
        {
            Text = "CSV",
            ToolTip =
                "Write out a CSV file containing the result for each case and its inputs. Allows for offline deep-dive review."
        };
        checkBox_CSV_implant.CheckedChanged += doImplantShadowing;

        row9ptl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_CSV_implant });

        checkBox_external_implant = new CheckBox
        {
            Text = "External",
            ToolTip =
                "Write out a file containing the result for each case and its inputs. Will require significantly more memory."
        };
        checkBox_external_implant.CheckedChanged += doImplantShadowing;

        row9ptl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_external_implant });

        comboBox_externalTypes_implant = new DropDown
        {
            DataContext = DataContext, ToolTip = "Choose your external file type"
        };
        comboBox_externalTypes_implant.SelectedIndexChanged += doImplantShadowing;
        comboBox_externalTypes_implant.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalTypeList);

        row9ptl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_externalTypes_implant });

        row9ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantShadowNom = new Label {Text = "Implant Shadowing (Mean)"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantShadowNom });

        textBox_implantShadowNom = new TextBox {ReadOnly = true};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = textBox_implantShadowNom });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantShadowMin = new Label {Text = "Implant Shadowing (Min)"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantShadowMin });

        textBox_implantShadowMin = new TextBox {ReadOnly = true};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = textBox_implantShadowMin });

        groupBox_implant_table.Rows.Add(new TableRow());

        lbl_implantShadowMax = new Label {Text = "Implant Shadowing (Max)"};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantShadowMax });

        textBox_implantShadowMax = new TextBox {ReadOnly = true};
        groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = textBox_implantShadowMax });
    }

    private void doImplantShadowing(object sender, EventArgs e)
    {
        if (implantUIFrozen)
        {
            return;
        }

        // Implant properties.
        commonVars.getImplantSettings().setComment(commentBox.Text);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.tilt, num_implantTiltAngle.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.tiltV, num_implantTiltAngleVar.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.twist, num_implantTwistAngle.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.twistV, num_implantTwistAngleVar.Value);

        // Shadow structure properties.
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.w, num_implantResistCD.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.wV, num_implantResistCDVar.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.h, num_implantResistHeight.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.hV, num_implantResistHeightVar.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.cRR, num_implantResistTopCRR.Value);
        commonVars.getImplantSettings().setDouble(EntropyImplantSettings.properties_d.cV, num_implantResistTopCRRVar.Value);

        commonVars.getImplantSimulationSettings().setResolution(1.0f);
        commonVars.getImplantSimulationSettings().setValue(EntropySettings.properties_i.nCases, Convert.ToInt32(num_implantNumOfCases.Value));
        commonVars.getImplantSimulationSettings().setValue(EntropySettings.properties_i.cSeg, Convert.ToInt32(num_implantCornerSegments.Value));

        if ((bool)checkBox_CSV_implant.Checked)
        {
            commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.csv, 1);
        }
        else
        {
            commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.csv, 0);
        }

        if ((bool)checkBox_external_implant.Checked)
        {
            comboBox_externalTypes_implant.Enabled = true;
            commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.external, 1);
            commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalType, (int)CommonVars.external_Type.svg);
        }
        else
        {
            comboBox_externalTypes_implant.Enabled = false;
            commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.external, 0);
        }

        doImplantShadowing();

        uiFollowChanges();
    }

    private void doImplantShadowing()
    {
        entropyControl.entropyRun_implant(1, null, false);
    }

    private void setImplantViewportCamera(double[] parameters)
    {
        otkVPSettings_implant.setCameraPos((float)parameters[0], (float)parameters[1]);
        otkVPSettings_implant.setZoomFactor((float)parameters[2]);

    }

    private double[] getImplantViewportCamera()
    {
        double x = otkVPSettings_implant.getCameraX();
        double y = otkVPSettings_implant.getCameraY();
        double zoom = otkVPSettings_implant.getZoomFactor();
        return new[] { x, y, zoom };
    }
}