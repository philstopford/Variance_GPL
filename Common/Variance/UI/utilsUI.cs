using System;
using entropyRNG;
using Error;
using Eto.Forms;
using info.lundin.math;

namespace Variance;

public partial class MainForm : Form
{
    private TextBox text_server, text_emailAddress, text_rngMapping;

    private NumericStepper num_port, num_zoomSpeed, num_fgOpacity, num_bgOpacity;

    private PasswordBox text_emailPwd;

    private CheckBox checkBox_EmailCompletion, checkBox_perJob, checkBox_SSL,
        checkBox_geoCore_enableCDVariation, checkBox_geoCore_tileLayerPreview,
        checkBox_OGLAA, checkBox_OGLFill, checkBox_OGLPoints,
        checkBox_friendlyNumbers;

    private Button button_emailTest, utilitiesBtn_Summary, btn_resetColors, btn_rngMapping_Add, btn_rngMapping_Edit, btn_rngMapping_Remove;

    private Label lbl_Layer1Color, lbl_Layer2Color, lbl_Layer3Color, lbl_Layer4Color,
        lbl_Layer5Color, lbl_Layer6Color, lbl_Layer7Color, lbl_Layer8Color,
        lbl_Layer9Color, lbl_Layer10Color, lbl_Layer11Color, lbl_Layer12Color,
        lbl_Layer13Color, lbl_Layer14Color, lbl_Layer15Color, lbl_Layer16Color,
        lbl_Result1Color, lbl_Result2Color, lbl_Result3Color, lbl_Result4Color,
        lbl_ss1Color, lbl_ss2Color, lbl_ss3Color,
        lbl_implantMinColor, lbl_implantMeanColor, lbl_implantMaxColor, lbl_implantResistColor,
        lbl_enabledColor, lbl_majorGridColor, lbl_minorGridColor, lbl_axisColor, lbl_vpbgColor, 
        lbl_Layer1Color_name, lbl_Layer2Color_name, lbl_Layer3Color_name, lbl_Layer4Color_name,
        lbl_Layer5Color_name, lbl_Layer6Color_name, lbl_Layer7Color_name, lbl_Layer8Color_name,
        lbl_Layer9Color_name, lbl_Layer10Color_name, lbl_Layer11Color_name, lbl_Layer12Color_name,
        lbl_Layer13Color_name, lbl_Layer14Color_name, lbl_Layer15Color_name, lbl_Layer16Color_name,
        lbl_Result1Color_name, lbl_Result2Color_name, lbl_Result3Color_name, lbl_Result4Color_name,
        lbl_ss1Color_name, lbl_ss2Color_name, lbl_ss3Color_name,
        lbl_implantMinColor_name, lbl_implantMeanColor_name, lbl_implantMaxColor_name, lbl_implantResistColor_name,
        lbl_enabledColor_name, lbl_majorGridColor_name, lbl_minorGridColor_name, lbl_axisColor_name, lbl_vpbgColor_name, 
        lbl_emailAddress, lbl_emailPwd, lbl_server, lbl_port, lbl_zoomSpeed, lbl_fgOpacity, lbl_bgOpacity;

    private ListBox listBox_rngCustomMapping;

    private GroupBox groupBox_utilities, groupBox_settings, groupBox_email, groupBox_openGL, prefs_geoCore, groupBox_rng, groupBox_misc;

    private void utilsUISetup()
    {
        settings_utilsUISetup();
    }

    private void utils_utilsUISetup(TableCell tc)
    {
        TableLayout groupBox_utilities_table = new();
        groupBox_utilities = new GroupBox {Text = "Utilities", Content = groupBox_utilities_table};

        groupBox_utilities_table.Rows.Add(new TableRow());

        utilitiesBtn_Summary = new Button {Text = "Create summary of DOE results"};
        groupBox_utilities_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(utilitiesBtn_Summary) });
        utilitiesBtn_Summary.Click += locateDOEResults;

        tc.Control = groupBox_utilities;
    }

    private void settings_utilsUISetup()
    {
        TableLayout groupBox_settings_table = new();
        groupBox_settings = new GroupBox {Text = "Settings", Content = groupBox_settings_table};

        tabPage_utilities_table.Rows.Add(new TableRow());
        tabPage_utilities_table.Rows[^1].Cells.Add(new TableCell { Control = groupBox_settings });
        tabPage_utilities_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding;

        tabPage_utilities_table.Rows.Add(new TableRow()); // padding

        groupBox_settings_table.Rows.Add(new TableRow());

        TableLayout row0tl = new();
        Panel row0Panel = new() {Content = row0tl};
        groupBox_settings_table.Rows[^1].Cells.Add(new TableCell { Control = row0Panel });

        row0tl.Rows.Add(new TableRow());
        TableCell settingsTC = new();
        row0tl.Rows[^1].Cells.Add(settingsTC);
        row0tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        utils_utilsUISetup(settingsTC);

        groupBox_settings_table.Rows.Add(new TableRow());
        TableLayout row1tl = new();
        Panel row1Panel = new() {Content = row1tl};
        groupBox_settings_table.Rows[^1].Cells.Add(new TableCell { Control = row1Panel });

        row1tl.Rows.Add(new TableRow());

        TableCell leftTC = new();
        TableLayout leftTL = new();
        Panel leftPanel = new() {Content = leftTL};
        leftTC.Control = leftPanel;
        row1tl.Rows[^1].Cells.Add(leftTC);

        TableCell rightTC = new();
        TableLayout rightTL = new();
        Panel rightPanel = new() {Content = rightTL};
        rightTC.Control = rightPanel;
        row1tl.Rows[^1].Cells.Add(rightTC);

        row1tl.Rows[^1].Cells.Add(new TableCell { Control = null }); //padding

        // Left-hand side.
        leftTL.Rows.Add(new TableRow());
        TableCell lRow0 = new();
        leftTL.Rows[^1].Cells.Add(lRow0);
        leftTL.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
        email_settings_utilsUISetup(lRow0);

        leftTL.Rows.Add(new TableRow());
        TableCell lRow1 = new();
        leftTL.Rows[^1].Cells.Add(lRow1);
        leftTL.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
        geoCore_settings_utilsUISetup(lRow1);

        leftTL.Rows.Add(new TableRow());
        TableCell lRow2 = new();
        leftTL.Rows[^1].Cells.Add(lRow2);
        leftTL.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
        rng_customMappingSetup(lRow2);

        leftTL.Rows.Add(new TableRow()); // padding

        rightTL.Rows.Add(new TableRow());
        TableCell rRow0 = new();
        rightTL.Rows[^1].Cells.Add(rRow0);
        rightTL.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
        openGL_settings_utilsUISetup(rRow0);

        rightTL.Rows.Add(new TableRow());
        TableCell rRow1 = new();
        rightTL.Rows[^1].Cells.Add(rRow1);
        rightTL.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
        misc_settings_utilsUISetup(rRow1);

        groupBox_settings_table.Rows.Add(new TableRow()); // padding
    }

    private void email_settings_utilsUISetup(TableCell tc)
    {
        TableLayout groupBox_email_table = new();
        groupBox_email = new GroupBox {Text = "Email Settings", Content = groupBox_email_table};

        tc.Control = groupBox_email;

        groupBox_email_table.Rows.Add(new TableRow());

        TableLayout row0tl = new();
        Panel row0 = new() {Content = row0tl};
        groupBox_email_table.Rows[^1].Cells.Add(new TableCell { Control = row0 });

        row0tl.Rows.Add(new TableRow());

        lbl_emailAddress = new Label {Text = "Address"};
        row0tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_emailAddress });

        text_emailAddress = new TextBox();
        row0tl.Rows[^1].Cells.Add(new TableCell { Control = text_emailAddress });

        groupBox_email_table.Rows.Add(new TableRow());

        TableLayout row1tl = new();
        Panel row1 = new() {Content = row1tl};
        groupBox_email_table.Rows[^1].Cells.Add(new TableCell { Control = row1 });

        row1tl.Rows.Add(new TableRow());

        lbl_emailPwd = new Label {Text = "Password"};
        row1tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_emailPwd });

        text_emailPwd = new PasswordBox();
        row1tl.Rows[^1].Cells.Add(new TableCell { Control = text_emailPwd });

        groupBox_email_table.Rows.Add(new TableRow());

        TableLayout row2tl = new();
        Panel row2 = new() {Content = row2tl};
        groupBox_email_table.Rows[^1].Cells.Add(new TableCell { Control = row2 });

        row2tl.Rows.Add(new TableRow());

        lbl_server = new Label {Text = "Server"};
        row2tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_server });

        text_server = new TextBox();
        row2tl.Rows[^1].Cells.Add(new TableCell { Control = text_server });

        groupBox_email_table.Rows.Add(new TableRow());

        TableLayout row3tl = new();
        Panel row3 = new() {Content = row3tl};
        groupBox_email_table.Rows[^1].Cells.Add(new TableCell { Control = row3 });

        row3tl.Rows.Add(new TableRow());

        checkBox_SSL = new CheckBox {Text = "SSL"};
        row3tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_SSL });

        lbl_port = new Label {Text = "Port"};
        row3tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_port });

        num_port = new NumericStepper {MinValue = 1, Value = 587, Increment = 1};
        setSize(num_port, 70);
        row3tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_port) });

        row3tl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_email_table.Rows.Add(new TableRow());

        TableLayout row4tl = new();
        Panel row4 = new() {Content = row4tl};
        groupBox_email_table.Rows[^1].Cells.Add(new TableCell { Control = row4 });

        row4tl.Rows.Add(new TableRow());

        checkBox_EmailCompletion = new CheckBox {Text = "On Completion"};
        row4tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_EmailCompletion });

        checkBox_perJob = new CheckBox {Text = "Per Job"};
        row4tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_perJob });

        button_emailTest = new Button {Text = "Test"};
        row4tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(button_emailTest) });

        row4tl.Rows[^1].Cells.Add(new TableCell { Control = null });
    }

    private void geoCore_settings_utilsUISetup(TableCell tc)
    {
        TableLayout prefs_geoCore_table = new();
        prefs_geoCore = new GroupBox {Text = "GDS/Oasis", Content = prefs_geoCore_table};
        tc.Control = prefs_geoCore;

        prefs_geoCore_table.Rows.Add(new TableRow());
        checkBox_geoCore_enableCDVariation = new CheckBox {Text = "Allow CD/bias variation"};
        prefs_geoCore_table.Rows[^1].Cells.Add(new TableCell { Control = checkBox_geoCore_enableCDVariation });
        prefs_geoCore_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        prefs_geoCore_table.Rows.Add(new TableRow());
        checkBox_geoCore_tileLayerPreview = new CheckBox {Text = "Use DOE tile for layer preview"};
        prefs_geoCore_table.Rows[^1].Cells.Add(new TableCell { Control = checkBox_geoCore_tileLayerPreview });
        prefs_geoCore_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
    }

    private void misc_settings_utilsUISetup(TableCell tc)
    {
        TableLayout tl = new();
        Panel p = new() {Content = tl};
        tc.Control = p;

        tl.Rows.Add(new TableRow());
        TableCell tc0 = new();
        tl.Rows[0].Cells.Add(tc0);
        tl.Rows.Add(new TableRow()); // padding

        TableLayout groupBox_misc_table = new();
        groupBox_misc = new GroupBox {Text = "Misc", Content = groupBox_misc_table};
        tc0.Control = groupBox_misc;

        groupBox_misc_table.Rows.Add(new TableRow());
        TableCell row0 = new();
        groupBox_misc_table.Rows[^1].Cells.Add(row0);
        miscRow0(row0);
    }

    private void miscRow0(TableCell tc)
    {
        TableLayout tl = new();
        tl.Rows.Add(new TableRow());
        Panel p = new() {Content = tl};
        tc.Control = p;

        tl.Rows.Add(new TableRow());

        checkBox_friendlyNumbers = new CheckBox {Text = "Friendly Numbers"};
        tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_friendlyNumbers });

        tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        tl.Rows.Add(new TableRow()); // padding.
    }

    private void openGL_settings_utilsUISetup(TableCell tc)
    {
        TableLayout tl = new();
        Panel p = new() {Content = tl};
        tc.Control = p;

        tl.Rows.Add(new TableRow());
        TableCell tc0 = new();
        tl.Rows[0].Cells.Add(tc0);
        tl.Rows.Add(new TableRow()); // padding

        TableLayout groupBox_openGL_table = new();
        groupBox_openGL = new GroupBox {Text = "OpenGL", Content = groupBox_openGL_table};
        tc0.Control = groupBox_openGL;

        groupBox_openGL_table.Rows.Add(new TableRow());
        TableCell row0 = new();
        groupBox_openGL_table.Rows[^1].Cells.Add(row0);
        openGLRow0(row0);

        groupBox_openGL_table.Rows.Add(new TableRow());
        TableCell row1 = new();
        groupBox_openGL_table.Rows[^1].Cells.Add(row1);
        openGLRow1(row1);

        groupBox_openGL_table.Rows.Add(new TableRow());
        TableCell row2 = new();
        groupBox_openGL_table.Rows[^1].Cells.Add(row2);
        openGLRow2(row2);
    }

    private void openGLRow0(TableCell tc)
    {
        TableLayout tl = new();
        tl.Rows.Add(new TableRow());

        Panel p = new() {Content = tl};
        tc.Control = p;

        Panel lRow0 = new();
        tl.Rows[^1].Cells.Add(new TableCell { Control = lRow0 });

        Panel rRow0 = new();
        tl.Rows[^1].Cells.Add(new TableCell { Control = rRow0 });

        TableLayout lRow0tl = new();
        lRow0.Content = lRow0tl;
        lRow0tl.Rows.Add(new TableRow());

        TableLayout rRow0tl = new();
        rRow0.Content = rRow0tl;
        rRow0tl.Rows.Add(new TableRow());

        TableLayout zoomTL = new();
        Panel zoomPnl = new() {Content = zoomTL};
        lRow0tl.Rows[^1].Cells.Add(new TableCell { Control = zoomPnl });

        zoomTL.Rows.Add(new TableRow());

        lbl_zoomSpeed = new Label {Text = "Zoom Increment"};
        zoomTL.Rows[^1].Cells.Add(new TableCell { Control = lbl_zoomSpeed });

        num_zoomSpeed = new NumericStepper {MinValue = 1, Increment = 1};
        setSize(num_zoomSpeed, 50);
        zoomTL.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_zoomSpeed) });

        zoomTL.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        TableLayout fgOpTL = new();
        Panel fgOpPnl = new() {Content = fgOpTL};
        rRow0tl.Rows[^1].Cells.Add(new TableCell { Control = fgOpPnl });

        fgOpTL.Rows.Add(new TableRow());

        lbl_fgOpacity = new Label {Text = "FG"};
        fgOpTL.Rows[^1].Cells.Add(new TableCell { Control = lbl_fgOpacity });

        num_fgOpacity = new NumericStepper {MinValue = 0.01f, Increment = 0.1f, DecimalPlaces = 2};
        setSize(num_fgOpacity, 50);
        fgOpTL.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_fgOpacity) });

        TableLayout bgOpTL = new();
        Panel bgOpPnl = new() {Content = bgOpTL};
        rRow0tl.Rows[^1].Cells.Add(new TableCell { Control = bgOpPnl });

        bgOpTL.Rows.Add(new TableRow());

        lbl_bgOpacity = new Label {Text = "BG"};
        bgOpTL.Rows[^1].Cells.Add(new TableCell { Control = lbl_bgOpacity });

        num_bgOpacity = new NumericStepper {MinValue = 0.01f, Increment = 0.1f, DecimalPlaces = 2};
        setSize(num_bgOpacity, 50);
        bgOpTL.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_bgOpacity) });

        tl.Rows[^1].Cells.Add(new TableCell { Control = null });

        tl.Rows.Add(new TableRow());

        Panel lRow1 = new();
        tl.Rows[^1].Cells.Add(new TableCell { Control = lRow1 });

        Panel rRow1 = new();
        tl.Rows[^1].Cells.Add(new TableCell { Control = rRow1 });

        TableLayout lRow1tl = new();
        lRow1.Content = lRow1tl;
        lRow1tl.Rows.Add(new TableRow());

        TableLayout rRow1tl = new();
        rRow1.Content = rRow1tl;
        rRow1tl.Rows.Add(new TableRow());

        TableLayout dispOptsTL = new();
        Panel dispOptsPnl = new() {Content = dispOptsTL};
        lRow1tl.Rows[^1].Cells.Add(new TableCell { Control = dispOptsPnl });

        dispOptsTL.Rows.Add(new TableRow());

        checkBox_OGLAA = new CheckBox {Text = "Antialiasing"};
        dispOptsTL.Rows[^1].Cells.Add(new TableCell { Control = checkBox_OGLAA });

        checkBox_OGLFill = new CheckBox {Text = "Fill"};
        dispOptsTL.Rows[^1].Cells.Add(new TableCell { Control = checkBox_OGLFill });

        checkBox_OGLPoints = new CheckBox {Text = "Points"};
        dispOptsTL.Rows[^1].Cells.Add(new TableCell { Control = checkBox_OGLPoints });
    }

    private void openGLRow1(TableCell tc)
    {
        Panel p = new();
        tc.Control = p;

        TableLayout swatchesTL = new();
        p.Content = swatchesTL;

        swatchesTL.Rows.Add(new TableRow());

        TableRow row0 = new();
        swatchesTL.Rows.Add(row0);

        opengl_swatchrow0(row0);

        TableRow row1 = new();
        swatchesTL.Rows.Add(row1);

        opengl_swatchrow1(row1);

        TableRow row2 = new();
        swatchesTL.Rows.Add(row2);

        opengl_swatchrow2(row2);

        TableRow row3 = new();
        swatchesTL.Rows.Add(row3);

        opengl_swatchrow3(row3);

        TableRow row4 = new();
        swatchesTL.Rows.Add(row4);

        opengl_swatchrow4(row4);

        TableRow row5 = new();
        swatchesTL.Rows.Add(row5);

        opengl_swatchrow5(row5);

        TableRow row6 = new();
        swatchesTL.Rows.Add(row6);

        opengl_swatchrow6(row6);

        TableRow row7 = new();
        swatchesTL.Rows.Add(row7);

        opengl_swatchrow7(row7);
    }

    private void opengl_swatchrow0(TableRow tr)
    {
        Panel c0 = new();
        tr.Cells.Add(new TableCell { Control = c0 });

        TableLayout c0TL = new();
        c0.Content = c0TL;
        c0TL.Rows.Add(new TableRow());

        lbl_minorGridColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().minor_Color)
        };
        setSize(lbl_minorGridColor, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_minorGridColor);

        lbl_minorGridColor_name = new Label {Text = "Minor Grid"};
        c0TL.Rows[0].Cells.Add(lbl_minorGridColor_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_majorGridColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().major_Color)
        };
        setSize(lbl_majorGridColor, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_majorGridColor);

        lbl_majorGridColor_name = new Label {Text = "Major Grid"};
        c1TL.Rows[0].Cells.Add(lbl_majorGridColor_name);

        Panel c2 = new();
        tr.Cells.Add(new TableCell { Control = c2 });

        TableLayout c2TL = new();
        c2.Content = c2TL;
        c2TL.Rows.Add(new TableRow());

        lbl_axisColor = new Label {BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().axis_Color)};
        setSize(lbl_axisColor, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_axisColor);

        lbl_axisColor_name = new Label {Text = "Axis"};
        c2TL.Rows[0].Cells.Add(lbl_axisColor_name);

        Panel c3 = new();
        tr.Cells.Add(new TableCell { Control = c3 });

        TableLayout c3TL = new();
        c3.Content = c3TL;
        c3TL.Rows.Add(new TableRow());

        lbl_vpbgColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().background_Color)
        };
        setSize(lbl_vpbgColor, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_vpbgColor);

        lbl_vpbgColor_name = new Label {Text = "VP Background"};
        c3TL.Rows[0].Cells.Add(lbl_vpbgColor_name);
    }

    private void opengl_swatchrow1(TableRow tr)
    {
        TableLayout c0TL = new();
        Panel c0 = new() {Content = c0TL};
        tr.Cells.Add(new TableCell { Control = c0 });

        c0TL.Rows.Add(new TableRow());

        lbl_Layer1Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer1_Color)
        };
        setSize(lbl_Layer1Color, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_Layer1Color);

        lbl_Layer1Color_name = new Label {Text = "Layer 1"};
        c0TL.Rows[0].Cells.Add(lbl_Layer1Color_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_Layer2Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer2_Color)
        };
        setSize(lbl_Layer2Color, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_Layer2Color);

        lbl_Layer2Color_name = new Label {Text = "Layer 2"};
        c1TL.Rows[0].Cells.Add(lbl_Layer2Color_name);

        TableLayout c2TL = new();
        Panel c2 = new() {Content = c2TL};
        tr.Cells.Add(new TableCell { Control = c2 });

        c2TL.Rows.Add(new TableRow());

        lbl_Layer3Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer3_Color)
        };
        setSize(lbl_Layer3Color, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_Layer3Color);

        lbl_Layer3Color_name = new Label {Text = "Layer 3"};
        c2TL.Rows[0].Cells.Add(lbl_Layer3Color_name);

        TableLayout c3TL = new();
        Panel c3 = new() {Content = c3TL};
        tr.Cells.Add(new TableCell { Control = c3 });

        c3TL.Rows.Add(new TableRow());

        lbl_Layer4Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer4_Color)
        };
        setSize(lbl_Layer4Color, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_Layer4Color);

        lbl_Layer4Color_name = new Label {Text = "Layer 4"};
        c3TL.Rows[0].Cells.Add(lbl_Layer4Color_name);
    }

    private void opengl_swatchrow2(TableRow tr)
    {
        TableLayout c0TL = new();
        Panel c0 = new() {Content = c0TL};
        tr.Cells.Add(new TableCell { Control = c0 });

        c0TL.Rows.Add(new TableRow());

        lbl_Layer5Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer5_Color)
        };
        setSize(lbl_Layer5Color, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_Layer5Color);

        lbl_Layer5Color_name = new Label {Text = "Layer 5"};
        c0TL.Rows[0].Cells.Add(lbl_Layer5Color_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_Layer6Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer6_Color)
        };
        setSize(lbl_Layer6Color, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_Layer6Color);

        lbl_Layer6Color_name = new Label {Text = "Layer 6"};
        c1TL.Rows[0].Cells.Add(lbl_Layer6Color_name);

        TableLayout c2TL = new();
        Panel c2 = new() {Content = c2TL};
        tr.Cells.Add(new TableCell { Control = c2 });

        c2TL.Rows.Add(new TableRow());

        lbl_Layer7Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer7_Color)
        };
        setSize(lbl_Layer7Color, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_Layer7Color);

        lbl_Layer7Color_name = new Label {Text = "Layer 7"};
        c2TL.Rows[0].Cells.Add(lbl_Layer7Color_name);

        TableLayout c3TL = new();
        Panel c3 = new() {Content = c3TL};
        tr.Cells.Add(new TableCell { Control = c3 });

        c3TL.Rows.Add(new TableRow());

        lbl_Layer8Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer8_Color)
        };
        setSize(lbl_Layer8Color, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_Layer8Color);

        lbl_Layer8Color_name = new Label {Text = "Layer 8"};
        c3TL.Rows[0].Cells.Add(lbl_Layer8Color_name);
    }

    private void opengl_swatchrow3(TableRow tr)
    {
        TableLayout c0TL = new();
        Panel c0 = new() {Content = c0TL};
        tr.Cells.Add(new TableCell { Control = c0 });

        c0TL.Rows.Add(new TableRow());

        lbl_Layer9Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer9_Color)
        };
        setSize(lbl_Layer9Color, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_Layer9Color);

        lbl_Layer9Color_name = new Label {Text = "Layer 9"};
        c0TL.Rows[0].Cells.Add(lbl_Layer9Color_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_Layer10Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer10_Color)
        };
        setSize(lbl_Layer10Color, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_Layer10Color);

        lbl_Layer10Color_name = new Label {Text = "Layer 10"};
        c1TL.Rows[0].Cells.Add(lbl_Layer10Color_name);

        TableLayout c2TL = new();
        Panel c2 = new() {Content = c2TL};
        tr.Cells.Add(new TableCell { Control = c2 });

        c2TL.Rows.Add(new TableRow());

        lbl_Layer11Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer11_Color)
        };
        setSize(lbl_Layer11Color, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_Layer11Color);

        lbl_Layer11Color_name = new Label {Text = "Layer 11"};
        c2TL.Rows[0].Cells.Add(lbl_Layer11Color_name);

        TableLayout c3TL = new();
        Panel c3 = new() {Content = c3TL};
        tr.Cells.Add(new TableCell { Control = c3 });

        c3TL.Rows.Add(new TableRow());

        lbl_Layer12Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer12_Color)
        };
        setSize(lbl_Layer12Color, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_Layer12Color);

        lbl_Layer12Color_name = new Label {Text = "Layer 12"};
        c3TL.Rows[0].Cells.Add(lbl_Layer12Color_name);
    }

    private void opengl_swatchrow4(TableRow tr)
    {
        TableLayout c0TL = new();
        Panel c0 = new() {Content = c0TL};
        tr.Cells.Add(new TableCell { Control = c0 });

        c0TL.Rows.Add(new TableRow());

        lbl_Layer13Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer13_Color)
        };
        setSize(lbl_Layer13Color, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_Layer13Color);

        lbl_Layer13Color_name = new Label {Text = "Layer 13"};
        c0TL.Rows[0].Cells.Add(lbl_Layer13Color_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_Layer14Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer14_Color)
        };
        setSize(lbl_Layer14Color, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_Layer14Color);

        lbl_Layer14Color_name = new Label {Text = "Layer 14"};
        c1TL.Rows[0].Cells.Add(lbl_Layer14Color_name);

        Panel c2 = new();
        tr.Cells.Add(new TableCell { Control = c2 });

        TableLayout c2TL = new();
        c2.Content = c2TL;
        c2TL.Rows.Add(new TableRow());

        lbl_Layer15Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer15_Color)
        };
        setSize(lbl_Layer15Color, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_Layer15Color);

        lbl_Layer15Color_name = new Label {Text = "Layer 15"};
        c2TL.Rows[0].Cells.Add(lbl_Layer15Color_name);

        TableLayout c3TL = new();
        Panel c3 = new() {Content = c3TL};
        tr.Cells.Add(new TableCell { Control = c3 });

        c3TL.Rows.Add(new TableRow());

        lbl_Layer16Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer16_Color)
        };
        setSize(lbl_Layer16Color, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_Layer16Color);

        lbl_Layer16Color_name = new Label {Text = "Layer 16"};
        c3TL.Rows[0].Cells.Add(lbl_Layer16Color_name);
    }

    private void opengl_swatchrow5(TableRow tr)
    {
        TableLayout c0TL = new();
        Panel c0 = new() {Content = c0TL};
        tr.Cells.Add(new TableCell { Control = c0 });

        c0TL.Rows.Add(new TableRow());

        lbl_Result1Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result_Color)
        };
        setSize(lbl_Result1Color, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_Result1Color);

        lbl_Result1Color_name = new Label {Text = "Result 1"};
        c0TL.Rows[0].Cells.Add(lbl_Result1Color_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_Result2Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result2_Color)
        };
        setSize(lbl_Result2Color, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_Result2Color);

        lbl_Result2Color_name = new Label {Text = "Result 2"};
        c1TL.Rows[0].Cells.Add(lbl_Result2Color_name);

        TableLayout c2TL = new();
        Panel c2 = new() {Content = c2TL};
        tr.Cells.Add(new TableCell { Control = c2 });

        c2TL.Rows.Add(new TableRow());

        lbl_Result3Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result3_Color)
        };
        setSize(lbl_Result3Color, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_Result3Color);

        lbl_Result3Color_name = new Label {Text = "Result 3", Width = 80};
        c2TL.Rows[0].Cells.Add(lbl_Result3Color_name);

        TableLayout c3TL = new();
        Panel c3 = new() {Content = c3TL};
        tr.Cells.Add(new TableCell { Control = c3 });

        c3TL.Rows.Add(new TableRow());

        lbl_Result4Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result4_Color)
        };
        setSize(lbl_Result4Color, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_Result4Color);

        lbl_Result4Color_name = new Label {Text = "Result 4"};
        c3TL.Rows[0].Cells.Add(lbl_Result4Color_name);
    }

    private void opengl_swatchrow6(TableRow tr)
    {
        TableLayout c0TL = new();
        Panel c0 = new() {Content = c0TL};
        tr.Cells.Add(new TableCell { Control = c0 });

        c0TL.Rows.Add(new TableRow());

        lbl_enabledColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().enabled_Color)
        };
        setSize(lbl_enabledColor, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_enabledColor);

        lbl_enabledColor_name = new Label {Text = "Enabled"};
        c0TL.Rows[0].Cells.Add(lbl_enabledColor_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_ss1Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().subshape1_Color)
        };
        setSize(lbl_ss1Color, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_ss1Color);

        lbl_ss1Color_name = new Label {Text = "Subshape 1"};
        c1TL.Rows[0].Cells.Add(lbl_ss1Color_name);

        TableLayout c2TL = new();
        Panel c2 = new() {Content = c2TL};
        tr.Cells.Add(new TableCell { Control = c2 });

        c2TL.Rows.Add(new TableRow());

        lbl_ss2Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().subshape2_Color)
        };
        setSize(lbl_ss2Color, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_ss2Color);

        lbl_ss2Color_name = new Label {Text = "Subshape 2"};
        c2TL.Rows[0].Cells.Add(lbl_ss2Color_name);

        TableLayout c3TL = new();
        Panel c3 = new() {Content = c3TL};
        tr.Cells.Add(new TableCell { Control = c3 });

        c3TL.Rows.Add(new TableRow());

        lbl_ss3Color = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().subshape3_Color)
        };
        setSize(lbl_ss3Color, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_ss3Color);

        lbl_ss3Color_name = new Label {Text = "Subshape 3"};
        c3TL.Rows[0].Cells.Add(lbl_ss3Color_name);
    }

    private void opengl_swatchrow7(TableRow tr)
    {
        TableLayout c0TL = new();
        Panel c0 = new() {Content = c0TL};
        tr.Cells.Add(new TableCell { Control = c0 });

        c0TL.Rows.Add(new TableRow());

        lbl_implantMinColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantMin_Color)
        };
        setSize(lbl_implantMinColor, label_Height);
        c0TL.Rows[0].Cells.Add(lbl_implantMinColor);

        lbl_implantMinColor_name = new Label {Text = "Min Shadow"};
        c0TL.Rows[0].Cells.Add(lbl_implantMinColor_name);

        TableLayout c1TL = new();
        Panel c1 = new() {Content = c1TL};
        tr.Cells.Add(new TableCell { Control = c1 });

        c1TL.Rows.Add(new TableRow());

        lbl_implantMeanColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantMean_Color)
        };
        setSize(lbl_implantMeanColor, label_Height);
        c1TL.Rows[0].Cells.Add(lbl_implantMeanColor);

        lbl_implantMeanColor_name = new Label {Text = "Mean Shadow"};
        c1TL.Rows[0].Cells.Add(lbl_implantMeanColor_name);

        TableLayout c2TL = new();
        Panel c2 = new() {Content = c2TL};
        tr.Cells.Add(new TableCell { Control = c2 });

        c2TL.Rows.Add(new TableRow());

        lbl_implantMaxColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantMax_Color)
        };
        setSize(lbl_implantMaxColor, label_Height);
        c2TL.Rows[0].Cells.Add(lbl_implantMaxColor);

        lbl_implantMaxColor_name = new Label {Text = "Max Shadow"};
        c2TL.Rows[0].Cells.Add(lbl_implantMaxColor_name);

        TableLayout c3TL = new();
        Panel c3 = new() {Content = c3TL};
        tr.Cells.Add(new TableCell { Control = c3 });

        c3TL.Rows.Add(new TableRow());

        lbl_implantResistColor = new Label
        {
            BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantResist_Color)
        };
        setSize(lbl_implantResistColor, label_Height);
        c3TL.Rows[0].Cells.Add(lbl_implantResistColor);

        lbl_implantResistColor_name = new Label {Text = "Resist"};
        c3TL.Rows[0].Cells.Add(lbl_implantResistColor_name);
    }

    private void openGLRow2(TableCell tc)
    {
        TableLayout tl = new();
        Panel p = new() {Content = tl};
        tc.Control = p;

        tl.Rows.Add(new TableRow());

        btn_resetColors = new Button {Text = "Reset"};
        setSize(btn_resetColors, 60, 21);
        tc.Control = TableLayout.AutoSized(btn_resetColors, centered: true);
    }

    private void addRNGMappingEqtn(object sender, EventArgs e)
    {
        string newString = text_rngMapping.Text;
        if (newString == "")
        {
            return;
        }

        // Only allow unique entries.
        if (commonVars.rngCustomMapping.IndexOf(newString) != -1)
        {
            return;
        }

        ExpressionParser parser = new();
        double value = RNG.nextdouble();
        while (value < 1E-15)
        {
            value = RNG.nextdouble();
        }
        parser.Values.Add("x", value);
        value = RNG.nextdouble();
        while (value < 1E-15)
        {
            value = RNG.nextdouble();
        }
        parser.Values.Add("y", value);
        value = RNG.nextdouble();
        while (value < 1E-15)
        {
            value = RNG.nextdouble();
        }
        parser.Values.Add("z", value);
        try
        {
            string equation = newString;
            // Substitute any Box-Muller queries with the full form.
            equation = equation.Replace("_gxy", "(sqrt(-2 * ln(y)) * cos(2 * PI * x))");
            equation = equation.Replace("_gyz", "(sqrt(-2 * ln(z)) * cos(2 * PI * y))");
            equation = equation.Replace("_gxz", "(sqrt(-2 * ln(z)) * cos(2 * PI * x))");

            parser.Parse(equation);
            commonVars.rngCustomMapping.Add(newString);
        }
        catch (ParserException pe)
        {
            ErrorReporter.showMessage_OK(pe.ToString(), "Parser error");
        }
        catch (Exception e2)
        {
            ErrorReporter.showMessage_OK(e2.ToString(), "Error");
        }
    }

    private void removeRNGMappingEqtn(object sender, EventArgs e)
    {
        int index = listBox_rngCustomMapping.SelectedIndex;

        // Forbid removal of default, Box-Muller, entry.
        if (index <= 0 || commonVars.rngCustomMapping[index] == CommonVars.boxMuller)
        {
            return;
        }

        commonVars.rngCustomMapping.RemoveAt(index);
    }

    private void editRNGMappingEqtn(object sender, EventArgs e)
    {
        if (commonVars.rngCustomMapping[listBox_rngCustomMapping.SelectedIndex] != CommonVars.boxMuller)
        {
            text_rngMapping.Text = commonVars.rngCustomMapping[listBox_rngCustomMapping.SelectedIndex];
        }
    }

    private void rng_customMappingSetup(TableCell tc)
    {
        TableLayout groupBox_rng_table = new();
        groupBox_rng = new GroupBox {Text = "RNG Mapping", Content = groupBox_rng_table};

        tc.Control = groupBox_rng;

        groupBox_rng_table.Rows.Add(new TableRow());
        listBox_rngCustomMapping = new ListBox();
        listBox_rngCustomMapping.BindDataContext(c => c.DataStore, (UIStringLists m) => m.rngMapping);
        groupBox_rng_table.Rows[^1].Cells.Add(listBox_rngCustomMapping);

        groupBox_rng_table.Rows.Add(new TableRow());

        text_rngMapping = new TextBox
        {
            ToolTip =
                "Enter your custom mapping equation here. Three independent RNG variables are available: x, y and z.\r\nTo save the equation, use the Add button.\r\nValidation is performed before the equation is saved.\r\nPlease review any error output and correct it."
        };
        groupBox_rng_table.Rows[^1].Cells.Add(text_rngMapping);

        groupBox_rng_table.Rows.Add(new TableRow());

        TableLayout btnTL = new();
        Panel btnRow = new() {Content = btnTL};

        groupBox_rng_table.Rows[^1].Cells.Add(btnRow);
        btnTL.Rows.Add(new TableRow());

        btn_rngMapping_Add = new Button
        {
            Text = "Add", ToolTip = "Add mapping equation to the list (duplicates will not be added)."
        };
        btn_rngMapping_Add.Click += addRNGMappingEqtn;
        btnTL.Rows[^1].Cells.Add(new TableCell { Control = btn_rngMapping_Add });

        btn_rngMapping_Edit = new Button
        {
            Text = "Edit", ToolTip = "Load selected mapping equation for editing (Box-Muller cannot be edited)."
        };
        btn_rngMapping_Edit.Click += editRNGMappingEqtn;
        btnTL.Rows[^1].Cells.Add(new TableCell { Control = btn_rngMapping_Edit });

        btn_rngMapping_Remove = new Button
        {
            Text = "Remove",
            ToolTip = "Remove selected mapping equation from list (Box-Muller cannot be removed)."
        };
        btn_rngMapping_Remove.Click += removeRNGMappingEqtn;
        btnTL.Rows[^1].Cells.Add(new TableCell { Control = btn_rngMapping_Remove });
    }
}