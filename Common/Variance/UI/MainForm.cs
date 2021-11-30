using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using entropyRNG;
using Error;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Eto.Veldrid;
using geoWrangler;
using Veldrid;
using VeldridEto;
using PixelFormat = Veldrid.PixelFormat;

namespace Variance;

/// <summary>
/// Your application's main form
/// </summary>
public partial class MainForm
{
    private CommonVars commonVars;

    private Entropy entropyControl;

    private VarianceContextGUI varianceContext;

    private Command quitCommand, helpCommand, aboutCommand, clearLayer, copyLayer, pasteLayer, newSim, openSim, revertSim, saveSim, saveAsSim;

    private List<string> notList, booleanList;

    private Replay simReplay;

    private string helpPath;
    private bool helpAvailable;

    private void setupUIDataContext(List<string> notList_, List<string> booleanList_)
    {
        DataContext = new UIStringLists
        {
            subShapeList = commonVars.subshapes,
            shapes = commonVars.getAvailableShapes(),
            noiseTypeList = commonVars.getNoiseTypes(),
            subShapePos = commonVars.getAvailableSubShapePositions(),
            tipLocs = commonVars.getAvailableTipsLocations(),
            rngTypeList = commonRNG.rngTypes,
            externalTypeList = commonVars.getExternalTypes(),
            externalFilterList = commonVars.getExternalFilterList(),
            calcModes = commonVars.calcMode_names,
            booleanList = booleanList_,
            openGLMode = commonVars.getOpenGLModeList(),
            notList = notList_,
            fallOffList = RayCast.fallOffList,
            polyFillList = commonVars.getPolyFillTypes(),
            geoCoreStructureList = commonVars.structureList,
            geoCoreLDList = commonVars.activeStructure_LayerDataTypeList,
            geoCoreStructureList_exp = commonVars.structureList_exp,
            subShapesList_exp = commonVars.subShapesList_exp,
            geoCoreLDList_exp = commonVars.activeStructure_LayerDataTypeList_exp,
            rngMapping = commonVars.rngCustomMapping,
            layerNames = commonVars.layerNames
        };
    }

    private void loadPrefs()
    {
        // We have to do this by hand, reading and parsing an XML file. Yay.
        string filename = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationSettings);
        filename += Path.DirectorySeparatorChar + "variance_prefs.xml";

        XElement prefs;
        try
        {
            prefs = XElement.Load(filename);
        }
        catch (Exception)
        {
            if (File.Exists(filename))
            {
                ErrorReporter.showMessage_OK("Prefs file exists, but can't be read. Using defaults.", "Preferences Error");
            }
            return; // file may not exist (new user) or is inaccessible. We have defaults so can just return without trouble.
        }

        try
        {
            varianceContext.vc.emailAddress = prefs.Descendants("email").Descendants("emailAddress").First().Value;
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.emailPwd = prefs.Descendants("email").Descendants("emailPassword").First().Value;
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.host = prefs.Descendants("email").Descendants("emailHost").First().Value;
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.port = prefs.Descendants("email").Descendants("emailPort").First().Value;
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.ssl = Convert.ToBoolean(prefs.Descendants("email").Descendants("emailSSL").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.geoCoreCDVariation = Convert.ToBoolean(prefs.Descendants("geoCore").Descendants("geoCoreCDVariation").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.layerPreviewDOETile = Convert.ToBoolean(prefs.Descendants("geoCore").Descendants("layerPreviewDOETile").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.AA = Convert.ToBoolean(prefs.Descendants("openGL").Descendants("openGLAA").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.FilledPolygons = Convert.ToBoolean(prefs.Descendants("openGL").Descendants("openGLFilledPolygons").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.drawPoints = Convert.ToBoolean(prefs.Descendants("openGL").Descendants("openGLPoints").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.openGLZoomFactor = Convert.ToInt32(prefs.Descendants("openGL").Descendants("openGLZoomFactor").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.BGOpacity = Convert.ToDouble(prefs.Descendants("openGL").Descendants("openGLBGOpacity").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.FGOpacity = Convert.ToDouble(prefs.Descendants("openGL").Descendants("openGLFGOpacity").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            varianceContext.vc.friendlyNumber = Convert.ToBoolean(prefs.Descendants("friendlyNumber").First().Value);
        }
        catch (Exception)
        {
        }

        varianceContext.vc.rngMappingEquations.Clear();
        varianceContext.vc.rngMappingEquations.Add("Box-Muller");
        try
        {
            string tempString = prefs.Descendants("rngMappingEquations").First().Value;
            string[] equations = tempString.Split(new[] { ';' });
            foreach (string t in equations)
            {
                if (t != "Box-Muller")
                {
                    varianceContext.vc.rngMappingEquations.Add(t);
                }
            }
        }
        catch (Exception)
        {

        }

        try
        {
            const string layerCol = "layer1Color";
            varianceContext.vc.colors.layer1_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").Single().Value);
            varianceContext.vc.colors.layer1_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").Single().Value);
            varianceContext.vc.colors.layer1_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").Single().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer2Color";
            varianceContext.vc.colors.layer2_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer2_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer2_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer3Color";
            varianceContext.vc.colors.layer3_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer3_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer3_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer4Color";
            varianceContext.vc.colors.layer4_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer4_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer4_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer5Color";
            varianceContext.vc.colors.layer5_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer5_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer5_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer6Color";
            varianceContext.vc.colors.layer6_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer6_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer6_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer7Color";
            varianceContext.vc.colors.layer7_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer7_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer7_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer8Color";
            varianceContext.vc.colors.layer8_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer8_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer8_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer9Color";
            varianceContext.vc.colors.layer9_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer9_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer9_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer10Color";
            varianceContext.vc.colors.layer10_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer10_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer10_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer11Color";
            varianceContext.vc.colors.layer11_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer11_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer11_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer12Color";
            varianceContext.vc.colors.layer12_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer12_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer12_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer13Color";
            varianceContext.vc.colors.layer13_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer13_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer13_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer14Color";
            varianceContext.vc.colors.layer14_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer14_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer14_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer15Color";
            varianceContext.vc.colors.layer15_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer15_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer15_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "layer16Color";
            varianceContext.vc.colors.layer16_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.layer16_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.layer16_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "subshape1Color";
            varianceContext.vc.colors.subshape1_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.subshape1_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.subshape1_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "subshape2Color";
            varianceContext.vc.colors.subshape2_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.subshape2_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.subshape2_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "subshape3Color";
            varianceContext.vc.colors.subshape3_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.subshape3_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.subshape3_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "result1Color";
            varianceContext.vc.colors.result_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.result_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.result_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "result2Color";
            varianceContext.vc.colors.result2_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.result2_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.result2_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "result3Color";
            varianceContext.vc.colors.result3_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.result3_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.result3_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "result4Color";
            varianceContext.vc.colors.result4_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.result4_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.result4_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "minImplantColor";
            varianceContext.vc.colors.implantMin_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.implantMin_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.implantMin_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "meanImplantColor";
            varianceContext.vc.colors.implantMean_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.implantMean_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.implantMean_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "maxImplantColor";
            varianceContext.vc.colors.implantMax_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.implantMax_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.implantMax_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "implantResistColor";
            varianceContext.vc.colors.implantResist_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.implantResist_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.implantResist_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "majorColor";
            varianceContext.vc.colors.major_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.major_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.major_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "axisColor";
            varianceContext.vc.colors.axis_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.axis_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.axis_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "minorColor";
            varianceContext.vc.colors.minor_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.minor_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.minor_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "backgroundColor";
            varianceContext.vc.colors.background_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.background_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.background_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        try
        {
            const string layerCol = "enabledColor";
            varianceContext.vc.colors.enabled_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
            varianceContext.vc.colors.enabled_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
            varianceContext.vc.colors.enabled_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
        }
        catch (Exception)
        {
        }

        varianceContext.vc.colors.rebuildLists();
    }

    private void savePrefs()
    {
        string filename = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationSettings);
        filename += Path.DirectorySeparatorChar + "variance_prefs.xml";

        try
        {
            XDocument prefsXML = new(
                new XElement("root"));
            // ReSharper disable once PossibleNullReferenceException
            prefsXML.Root.Add(new XElement("version", CentralProperties.version));

            XElement emailPrefs = new("email",
                new XElement("emailAddress", varianceContext.vc.emailAddress),
                new XElement("emailPassword", varianceContext.vc.emailPwd),
                new XElement("emailHost", varianceContext.vc.host),
                new XElement("emailSSL", varianceContext.vc.ssl),
                new XElement("emailPort", varianceContext.vc.port));
            prefsXML.Root.Add(emailPrefs);

            XElement geoCorePrefs = new("geoCore",
                new XElement("geoCoreCDVariation", varianceContext.vc.geoCoreCDVariation),
                new XElement("layerPreviewDOETile", varianceContext.vc.layerPreviewDOETile));
            prefsXML.Root.Add(geoCorePrefs);

            XElement openGLPrefs = new("openGL",
                new XElement("openGLAA", varianceContext.vc.AA),
                new XElement("openGLFilledPolygons", varianceContext.vc.FilledPolygons),
                new XElement("openGLPoints", varianceContext.vc.drawPoints),
                new XElement("openGLZoomFactor", varianceContext.vc.openGLZoomFactor),
                new XElement("openGLFGOpacity", varianceContext.vc.FGOpacity),
                new XElement("openGLBGOpacity", varianceContext.vc.BGOpacity));
            prefsXML.Root.Add(openGLPrefs);

            prefsXML.Root.Add(new XElement("friendlyNumber", varianceContext.vc.friendlyNumber));

            string equationString = "";
            for (int i = 0; i < commonVars.rngCustomMapping.Count; i++)
            {
                if (i > 0)
                {
                    equationString += ";";
                }
                equationString += commonVars.rngCustomMapping[i];
            }
            XElement rngMappingEquations = new("rngMappingEquations", equationString); //  commonVars.rngCustomMapping);
            prefsXML.Root.Add(rngMappingEquations);

            XElement colorPrefs = new("colors");

            XElement layer1Color = new("layer1Color",
                new XElement("R", varianceContext.vc.colors.layer1_Color.R),
                new XElement("G", varianceContext.vc.colors.layer1_Color.G),
                new XElement("B", varianceContext.vc.colors.layer1_Color.B));
            colorPrefs.Add(layer1Color);

            XElement layer2Color = new("layer2Color",
                new XElement("R", varianceContext.vc.colors.layer2_Color.R),
                new XElement("G", varianceContext.vc.colors.layer2_Color.G),
                new XElement("B", varianceContext.vc.colors.layer2_Color.B));
            colorPrefs.Add(layer2Color);

            XElement layer3Color = new("layer3Color",
                new XElement("R", varianceContext.vc.colors.layer3_Color.R),
                new XElement("G", varianceContext.vc.colors.layer3_Color.G),
                new XElement("B", varianceContext.vc.colors.layer3_Color.B));
            colorPrefs.Add(layer3Color);

            XElement layer4Color = new("layer4Color",
                new XElement("R", varianceContext.vc.colors.layer4_Color.R),
                new XElement("G", varianceContext.vc.colors.layer4_Color.G),
                new XElement("B", varianceContext.vc.colors.layer4_Color.B));
            colorPrefs.Add(layer4Color);

            XElement layer5Color = new("layer5Color",
                new XElement("R", varianceContext.vc.colors.layer5_Color.R),
                new XElement("G", varianceContext.vc.colors.layer5_Color.G),
                new XElement("B", varianceContext.vc.colors.layer5_Color.B));
            colorPrefs.Add(layer5Color);

            XElement layer6Color = new("layer6Color",
                new XElement("R", varianceContext.vc.colors.layer6_Color.R),
                new XElement("G", varianceContext.vc.colors.layer6_Color.G),
                new XElement("B", varianceContext.vc.colors.layer6_Color.B));
            colorPrefs.Add(layer6Color);

            XElement layer7Color = new("layer7Color",
                new XElement("R", varianceContext.vc.colors.layer7_Color.R),
                new XElement("G", varianceContext.vc.colors.layer7_Color.G),
                new XElement("B", varianceContext.vc.colors.layer7_Color.B));
            colorPrefs.Add(layer7Color);

            XElement layer8Color = new("layer8Color",
                new XElement("R", varianceContext.vc.colors.layer8_Color.R),
                new XElement("G", varianceContext.vc.colors.layer8_Color.G),
                new XElement("B", varianceContext.vc.colors.layer8_Color.B));
            colorPrefs.Add(layer8Color);

            XElement layer9Color = new("layer9Color",
                new XElement("R", varianceContext.vc.colors.layer9_Color.R),
                new XElement("G", varianceContext.vc.colors.layer9_Color.G),
                new XElement("B", varianceContext.vc.colors.layer9_Color.B));
            colorPrefs.Add(layer9Color);

            XElement layer10Color = new("layer10Color",
                new XElement("R", varianceContext.vc.colors.layer10_Color.R),
                new XElement("G", varianceContext.vc.colors.layer10_Color.G),
                new XElement("B", varianceContext.vc.colors.layer10_Color.B));
            colorPrefs.Add(layer10Color);

            XElement layer11Color = new("layer11Color",
                new XElement("R", varianceContext.vc.colors.layer11_Color.R),
                new XElement("G", varianceContext.vc.colors.layer11_Color.G),
                new XElement("B", varianceContext.vc.colors.layer11_Color.B));
            colorPrefs.Add(layer11Color);

            XElement layer12Color = new("layer12Color",
                new XElement("R", varianceContext.vc.colors.layer12_Color.R),
                new XElement("G", varianceContext.vc.colors.layer12_Color.G),
                new XElement("B", varianceContext.vc.colors.layer12_Color.B));
            colorPrefs.Add(layer12Color);

            XElement layer13Color = new("layer13Color",
                new XElement("R", varianceContext.vc.colors.layer13_Color.R),
                new XElement("G", varianceContext.vc.colors.layer13_Color.G),
                new XElement("B", varianceContext.vc.colors.layer13_Color.B));
            colorPrefs.Add(layer13Color);

            XElement layer14Color = new("layer14Color",
                new XElement("R", varianceContext.vc.colors.layer14_Color.R),
                new XElement("G", varianceContext.vc.colors.layer14_Color.G),
                new XElement("B", varianceContext.vc.colors.layer14_Color.B));
            colorPrefs.Add(layer14Color);

            XElement layer15Color = new("layer15Color",
                new XElement("R", varianceContext.vc.colors.layer15_Color.R),
                new XElement("G", varianceContext.vc.colors.layer15_Color.G),
                new XElement("B", varianceContext.vc.colors.layer15_Color.B));
            colorPrefs.Add(layer15Color);

            XElement layer16Color = new("layer16Color",
                new XElement("R", varianceContext.vc.colors.layer16_Color.R),
                new XElement("G", varianceContext.vc.colors.layer16_Color.G),
                new XElement("B", varianceContext.vc.colors.layer16_Color.B));
            colorPrefs.Add(layer16Color);

            XElement result1Color = new("result1Color",
                new XElement("R", varianceContext.vc.colors.result_Color.R),
                new XElement("G", varianceContext.vc.colors.result_Color.G),
                new XElement("B", varianceContext.vc.colors.result_Color.B));
            colorPrefs.Add(result1Color);

            XElement result2Color = new("result2Color",
                new XElement("R", varianceContext.vc.colors.result2_Color.R),
                new XElement("G", varianceContext.vc.colors.result2_Color.G),
                new XElement("B", varianceContext.vc.colors.result2_Color.B));
            colorPrefs.Add(result2Color);

            XElement result3Color = new("result3Color",
                new XElement("R", varianceContext.vc.colors.result3_Color.R),
                new XElement("G", varianceContext.vc.colors.result3_Color.G),
                new XElement("B", varianceContext.vc.colors.result3_Color.B));
            colorPrefs.Add(result3Color);

            XElement result4Color = new("result4Color",
                new XElement("R", varianceContext.vc.colors.result4_Color.R),
                new XElement("G", varianceContext.vc.colors.result4_Color.G),
                new XElement("B", varianceContext.vc.colors.result4_Color.B));
            colorPrefs.Add(result4Color);

            XElement minImplantColor = new("minImplantColor",
                new XElement("R", varianceContext.vc.colors.implantMin_Color.R),
                new XElement("G", varianceContext.vc.colors.implantMin_Color.G),
                new XElement("B", varianceContext.vc.colors.implantMin_Color.B));
            colorPrefs.Add(minImplantColor);

            XElement meanImplantColor = new("meanImplantColor",
                new XElement("R", varianceContext.vc.colors.implantMean_Color.R),
                new XElement("G", varianceContext.vc.colors.implantMean_Color.G),
                new XElement("B", varianceContext.vc.colors.implantMean_Color.B));
            colorPrefs.Add(meanImplantColor);

            XElement maxImplantColor = new("maxImplantColor",
                new XElement("R", varianceContext.vc.colors.implantMax_Color.R),
                new XElement("G", varianceContext.vc.colors.implantMax_Color.G),
                new XElement("B", varianceContext.vc.colors.implantMax_Color.B));
            colorPrefs.Add(maxImplantColor);

            XElement implantResistColor = new("implantResistColor",
                new XElement("R", varianceContext.vc.colors.implantResist_Color.R),
                new XElement("G", varianceContext.vc.colors.implantResist_Color.G),
                new XElement("B", varianceContext.vc.colors.implantResist_Color.B));
            colorPrefs.Add(implantResistColor);

            XElement subshape1Color = new("subshape1Color",
                new XElement("R", varianceContext.vc.colors.subshape1_Color.R),
                new XElement("G", varianceContext.vc.colors.subshape1_Color.G),
                new XElement("B", varianceContext.vc.colors.subshape1_Color.B));
            colorPrefs.Add(subshape1Color);

            XElement subshape2Color = new("subshape2Color",
                new XElement("R", varianceContext.vc.colors.subshape2_Color.R),
                new XElement("G", varianceContext.vc.colors.subshape2_Color.G),
                new XElement("B", varianceContext.vc.colors.subshape2_Color.B));
            colorPrefs.Add(subshape2Color);

            XElement subshape3Color = new("subshape3Color",
                new XElement("R", varianceContext.vc.colors.subshape3_Color.R),
                new XElement("G", varianceContext.vc.colors.subshape3_Color.G),
                new XElement("B", varianceContext.vc.colors.subshape3_Color.B));
            colorPrefs.Add(subshape3Color);

            XElement enabledColor = new("enabledColor",
                new XElement("R", varianceContext.vc.colors.enabled_Color.R),
                new XElement("G", varianceContext.vc.colors.enabled_Color.G),
                new XElement("B", varianceContext.vc.colors.enabled_Color.B));
            colorPrefs.Add(enabledColor);

            XElement axisColor = new("axisColor",
                new XElement("R", varianceContext.vc.colors.axis_Color.R),
                new XElement("G", varianceContext.vc.colors.axis_Color.G),
                new XElement("B", varianceContext.vc.colors.axis_Color.B));
            colorPrefs.Add(axisColor);

            XElement majorColor = new("majorColor",
                new XElement("R", varianceContext.vc.colors.major_Color.R),
                new XElement("G", varianceContext.vc.colors.major_Color.G),
                new XElement("B", varianceContext.vc.colors.major_Color.B));
            colorPrefs.Add(majorColor);

            XElement minorColor = new("minorColor",
                new XElement("R", varianceContext.vc.colors.minor_Color.R),
                new XElement("G", varianceContext.vc.colors.minor_Color.G),
                new XElement("B", varianceContext.vc.colors.minor_Color.B));
            colorPrefs.Add(minorColor);

            XElement backgroundColor = new("backgroundColor",
                new XElement("R", varianceContext.vc.colors.background_Color.R),
                new XElement("G", varianceContext.vc.colors.background_Color.G),
                new XElement("B", varianceContext.vc.colors.background_Color.B));
            colorPrefs.Add(backgroundColor);

            prefsXML.Root.Add(colorPrefs);

            prefsXML.Save(filename);
        }
        catch (Exception)
        {
            ErrorReporter.showMessage_OK("Failed to save preferences", "Error");
        }
    }

    private void uiVars()
    {
        // Figure out host UI element sizes to assist in layout, at least vertically.
        // Set controls to null afterwards.
        label_Height = 13;
            
        simulationOutputGroupBoxHeight = 57;
        simulationSettingsGroupBoxHeight = 180;
        userGuidanceWidth = 395;
        userGuidanceHeight = simulationOutputGroupBoxHeight + simulationSettingsGroupBoxHeight + 22;

        simButtonWidth = 64;
        simButtonHeight = 55;

        replayNumWidth = 80;

        multiThreadWarnWidth = 300;
        resultFieldWidth = 344;

        previewWidth = 450;

        oneDGuidanceWidth = 271;

        oneDLabelWidth = 132;

        oneDMinInsLblWidth = 100;

        oneDMinInsWidth = 100;

        commentBoxWidth = previewWidth + 2;
        commentBoxHeight = 160 - label_Height * 2;
    }

    private void createLBContextMenu()
    {
        listbox_menu = new ContextMenu();
        int itemIndex = 0;
        lb_copy = new ButtonMenuItem { Text = "Copy" };
        listbox_menu.Items.Add(lb_copy);
        listbox_menu.Items[itemIndex].Click += delegate
        {
            copy();
        };
        itemIndex++;
        lb_paste = new ButtonMenuItem { Text = "Paste" };
        listbox_menu.Items.Add(lb_paste);
        listbox_menu.Items[itemIndex].Click += delegate
        {
            paste();
        };
        itemIndex++;
        lb_clear = new ButtonMenuItem { Text = "Clear" };
        listbox_menu.Items.Add(lb_clear);
        listbox_menu.Items[itemIndex].Click += delegate
        {
            clear();
        };
        itemIndex++;
        lb_enableDisable = new ButtonMenuItem { Text = "Enable" };
        listbox_menu.Items.Add(lb_enableDisable);
        listbox_menu.Items[itemIndex].Click += delegate
        {
            enableDisable();
        };
        // itemIndex++;
    }

    private void updateLBContextMenu()
    {
        try
        {
            lb_paste.Enabled = commonVars.isCopyPrepped();
        }
        catch (Exception)
        {
            lb_paste.Enabled = false;
        }

        int index = getSelectedLayerIndex();
        lb_enableDisable.Text = commonVars.getLayerSettings(index).getInt(EntropyLayerSettings.properties_i.enabled) == 1 ? "Disable" : "Enable";
    }

    private void enableDisable()
    {
        int index = getSelectedLayerIndex();
        commonVars.getLayerSettings(index).setInt(EntropyLayerSettings.properties_i.enabled,
            commonVars.getLayerSettings(index).getInt(EntropyLayerSettings.properties_i.enabled) == 1 ? 0 : 1);
        set_ui_from_settings(index);
        do2DLayerUI(index);
    }


    public MainForm(VarianceContextGUI _varianceContext)
    {
        pMainForm(_varianceContext);
    }

    private void pMainForm(VarianceContextGUI _varianceContext)
    {
        // Figure out whether we should display the help menu, if documentation is available.
        helpPath = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), "Documentation", "index.html");
        helpAvailable = File.Exists(helpPath);

        UI(_varianceContext);
    }

    private bool _veldridReady;

    private bool VeldridReady
    {
        get => _veldridReady;
        set
        {
            _veldridReady = value;

            SetUpVeldrid();
        }
    }

    private bool _formReady;

    private bool FormReady
    {
        get => _formReady;
        set
        {
            _formReady = value;

            SetUpVeldrid();
        }
    }

    private void SetUpVeldrid()
    {
        if (!(FormReady && VeldridReady))
        {
            return;
        }

        viewPort.SetUpVeldrid();

        viewPort.Clock.Start();
        createVPContextMenu();
    }

    private void UI(VarianceContextGUI _varianceContext)
    {
        if (_varianceContext == null) // safety net.
        {
            int HTCount = Environment.ProcessorCount;
            varianceContext = new VarianceContextGUI(false, "", -1, HTCount, VeldridSurface.PreferredBackend);
        }
        else
        {
            varianceContext = _varianceContext;
        }

        Shown += (_, _) => FormReady = true;

        Resizable = true;
        Maximizable = true;

        loadPrefs();

        uiVars();

        createLBContextMenu();

        commonVars = new CommonVars(varianceContext.vc);
        simReplay = new Replay(ref commonVars);

        geoGBVisible = new bool[CentralProperties.maxLayersForMC];
        subShapeGBVisible = new bool[CentralProperties.maxLayersForMC];
        booleanGBVisible = new bool[CentralProperties.maxLayersForMC];

        viewport_settings();

        notList = new List<string> { "", "!" };
        booleanList = new List<string> { "&", "|" };

        mainTable = new TableLayout();

        setupUIDataContext(notList, booleanList);

        GraphicsDeviceOptions options = new(
            false,
            PixelFormat.R32_Float,
            false,
            ResourceBindingModel.Improved);

        vSurface = new VeldridSurface(varianceContext.backend, options);
        vSurface.VeldridInitialized += (_, _) => VeldridReady = true;

        commonVars.titleText += " " + vSurface.Backend;

        Title = commonVars.titleText;

        string exeDirectory;
        string shaders;
        if (Platform.IsMac)
        {
            // AppContext.BaseDirectory is too simple for the case of the Mac
            // projects. When building an app bundle that depends on the Mono
            // framework being installed, it properly returns the path of the
            // executable in Eto.Veldrid.app/Contents/MacOS. When building an
            // app bundle that instead bundles Mono by way of mkbundle, on the
            // other hand, it returns the directory containing the .app.

            // ReSharper disable once PossibleNullReferenceException
            exeDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            shaders = Path.Combine("..", "Resources", "shaders");
        }
        else
        {
            exeDirectory = AppContext.BaseDirectory;
            shaders = "shaders";
        }

        viewPort = new VeldridDriver(ref mcVPSettings[0], ref vSurface)
        {
            Surface = vSurface,
            ExecutableDirectory = exeDirectory,
            ShaderSubdirectory = shaders
        };

        vSurface.Size = new Size(viewportSize, viewportSize);
        viewPort.updateHostFunc = viewportUpdateHost;

        const string viewportToolTipText = "(w/a/s/d) to navigate\r\n(r) to reset\r\n(n/m) to zoom\r\n(f) to freeze/thaw\r\n(x) to zoom extents";
        vp = new Panel();
        if (!Platform.IsMac) // This color setting causes issues on Mac where the viewport doesn't show until the mouse passes over.
        {
            vp.BackgroundColor = Colors.Black;
        }
        TableLayout vp_content = new();
        vp_content.Rows.Add(new TableRow());
        vp_content.Rows[0].Cells.Add(new TableCell { Control = vSurface });
        vp.Content = vp_content;
        vp.ToolTip = viewportToolTipText;

        vp.Content = vSurface;

        // Splitter and the like.
        setup_layout();

        setup_simPreviewBox();

        // Configure the tab UI.
        setup_tabs();

        oneDTabSetup();

        implantTabSetup();

        twoD_SettingsUISetup();
        twoD_DOEUISetup();
        paSearchUI_setup();
        utilsUISetup();

        rngLabelIndex = -1;

        commands();

        TableLayout gadgets_tableLayout = new();

        controls.Content = TableLayout.AutoSized(gadgets_tableLayout, centered: true);

        gadgets_tableLayout.Rows.Add(new TableRow());
        TableCell gtc0 = new();
        gadgets_tableLayout.Rows[0].Cells.Add(gtc0);

        // Upper section needs a nested layout

        upperGadgets_panel = new Panel();
        gtc0.Control = TableLayout.AutoSized(upperGadgets_panel, centered: true);

        upperGadgets_table = new TableLayout();
        upperGadgets_panel.Content = upperGadgets_table;

        upperGadgets_table.Rows.Add(new TableRow());
        TableCell u_gtc0 = new();

        upperGadgets_table.Rows[0].Cells.Add(u_gtc0);
        setupOmitLayerCheckboxes(u_gtc0);
        upperGadgets_table.Rows[0].Cells.Add(new TableCell { Control = null });

        upperGadgets_table.Rows.Add(new TableRow());
        TableCell u_gtc1 = new();
        upperGadgets_table.Rows[1].Cells.Add(u_gtc1);
        setupBGLayerCheckboxes(u_gtc1);
        upperGadgets_table.Rows[1].Cells.Add(new TableCell { Control = null });

        // Lower section

        gadgets_tableLayout.Rows.Add(new TableRow());
        TableCell gtc2 = new();
        setupComment(gtc2);
        gadgets_tableLayout.Rows[^1].Cells.Add(gtc2);
        gadgets_tableLayout.Rows[^1].Cells.Add(new TableCell { Control = null });

        // Buttons
        gadgets_tableLayout.Rows.Add(new TableRow());
        TableCell gtc3 = new();
        setup_buttons(gtc3);
        gadgets_tableLayout.Rows[^1].Cells.Add(gtc3);
        gadgets_tableLayout.Rows[^1].Cells.Add(new TableCell { Control = null });

        // Configure checkboxes and zoom gadgets.
        gadgets_tableLayout.Rows.Add(new TableRow());
        TableCell gtc4 = new();
        viewport_gadgets(gtc4);
        gadgets_tableLayout.Rows[^1].Cells.Add(gtc4);
        gadgets_tableLayout.Rows[^1].Cells.Add(new TableCell { Control = null });

        gadgets_tableLayout.Rows[^1].ScaleHeight = true;

        setupGUI();

        if (varianceContext.vc.xmlFileArg != "")
        {
            updateStatusLine("Loading project. Please wait.");
            string loaderString = commonVars.storage.loadSimulationSettings(currentVersion: CentralProperties.version, 
                filename: varianceContext.vc.xmlFileArg, 
                simulationSettings: commonVars.getSimulationSettings(), 
                simulationSettings_nonSim: commonVars.getSimulationSettings_nonSim(), 
                listOfSettings: commonVars.getListOfSettings(), 
                implantSimSettings: commonVars.getImplantSimulationSettings(), 
                implantSettings_nonSim: commonVars.getImplantSettings_nonSim(), 
                implantSettings_: commonVars.getImplantSettings(), 
                nonSimulationSettings_: commonVars.getNonSimulationSettings()
            );
            if (loaderString != "")
            {
                updateStatusLine("Project loading failed.");
                ErrorReporter.showMessage_OK(loaderString, "Error during load");
            }
            else
            {
                updateStatusLine("Project loaded successfully");
                commonVars.projectFileName = varianceContext.vc.xmlFileArg;
                Title = commonVars.titleText + " - " + commonVars.projectFileName;
            }
        }
        tabControl_main.SelectedIndexChanged += mainTabHandler;

        setReplayControls();

        globalUIFrozen = false;
        layerUIFrozen_exp = false;
        entropySettingsChanged(null, EventArgs.Empty);
        createVPContextMenu();
        viewPort.reset();
        viewPort.updateViewport();

        setDefaultIndices();
        do2DLayerUI(0, updateUI: true);
        addAllUIHandlers();
        commonVars.setHashes();

        tabControl_main.DragEnter += dragEvent;
        tabControl_main.DragOver += dragEvent;

        tabControl_main.DragDrop += dragAndDrop;
    }

    private enum layerLookUpOrder { ICV, OCV, SCDU, TCDU, XOL, YOL, HTPV, HTNV, VTPV, VTNV, WOB, LWR, LWR2 }

    private void setDefaultIndices()
    {
        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            comboBox_geoEqtn_Op[i].SelectedIndex = 0;
        }

        foreach (DropDown t in comboBox_geoEqtn_Op_2Layer)
        {
            t.SelectedIndex = 0;
        }

        foreach (DropDown t in comboBox_geoEqtn_Op_4Layer)
        {
            t.SelectedIndex = 0;
        }

        foreach (DropDown t in comboBox_geoEqtn_Op_8Layer)
        {
            t.SelectedIndex = 0;
        }

        comboBox_calcModes.SelectedIndex = 0;

        comboBox_RNG.SelectedIndex = 0;

        comboBox_implantRNG.SelectedIndex = 0;

        try
        {
            listBox_layers.SelectedIndex = 0;
        }
        catch (Exception)
        {

        }
    }

    private void zoomChanged(object sender, EventArgs e)
    {
        pZoomChanged();
    }

    private void pZoomChanged()
    {
        if (openGLErrorReported)
        {
            return;
        }
        if (globalUIFrozen)
        {
            return;
        }

        double zoomValue = Convert.ToDouble(num_viewportZoom.Value);
        if (zoomValue < 0.0001)
        {
            zoomValue = 0.0001;
        }

        viewPort.ovpSettings.setZoomFactor((float)(1.0f / zoomValue));

        updateViewport();
    }

    private void posChanged(object sender, EventArgs e)
    {
        if (openGLErrorReported)
        {
            return;
        }
        if (globalUIFrozen)
        {
            return;
        }

        moveCamera((float)num_viewportX.Value, (float)num_viewportY.Value);
    }

    private void moveCamera(float x, float y)
    {
        viewPort.ovpSettings.setCameraPos(x, y);
        updateViewport();
    }

    private void resetViewPorts()
    {
        foreach (OVPSettings t in mcVPSettings)
        {
            t.fullReset();
        }

        otkVPSettings_implant.fullReset();
    }

    private void viewportUpdateHost()
    {
        num_viewportZoom.ValueChanged -= zoomChanged;
        num_viewportZoom.Value = 1.0f / viewPort.ovpSettings.getZoomFactor(); // use the 'ref' nature of the settings to pull the corresponding zoom value to the UI.
        num_viewportX.ValueChanged -= posChanged;
        num_viewportY.ValueChanged -= posChanged;
        num_viewportX.Value = viewPort.ovpSettings.getCameraX();
        num_viewportY.Value = viewPort.ovpSettings.getCameraY();
        num_viewportX.ValueChanged += posChanged;
        num_viewportY.ValueChanged += posChanged;
        num_viewportZoom.ValueChanged += zoomChanged;
        createVPContextMenu();
    }

    private void createVPContextMenu()
    {
        if (viewPort.dragging)
        {
            return;
        }
        // Single viewport now mandates regeneration of the context menu each time, to allow for entry screening.
        vp_menu = new ContextMenu();

        int mainIndex = getMainSelectedIndex();
        int subIndex = getSubTabSelectedIndex();

        int itemIndex = 0;
        vp_menu.Items.Add(new ButtonMenuItem { Text = "Reset (r)" });
        vp_menu.Items[itemIndex].Click += delegate
        {
            viewPort.reset();
            updateViewport();
        };
        itemIndex++;

        var VPMenuDisplayOptionsMenu = vp_menu.Items.GetSubmenu("Display Options");
        itemIndex++;
        int displayOptionsSubItemIndex = 0;
        VPMenuDisplayOptionsMenu.Items.Add(new ButtonMenuItem { Text = "Toggle AA" });
        VPMenuDisplayOptionsMenu.Items[displayOptionsSubItemIndex].Click += delegate
        {
            viewPort.ovpSettings.aA(!viewPort.ovpSettings.aA());
            updateViewport();
        };
        displayOptionsSubItemIndex++;
        VPMenuDisplayOptionsMenu.Items.Add(new ButtonMenuItem { Text = "Toggle Fill" });
        VPMenuDisplayOptionsMenu.Items[displayOptionsSubItemIndex].Click += delegate
        {
            viewPort.ovpSettings.drawFilled(!viewPort.ovpSettings.drawFilled());
            if (mainIndex == (int)CommonVars.upperTabNames.Implant)
            {
                doImplantShadowing();
            }
            else
            {
                switch (subIndex)
                {
                    case (int)CommonVars.twoDTabNames.settings:
                    case (int)CommonVars.twoDTabNames.paSearch:
                        entropySettingsChanged(null);
                        break;
                    case (int)CommonVars.twoDTabNames.DOE:
                        doeSettingsChanged();
                        break;
                    default:
                        generatePreviewPanelContent(getSelectedLayerIndex());
                        break;
                }
            }
            updateViewport();
        };
        displayOptionsSubItemIndex++;
        VPMenuDisplayOptionsMenu.Items.Add(new ButtonMenuItem { Text = "Toggle Points" });
        VPMenuDisplayOptionsMenu.Items[displayOptionsSubItemIndex].Click += delegate
        {
            viewPort.ovpSettings.drawPoints(!viewPort.ovpSettings.drawPoints());
            updateViewport();
        };

        if (mainIndex != (int)CommonVars.upperTabNames.Implant)
        {
            vp_menu.Items.Add(viewPort.ovpSettings.isLocked()
                ? new ButtonMenuItem {Text = "Thaw (f)"}
                : new ButtonMenuItem {Text = "Freeze (f)"});
            freezeThawIndex = itemIndex;
            vp_menu.Items[freezeThawIndex].Click += delegate
            {
                viewPort.freeze_thaw();
            };
            itemIndex++;
            vp_menu.Items.AddSeparator();
            itemIndex++;
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Save bookmark" });
            vp_menu.Items[itemIndex].Click += delegate
            {
                viewPort.saveLocation();
            };
            itemIndex++;
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Load bookmark" });
            vp_menu.Items[itemIndex].Click += delegate
            {
                viewPort.loadLocation();
            };
            loadBookmarkIndex = itemIndex;
            if (!viewPort.savedLocation_valid)
            {
                vp_menu.Items[loadBookmarkIndex].Enabled = false;
            }
            itemIndex++;
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Use location for all viewports" });
            vp_menu.Items[itemIndex].Click += delegate
            {
                applyLocationToViewports(viewPort.ovpSettings.getCameraPos());
            };
            itemIndex++;
            if (subIndex == (int)CommonVars.twoDTabNames.DOE)
            {
                vp_menu.Items.Add(new ButtonMenuItem { Text = "Go to bottom left corner" });
                vp_menu.Items[itemIndex].Click += delegate
                {
                    double blX = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                    double blY = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + subIndex].setCameraPos((float)blX, (float)blY);
                };
                itemIndex++;
            }
        }
        vp_menu.Items.AddSeparator();
        itemIndex++;
        vp_menu.Items.Add(new ButtonMenuItem { Text = "Zoom Extents (x)" });
        vp_menu.Items[itemIndex].Click += delegate
        {
            viewPort.zoomExtents(-1);
        };
        itemIndex++;
        vp_menu.Items.AddSeparator();
        itemIndex++;
        vp_menu.Items.Add(new ButtonMenuItem { Text = "Zoom In (m)" });
        vp_menu.Items[itemIndex].Click += delegate
        {
            viewPort.zoomIn(-1);
            updateViewport();
        };
        itemIndex++;

        var VPMenuZoomInMenu = vp_menu.Items.GetSubmenu("Fast Zoom In");
        itemIndex++;
        int zoomInSubItemIndex = 0;
        VPMenuZoomInMenu.Items.Add(new ButtonMenuItem { Text = "Zoom In (x5)" });
        VPMenuZoomInMenu.Items[zoomInSubItemIndex].Click += delegate
        {
            viewPort.fastZoomIn(-50);
            updateViewport();
        };
        zoomInSubItemIndex++;
        VPMenuZoomInMenu.Items.Add(new ButtonMenuItem { Text = "Zoom In (x10)" });
        VPMenuZoomInMenu.Items[zoomInSubItemIndex].Click += delegate
        {
            viewPort.fastZoomIn(-100);
            updateViewport();
        };
        zoomInSubItemIndex++;
        VPMenuZoomInMenu.Items.Add(new ButtonMenuItem { Text = "Zoom In (x50)" });
        VPMenuZoomInMenu.Items[zoomInSubItemIndex].Click += delegate
        {
            viewPort.fastZoomIn(-500);
            updateViewport();
        };
        zoomInSubItemIndex++;
        VPMenuZoomInMenu.Items.Add(new ButtonMenuItem { Text = "Zoom In (x100)" });
        VPMenuZoomInMenu.Items[zoomInSubItemIndex].Click += delegate
        {
            viewPort.fastZoomIn(-1000);
            updateViewport();
        };

        vp_menu.Items.AddSeparator();
        itemIndex++;

        vp_menu.Items.Add(new ButtonMenuItem { Text = "Zoom Out (n)" });
        vp_menu.Items[itemIndex].Click += delegate
        {
            viewPort.zoomOut(-1);
            updateViewport();
        };
        itemIndex++;

        var VPMenuZoomOutMenu = vp_menu.Items.GetSubmenu("Fast Zoom Out");
        itemIndex++;
        int zoomOutSubItemIndex = 0;
        VPMenuZoomOutMenu.Items.Add(new ButtonMenuItem { Text = "Zoom Out (x5)" });
        VPMenuZoomOutMenu.Items[zoomOutSubItemIndex].Click += delegate
        {
            viewPort.fastZoomOut(-50);
            updateViewport();
        };
        zoomOutSubItemIndex++;
        VPMenuZoomOutMenu.Items.Add(new ButtonMenuItem { Text = "Zoom Out (x10)" });
        VPMenuZoomOutMenu.Items[zoomOutSubItemIndex].Click += delegate
        {
            viewPort.fastZoomOut(-100);
            updateViewport();
        };
        zoomOutSubItemIndex++;
        VPMenuZoomOutMenu.Items.Add(new ButtonMenuItem { Text = "Zoom Out (x50)" });
        VPMenuZoomOutMenu.Items[zoomOutSubItemIndex].Click += delegate
        {
            viewPort.fastZoomOut(-500);
            updateViewport();
        };
        zoomOutSubItemIndex++;
        VPMenuZoomOutMenu.Items.Add(new ButtonMenuItem { Text = "Zoom Out (x100)" });
        VPMenuZoomOutMenu.Items[zoomOutSubItemIndex].Click += delegate
        {
            viewPort.fastZoomOut(-1000);
            updateViewport();
        };

        if (mainIndex != (int)CommonVars.upperTabNames.Implant)
        {
            vp_menu.Items[freezeThawIndex].Text = viewPort.ovpSettings.isLocked() ? "Thaw (f)" : "Freeze (f)";

            vp_menu.Items[loadBookmarkIndex].Enabled = viewPort.savedLocation_valid;
        }

        int svgIndex = itemIndex + 1;

        vp_menu.Items.AddSeparator();

        vp_menu.Items.Add(new ButtonMenuItem { Text = "Save SVG" });
        vp_menu.Items[svgIndex].Click += delegate
        {
            saveViewportSVG_File();
        };

        int layoutIndex = svgIndex + 1;
        vp_menu.Items.Add(new ButtonMenuItem { Text = "Save Layout" });
        vp_menu.Items[layoutIndex].Click += delegate
        {
            if (mainIndex == (int)CommonVars.upperTabNames.twoD && subIndex == (int)CommonVars.twoDTabNames.layer)
            {
                exportActiveLayerToLayout();
            }
            else
            {
                saveViewportLayout_File();
            }
        };

        viewPort.setContextMenu(ref vp_menu);
    }

    private void setup_tabs()
    {
        tabPage2_table = new TableLayout();
        tabPage2 = new TabPage {Text = "2D Calculation", Content = tabPage2_table};

        tabControl_main.Pages.Add(tabPage2);

        Scrollable tabPage_implant_scrollable = new();
        tabPage_implant_table = new TableLayout();
        tabPage_implant_scrollable.Content = tabPage_implant_table;

        tabPage_implant = new TabPage {Text = "Implant", Content = tabPage_implant_scrollable};


        tabControl_main.Pages.Add(tabPage_implant);

        tab_1DCalc = new TabPage {Text = "1D Calculation"};
        tabControl_main.Pages.Add(tab_1DCalc);

        tabPage_utilities_table = new TableLayout();
        Scrollable tabPage_utilities_scrollable = new() {Content = tabPage_utilities_table};

        tabPage_utilities = new TabPage {Text = "Utils", Content = tabPage_utilities_scrollable};


        tabControl_main.Pages.Add(tabPage_utilities);

        tabPage2_table.Rows.Add(new TableRow());

        tabControl_2D_simsettings = new TabControl();
        tabPage2_table.Rows[^1].Cells.Add(new TableCell { Control = tabControl_2D_simsettings });

        Scrollable tabPage_2D_Settings_scrollable = new();
        tabPage_2D_Settings_table = new TableLayout();
        tabPage_2D_Settings_scrollable.Content = tabPage_2D_Settings_table;

        twoD_LayerUISetup();

        tabPage_2D_Settings = new TabPage {Text = "Settings", Content = tabPage_2D_Settings_scrollable};
        tabControl_2D_simsettings.Pages.Add(tabPage_2D_Settings);

        tabPage_2D_DOE_table = new TableLayout();
        Scrollable tabPage_2D_DOE_scrollable = new() {Content = tabPage_2D_DOE_table};

        tabPage_2D_DOE = new TabPage {Text = "DOE", Content = tabPage_2D_DOE_scrollable};

        tabControl_2D_simsettings.Pages.Add(tabPage_2D_DOE);

        tabPage_2D_PASearch_table = new TableLayout();
        tabPage_2D_PASearch_scrollable = new Scrollable {Content = tabPage_2D_PASearch_table};

        tabPage_2D_PASearch = new TabPage {Text = "PA Search", Content = tabPage_2D_PASearch_scrollable};


        tabControl_2D_simsettings.Pages.Add(tabPage_2D_PASearch);
    }

    private void setup_layout()
    {
        // mainPanel is tab UI.
        Panel mainPanel = new() {Size = new Size(920, 800), Content = mainTable};
        // force the UI out to contain the panel. Hope this will be redundant eventually with the table UI.
        // rightPanel will take viewport and controls.
        Panel rightPanel = new();
        Panel mainSplitter = new()
        {
            Content = new Splitter
            {
                Orientation = Orientation.Horizontal,
                FixedPanel = SplitterFixedPanel.Panel1,
                Panel1 = mainPanel,
                Panel2 = rightPanel
            }
        };

        // Controls
        controls = new Scrollable();

        rightPanel.Content = new Splitter
        {
            Orientation = Orientation.Vertical,
            FixedPanel = SplitterFixedPanel.Panel1,
            Panel1 = controls,
            Panel2 = vp
        };

        // We need an additional level here to take the status label and progress bar in the lower portion of the UI.
        Panel outerPanel_upper = new() {Content = mainSplitter};
        TableLayout outerPanel_lower_content = new();
        Panel outerPanel_lower = new() {Content = outerPanel_lower_content};
        TableRow oplc_row0 = new();
        outerPanel_lower_content.Rows.Add(oplc_row0);
        Panel outerPanel = new()
        {
            Content = new Splitter
            {
                Orientation = Orientation.Vertical,
                FixedPanel = SplitterFixedPanel.Panel2,
                Panel1 = outerPanel_upper,
                Panel2 = outerPanel_lower
            }
        };

        // Assign the outer panel to the contents.
        Content = outerPanel;

        mainTable.Rows.Add(new TableRow());

        tabControl_main = new TabControl();
        mainTable.Rows[0].Cells.Add(new TableCell { Control = tabControl_main });

        // Stick our progress indicator and status label in here.
        statusReadout = new Label {Text = "Welcome", TextAlignment = TextAlignment.Center};
        oplc_row0.Cells.Add(new TableCell { Control = statusReadout, ScaleWidth = true });

        statusProgressBar = new ProgressBar {Value = 0, Size = new Size(100, label_Height)};
        oplc_row0.Cells.Add(new TableCell { Control = statusProgressBar, ScaleWidth = false });
    }

    private void setupComment(TableCell tc)
    {
        commentBox = new RichTextArea {Wrap = true, ReadOnly = false};
        commentBox.LostFocus += commentChanged;
        Panel p = new() {Content = commentBox};
        tc.Control = TableLayout.AutoSized(p, centered: true);
        setSize(commentBox, commentBoxWidth, commentBoxHeight);
    }

    private void launchHelp(object sender, EventArgs e)
    {
        if (helpAvailable)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(helpPath)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
    }

    private void commands()
    {
        Closed += quitHandler;

        quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
        quitCommand.Executed += quit;
        Application.Instance.Terminating += quitHandler; // write our prefs out.

        aboutCommand = new Command { MenuText = "About..." };
        aboutCommand.Executed += aboutMe;

        helpCommand = new Command { MenuText = "Help...", Shortcut = Keys.F1 };
        helpCommand.Executed += launchHelp;

        clearLayer = new Command { MenuText = "Clear Layer", ToolBarText = "Clear Layer", Shortcut = Application.Instance.CommonModifier | Keys.X };
        clearLayer.Executed += clearHandler;

        copyLayer = new Command { MenuText = "Copy", ToolBarText = "Copy", Shortcut = Application.Instance.CommonModifier | Keys.C };
        copyLayer.Executed += copyHandler;

        pasteLayer = new Command { MenuText = "Paste", ToolBarText = "Paste", Shortcut = Application.Instance.CommonModifier | Keys.V };
        pasteLayer.Executed += pasteHandler;

        newSim = new Command { MenuText = "New", ToolBarText = "New", Shortcut = Application.Instance.CommonModifier | Keys.N };
        newSim.Executed += newHandler;

        openSim = new Command { MenuText = "Open", ToolBarText = "Open", Shortcut = Application.Instance.CommonModifier | Keys.O };
        openSim.Executed += openHandler;

        revertSim = new Command { MenuText = "Revert", ToolBarText = "Revert", Shortcut = Application.Instance.CommonModifier | Keys.R };
        revertSim.Executed += revertHandler;
        revertSim.Enabled = false;

        saveSim = new Command { MenuText = "Save", ToolBarText = "Save", Shortcut = Application.Instance.CommonModifier | Keys.S };
        saveSim.Executed += saveHandler;

        saveAsSim = new Command { MenuText = "Save As", ToolBarText = "Save As", Shortcut = Application.Instance.CommonModifier | Keys.Shift | Keys.S };
        saveAsSim.Executed += saveAsHandler;

        // create menu
        Menu = new MenuBar
        {
            Items = {
                //File submenu
                new ButtonMenuItem { Text = "&File", Items = { newSim, openSim, revertSim, saveSim, saveAsSim } },
                new ButtonMenuItem { Text = "&Edit", Items = { copyLayer, pasteLayer, clearLayer } }
            },
            QuitItem = quitCommand,
            HelpItems = {
                helpCommand
            },
            AboutItem = aboutCommand
        };

        helpCommand.Enabled = helpAvailable;
    }

    private void setup_buttons(TableCell tc)
    {
        Panel p = new();
        tc.Control = TableLayout.AutoSized(p, centered: true);

        TableLayout buttons_table = new();
        p.Content = buttons_table;
        buttons_table.Rows.Add(new TableRow());

        btn_singleCPU = new Button {Text = "Single\r\nCPU"};
        btn_singleCPU.Click += monteCarloSingleThreadEventHandler;
        setSize(btn_singleCPU, simButtonWidth, simButtonHeight);
        // buttons_table.Rows[0].Cells.Add(new TableCell() { Control = btn_singleCPU });

        btn_multiCPU = new Button {Text = "Multi\r\nCPU"};
        setSize(btn_multiCPU, simButtonWidth, simButtonHeight);
        btn_multiCPU.Click += monteCarloMultipleThreadEventHandler;
        buttons_table.Rows[0].Cells.Add(new TableCell { Control = btn_multiCPU });

        btn_Cancel = new Button {Text = "Cancel"};
        setSize(btn_Cancel, simButtonWidth, simButtonHeight);
        btn_Cancel.Click += btnCancel;
        buttons_table.Rows[0].Cells.Add(new TableCell { Control = btn_Cancel });

        btn_STOP = new Button {Text = "STOP"};
        setSize(btn_STOP, simButtonWidth, simButtonHeight);
        btn_STOP.Click += btnSTOP;
        buttons_table.Rows[0].Cells.Add(new TableCell { Control = btn_STOP });
    }

    private void viewport_gadgets(TableCell tc)
    {
        TableLayout viewport_gadgets_tl = new();
        Panel p = new() {Content = viewport_gadgets_tl};
        tc.Control = p; // TableLayout.AutoSized(p, centered: true);

        viewport_gadgets_tl.Rows.Add(new TableRow());

        lbl_simPreviewZoom = new Label {Text = "Zoom"};
        viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell { Control = lbl_simPreviewZoom });

        num_viewportZoom = new NumericStepper {DecimalPlaces = 2, Increment = 0.01, Value = 1.0, MinValue = 0.001};
        setSize(num_viewportZoom, 55);
        num_viewportZoom.LostFocus += zoomChanged;
        viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_viewportZoom) });

        viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell { Control = null, ScaleWidth = true }); // padding

        lbl_viewportPos = new Label {Text = "Position"};
        viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell { Control = lbl_viewportPos });

        num_viewportX = new NumericStepper {DecimalPlaces = 2, Increment = 0.01, Value = 0};
        setSize(num_viewportX, 75);
        num_viewportX.LostFocus += posChanged;
        viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_viewportX) });

        num_viewportY = new NumericStepper {DecimalPlaces = 2, Increment = 0.01, Value = 0};
        setSize(num_viewportY, 75);
        num_viewportY.LostFocus += posChanged;
        viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_viewportY) });
    }

    private void viewport_settings()
    {
        mcVPSettings = new OVPSettings[CentralProperties.maxLayersForMC + 1 + 1]; // 16 layers, plus simulation and DOE
        for (int i = 0; i < mcVPSettings.Length; i++)
        {
            mcVPSettings[i] = new OVPSettings();
            mcVPSettings[i].setBaseZoom(0.25f);
            mcVPSettings[i].drawFilled(commonVars.getOpenGLProp(CommonVars.properties_gl.fill));
            mcVPSettings[i].drawPoints(commonVars.getOpenGLProp(CommonVars.properties_gl.points));
            mcVPSettings[i].minorGridColor = Color.FromArgb(commonVars.getColors().minor_Color.toArgb());
            mcVPSettings[i].majorGridColor = Color.FromArgb(commonVars.getColors().major_Color.toArgb());
            mcVPSettings[i].axisColor = Color.FromArgb(commonVars.getColors().axis_Color.toArgb());
            mcVPSettings[i].backColor = Color.FromArgb(commonVars.getColors().background_Color.toArgb());
        }
        mcVPSettings[^2].drawDrawn(true);
        mcVPSettings[^1].drawDrawn(true);

        otkVPSettings_implant = new OVPSettings(45.0f, 45.0f);
        otkVPSettings_implant.setBaseZoom(0.25f);
        otkVPSettings_implant.drawDrawn(true);
        otkVPSettings_implant.drawFilled(commonVars.getOpenGLProp(CommonVars.properties_gl.fill));
        otkVPSettings_implant.drawPoints(commonVars.getOpenGLProp(CommonVars.properties_gl.points));
        otkVPSettings_implant.minorGridColor = Color.FromArgb(commonVars.getColors().minor_Color.toArgb());
        otkVPSettings_implant.majorGridColor = Color.FromArgb(commonVars.getColors().major_Color.toArgb());
        otkVPSettings_implant.axisColor = Color.FromArgb(commonVars.getColors().axis_Color.toArgb());
        otkVPSettings_implant.backColor = Color.FromArgb(commonVars.getColors().background_Color.toArgb());
    }

    private void setupOmitLayerCheckboxes(TableCell tc)
    {
        TableLayout omitLayerBox_table = new();
        omitLayerBox_table.Rows.Add(new TableRow());
        omitLayerBox_table.Rows.Add(new TableRow());

        omitLayerBox = new GroupBox {Text = "Omit layers (Boolean)", Content = omitLayerBox_table};

        Panel p = new() {Content = omitLayerBox};
        tc.Control = TableLayout.AutoSized(p, centered: true);

        cB_omit = new CheckBox[CentralProperties.maxLayersForMC];
        int rowIndex = 0;
        for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
        {
            // Don't add a button for our current layer
            cB_omit[cb] = new CheckBox {Text = (cb + 1).ToString()};

            TableCell tc0 = new() {Control = cB_omit[cb]};
            omitLayerBox_table.Rows[rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (cb + 1 == CentralProperties.maxLayersForMC / 2)
            {
                rowIndex++;
            }
        }
    }

    private void updateOmitLayerCheckboxes()
    {
        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            cB_omit[i].CheckedChanged -= omitLayerCheckboxChanged;
            cB_omit[i].Checked = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.omit) == 1;
            cB_omit[i].CheckedChanged += omitLayerCheckboxChanged;
        }
    }

    private void setupBGLayerCheckboxes(TableCell tc)
    {
        bgLayerBox_table = new TableLayout();
        bgLayerBox = new GroupBox {Text = "Background layers", Content = bgLayerBox_table};

        bgLayerBox_table.Rows.Add(new TableRow());
        bgLayerBox_table.Rows.Add(new TableRow());

        Panel p = new() {Content = bgLayerBox};
        tc.Control = TableLayout.AutoSized(p, centered: true);

        cB_bg = new CheckBox[CentralProperties.maxLayersForMC];

        int rowIndex = 0;
        for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
        {
            // Don't add a button for our current layer
            cB_bg[cb] = new CheckBox {Text = (cb + 1).ToString()};

            TableCell tc0 = new() {Control = cB_bg[cb]};
            bgLayerBox_table.Rows[rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (cb + 1 == CentralProperties.maxLayersForMC / 2)
            {
                rowIndex++;
            }
        }
    }

    private void setup_simPreviewBox()
    {
        TableLayout simPreviewBox_table = new();
        simPreviewBox_table.Rows.Add(new TableRow());

        simPreviewBox = new GroupBox {Text = "Simulation Elements", Content = simPreviewBox_table};

        cB_displayShapes = new CheckBox {Text = "Display input shapes for each case", Checked = true};
        simPreviewBox_table.Rows[0].Cells.Add(new TableCell { Control = cB_displayShapes });

        cB_displayResults = new CheckBox {Text = "Display results for each case", Checked = true};
        simPreviewBox_table.Rows[0].Cells.Add(new TableCell { Control = cB_displayResults });

        simPreviewBox_table.Rows[0].Cells.Add(new TableCell { Control = null });
    }

    private static void setSize(Button _control, int width, int height)
    {
        _control.Width = width;
        _control.Height = height;
    }

    private static void setSize(TextArea _control, int width, int height)
    {
        _control.Width = width;
        _control.Height = height;
    }

    private static void setSize(RichTextArea _control, int width, int height)
    {
        _control.Width = width;
        _control.Height = height;
    }

    private static void setSize(CommonControl _control, int width)
    {
        _control.Width = width;
    }
}