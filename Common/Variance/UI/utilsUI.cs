using Error;
using Eto.Forms;
using info.lundin.math;
using System;

namespace Variance
{
    public partial class MainForm : Form
    {
        TextBox text_server, text_emailAddress, text_rngMapping;

        NumericStepper num_port, num_zoomSpeed, num_fgOpacity, num_bgOpacity;

        PasswordBox text_emailPwd;

        CheckBox checkBox_EmailCompletion, checkBox_perJob, checkBox_SSL,
                    checkBox_geoCore_enableCDVariation, checkBox_geoCore_tileLayerPreview,
                    checkBox_OGLAA, checkBox_OGLFill, checkBox_OGLPoints,
                    checkBox_friendlyNumbers;

        Button button_emailTest, utilitiesBtn_Summary, btn_resetColors, btn_rngMapping_Add, btn_rngMapping_Edit, btn_rngMapping_Remove;

        Label lbl_Layer1Color, lbl_Layer2Color, lbl_Layer3Color, lbl_Layer4Color,
                lbl_Layer5Color, lbl_Layer6Color, lbl_Layer7Color, lbl_Layer8Color,
                lbl_Layer9Color, lbl_Layer10Color, lbl_Layer11Color, lbl_Layer12Color,
                lbl_Layer13Color, lbl_Layer14Color, lbl_Layer15Color, lbl_Layer16Color,
                lbl_Result1Color, lbl_Result2Color, lbl_Result3Color, lbl_Result4Color,
                lbl_ss1Color, lbl_ss2Color, lbl_ss3Color,
                lbl_implantMinColor, lbl_implantMeanColor, lbl_implantMaxColor, lbl_implantResistColor,
                lbl_enabledColor, lbl_majorGridColor, lbl_minorGridColor,
                lbl_Layer1Color_name, lbl_Layer2Color_name, lbl_Layer3Color_name, lbl_Layer4Color_name,
                lbl_Layer5Color_name, lbl_Layer6Color_name, lbl_Layer7Color_name, lbl_Layer8Color_name,
                lbl_Layer9Color_name, lbl_Layer10Color_name, lbl_Layer11Color_name, lbl_Layer12Color_name,
                lbl_Layer13Color_name, lbl_Layer14Color_name, lbl_Layer15Color_name, lbl_Layer16Color_name,
                lbl_Result1Color_name, lbl_Result2Color_name, lbl_Result3Color_name, lbl_Result4Color_name,
                lbl_ss1Color_name, lbl_ss2Color_name, lbl_ss3Color_name,
                lbl_implantMinColor_name, lbl_implantMeanColor_name, lbl_implantMaxColor_name, lbl_implantResistColor_name,
                lbl_enabledColor_name, lbl_majorGridColor_name, lbl_minorGridColor_name,
                lbl_emailAddress, lbl_emailPwd, lbl_server, lbl_port, lbl_zoomSpeed, lbl_fgOpacity, lbl_bgOpacity;

        ListBox listBox_rngCustomMapping;

        GroupBox groupBox_utilities, groupBox_settings, groupBox_email, groupBox_openGL, prefs_geoCore, groupBox_rng, groupBox_misc;

        void utilsUISetup()
        {
            settings_utilsUISetup();
        }

        void utils_utilsUISetup(TableCell tc)
        {
            groupBox_utilities = new GroupBox();
            TableLayout groupBox_utilities_table = new TableLayout();
            groupBox_utilities.Text = "Utilities";
            groupBox_utilities.Content = groupBox_utilities_table;

            groupBox_utilities_table.Rows.Add(new TableRow());

            utilitiesBtn_Summary = new Button();
            utilitiesBtn_Summary.Text = "Create summary of DOE results";
            groupBox_utilities_table.Rows[groupBox_utilities_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(utilitiesBtn_Summary) });
            utilitiesBtn_Summary.Click += locateDOEResults;

            tc.Control = groupBox_utilities;
        }

        void settings_utilsUISetup()
        {
            groupBox_settings = new GroupBox();
            TableLayout groupBox_settings_table = new TableLayout();
            groupBox_settings.Text = "Settings";
            groupBox_settings.Content = groupBox_settings_table;

            tabPage_utilities_table.Rows.Add(new TableRow());
            tabPage_utilities_table.Rows[tabPage_utilities_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = groupBox_settings });
            tabPage_utilities_table.Rows[tabPage_utilities_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding;

            tabPage_utilities_table.Rows.Add(new TableRow()); // padding

            groupBox_settings_table.Rows.Add(new TableRow());
            Panel row0Panel = new Panel();
            groupBox_settings_table.Rows[groupBox_settings_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row0Panel });

            TableLayout row0tl = new TableLayout();
            row0Panel.Content = row0tl;
            row0tl.Rows.Add(new TableRow());
            TableCell settingsTC = new TableCell();
            row0tl.Rows[row0tl.Rows.Count - 1].Cells.Add(settingsTC);
            row0tl.Rows[row0tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding

            utils_utilsUISetup(settingsTC);

            groupBox_settings_table.Rows.Add(new TableRow());
            Panel row1Panel = new Panel();
            groupBox_settings_table.Rows[groupBox_settings_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row1Panel });

            TableLayout row1tl = new TableLayout();
            row1Panel.Content = row1tl;
            row1tl.Rows.Add(new TableRow());

            TableCell leftTC = new TableCell();
            Panel leftPanel = new Panel();
            leftTC.Control = leftPanel;
            row1tl.Rows[row1tl.Rows.Count - 1].Cells.Add(leftTC);

            TableLayout leftTL = new TableLayout();
            leftPanel.Content = leftTL;

            TableCell rightTC = new TableCell();
            Panel rightPanel = new Panel();
            rightTC.Control = rightPanel;
            row1tl.Rows[row1tl.Rows.Count - 1].Cells.Add(rightTC);

            TableLayout rightTL = new TableLayout();
            rightPanel.Content = rightTL;

            row1tl.Rows[row1tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); //padding

            // Left-hand side.
            leftTL.Rows.Add(new TableRow());
            TableCell lRow0 = new TableCell();
            leftTL.Rows[leftTL.Rows.Count - 1].Cells.Add(lRow0);
            leftTL.Rows[leftTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding
            email_settings_utilsUISetup(lRow0);

            leftTL.Rows.Add(new TableRow());
            TableCell lRow1 = new TableCell();
            leftTL.Rows[leftTL.Rows.Count - 1].Cells.Add(lRow1);
            leftTL.Rows[leftTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding
            geoCore_settings_utilsUISetup(lRow1);

            leftTL.Rows.Add(new TableRow());
            TableCell lRow2 = new TableCell();
            leftTL.Rows[leftTL.Rows.Count - 1].Cells.Add(lRow2);
            leftTL.Rows[leftTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding
            rng_customMappingSetup(lRow2);

            leftTL.Rows.Add(new TableRow()); // padding

            rightTL.Rows.Add(new TableRow());
            TableCell rRow0 = new TableCell();
            rightTL.Rows[rightTL.Rows.Count - 1].Cells.Add(rRow0);
            rightTL.Rows[rightTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding
            openGL_settings_utilsUISetup(rRow0);

            rightTL.Rows.Add(new TableRow());
            TableCell rRow1 = new TableCell();
            rightTL.Rows[rightTL.Rows.Count - 1].Cells.Add(rRow1);
            rightTL.Rows[rightTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding
            misc_settings_utilsUISetup(rRow1);

            groupBox_settings_table.Rows.Add(new TableRow()); // padding
        }

        void email_settings_utilsUISetup(TableCell tc)
        {
            groupBox_email = new GroupBox();
            TableLayout groupBox_email_table = new TableLayout();
            groupBox_email.Text = "Email Settings";
            groupBox_email.Content = groupBox_email_table;

            tc.Control = groupBox_email;

            groupBox_email_table.Rows.Add(new TableRow());

            Panel row0 = new Panel();
            groupBox_email_table.Rows[groupBox_email_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row0 });

            TableLayout row0tl = new TableLayout();
            row0.Content = row0tl;
            row0tl.Rows.Add(new TableRow());

            lbl_emailAddress = new Label();
            lbl_emailAddress.Text = "Address";
            row0tl.Rows[row0tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_emailAddress });

            text_emailAddress = new TextBox();
            row0tl.Rows[row0tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = text_emailAddress });

            groupBox_email_table.Rows.Add(new TableRow());

            Panel row1 = new Panel();
            groupBox_email_table.Rows[groupBox_email_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row1 });

            TableLayout row1tl = new TableLayout();
            row1.Content = row1tl;
            row1tl.Rows.Add(new TableRow());

            lbl_emailPwd = new Label();
            lbl_emailPwd.Text = "Password";
            row1tl.Rows[row1tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_emailPwd });

            text_emailPwd = new PasswordBox();
            row1tl.Rows[row1tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = text_emailPwd });

            groupBox_email_table.Rows.Add(new TableRow());

            Panel row2 = new Panel();
            groupBox_email_table.Rows[groupBox_email_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row2 });

            TableLayout row2tl = new TableLayout();
            row2.Content = row2tl;
            row2tl.Rows.Add(new TableRow());

            lbl_server = new Label();
            lbl_server.Text = "Server";
            row2tl.Rows[row2tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_server });

            text_server = new TextBox();
            row2tl.Rows[row2tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = text_server });

            groupBox_email_table.Rows.Add(new TableRow());

            Panel row3 = new Panel();
            groupBox_email_table.Rows[groupBox_email_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row3 });

            TableLayout row3tl = new TableLayout();
            row3.Content = row3tl;
            row3tl.Rows.Add(new TableRow());

            checkBox_SSL = new CheckBox();
            checkBox_SSL.Text = "SSL";
            row3tl.Rows[row3tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_SSL });

            lbl_port = new Label();
            lbl_port.Text = "Port";
            row3tl.Rows[row3tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_port });

            num_port = new NumericStepper();
            num_port.MinValue = 1;
            num_port.Value = 587;
            num_port.Increment = 1;
            setSize(num_port, 70, label_Height);
            row3tl.Rows[row3tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_port) });

            row3tl.Rows[row3tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            groupBox_email_table.Rows.Add(new TableRow());

            Panel row4 = new Panel();
            groupBox_email_table.Rows[groupBox_email_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row4 });

            TableLayout row4tl = new TableLayout();
            row4.Content = row4tl;
            row4tl.Rows.Add(new TableRow());

            checkBox_EmailCompletion = new CheckBox();
            checkBox_EmailCompletion.Text = "On Completion";
            row4tl.Rows[row4tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_EmailCompletion });

            checkBox_perJob = new CheckBox();
            checkBox_perJob.Text = "Per Job";
            row4tl.Rows[row4tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_perJob });

            button_emailTest = new Button();
            button_emailTest.Text = "Test";
            row4tl.Rows[row4tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(button_emailTest) });

            row4tl.Rows[row4tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });
        }

        void geoCore_settings_utilsUISetup(TableCell tc)
        {
            prefs_geoCore = new GroupBox();
            TableLayout prefs_geoCore_table = new TableLayout();
            prefs_geoCore.Text = "GDS/Oasis";
            prefs_geoCore.Content = prefs_geoCore_table;
            tc.Control = prefs_geoCore;

            prefs_geoCore_table.Rows.Add(new TableRow());
            checkBox_geoCore_enableCDVariation = new CheckBox();
            checkBox_geoCore_enableCDVariation.Text = "Allow CD/bias variation";
            prefs_geoCore_table.Rows[prefs_geoCore_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_geoCore_enableCDVariation });
            prefs_geoCore_table.Rows[prefs_geoCore_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding

            prefs_geoCore_table.Rows.Add(new TableRow());
            checkBox_geoCore_tileLayerPreview = new CheckBox();
            checkBox_geoCore_tileLayerPreview.Text = "Use DOE tile for layer preview";
            prefs_geoCore_table.Rows[prefs_geoCore_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_geoCore_tileLayerPreview });
            prefs_geoCore_table.Rows[prefs_geoCore_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding
        }

        void misc_settings_utilsUISetup(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout tl = new TableLayout();
            p.Content = tl;
            tl.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tl.Rows[0].Cells.Add(tc0);
            tl.Rows.Add(new TableRow()); // padding

            groupBox_misc = new GroupBox();
            tc0.Control = groupBox_misc;
            TableLayout groupBox_misc_table = new TableLayout();
            groupBox_misc.Text = "Misc";
            groupBox_misc.Content = groupBox_misc_table;

            groupBox_misc_table.Rows.Add(new TableRow());
            TableCell row0 = new TableCell();
            groupBox_misc_table.Rows[groupBox_misc_table.Rows.Count - 1].Cells.Add(row0);
            miscRow0(row0);
        }

        void miscRow0(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());
            p.Content = tl;

            tl.Rows.Add(new TableRow());

            checkBox_friendlyNumbers = new CheckBox();
            checkBox_friendlyNumbers.Text = "Friendly Numbers";
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_friendlyNumbers });

            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding

            tl.Rows.Add(new TableRow()); // padding.
        }

        void openGL_settings_utilsUISetup(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout tl = new TableLayout();
            p.Content = tl;
            tl.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tl.Rows[0].Cells.Add(tc0);
            tl.Rows.Add(new TableRow()); // padding

            groupBox_openGL = new GroupBox();
            tc0.Control = groupBox_openGL;
            TableLayout groupBox_openGL_table = new TableLayout();
            groupBox_openGL.Text = "OpenGL";
            groupBox_openGL.Content = groupBox_openGL_table;

            groupBox_openGL_table.Rows.Add(new TableRow());
            TableCell row0 = new TableCell();
            groupBox_openGL_table.Rows[groupBox_openGL_table.Rows.Count - 1].Cells.Add(row0);
            openGLRow0(row0);

            groupBox_openGL_table.Rows.Add(new TableRow());
            TableCell row1 = new TableCell();
            groupBox_openGL_table.Rows[groupBox_openGL_table.Rows.Count - 1].Cells.Add(row1);
            openGLRow1(row1);

            groupBox_openGL_table.Rows.Add(new TableRow());
            TableCell row2 = new TableCell();
            groupBox_openGL_table.Rows[groupBox_openGL_table.Rows.Count - 1].Cells.Add(row2);
            openGLRow2(row2);
        }

        void openGLRow0(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());
            p.Content = tl;

            Panel lRow0 = new Panel();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lRow0 });

            Panel rRow0 = new Panel();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = rRow0 });

            TableLayout lRow0tl = new TableLayout();
            lRow0.Content = lRow0tl;
            lRow0tl.Rows.Add(new TableRow());

            TableLayout rRow0tl = new TableLayout();
            rRow0.Content = rRow0tl;
            rRow0tl.Rows.Add(new TableRow());

            Panel zoomPnl = new Panel();
            TableLayout zoomTL = new TableLayout();
            zoomPnl.Content = zoomTL;
            lRow0tl.Rows[lRow0tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = zoomPnl });

            zoomTL.Rows.Add(new TableRow());

            lbl_zoomSpeed = new Label();
            lbl_zoomSpeed.Text = "Zoom Increment";
            zoomTL.Rows[zoomTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_zoomSpeed });

            num_zoomSpeed = new NumericStepper();
            num_zoomSpeed.MinValue = 1;
            num_zoomSpeed.Increment = 1;
            setSize(num_zoomSpeed, 50, num_Height);
            zoomTL.Rows[zoomTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_zoomSpeed) });

            zoomTL.Rows[zoomTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding

            Panel fgOpPnl = new Panel();
            TableLayout fgOpTL = new TableLayout();
            fgOpPnl.Content = fgOpTL;
            rRow0tl.Rows[rRow0tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = fgOpPnl });

            fgOpTL.Rows.Add(new TableRow());

            lbl_fgOpacity = new Label();
            lbl_fgOpacity.Text = "FG";
            fgOpTL.Rows[fgOpTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_fgOpacity });

            num_fgOpacity = new NumericStepper();
            num_fgOpacity.MinValue = 0.01f;
            num_fgOpacity.Increment = 0.1f;
            num_fgOpacity.DecimalPlaces = 2;
            setSize(num_fgOpacity, 50, num_Height);
            fgOpTL.Rows[fgOpTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_fgOpacity) });

            Panel bgOpPnl = new Panel();
            TableLayout bgOpTL = new TableLayout();
            bgOpPnl.Content = bgOpTL;
            rRow0tl.Rows[rRow0tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = bgOpPnl });

            bgOpTL.Rows.Add(new TableRow());

            lbl_bgOpacity = new Label();
            lbl_bgOpacity.Text = "BG";
            bgOpTL.Rows[bgOpTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_bgOpacity });

            num_bgOpacity = new NumericStepper();
            num_bgOpacity.MinValue = 0.01f;
            num_bgOpacity.Increment = 0.1f;
            num_bgOpacity.DecimalPlaces = 2;
            setSize(num_bgOpacity, 50, num_Height);
            bgOpTL.Rows[bgOpTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_bgOpacity) });

            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            tl.Rows.Add(new TableRow());

            Panel lRow1 = new Panel();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lRow1 });

            Panel rRow1 = new Panel();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = rRow1 });

            TableLayout lRow1tl = new TableLayout();
            lRow1.Content = lRow1tl;
            lRow1tl.Rows.Add(new TableRow());

            TableLayout rRow1tl = new TableLayout();
            rRow1.Content = rRow1tl;
            rRow1tl.Rows.Add(new TableRow());

            Panel dispOptsPnl = new Panel();
            TableLayout dispOptsTL = new TableLayout();
            dispOptsPnl.Content = dispOptsTL;
            lRow1tl.Rows[lRow1tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = dispOptsPnl });

            dispOptsTL.Rows.Add(new TableRow());

            checkBox_OGLAA = new CheckBox();
            checkBox_OGLAA.Text = "Antialiasing";
            dispOptsTL.Rows[dispOptsTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_OGLAA });

            checkBox_OGLFill = new CheckBox();
            checkBox_OGLFill.Text = "Fill";
            dispOptsTL.Rows[dispOptsTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_OGLFill });

            checkBox_OGLPoints = new CheckBox();
            checkBox_OGLPoints.Text = "Points";
            dispOptsTL.Rows[dispOptsTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_OGLPoints });
        }

        void openGLRow1(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout swatchesTL = new TableLayout();
            p.Content = swatchesTL;

            swatchesTL.Rows.Add(new TableRow());

            TableRow row0 = new TableRow();
            swatchesTL.Rows.Add(row0);

            opengl_swatchrow0(row0);

            TableRow row1 = new TableRow();
            swatchesTL.Rows.Add(row1);

            opengl_swatchrow1(row1);

            TableRow row2 = new TableRow();
            swatchesTL.Rows.Add(row2);

            opengl_swatchrow2(row2);

            TableRow row3 = new TableRow();
            swatchesTL.Rows.Add(row3);

            opengl_swatchrow3(row3);

            TableRow row4 = new TableRow();
            swatchesTL.Rows.Add(row4);

            opengl_swatchrow4(row4);

            TableRow row5 = new TableRow();
            swatchesTL.Rows.Add(row5);

            opengl_swatchrow5(row5);

            TableRow row6 = new TableRow();
            swatchesTL.Rows.Add(row6);

            opengl_swatchrow6(row6);

            TableRow row7 = new TableRow();
            swatchesTL.Rows.Add(row7);

            opengl_swatchrow7(row7);
        }

        void opengl_swatchrow0(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_minorGridColor = new Label();
            lbl_minorGridColor.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().minor_Color);
            setSize(lbl_minorGridColor, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_minorGridColor);

            lbl_minorGridColor_name = new Label();
            lbl_minorGridColor_name.Text = "Minor Grid";
            c0TL.Rows[0].Cells.Add(lbl_minorGridColor_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_majorGridColor = new Label();
            lbl_majorGridColor.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().major_Color);
            setSize(lbl_majorGridColor, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_majorGridColor);

            lbl_majorGridColor_name = new Label();
            lbl_majorGridColor_name.Text = "Major Grid";
            c1TL.Rows[0].Cells.Add(lbl_majorGridColor_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });
        }

        void opengl_swatchrow1(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_Layer1Color = new Label();
            lbl_Layer1Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer1_Color);
            setSize(lbl_Layer1Color, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_Layer1Color);

            lbl_Layer1Color_name = new Label();
            lbl_Layer1Color_name.Text = "Layer 1";
            c0TL.Rows[0].Cells.Add(lbl_Layer1Color_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_Layer2Color = new Label();
            lbl_Layer2Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer2_Color);
            setSize(lbl_Layer2Color, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_Layer2Color);

            lbl_Layer2Color_name = new Label();
            lbl_Layer2Color_name.Text = "Layer 2";
            c1TL.Rows[0].Cells.Add(lbl_Layer2Color_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            TableLayout c2TL = new TableLayout();
            c2.Content = c2TL;
            c2TL.Rows.Add(new TableRow());

            lbl_Layer3Color = new Label();
            lbl_Layer3Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer3_Color);
            setSize(lbl_Layer3Color, label_Height, label_Height);
            c2TL.Rows[0].Cells.Add(lbl_Layer3Color);

            lbl_Layer3Color_name = new Label();
            lbl_Layer3Color_name.Text = "Layer 3";
            c2TL.Rows[0].Cells.Add(lbl_Layer3Color_name);

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });

            TableLayout c3TL = new TableLayout();
            c3.Content = c3TL;
            c3TL.Rows.Add(new TableRow());

            lbl_Layer4Color = new Label();
            lbl_Layer4Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer4_Color);
            setSize(lbl_Layer4Color, label_Height, label_Height);
            c3TL.Rows[0].Cells.Add(lbl_Layer4Color);

            lbl_Layer4Color_name = new Label();
            lbl_Layer4Color_name.Text = "Layer 4";
            c3TL.Rows[0].Cells.Add(lbl_Layer4Color_name);
        }

        void opengl_swatchrow2(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_Layer5Color = new Label();
            lbl_Layer5Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer5_Color);
            setSize(lbl_Layer5Color, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_Layer5Color);

            lbl_Layer5Color_name = new Label();
            lbl_Layer5Color_name.Text = "Layer 5";
            c0TL.Rows[0].Cells.Add(lbl_Layer5Color_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_Layer6Color = new Label();
            lbl_Layer6Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer6_Color);
            setSize(lbl_Layer6Color, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_Layer6Color);

            lbl_Layer6Color_name = new Label();
            lbl_Layer6Color_name.Text = "Layer 6";
            c1TL.Rows[0].Cells.Add(lbl_Layer6Color_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            TableLayout c2TL = new TableLayout();
            c2.Content = c2TL;
            c2TL.Rows.Add(new TableRow());

            lbl_Layer7Color = new Label();
            lbl_Layer7Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer7_Color);
            setSize(lbl_Layer7Color, label_Height, label_Height);
            c2TL.Rows[0].Cells.Add(lbl_Layer7Color);

            lbl_Layer7Color_name = new Label();
            lbl_Layer7Color_name.Text = "Layer 7";
            c2TL.Rows[0].Cells.Add(lbl_Layer7Color_name);

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });

            TableLayout c3TL = new TableLayout();
            c3.Content = c3TL;
            c3TL.Rows.Add(new TableRow());

            lbl_Layer8Color = new Label();
            lbl_Layer8Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer8_Color);
            setSize(lbl_Layer8Color, label_Height, label_Height);
            c3TL.Rows[0].Cells.Add(lbl_Layer8Color);

            lbl_Layer8Color_name = new Label();
            lbl_Layer8Color_name.Text = "Layer 8";
            c3TL.Rows[0].Cells.Add(lbl_Layer8Color_name);
        }

        void opengl_swatchrow3(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_Layer9Color = new Label();
            lbl_Layer9Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer9_Color);
            setSize(lbl_Layer9Color, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_Layer9Color);

            lbl_Layer9Color_name = new Label();
            lbl_Layer9Color_name.Text = "Layer 9";
            c0TL.Rows[0].Cells.Add(lbl_Layer9Color_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_Layer10Color = new Label();
            lbl_Layer10Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer10_Color);
            setSize(lbl_Layer10Color, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_Layer10Color);

            lbl_Layer10Color_name = new Label();
            lbl_Layer10Color_name.Text = "Layer 10";
            c1TL.Rows[0].Cells.Add(lbl_Layer10Color_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            TableLayout c2TL = new TableLayout();
            c2.Content = c2TL;
            c2TL.Rows.Add(new TableRow());

            lbl_Layer11Color = new Label();
            lbl_Layer11Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer11_Color);
            setSize(lbl_Layer11Color, label_Height, label_Height);
            c2TL.Rows[0].Cells.Add(lbl_Layer11Color);

            lbl_Layer11Color_name = new Label();
            lbl_Layer11Color_name.Text = "Layer 11";
            c2TL.Rows[0].Cells.Add(lbl_Layer11Color_name);

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });

            TableLayout c3TL = new TableLayout();
            c3.Content = c3TL;
            c3TL.Rows.Add(new TableRow());

            lbl_Layer12Color = new Label();
            lbl_Layer12Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer12_Color);
            setSize(lbl_Layer12Color, label_Height, label_Height);
            c3TL.Rows[0].Cells.Add(lbl_Layer12Color);

            lbl_Layer12Color_name = new Label();
            lbl_Layer12Color_name.Text = "Layer 12";
            c3TL.Rows[0].Cells.Add(lbl_Layer12Color_name);
        }

        void opengl_swatchrow4(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_Layer13Color = new Label();
            lbl_Layer13Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer13_Color);
            setSize(lbl_Layer13Color, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_Layer13Color);

            lbl_Layer13Color_name = new Label();
            lbl_Layer13Color_name.Text = "Layer 13";
            c0TL.Rows[0].Cells.Add(lbl_Layer13Color_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_Layer14Color = new Label();
            lbl_Layer14Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer14_Color);
            setSize(lbl_Layer14Color, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_Layer14Color);

            lbl_Layer14Color_name = new Label();
            lbl_Layer14Color_name.Text = "Layer 14";
            c1TL.Rows[0].Cells.Add(lbl_Layer14Color_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            TableLayout c2TL = new TableLayout();
            c2.Content = c2TL;
            c2TL.Rows.Add(new TableRow());

            lbl_Layer15Color = new Label();
            lbl_Layer15Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer15_Color);
            setSize(lbl_Layer15Color, label_Height, label_Height);
            c2TL.Rows[0].Cells.Add(lbl_Layer15Color);

            lbl_Layer15Color_name = new Label();
            lbl_Layer15Color_name.Text = "Layer 15";
            c2TL.Rows[0].Cells.Add(lbl_Layer15Color_name);

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });

            TableLayout c3TL = new TableLayout();
            c3.Content = c3TL;
            c3TL.Rows.Add(new TableRow());

            lbl_Layer16Color = new Label();
            lbl_Layer16Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().layer16_Color);
            setSize(lbl_Layer16Color, label_Height, label_Height);
            c3TL.Rows[0].Cells.Add(lbl_Layer16Color);

            lbl_Layer16Color_name = new Label();
            lbl_Layer16Color_name.Text = "Layer 16";
            c3TL.Rows[0].Cells.Add(lbl_Layer16Color_name);
        }

        void opengl_swatchrow5(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_Result1Color = new Label();
            lbl_Result1Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result_Color);
            setSize(lbl_Result1Color, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_Result1Color);

            lbl_Result1Color_name = new Label();
            lbl_Result1Color_name.Text = "Result 1";
            c0TL.Rows[0].Cells.Add(lbl_Result1Color_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_Result2Color = new Label();
            lbl_Result2Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result2_Color);
            setSize(lbl_Result2Color, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_Result2Color);

            lbl_Result2Color_name = new Label();
            lbl_Result2Color_name.Text = "Result 2";
            c1TL.Rows[0].Cells.Add(lbl_Result2Color_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            TableLayout c2TL = new TableLayout();
            c2.Content = c2TL;
            c2TL.Rows.Add(new TableRow());

            lbl_Result3Color = new Label();
            lbl_Result3Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result3_Color);
            setSize(lbl_Result3Color, label_Height, label_Height);
            c2TL.Rows[0].Cells.Add(lbl_Result3Color);

            lbl_Result3Color_name = new Label();
            lbl_Result3Color_name.Text = "Result 3";
            lbl_Result3Color_name.Width = 80;
            c2TL.Rows[0].Cells.Add(lbl_Result3Color_name);

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });

            TableLayout c3TL = new TableLayout();
            c3.Content = c3TL;
            c3TL.Rows.Add(new TableRow());

            lbl_Result4Color = new Label();
            lbl_Result4Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().result4_Color);
            setSize(lbl_Result4Color, label_Height, label_Height);
            c3TL.Rows[0].Cells.Add(lbl_Result4Color);

            lbl_Result4Color_name = new Label();
            lbl_Result4Color_name.Text = "Result 4";
            c3TL.Rows[0].Cells.Add(lbl_Result4Color_name);

        }

        void opengl_swatchrow6(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_enabledColor = new Label();
            lbl_enabledColor.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().enabled_Color);
            setSize(lbl_enabledColor, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_enabledColor);

            lbl_enabledColor_name = new Label();
            lbl_enabledColor_name.Text = "Enabled";
            c0TL.Rows[0].Cells.Add(lbl_enabledColor_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_ss1Color = new Label();
            lbl_ss1Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().subshape1_Color);
            setSize(lbl_ss1Color, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_ss1Color);

            lbl_ss1Color_name = new Label();
            lbl_ss1Color_name.Text = "Subshape 1";
            c1TL.Rows[0].Cells.Add(lbl_ss1Color_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            TableLayout c2TL = new TableLayout();
            c2.Content = c2TL;
            c2TL.Rows.Add(new TableRow());

            lbl_ss2Color = new Label();
            lbl_ss2Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().subshape2_Color);
            setSize(lbl_ss2Color, label_Height, label_Height);
            c2TL.Rows[0].Cells.Add(lbl_ss2Color);

            lbl_ss2Color_name = new Label();
            lbl_ss2Color_name.Text = "Subshape 2";
            c2TL.Rows[0].Cells.Add(lbl_ss2Color_name);

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });

            TableLayout c3TL = new TableLayout();
            c3.Content = c3TL;
            c3TL.Rows.Add(new TableRow());

            lbl_ss3Color = new Label();
            lbl_ss3Color.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().subshape3_Color);
            setSize(lbl_ss3Color, label_Height, label_Height);
            c3TL.Rows[0].Cells.Add(lbl_ss3Color);

            lbl_ss3Color_name = new Label();
            lbl_ss3Color_name.Text = "Subshape 3";
            c3TL.Rows[0].Cells.Add(lbl_ss3Color_name);
        }

        void opengl_swatchrow7(TableRow tr)
        {
            Panel c0 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c0 });

            TableLayout c0TL = new TableLayout();
            c0.Content = c0TL;
            c0TL.Rows.Add(new TableRow());

            lbl_implantMinColor = new Label();
            lbl_implantMinColor.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantMin_Color);
            setSize(lbl_implantMinColor, label_Height, label_Height);
            c0TL.Rows[0].Cells.Add(lbl_implantMinColor);

            lbl_implantMinColor_name = new Label();
            lbl_implantMinColor_name.Text = "Min Shadow";
            c0TL.Rows[0].Cells.Add(lbl_implantMinColor_name);

            Panel c1 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c1 });

            TableLayout c1TL = new TableLayout();
            c1.Content = c1TL;
            c1TL.Rows.Add(new TableRow());

            lbl_implantMeanColor = new Label();
            lbl_implantMeanColor.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantMean_Color);
            setSize(lbl_implantMeanColor, label_Height, label_Height);
            c1TL.Rows[0].Cells.Add(lbl_implantMeanColor);

            lbl_implantMeanColor_name = new Label();
            lbl_implantMeanColor_name.Text = "Mean Shadow";
            c1TL.Rows[0].Cells.Add(lbl_implantMeanColor_name);

            Panel c2 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c2 });

            TableLayout c2TL = new TableLayout();
            c2.Content = c2TL;
            c2TL.Rows.Add(new TableRow());

            lbl_implantMaxColor = new Label();
            lbl_implantMaxColor.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantMax_Color);
            setSize(lbl_implantMaxColor, label_Height, label_Height);
            c2TL.Rows[0].Cells.Add(lbl_implantMaxColor);

            lbl_implantMaxColor_name = new Label();
            lbl_implantMaxColor_name.Text = "Max Shadow";
            c2TL.Rows[0].Cells.Add(lbl_implantMaxColor_name);

            Panel c3 = new Panel();
            tr.Cells.Add(new TableCell() { Control = c3 });

            TableLayout c3TL = new TableLayout();
            c3.Content = c3TL;
            c3TL.Rows.Add(new TableRow());

            lbl_implantResistColor = new Label();
            lbl_implantResistColor.BackgroundColor = UIHelper.myColorToColor(commonVars.getColors().implantResist_Color);
            setSize(lbl_implantResistColor, label_Height, label_Height);
            c3TL.Rows[0].Cells.Add(lbl_implantResistColor);

            lbl_implantResistColor_name = new Label();
            lbl_implantResistColor_name.Text = "Resist";
            c3TL.Rows[0].Cells.Add(lbl_implantResistColor_name);
        }

        void openGLRow2(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p;

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());
            p.Content = tl;


            btn_resetColors = new Button();
            btn_resetColors.Text = "Reset";
            setSize(btn_resetColors, 60, 21);
            tc.Control = TableLayout.AutoSized(btn_resetColors, centered: true);
        }

        void addRNGMappingEqtn(object sender, EventArgs e)
        {
            string newString = text_rngMapping.Text;
            if (newString == "")
            {
                return;
            }

            // Only allow unique entries.
            if (commonVars.rngCustomMapping.IndexOf(newString) == -1)
            {
                ExpressionParser parser = new ExpressionParser();
                double value = entropyRNG.RNG.nextdouble();
                while (value < 1E-15)
                {
                    value = entropyRNG.RNG.nextdouble();
                }
                parser.Values.Add("x", value);
                value = entropyRNG.RNG.nextdouble();
                while (value < 1E-15)
                {
                    value = entropyRNG.RNG.nextdouble();
                }
                parser.Values.Add("y", value);
                value = entropyRNG.RNG.nextdouble();
                while (value < 1E-15)
                {
                    value = entropyRNG.RNG.nextdouble();
                }
                parser.Values.Add("z", value);
                try
                {
                    string equation = newString;
                    // Substitute any Box-Muller queries with the full form.
                    equation = equation.Replace("_gxy", "(sqrt(-2 * ln(y)) * cos(2 * PI * x))");
                    equation = equation.Replace("_gyz", "(sqrt(-2 * ln(z)) * cos(2 * PI * y))");
                    equation = equation.Replace("_gxz", "(sqrt(-2 * ln(z)) * cos(2 * PI * x))");

                    double validation = parser.Parse(equation);
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
        }

        void removeRNGMappingEqtn(object sender, EventArgs e)
        {
            int index = listBox_rngCustomMapping.SelectedIndex;

            // Forbid removal of default, Box-Muller, entry.
            if ((index <= 0) || (commonVars.rngCustomMapping[index] == CommonVars.boxMuller))
            {
                return;
            }

            commonVars.rngCustomMapping.RemoveAt(index);
        }

        void editRNGMappingEqtn(object sender, EventArgs e)
        {
            if (commonVars.rngCustomMapping[listBox_rngCustomMapping.SelectedIndex] != CommonVars.boxMuller)
            {
                text_rngMapping.Text = commonVars.rngCustomMapping[listBox_rngCustomMapping.SelectedIndex];
            }
        }

        void rng_customMappingSetup(TableCell tc)
        {
            groupBox_rng = new GroupBox();
            tc.Control = groupBox_rng;

            TableLayout groupBox_rng_table = new TableLayout();
            groupBox_rng.Text = "RNG Mapping";
            groupBox_rng.Content = groupBox_rng_table;

            groupBox_rng_table.Rows.Add(new TableRow());
            listBox_rngCustomMapping = new ListBox();
            listBox_rngCustomMapping.BindDataContext(c => c.DataStore, (UIStringLists m) => m.rngMapping);
            groupBox_rng_table.Rows[groupBox_rng_table.Rows.Count - 1].Cells.Add(listBox_rngCustomMapping);

            groupBox_rng_table.Rows.Add(new TableRow());

            text_rngMapping = new TextBox();
            text_rngMapping.ToolTip = "Enter your custom mapping equation here. Three independent RNG variables are available: x, y and z.\r\nTo save the equation, use the Add button.\r\nValidation is performed before the equation is saved.\r\nPlease review any error output and correct it.";
            groupBox_rng_table.Rows[groupBox_rng_table.Rows.Count - 1].Cells.Add(text_rngMapping);

            groupBox_rng_table.Rows.Add(new TableRow());

            Panel btnRow = new Panel();

            groupBox_rng_table.Rows[groupBox_rng_table.Rows.Count - 1].Cells.Add(btnRow);

            TableLayout btnTL = new TableLayout();
            btnRow.Content = btnTL;
            btnTL.Rows.Add(new TableRow());

            btn_rngMapping_Add = new Button();
            btn_rngMapping_Add.Text = "Add";
            btn_rngMapping_Add.ToolTip = "Add mapping equation to the list (duplicates will not be added).";
            btn_rngMapping_Add.Click += addRNGMappingEqtn;
            btnTL.Rows[btnTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = btn_rngMapping_Add });

            btn_rngMapping_Edit = new Button();
            btn_rngMapping_Edit.Text = "Edit";
            btn_rngMapping_Edit.ToolTip = "Load selected mapping equation for editing (Box-Muller cannot be edited).";
            btn_rngMapping_Edit.Click += editRNGMappingEqtn;
            btnTL.Rows[btnTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = btn_rngMapping_Edit });

            btn_rngMapping_Remove = new Button();
            btn_rngMapping_Remove.Text = "Remove";
            btn_rngMapping_Remove.ToolTip = "Remove selected mapping equation from list (Box-Muller cannot be removed).";
            btn_rngMapping_Remove.Click += removeRNGMappingEqtn;
            btnTL.Rows[btnTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = btn_rngMapping_Remove });
        }
    }
}