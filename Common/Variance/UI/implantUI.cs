using System;
using Eto.Forms;

namespace Variance
{
    public partial class MainForm
    {
        NumericStepper num_implantResistCD, num_implantResistCDVar,
                            num_implantResistHeight, num_implantResistHeightVar,
                            num_implantResistTopCRR, num_implantResistTopCRRVar,
                            num_implantTiltAngle, num_implantTiltAngleVar,
                            num_implantTwistAngle, num_implantTwistAngleVar,
                            num_implantCornerSegments, num_implantNumOfCases;

        TextBox textBox_implantShadowNom, textBox_implantShadowMin, textBox_implantShadowMax;

        Label lbl_implantResistCD, lbl_implantResistHeight, lbl_implantResistTopCRR,
                        lbl_implantTiltAngle,
                        lbl_implantTwistAngle, lbl_implantShadowNom, lbl_implantShadowMin, lbl_implantShadowMax,
                        lbl_implantCornerSegments, lbl_implantNumOfCases,
                        lbl_implantNom, lbl_implantVar, lbl_implantRNG;

        GroupBox groupBox_implant;

        CheckBox checkBox_CSV_implant, checkBox_external_implant;

        DropDown comboBox_implantRNG, comboBox_externalTypes_implant;

        void implantTabSetup()
        {
            groupBox_implant = new GroupBox();
            groupBox_implant.Text = "Implant Conditions";
            tabPage_implant_table.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tc0.Control = groupBox_implant;
            tabPage_implant_table.Rows[0].Cells.Add(tc0);
            tabPage_implant_table.Rows[0].Cells.Add(new TableCell { Control = null });
            tabPage_implant_table.Rows.Add(new TableRow());

            TableLayout groupBox_implant_table = new TableLayout();
            groupBox_implant.Content = groupBox_implant_table;

            groupBox_implant_table.Rows.Add(new TableRow());
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = null });

            Panel row0p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row0p });

            TableLayout row0ptl = new TableLayout();
            row0p.Content = row0ptl;
            row0ptl.Rows.Add(new TableRow());

            lbl_implantNom = new Label();
            lbl_implantNom.Text = "Nominal";
            row0ptl.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantNom });

            lbl_implantVar = new Label();
            lbl_implantVar.Text = "3-sigma";
            row0ptl.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantVar });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantResistCD = new Label();
            lbl_implantResistCD.Text = "Developed resist CD";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantResistCD });

            Panel row1p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row1p });

            TableLayout row1ptl = new TableLayout();
            row1p.Content = row1ptl;
            row1ptl.Rows.Add(new TableRow());

            num_implantResistCD = new NumericStepper();
            num_implantResistCD.Value = 1;
            num_implantResistCD.DecimalPlaces = 2;
            num_implantResistCD.MinValue = 1;
            num_implantResistCD.Increment = 0.1;
            num_implantResistCD.LostFocus += doImplantShadowing;
            setSize(num_implantResistCD, 50);
            row1ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistCD) });

            num_implantResistCDVar = new NumericStepper();
            num_implantResistCDVar.Value = 0;
            num_implantResistCDVar.DecimalPlaces = 2;
            num_implantResistCDVar.MinValue = 0;
            num_implantResistCDVar.Increment = 0.1;
            num_implantResistCDVar.LostFocus += doImplantShadowing;
            setSize(num_implantResistCDVar, 50);
            row1ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistCDVar) });

            row1ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantResistHeight = new Label();
            lbl_implantResistHeight.Text = "Developed resist height";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantResistHeight });

            Panel row2p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row2p });

            TableLayout row2ptl = new TableLayout();
            row2p.Content = row2ptl;
            row2ptl.Rows.Add(new TableRow());

            num_implantResistHeight = new NumericStepper();
            num_implantResistHeight.Value = 1;
            num_implantResistHeight.DecimalPlaces = 2;
            num_implantResistHeight.MinValue = 1;
            num_implantResistHeight.Increment = 0.1;
            num_implantResistHeight.LostFocus += doImplantShadowing;
            setSize(num_implantResistHeight, 50);
            row2ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistHeight) });

            num_implantResistHeightVar = new NumericStepper();
            num_implantResistHeightVar.Value = 0;
            num_implantResistHeightVar.DecimalPlaces = 2;
            num_implantResistHeightVar.MinValue = 0;
            num_implantResistHeightVar.Increment = 0.1;
            num_implantResistHeightVar.LostFocus += doImplantShadowing;
            setSize(num_implantResistHeightVar, 50);
            row2ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistHeightVar) });

            row2ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantResistTopCRR = new Label();
            lbl_implantResistTopCRR.Text = "Resist Top Corner Rounding";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantResistTopCRR });

            Panel row3p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row3p });

            TableLayout row3ptl = new TableLayout();
            row3p.Content = row3ptl;
            row3ptl.Rows.Add(new TableRow());

            num_implantResistTopCRR = new NumericStepper();
            num_implantResistTopCRR.Value = 0;
            num_implantResistTopCRR.DecimalPlaces = 2;
            num_implantResistTopCRR.MinValue = 0;
            num_implantResistTopCRR.Increment = 0.1;
            num_implantResistTopCRR.LostFocus += doImplantShadowing;
            setSize(num_implantResistTopCRR, 50);
            row3ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistTopCRR) });

            num_implantResistTopCRRVar = new NumericStepper();
            num_implantResistTopCRRVar.Value = 0;
            num_implantResistTopCRRVar.DecimalPlaces = 2;
            num_implantResistTopCRRVar.MinValue = 0;
            num_implantResistTopCRRVar.LostFocus += doImplantShadowing;
            setSize(num_implantResistTopCRRVar, 50);
            row3ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantResistTopCRRVar) });

            row3ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantTiltAngle = new Label();
            lbl_implantTiltAngle.Text = "Implant Tilt Angle (deg)";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantTiltAngle });

            Panel row4p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row4p });

            TableLayout row4ptl = new TableLayout();
            row4p.Content = row4ptl;
            row4ptl.Rows.Add(new TableRow());

            num_implantTiltAngle = new NumericStepper();
            num_implantTiltAngle.Value = 0;
            num_implantTiltAngle.DecimalPlaces = 2;
            num_implantTiltAngle.MinValue = 0;
            num_implantTiltAngle.MaxValue = 90;
            num_implantTiltAngle.LostFocus += doImplantShadowing;
            setSize(num_implantTiltAngle, 50);
            row4ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTiltAngle) });

            num_implantTiltAngleVar = new NumericStepper();
            num_implantTiltAngleVar.Value = 0;
            num_implantTiltAngleVar.DecimalPlaces = 2;
            num_implantTiltAngleVar.MinValue = 0;
            num_implantTiltAngleVar.MaxValue = 90;
            num_implantTiltAngleVar.LostFocus += doImplantShadowing;
            setSize(num_implantTiltAngleVar, 50);
            row4ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTiltAngleVar) });

            row4ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantTwistAngle = new Label();
            lbl_implantTwistAngle.Text = "Implant Twist Angle (deg)";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantTwistAngle });

            Panel row5p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row5p });

            TableLayout row5ptl = new TableLayout();
            row5p.Content = row5ptl;
            row5ptl.Rows.Add(new TableRow());

            num_implantTwistAngle = new NumericStepper();
            num_implantTwistAngle.Value = 0;
            num_implantTwistAngle.DecimalPlaces = 2;
            num_implantTwistAngle.MinValue = 0;
            num_implantTwistAngle.LostFocus += doImplantShadowing;
            setSize(num_implantTwistAngle, 50);
            row5ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTwistAngle) });

            num_implantTwistAngleVar = new NumericStepper();
            num_implantTwistAngleVar.Value = 0;
            num_implantTwistAngleVar.DecimalPlaces = 2;
            num_implantTwistAngleVar.MinValue = 0;
            num_implantTwistAngleVar.LostFocus += doImplantShadowing;
            setSize(num_implantTwistAngleVar, 50);
            row5ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantTwistAngleVar) });

            row5ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantCornerSegments = new Label();
            lbl_implantCornerSegments.Text = "Corner Segments";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantCornerSegments });

            Panel row6p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row6p });

            TableLayout row6ptl = new TableLayout();
            row6p.Content = row6ptl;
            row6ptl.Rows.Add(new TableRow());

            num_implantCornerSegments = new NumericStepper();
            num_implantCornerSegments.Value = 90;
            num_implantCornerSegments.MinValue = 2;
            num_implantCornerSegments.Increment = 1;
            num_implantCornerSegments.LostFocus += doImplantShadowing;
            setSize(num_implantCornerSegments, 80);
            row6ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantCornerSegments) });

            row6ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantNumOfCases = new Label();
            lbl_implantNumOfCases.Text = "Number of Cases";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantNumOfCases });

            Panel row7p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row7p });

            TableLayout row7ptl = new TableLayout();
            row7p.Content = row7ptl;
            row7ptl.Rows.Add(new TableRow());

            num_implantNumOfCases = new NumericStepper();
            num_implantNumOfCases.Value = 25000;
            num_implantNumOfCases.DecimalPlaces = 0;
            num_implantNumOfCases.MinValue = 1;
            num_implantNumOfCases.LostFocus += doImplantShadowing;
            setSize(num_implantNumOfCases, 80);

            row7ptl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_implantNumOfCases) });

            row7ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantRNG = new Label();
            lbl_implantRNG.Text = "RNG";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantRNG });

            Panel row8p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row8p });

            TableLayout row8ptl = new TableLayout();
            row8p.Content = row8ptl;
            row8ptl.Rows.Add(new TableRow());

            comboBox_implantRNG = new DropDown();
            comboBox_implantRNG.DataContext = DataContext;
            comboBox_implantRNG.BindDataContext(c => c.DataStore, (UIStringLists m) => m.rngTypeList);
            comboBox_implantRNG.SelectedIndexChanged += doImplantShadowing;
            setSize(comboBox_implantRNG, 175);

            row8ptl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_implantRNG });

            row8ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = null });

            Panel row9p = new Panel();
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = row9p });

            TableLayout row9ptl = new TableLayout();
            row9p.Content = row9ptl;
            row9ptl.Rows.Add(new TableRow());

            checkBox_CSV_implant = new CheckBox();
            checkBox_CSV_implant.Text = "CSV";
            checkBox_CSV_implant.CheckedChanged += doImplantShadowing;
            checkBox_CSV_implant.ToolTip = "Write out a CSV file containing the result for each case and its inputs. Allows for offline deep-dive review.";

            row9ptl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_CSV_implant });

            checkBox_external_implant = new CheckBox();
            checkBox_external_implant.Text = "External";
            checkBox_external_implant.CheckedChanged += doImplantShadowing;
            checkBox_external_implant.ToolTip = "Write out a file containing the result for each case and its inputs. Will require significantly more memory.";

            row9ptl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_external_implant });

            comboBox_externalTypes_implant = new DropDown();
            comboBox_externalTypes_implant.DataContext = DataContext;
            comboBox_externalTypes_implant.SelectedIndexChanged += doImplantShadowing;
            comboBox_externalTypes_implant.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalTypeList);
            comboBox_externalTypes_implant.ToolTip = "Choose your external file type";

            row9ptl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_externalTypes_implant });

            row9ptl.Rows[^1].Cells.Add(new TableCell { Control = null });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantShadowNom = new Label();
            lbl_implantShadowNom.Text = "Implant Shadowing (Mean)";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantShadowNom });

            textBox_implantShadowNom = new TextBox();
            textBox_implantShadowNom.ReadOnly = true;
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = textBox_implantShadowNom });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantShadowMin = new Label();
            lbl_implantShadowMin.Text = "Implant Shadowing (Min)";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantShadowMin });

            textBox_implantShadowMin = new TextBox();
            textBox_implantShadowMin.ReadOnly = true;
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = textBox_implantShadowMin });

            groupBox_implant_table.Rows.Add(new TableRow());

            lbl_implantShadowMax = new Label();
            lbl_implantShadowMax.Text = "Implant Shadowing (Max)";
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_implantShadowMax });

            textBox_implantShadowMax = new TextBox();
            textBox_implantShadowMax.ReadOnly = true;
            groupBox_implant_table.Rows[^1].Cells.Add(new TableCell { Control = textBox_implantShadowMax });
        }

        void doImplantShadowing(object sender, EventArgs e)
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
                commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalType, (Int32)CommonVars.external_Type.svg);
            }
            else
            {
                comboBox_externalTypes_implant.Enabled = false;
                commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.external, 0);
            }

            doImplantShadowing();

            uiFollowChanges();
        }

        void doImplantShadowing()
        {
            entropyControl.entropyRun_implant(1, null, false);
        }

        void setImplantViewportCamera(double[] parameters)
        {
            otkVPSettings_implant.setCameraPos((float)parameters[0], (float)parameters[1]);
            otkVPSettings_implant.setZoomFactor((float)parameters[2]);

        }

        double[] getImplantViewportCamera()
        {
            double x = otkVPSettings_implant.getCameraX();
            double y = otkVPSettings_implant.getCameraY();
            double zoom = otkVPSettings_implant.getZoomFactor();
            return new[] { x, y, zoom };
        }
    }
}