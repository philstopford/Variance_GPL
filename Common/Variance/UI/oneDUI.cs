using System;
using Eto.Forms;
using resources;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 1D UI stuff
        Label lbl_1D_Guidance, lbl_minins, lbl_nSigmaRule1D, lbl_threeSigmaRule1D, lbl_fourSigmaRule1D, lbl_sigmaEdgeTol1D,
                 lbl_threeSigmaEdgeTol1D, lbl_fourSigmaEdgeTol1D,
                 lbl_layerOnePhys, lbl_layerOneDrawn, lbl_layerOneBias, lbl_layerOneAddVar, lbl_layerOneLWR, lbl_layerOneCDU,
                 lbl_layerOneOverlayX, lbl_layerOneOverlayY, lbl_layerOneName,
                 lbl_layerTwoPhys, lbl_layerTwoDrawn, lbl_layerTwoBias, lbl_layerTwoAddVar, lbl_layerTwoLWR, lbl_layerTwoCDU,
                 lbl_layerTwoOverlayX, lbl_layerTwoOverlayY, lbl_layerTwoName;
        TextBox text_minins, text_nSigmaEdgeTol1D, text_nSigmaRule1D, text_fourSigmaRule1D, text_threeSigmaRule1D,
                input_nsigma1D, text_fourSigmaEdgeTol1D, text_threeSigmaEdgeTol1D,
                text_layerOnePhys, text_layerOneDrawn, text_layerOneBias, text_layerOneAddVar, text_layerOneLWR, text_layerOneCDU,
                text_layerOneOverlayX, text_layerOneOverlayY, text_layerOneName,
                text_layerTwoPhys, text_layerTwoDrawn, text_layerTwoBias, text_layerTwoAddVar, text_layerTwoLWR, text_layerTwoCDU,
                text_layerTwoOverlayX, text_layerTwoOverlayY, text_layerTwoName;
        GroupBox groupBox_layerOne1D, groupBox_layerTwo1D, groupBox_results_1D;

        void InitializeTooltips_1D()
        {
            string xOverlay = "X overlay in nm";
            text_layerOneOverlayX.ToolTip = xOverlay;
            lbl_layerOneOverlayX.ToolTip = xOverlay;
            text_layerTwoOverlayX.ToolTip = xOverlay;
            lbl_layerTwoOverlayX.ToolTip = xOverlay;

            string yOverlay = "Y overlay in nm";
            text_layerOneOverlayY.ToolTip = yOverlay;
            lbl_layerOneOverlayY.ToolTip = yOverlay;
            text_layerTwoOverlayY.ToolTip = yOverlay;
            lbl_layerTwoOverlayY.ToolTip = yOverlay;

            string CDU = "CDU (per shape) in nm";
            text_layerOneCDU.ToolTip = CDU;
            lbl_layerOneCDU.ToolTip = CDU;
            text_layerTwoCDU.ToolTip = CDU;
            lbl_layerTwoCDU.ToolTip = CDU;

            string LWR = "LWR (not LER!) in nm";
            text_layerOneLWR.ToolTip = LWR;
            lbl_layerOneLWR.ToolTip = LWR;
            text_layerTwoLWR.ToolTip = LWR;
            lbl_layerTwoLWR.ToolTip = LWR;

            string miscVar = "Other variation in nm";
            text_layerOneAddVar.ToolTip = miscVar;
            lbl_layerOneAddVar.ToolTip = miscVar;
            text_layerTwoAddVar.ToolTip = miscVar;
            lbl_layerTwoAddVar.ToolTip = miscVar;

            string bias = "Bias in nm";
            text_layerOneBias.ToolTip = bias;
            lbl_layerOneBias.ToolTip = bias;
            text_layerTwoBias.ToolTip = bias;
            lbl_layerTwoBias.ToolTip = bias;

            string drawn = "Drawn width in nm";
            text_layerOneDrawn.ToolTip = drawn;
            lbl_layerOneDrawn.ToolTip = drawn;
            text_layerTwoDrawn.ToolTip = drawn;
            lbl_layerTwoDrawn.ToolTip = drawn;

            string minins = "Minimum insulator in nm";
            text_minins.ToolTip = minins;
            lbl_minins.ToolTip = minins;

            string nSigma = "Desired sigma value";
            input_nsigma1D.ToolTip = nSigma;
        }

        void oneDTabSetup()
        {
            // 1D tab
            tab_1DCalc_table = new TableLayout();

            tab_1DCalc.Content = new Scrollable { Content = TableLayout.AutoSized(tab_1DCalc_table, centered: true) };

            tab_1DCalc_table.Rows.Add(new TableRow());
            lbl_1D_Guidance = new Label();
            lbl_1D_Guidance.Text = "All inputs should be 3-sigma values";
            lbl_1D_Guidance.Width = oneDGuidanceWidth;
            tab_1DCalc_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(lbl_1D_Guidance, centered: true) });

            Panel oneDLayers_pnl = new Panel();
            tab_1DCalc_table.Rows.Add(new TableRow());
            tab_1DCalc_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(oneDLayers_pnl, centered: true) });

            oneDLayers_setup(ref oneDLayers_pnl);

            Panel minIns_pnl = new Panel();
            tab_1DCalc_table.Rows.Add(new TableRow());
            tab_1DCalc_table.Rows[^1].Cells.Add(new TableCell { Control = minIns_pnl });

            TableLayout minIns_table = new TableLayout();
            minIns_pnl.Content = TableLayout.AutoSized(minIns_table, centered: true);
            minIns_table.Rows.Add(new TableRow());

            lbl_minins = new Label();
            lbl_minins.Width = oneDMinInsLblWidth;
            lbl_minins.Text = "Minimum Insulator";
            minIns_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_minins });

            text_minins = new TextBox();
            text_minins.Width = oneDMinInsWidth;
            minIns_table.Rows[^1].Cells.Add(new TableCell { Control = text_minins });
            text_minins.LostFocus += oneDEventHandler;
            minIns_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding.

            // Image
            Panel oneDImagePanel = new Panel();
            ImageView oneDImage = new ImageView();
            oneDImage.Image = images.oneDImage();
            oneDImagePanel.Size = oneDImage.Image.Size;
            oneDImagePanel.Content = oneDImage;
            tab_1DCalc_table.Rows.Add(new TableRow());
            tab_1DCalc_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(oneDImagePanel, centered: true) });

            tab_1DCalc_table.Rows.Add(new TableRow());

            Panel results_panel = new Panel();
            TableLayout results_tl = new TableLayout();
            results_panel.Content = results_tl;
            results_tl.Rows.Add(new TableRow());

            groupBox_results_1D = new GroupBox();
            results_tl.Rows[0].Cells.Add(new TableCell { Control = groupBox_results_1D });
            results_tl.Rows[0].Cells.Add(new TableCell { Control = null });

            tab_1DCalc_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(results_panel, centered: true) });

            groupBox_results_1D.Text = "RESULTS";

            TableLayout groupBox_results_1D_table = new TableLayout();
            groupBox_results_1D.Content = groupBox_results_1D_table;

            groupBox_results_1D_table.Rows.Add(new TableRow());

            Panel left0 = new Panel();
            Panel right0 = new Panel();
            groupBox_results_1D_table.Rows[^1].Cells.Add(new TableCell { Control = left0 });
            groupBox_results_1D_table.Rows[^1].Cells.Add(new TableCell { Control = right0 });

            TableLayout l0_tl = new TableLayout();
            left0.Content = l0_tl;

            l0_tl.Rows.Add(new TableRow());

            Label lbl_threeSigmaEdgeTol1D_prefix = new Label();
            lbl_threeSigmaEdgeTol1D_prefix.Text = "3";

            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_threeSigmaEdgeTol1D_prefix });

            lbl_threeSigmaEdgeTol1D = new Label();
            lbl_threeSigmaEdgeTol1D.Text = "Sigma Edge Tolerance";

            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_threeSigmaEdgeTol1D });

            text_threeSigmaEdgeTol1D = new TextBox();
            text_threeSigmaEdgeTol1D.Width = 100;
            text_threeSigmaEdgeTol1D.ReadOnly = true;
            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_threeSigmaEdgeTol1D });

            l0_tl.Rows.Add(new TableRow());

            Label lbl_fourSigmaEdgeTol1D_prefix = new Label();
            lbl_fourSigmaEdgeTol1D_prefix.Text = "4";

            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_fourSigmaEdgeTol1D_prefix });

            lbl_fourSigmaEdgeTol1D = new Label();
            lbl_fourSigmaEdgeTol1D.Text = "Sigma Edge Tolerance";

            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_fourSigmaEdgeTol1D });

            text_fourSigmaEdgeTol1D = new TextBox();
            text_fourSigmaEdgeTol1D.Width = 100;
            text_fourSigmaEdgeTol1D.ReadOnly = true;
            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_fourSigmaEdgeTol1D });

            l0_tl.Rows.Add(new TableRow());

            input_nsigma1D = new TextBox();
            input_nsigma1D.Text = "";
            input_nsigma1D.Width = 20;
            input_nsigma1D.TextChanged += oneDEventHandler;

            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = input_nsigma1D });

            lbl_sigmaEdgeTol1D = new Label();
            lbl_sigmaEdgeTol1D.Text = "Sigma Edge Tolerance";

            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_sigmaEdgeTol1D });

            text_nSigmaEdgeTol1D = new TextBox();
            text_nSigmaEdgeTol1D.Width = 100;
            text_nSigmaEdgeTol1D.ReadOnly = true;
            l0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_nSigmaEdgeTol1D });


            TableLayout r0_tl = new TableLayout();
            right0.Content = r0_tl;

            r0_tl.Rows.Add(new TableRow());

            Label lbl_threeSigmaRule1D_prefix = new Label();
            lbl_threeSigmaRule1D_prefix.Text = "3";
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_threeSigmaRule1D_prefix });

            lbl_threeSigmaRule1D = new Label();
            lbl_threeSigmaRule1D.Text = "Sigma Rule";
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_threeSigmaRule1D });

            text_threeSigmaRule1D = new TextBox();
            text_threeSigmaRule1D.Width = 100;
            text_threeSigmaRule1D.ReadOnly = true;
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_threeSigmaRule1D });

            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            r0_tl.Rows.Add(new TableRow());

            Label lbl_fourSigmaRule1D_prefix = new Label();
            lbl_fourSigmaRule1D_prefix.Text = "3";
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_fourSigmaRule1D_prefix });

            lbl_fourSigmaRule1D = new Label();
            lbl_fourSigmaRule1D.Text = "Sigma Rule";
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_fourSigmaRule1D });

            text_fourSigmaRule1D = new TextBox();
            text_fourSigmaRule1D.Width = 100;
            text_fourSigmaRule1D.ReadOnly = true;
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_fourSigmaRule1D });

            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            r0_tl.Rows.Add(new TableRow());

            Label lbl_nSigmaRule1D_prefix = new Label();
            lbl_nSigmaRule1D_prefix.Text = "n";
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_nSigmaRule1D_prefix });

            lbl_nSigmaRule1D = new Label();
            lbl_nSigmaRule1D.Text = "Sigma Rule";
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_nSigmaRule1D });

            text_nSigmaRule1D = new TextBox();
            text_nSigmaRule1D.Width = 100;
            text_nSigmaRule1D.ReadOnly = true;
            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_nSigmaRule1D });

            r0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

            InitializeTooltips_1D();
        }

        void oneDLayers_setup(ref Panel oneDLayers_pnl)
        {
            TableLayout oneDLayers_table = new TableLayout();
            oneDLayers_pnl.Content = oneDLayers_table;
            oneDLayers_table.Rows.Add(new TableRow());

            // Layer 1 group box for 1D.
            groupBox_layerOne1D = new GroupBox();
            groupBox_layerOne1D.Text = "LAYER #1";
            TableLayout groupBox_layerOne1D_table = new TableLayout();
            groupBox_layerOne1D.Content = groupBox_layerOne1D_table;
            oneDLayers_table.Rows[0].Cells.Add(new TableCell { Control = groupBox_layerOne1D });

            // Layer 2 group box for 1D.
            groupBox_layerTwo1D = new GroupBox();
            groupBox_layerTwo1D.Text = "LAYER #2";
            TableLayout groupBox_layerTwo1D_table = new TableLayout();
            groupBox_layerTwo1D.Content = groupBox_layerTwo1D_table;
            oneDLayers_table.Rows[0].Cells.Add(new TableCell { Control = groupBox_layerTwo1D });

            oneDLayer1_setup(groupBox_layerOne1D_table);

            oneDLayer2_setup(groupBox_layerTwo1D_table);

            oneDLayers_table.Rows[0].Cells.Add(new TableCell { Control = null }); // Padding

        }

        void oneDLayer1_setup(TableLayout groupBox_layerOne1D_table)
        {
            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneName = new Label();
            lbl_layerOneName.Width = oneDLabelWidth;
            lbl_layerOneName.Text = "Name";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneName, ScaleWidth = true });

            text_layerOneName = new TextBox();
            text_layerOneName.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneName });

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneOverlayX = new Label();
            lbl_layerOneOverlayX.Width = oneDLabelWidth;
            lbl_layerOneOverlayX.Text = "Overlay X";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneOverlayX, ScaleWidth = true });

            text_layerOneOverlayX = new TextBox();
            text_layerOneOverlayX.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneOverlayX });
            text_layerOneOverlayX.LostFocus += oneDEventHandler;

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneOverlayY = new Label();
            lbl_layerOneOverlayY.Width = oneDLabelWidth;
            lbl_layerOneOverlayY.Text = "Overlay Y";

            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneOverlayY, ScaleWidth = true });

            text_layerOneOverlayY = new TextBox();
            text_layerOneOverlayY.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneOverlayY });
            text_layerOneOverlayY.LostFocus += oneDEventHandler;

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneCDU = new Label();
            lbl_layerOneCDU.Width = oneDLabelWidth;
            lbl_layerOneCDU.Text = "CDU";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneCDU, ScaleWidth = true });

            text_layerOneCDU = new TextBox();
            text_layerOneCDU.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneCDU });
            text_layerOneCDU.LostFocus += oneDEventHandler;

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneLWR = new Label();
            lbl_layerOneLWR.Width = oneDLabelWidth;
            lbl_layerOneLWR.Text = "LWR";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneLWR, ScaleWidth = true });

            text_layerOneLWR = new TextBox();
            text_layerOneLWR.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneLWR });
            text_layerOneLWR.LostFocus += oneDEventHandler;

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneAddVar = new Label();
            lbl_layerOneAddVar.Width = oneDLabelWidth;
            lbl_layerOneAddVar.Text = "Other variation";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneAddVar, ScaleWidth = true });

            text_layerOneAddVar = new TextBox();
            text_layerOneAddVar.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneAddVar });
            text_layerOneAddVar.LostFocus += oneDEventHandler;

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneBias = new Label();
            lbl_layerOneBias.Width = oneDLabelWidth;
            lbl_layerOneBias.Text = "Bias";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneBias, ScaleWidth = true });

            text_layerOneBias = new TextBox();
            text_layerOneBias.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneBias });
            text_layerOneBias.LostFocus += oneDEventHandler;

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOneDrawn = new Label();
            lbl_layerOneDrawn.Width = oneDLabelWidth;
            lbl_layerOneDrawn.Text = "Drawn";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOneDrawn, ScaleWidth = true });

            text_layerOneDrawn = new TextBox();
            text_layerOneDrawn.Width = 100;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOneDrawn });
            text_layerOneDrawn.LostFocus += oneDEventHandler;

            groupBox_layerOne1D_table.Rows.Add(new TableRow());

            lbl_layerOnePhys = new Label();
            lbl_layerOnePhys.Width = oneDLabelWidth;
            lbl_layerOnePhys.Text = "Physical";
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerOnePhys, ScaleWidth = true });

            text_layerOnePhys = new TextBox();
            text_layerOnePhys.Width = 100;
            text_layerOnePhys.ReadOnly = true;
            groupBox_layerOne1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerOnePhys });
        }

        void oneDLayer2_setup(TableLayout groupBox_layerTwo1D_table)
        {
            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoName = new Label();
            lbl_layerTwoName.Width = oneDLabelWidth;
            lbl_layerTwoName.Text = "Name";
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoName, ScaleWidth = true });

            text_layerTwoName = new TextBox();
            text_layerTwoName.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoName });

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoOverlayX = new Label();
            lbl_layerTwoOverlayX.Width = oneDLabelWidth;
            lbl_layerTwoOverlayX.Text = "Overlay X";

            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoOverlayX, ScaleWidth = true });

            text_layerTwoOverlayX = new TextBox();
            text_layerTwoOverlayX.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoOverlayX });
            text_layerTwoOverlayX.LostFocus += oneDEventHandler;

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoOverlayY = new Label();
            lbl_layerTwoOverlayY.Width = oneDLabelWidth;
            lbl_layerTwoOverlayY.Text = "Overlay Y";

            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoOverlayY, ScaleWidth = true });

            text_layerTwoOverlayY = new TextBox();
            text_layerTwoOverlayY.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoOverlayY });
            text_layerTwoOverlayY.LostFocus += oneDEventHandler;

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoCDU = new Label();
            lbl_layerTwoCDU.Width = oneDLabelWidth;
            lbl_layerTwoCDU.Text = "CDU";
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoCDU, ScaleWidth = true });

            text_layerTwoCDU = new TextBox();
            text_layerTwoCDU.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoCDU });
            text_layerTwoCDU.LostFocus += oneDEventHandler;

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoLWR = new Label();
            lbl_layerTwoLWR.Width = oneDLabelWidth;
            lbl_layerTwoLWR.Text = "LWR";
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoLWR, ScaleWidth = true });

            text_layerTwoLWR = new TextBox();
            text_layerTwoLWR.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoLWR });
            text_layerTwoLWR.LostFocus += oneDEventHandler;

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoAddVar = new Label();
            lbl_layerTwoAddVar.Width = oneDLabelWidth;
            lbl_layerTwoAddVar.Text = "Other variation";
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoAddVar, ScaleWidth = true });

            text_layerTwoAddVar = new TextBox();
            text_layerTwoAddVar.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoAddVar });
            text_layerTwoAddVar.LostFocus += oneDEventHandler;

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoBias = new Label();
            lbl_layerTwoBias.Width = oneDLabelWidth;
            lbl_layerTwoBias.Text = "Bias";
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoBias, ScaleWidth = true });

            text_layerTwoBias = new TextBox();
            text_layerTwoBias.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoBias });
            text_layerTwoBias.LostFocus += oneDEventHandler;

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoDrawn = new Label();
            lbl_layerTwoDrawn.Width = oneDLabelWidth;
            lbl_layerTwoDrawn.Text = "Drawn";
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoDrawn, ScaleWidth = true });

            text_layerTwoDrawn = new TextBox();
            text_layerTwoDrawn.Width = 100;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoDrawn });
            text_layerTwoDrawn.LostFocus += oneDEventHandler;

            groupBox_layerTwo1D_table.Rows.Add(new TableRow());

            lbl_layerTwoPhys = new Label();
            lbl_layerTwoPhys.Width = oneDLabelWidth;
            lbl_layerTwoPhys.Text = "Physical";
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = lbl_layerTwoPhys, ScaleWidth = true });

            text_layerTwoPhys = new TextBox();
            text_layerTwoPhys.Width = 100;
            text_layerTwoPhys.ReadOnly = true;
            groupBox_layerTwo1D_table.Rows[^1].Cells.Add(new TableCell { Control = text_layerTwoPhys });
        }

        void oneDEventHandler(object sender, EventArgs e)
        {
            do1DPhysical();
            do1DCalculation();
        }

        void do1DPhysical()
        {
            int layer1OK1;
            int layer1OK2;
            int layer2OK1;
            int layer2OK2;
            double layer1Bias = 0.0f;
            double layer1Drawn = 0.0f;
            double layer2Bias = 0.0f;
            double layer2Drawn = 0.0f;
            try
            {
                layer1Bias = double.Parse(text_layerOneBias.Text);
                layer1OK1 = 1;
            }
            catch (ArgumentNullException)
            {
                layer1OK1 = 0;
            }
            catch (FormatException)
            {
                layer1OK1 = 0;
            }
            try
            {
                layer1Drawn = double.Parse(text_layerOneDrawn.Text);
                layer1OK2 = 1;
            }
            catch (ArgumentNullException)
            {
                layer1OK2 = 0;
            }
            catch (FormatException)
            {
                layer1OK2 = 0;
            }
            try
            {
                layer2Bias = double.Parse(text_layerTwoBias.Text);
                layer2OK1 = 1;
            }
            catch (ArgumentNullException)
            {
                layer2OK1 = 0;
            }
            catch (FormatException)
            {
                layer2OK1 = 0;
            }
            try
            {
                layer2Drawn = double.Parse(text_layerTwoDrawn.Text);
                layer2OK2 = 1;
            }
            catch (ArgumentNullException)
            {
                layer2OK2 = 0;
            }
            catch (FormatException)
            {
                layer2OK2 = 0;
            }
            double physicalValueLayer1 = (layer1OK2 * layer1Drawn) + (layer1OK1 * layer1Bias);
            double physicalValueLayer2 = (layer2OK2 * layer2Drawn) + (layer2OK1 * layer2Bias);

            if (physicalValueLayer1 != 0.0f)
            {
                text_layerOnePhys.Text = physicalValueLayer1.ToString();
            }
            else
            {
                text_layerOnePhys.Text = "";
            }
            if (physicalValueLayer2 != 0.0f)
            {
                text_layerTwoPhys.Text = physicalValueLayer2.ToString();
            }
            else
            {
                text_layerTwoPhys.Text = "";
            }
        }

        void do1DCalculation()
        {
            Boolean inputValid = true;
            Boolean nSigmaReq;

            // Take our inputs and validate the conversion to double in each case.
            // No magic here.\
            double nSigmaValue = 0.0f;
            double layer1OverlayX = 0.0f;
            double layer1OverlayY = 0.0f;
            double layer2OverlayX = 0.0f;
            double layer2OverlayY = 0.0f;
            double layer1CDU = 0.0f;
            double layer1LWR = 0.0f;
            double layer2CDU = 0.0f;
            double layer2LWR = 0.0f;
            double layer1Bias = 0.0f;
            double layer2Bias = 0.0f;
            double layer1AddVar = 0.0f;
            double layer2AddVar = 0.0f;
            double minIns = 0.0f;

            try
            {
                nSigmaValue = double.Parse(input_nsigma1D.Text);
                nSigmaReq = true;
            }
            catch (ArgumentNullException)
            {
                nSigmaReq = false;
            }
            catch (FormatException)
            {
                nSigmaReq = false;
            }

            try
            {
                layer1OverlayX = double.Parse(text_layerOneOverlayX.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }
            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer1OverlayY = double.Parse(text_layerOneOverlayY.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }
            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer1CDU = double.Parse(text_layerOneCDU.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }
            try
            {
                layer1LWR = double.Parse(text_layerOneLWR.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }
            if (!inputValid)
            {
                reset1DFields();
                return;
            }
            try
            {
                layer1AddVar = double.Parse(text_layerOneAddVar.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer1Bias = double.Parse(text_layerOneBias.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer2OverlayX = double.Parse(text_layerTwoOverlayX.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer2OverlayY = double.Parse(text_layerTwoOverlayY.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer2CDU = double.Parse(text_layerTwoCDU.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer2LWR = double.Parse(text_layerTwoLWR.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer2AddVar = double.Parse(text_layerTwoAddVar.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                layer2Bias = double.Parse(text_layerTwoBias.Text);
            }
            catch (ArgumentNullException)
            {
                inputValid = false;
            }
            catch (FormatException)
            {
                inputValid = false;
            }

            if (!inputValid)
            {
                reset1DFields();
                return;
            }

            try
            {
                minIns = double.Parse(text_minins.Text);
            }
            catch (ArgumentNullException)
            {
            }
            catch (FormatException)
            {
            }


            // OK - if we have invalid input, let's blank our output targets.
            if (inputValid)
            {
                double threeSigma = UtilityFuncs.do1DCalculation_3sig(
                    layer1OverlayX, layer1OverlayY, layer1CDU, layer1LWR, layer1Bias, layer1AddVar,
                    layer2OverlayX, layer2OverlayY, layer2CDU, layer2LWR, layer2Bias, layer2AddVar, minIns);
                double threeSigmaEdge = UtilityFuncs.do1DCalculation_3sig_edge(
                    layer1OverlayX, layer1OverlayY, layer1CDU, layer1LWR, layer1AddVar,
                    layer2OverlayX, layer2OverlayY, layer2CDU, layer2LWR, layer2AddVar);
                text_threeSigmaRule1D.Text = threeSigma.ToString("0.##");
                text_threeSigmaEdgeTol1D.Text = threeSigmaEdge.ToString("0.##");

                double fourSigma = UtilityFuncs.do1DCalculation_4sig(
                    layer1OverlayX, layer1OverlayY, layer1CDU, layer1LWR, layer1Bias, layer1AddVar,
                    layer2OverlayX, layer2OverlayY, layer2CDU, layer2LWR, layer2Bias, layer2AddVar, minIns);
                double fourSigmaEdge = (4 * threeSigmaEdge) / 3;

                text_fourSigmaRule1D.Text = fourSigma.ToString("0.##");
                text_fourSigmaEdgeTol1D.Text = fourSigmaEdge.ToString("0.##");
                if (nSigmaReq)
                {
                    double nSigma = UtilityFuncs.do1DCalculation_nsig(nSigmaValue,
                    layer1OverlayX, layer1OverlayY, layer1CDU, layer1LWR, layer1Bias, layer1AddVar,
                    layer2OverlayX, layer2OverlayY, layer2CDU, layer2LWR, layer2Bias, layer2AddVar, minIns);
                    double nSigmaEdge = (nSigmaValue * threeSigmaEdge) / 3;
                    text_nSigmaRule1D.Text = nSigma.ToString("0.##");
                    text_nSigmaEdgeTol1D.Text = nSigmaEdge.ToString("0.##");
                }
                else
                {
                    text_nSigmaRule1D.Text = "";
                }
            }
            else
            {
                reset1DFields();
            }

        }

        void reset1DFields()
        {
            text_threeSigmaRule1D.Text = "";
            text_fourSigmaRule1D.Text = "";
            text_nSigmaRule1D.Text = "";
            text_threeSigmaEdgeTol1D.Text = "";
            text_fourSigmaEdgeTol1D.Text = "";
            text_nSigmaEdgeTol1D.Text = "";
        }
    }
}