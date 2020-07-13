using entropyRNG;
using Error;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using VeldridEto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using Eto.Veldrid;
using Veldrid;

namespace Variance
{
    /// <summary>
    /// Your application's main form
    /// </summary>
    public partial class MainForm : Form
    {
        CommonVars commonVars;

        Entropy entropyControl;

        VarianceContextGUI varianceContext;

        Command quitCommand, helpCommand, aboutCommand, clearLayer, copyLayer, pasteLayer, newSim, openSim, revertSim, saveSim, saveAsSim;

        List<string> notList, booleanList;

        Replay simReplay;

        string helpPath;
        bool helpAvailable;

        void setupUIDataContext(List<string> notList_, List<string> booleanList_)
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
                calcModes = commonVars.calcMode_names,
                booleanList = booleanList_,
                openGLMode = commonVars.getOpenGLModeList(),
                notList = notList_,
                polyFillList = commonVars.getPolyFillTypes(),
                geoCoreStructureList = commonVars.structureList,
                geoCoreLDList = commonVars.activeStructure_LayerDataTypeList,
                geoCoreStructureList_exp = commonVars.structureList_exp,
                subShapesList_exp = commonVars.subShapesList_exp,
                geoCoreLDList_exp = commonVars.activeStructure_LayerDataTypeList_exp,
                rngMapping = commonVars.rngCustomMapping,
                layerNames = commonVars.layerNames,
            };
        }

        void loadPrefs()
        {
            // We have to do this by hand, reading and parsing an XML file. Yay.
            string filename = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationSettings);
            filename += System.IO.Path.DirectorySeparatorChar + "variance_prefs.xml";

            XElement prefs;
            try
            {
                prefs = XElement.Load(filename);
            }
            catch (Exception)
            {
                if (System.IO.File.Exists(filename))
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
                string[] equations = tempString.Split(new char[] { ';' });
                for (int i = 0; i < equations.Length; i++)
                {
                    if (equations[i] != "Box-Muller")
                    {
                        varianceContext.vc.rngMappingEquations.Add(equations[i]);
                    }
                }
            }
            catch (Exception)
            {

            }

            try
            {
                string layerCol = "layer1Color";
                varianceContext.vc.colors.layer1_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").Single().Value);
                varianceContext.vc.colors.layer1_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").Single().Value);
                varianceContext.vc.colors.layer1_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").Single().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer2Color";
                varianceContext.vc.colors.layer2_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer2_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer2_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer3Color";
                varianceContext.vc.colors.layer3_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer3_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer3_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer4Color";
                varianceContext.vc.colors.layer4_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer4_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer4_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer5Color";
                varianceContext.vc.colors.layer5_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer5_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer5_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer6Color";
                varianceContext.vc.colors.layer6_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer6_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer6_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer7Color";
                varianceContext.vc.colors.layer7_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer7_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer7_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer8Color";
                varianceContext.vc.colors.layer8_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer8_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer8_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer9Color";
                varianceContext.vc.colors.layer9_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer9_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer9_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer10Color";
                varianceContext.vc.colors.layer10_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer10_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer10_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer11Color";
                varianceContext.vc.colors.layer11_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer11_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer11_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer12Color";
                varianceContext.vc.colors.layer12_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer12_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer12_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer13Color";
                varianceContext.vc.colors.layer13_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer13_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer13_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer14Color";
                varianceContext.vc.colors.layer14_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer14_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer14_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer15Color";
                varianceContext.vc.colors.layer15_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer15_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer15_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "layer16Color";
                varianceContext.vc.colors.layer16_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.layer16_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.layer16_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "subshape1Color";
                varianceContext.vc.colors.subshape1_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.subshape1_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.subshape1_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "subshape2Color";
                varianceContext.vc.colors.subshape2_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.subshape2_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.subshape2_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "subshape3Color";
                varianceContext.vc.colors.subshape3_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.subshape3_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.subshape3_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "result1Color";
                varianceContext.vc.colors.result_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.result_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.result_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "result2Color";
                varianceContext.vc.colors.result2_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.result2_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.result2_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "result3Color";
                varianceContext.vc.colors.result3_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.result3_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.result3_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "result4Color";
                varianceContext.vc.colors.result4_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.result4_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.result4_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "minImplantColor";
                varianceContext.vc.colors.implantMin_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.implantMin_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.implantMin_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "meanImplantColor";
                varianceContext.vc.colors.implantMean_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.implantMean_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.implantMean_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "maxImplantColor";
                varianceContext.vc.colors.implantMax_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.implantMax_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.implantMax_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "implantResistColor";
                varianceContext.vc.colors.implantResist_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.implantResist_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.implantResist_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "majorColor";
                varianceContext.vc.colors.major_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.major_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.major_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "minorColor";
                varianceContext.vc.colors.minor_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.minor_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.minor_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            try
            {
                string layerCol = "enabledColor";
                varianceContext.vc.colors.enabled_Color.R = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("R").First().Value);
                varianceContext.vc.colors.enabled_Color.G = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("G").First().Value);
                varianceContext.vc.colors.enabled_Color.B = Convert.ToInt32(prefs.Descendants("colors").Descendants(layerCol).Descendants("B").First().Value);
            }
            catch (Exception)
            {
            }

            varianceContext.vc.colors.rebuildLists();
        }

        void savePrefs()
        {
            string filename = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationSettings);
            filename += System.IO.Path.DirectorySeparatorChar + "variance_prefs.xml";

            try
            {
                XDocument prefsXML = new XDocument(
                    new XElement("root"));
                prefsXML.Root.Add(new XElement("version", CentralProperties.version));

                XElement emailPrefs = new XElement("email",
                    new XElement("emailAddress", varianceContext.vc.emailAddress),
                    new XElement("emailPassword", varianceContext.vc.emailPwd),
                    new XElement("emailHost", varianceContext.vc.host),
                    new XElement("emailSSL", varianceContext.vc.ssl),
                    new XElement("emailPort", varianceContext.vc.port));
                prefsXML.Root.Add(emailPrefs);

                XElement geoCorePrefs = new XElement("geoCore",
                    new XElement("geoCoreCDVariation", varianceContext.vc.geoCoreCDVariation),
                    new XElement("layerPreviewDOETile", varianceContext.vc.layerPreviewDOETile));
                prefsXML.Root.Add(geoCorePrefs);

                XElement openGLPrefs = new XElement("openGL",
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
                XElement rngMappingEquations = new XElement("rngMappingEquations", equationString); //  commonVars.rngCustomMapping);
                prefsXML.Root.Add(rngMappingEquations);

                XElement colorPrefs = new XElement("colors");

                XElement layer1Color = new XElement("layer1Color",
                    new XElement("R", varianceContext.vc.colors.layer1_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer1_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer1_Color.B));
                colorPrefs.Add(layer1Color);

                XElement layer2Color = new XElement("layer2Color",
                    new XElement("R", varianceContext.vc.colors.layer2_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer2_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer2_Color.B));
                colorPrefs.Add(layer2Color);

                XElement layer3Color = new XElement("layer3Color",
                    new XElement("R", varianceContext.vc.colors.layer3_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer3_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer3_Color.B));
                colorPrefs.Add(layer3Color);

                XElement layer4Color = new XElement("layer4Color",
                    new XElement("R", varianceContext.vc.colors.layer4_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer4_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer4_Color.B));
                colorPrefs.Add(layer4Color);

                XElement layer5Color = new XElement("layer5Color",
                    new XElement("R", varianceContext.vc.colors.layer5_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer5_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer5_Color.B));
                colorPrefs.Add(layer5Color);

                XElement layer6Color = new XElement("layer6Color",
                    new XElement("R", varianceContext.vc.colors.layer6_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer6_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer6_Color.B));
                colorPrefs.Add(layer6Color);

                XElement layer7Color = new XElement("layer7Color",
                    new XElement("R", varianceContext.vc.colors.layer7_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer7_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer7_Color.B));
                colorPrefs.Add(layer7Color);

                XElement layer8Color = new XElement("layer8Color",
                    new XElement("R", varianceContext.vc.colors.layer8_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer8_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer8_Color.B));
                colorPrefs.Add(layer8Color);

                XElement layer9Color = new XElement("layer9Color",
                    new XElement("R", varianceContext.vc.colors.layer9_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer9_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer9_Color.B));
                colorPrefs.Add(layer9Color);

                XElement layer10Color = new XElement("layer10Color",
                    new XElement("R", varianceContext.vc.colors.layer10_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer10_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer10_Color.B));
                colorPrefs.Add(layer10Color);

                XElement layer11Color = new XElement("layer11Color",
                    new XElement("R", varianceContext.vc.colors.layer11_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer11_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer11_Color.B));
                colorPrefs.Add(layer11Color);

                XElement layer12Color = new XElement("layer12Color",
                    new XElement("R", varianceContext.vc.colors.layer12_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer12_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer12_Color.B));
                colorPrefs.Add(layer12Color);

                XElement layer13Color = new XElement("layer13Color",
                    new XElement("R", varianceContext.vc.colors.layer13_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer13_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer13_Color.B));
                colorPrefs.Add(layer13Color);

                XElement layer14Color = new XElement("layer14Color",
                    new XElement("R", varianceContext.vc.colors.layer14_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer14_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer14_Color.B));
                colorPrefs.Add(layer14Color);

                XElement layer15Color = new XElement("layer15Color",
                    new XElement("R", varianceContext.vc.colors.layer15_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer15_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer15_Color.B));
                colorPrefs.Add(layer15Color);

                XElement layer16Color = new XElement("layer16Color",
                    new XElement("R", varianceContext.vc.colors.layer16_Color.R),
                    new XElement("G", varianceContext.vc.colors.layer16_Color.G),
                    new XElement("B", varianceContext.vc.colors.layer16_Color.B));
                colorPrefs.Add(layer16Color);

                XElement result1Color = new XElement("result1Color",
                    new XElement("R", varianceContext.vc.colors.result_Color.R),
                    new XElement("G", varianceContext.vc.colors.result_Color.G),
                    new XElement("B", varianceContext.vc.colors.result_Color.B));
                colorPrefs.Add(result1Color);

                XElement result2Color = new XElement("result2Color",
                    new XElement("R", varianceContext.vc.colors.result2_Color.R),
                    new XElement("G", varianceContext.vc.colors.result2_Color.G),
                    new XElement("B", varianceContext.vc.colors.result2_Color.B));
                colorPrefs.Add(result2Color);

                XElement result3Color = new XElement("result3Color",
                    new XElement("R", varianceContext.vc.colors.result3_Color.R),
                    new XElement("G", varianceContext.vc.colors.result3_Color.G),
                    new XElement("B", varianceContext.vc.colors.result3_Color.B));
                colorPrefs.Add(result3Color);

                XElement result4Color = new XElement("result4Color",
                    new XElement("R", varianceContext.vc.colors.result4_Color.R),
                    new XElement("G", varianceContext.vc.colors.result4_Color.G),
                    new XElement("B", varianceContext.vc.colors.result4_Color.B));
                colorPrefs.Add(result4Color);

                XElement minImplantColor = new XElement("minImplantColor",
                    new XElement("R", varianceContext.vc.colors.implantMin_Color.R),
                    new XElement("G", varianceContext.vc.colors.implantMin_Color.G),
                    new XElement("B", varianceContext.vc.colors.implantMin_Color.B));
                colorPrefs.Add(minImplantColor);

                XElement meanImplantColor = new XElement("meanImplantColor",
                    new XElement("R", varianceContext.vc.colors.implantMean_Color.R),
                    new XElement("G", varianceContext.vc.colors.implantMean_Color.G),
                    new XElement("B", varianceContext.vc.colors.implantMean_Color.B));
                colorPrefs.Add(meanImplantColor);

                XElement maxImplantColor = new XElement("maxImplantColor",
                    new XElement("R", varianceContext.vc.colors.implantMax_Color.R),
                    new XElement("G", varianceContext.vc.colors.implantMax_Color.G),
                    new XElement("B", varianceContext.vc.colors.implantMax_Color.B));
                colorPrefs.Add(maxImplantColor);

                XElement implantResistColor = new XElement("implantResistColor",
                    new XElement("R", varianceContext.vc.colors.implantResist_Color.R),
                    new XElement("G", varianceContext.vc.colors.implantResist_Color.G),
                    new XElement("B", varianceContext.vc.colors.implantResist_Color.B));
                colorPrefs.Add(implantResistColor);

                XElement subshape1Color = new XElement("subshape1Color",
                    new XElement("R", varianceContext.vc.colors.subshape1_Color.R),
                    new XElement("G", varianceContext.vc.colors.subshape1_Color.G),
                    new XElement("B", varianceContext.vc.colors.subshape1_Color.B));
                colorPrefs.Add(subshape1Color);

                XElement subshape2Color = new XElement("subshape2Color",
                    new XElement("R", varianceContext.vc.colors.subshape2_Color.R),
                    new XElement("G", varianceContext.vc.colors.subshape2_Color.G),
                    new XElement("B", varianceContext.vc.colors.subshape2_Color.B));
                colorPrefs.Add(subshape2Color);

                XElement subshape3Color = new XElement("subshape3Color",
                    new XElement("R", varianceContext.vc.colors.subshape3_Color.R),
                    new XElement("G", varianceContext.vc.colors.subshape3_Color.G),
                    new XElement("B", varianceContext.vc.colors.subshape3_Color.B));
                colorPrefs.Add(subshape3Color);

                XElement enabledColor = new XElement("enabledColor",
                    new XElement("R", varianceContext.vc.colors.enabled_Color.R),
                    new XElement("G", varianceContext.vc.colors.enabled_Color.G),
                    new XElement("B", varianceContext.vc.colors.enabled_Color.B));
                colorPrefs.Add(enabledColor);

                XElement majorColor = new XElement("majorColor",
                    new XElement("R", varianceContext.vc.colors.major_Color.R),
                    new XElement("G", varianceContext.vc.colors.major_Color.G),
                    new XElement("B", varianceContext.vc.colors.major_Color.B));
                colorPrefs.Add(majorColor);

                XElement minorColor = new XElement("minorColor",
                    new XElement("R", varianceContext.vc.colors.minor_Color.R),
                    new XElement("G", varianceContext.vc.colors.minor_Color.G),
                    new XElement("B", varianceContext.vc.colors.minor_Color.B));
                colorPrefs.Add(minorColor);

                prefsXML.Root.Add(colorPrefs);

                prefsXML.Save(filename);
            }
            catch (Exception)
            {
                ErrorReporter.showMessage_OK("Failed to save preferences", "Error");
            }
        }

        void uiVars()
        {
            // Figure out host UI element sizes to assist in layout, at least vertically.
            // Set controls to null afterwards.
            label_Height = 13; //  (int)Math.Ceiling(qLabel.Font.MeasureString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVVWXYZ").Height);
            num_Height = 13; //  (int)Math.Ceiling(qNum.Font.MeasureString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVVWXYZ").Height);
            checkBox_Height = 13; // (int)Math.Ceiling(qCheckBox.Font.MeasureString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVVWXYZ").Height);
            comboBox_Height = 13; // (int)Math.Ceiling(qDropDown.Font.MeasureString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVVWXYZ").Height);
            radioButton_Height = 13; // (int)Math.Ceiling(qRButton.Font.MeasureString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVVWXYZ").Height);

            uiScaleFactor = 0.66f;

            simulationOutputGroupBoxHeight = 57;
            simulationSettingsGroupBoxHeight = 180;
            userGuidanceWidth = 395;
            userGuidanceHeight = simulationOutputGroupBoxHeight + simulationSettingsGroupBoxHeight + 22;

            simButtonWidth = 64;
            simButtonHeight = 55;

            replayNumWidth = 80;
            replayNumHeight = 21;

            multiThreadWarnWidth = 300;
            resultFieldWidth = 344;

            previewWidth = 450;

            oneDGuidanceWidth = 271;

            oneDLabelWidth = 132;

            oneDMinInsLblWidth = 100;

            oneDMinInsWidth = 100;

            omitLayerBoxY = 2;

            commentBoxWidth = previewWidth + 2;
            commentBoxHeight = 160 - (label_Height * 2);
        }

        public MainForm(ref bool doPrompts, VarianceContextGUI _varianceContext)
        {
            pMainForm(ref doPrompts, _varianceContext);
        }

        void pMainForm(ref bool doPrompts, VarianceContextGUI _varianceContext)
        {
            doPrompts = true;

            // Figure out whether we should display the help menu, if documentation is available.
            // string basePath = AppContext.BaseDirectory; // Disabled this as release builds do not seem to populate this field. Use the above complex approach instead.
            helpPath = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), "Documentation", "index.html");
            helpAvailable = File.Exists(helpPath);

            UI(_varianceContext);
        }

        void q()
        {
            Application.Instance.Quit();
        }

        private bool _veldridReady = false;
        public bool VeldridReady
        {
            get { return _veldridReady; }
            private set
            {
                _veldridReady = value;

                SetUpVeldrid();
            }
        }

        private bool _formReady = false;
        public bool FormReady
        {
            get { return _formReady; }
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

            // Title = $"Veldrid backend: {vSurface.Backend.ToString()}";

            viewPort.Clock.Start();
            createVPContextMenu();
        }

        void UI(VarianceContextGUI _varianceContext)
        {
            if (_varianceContext == null) // safety net.
            {
                Int32 HTCount = Environment.ProcessorCount;
                varianceContext = new VarianceContextGUI(false, "", -1, HTCount, VeldridSurface.PreferredBackend);
            }
            else
            {
                varianceContext = _varianceContext;
            }

            Shown += (sender, e) => FormReady = true;

            Resizable = true;
            Maximizable = true;

            loadPrefs();

            uiVars();

            commonVars = new CommonVars(varianceContext.vc);
            simReplay = new Replay(ref commonVars);

            geoGBVisible = new bool[CentralProperties.maxLayersForMC];
            subShapeGBVisible = new bool[CentralProperties.maxLayersForMC];
            booleanGBVisible = new bool[CentralProperties.maxLayersForMC];

            viewport_settings();

            notList = new List<string>() { "", "!" };
            booleanList = new List<string>() { "&", "|" };

            mainTable = new TableLayout();

            setupUIDataContext(notList, booleanList);

            GraphicsDeviceOptions options = new GraphicsDeviceOptions(
                                                false,
                                                Veldrid.PixelFormat.R32_Float,
                                                false,
                                                ResourceBindingModel.Improved);

            vSurface = new VeldridSurface(varianceContext.backend, options);
            vSurface.VeldridInitialized += (sender, e) => VeldridReady = true;

            commonVars.titleText += " " + vSurface.Backend.ToString();

            Title = commonVars.titleText;

            string exeDirectory = "";
            string shaders = "";
            if (Platform.IsMac)
            {
                // AppContext.BaseDirectory is too simple for the case of the Mac
                // projects. When building an app bundle that depends on the Mono
                // framework being installed, it properly returns the path of the
                // executable in Eto.Veldrid.app/Contents/MacOS. When building an
                // app bundle that instead bundles Mono by way of mkbundle, on the
                // other hand, it returns the directory containing the .app.

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

            string viewportToolTipText = "(w/a/s/d) to navigate\r\n(r) to reset\r\n(n/m) to zoom\r\n(f) to freeze/thaw";
            vp = new Panel();
            //vp.Size = new Size(viewPort.Width + 2, viewPort.Height + 2);
            if (!Platform.IsMac) // This color setting causes issues on Mac where the viewport doesn't show until the mouse passes over.
            {
                vp.BackgroundColor = Eto.Drawing.Colors.Black;
            }
            TableLayout vp_content = new TableLayout();
            vp_content.Rows.Add(new TableRow());
            vp_content.Rows[0].Cells.Add(new TableCell() { Control = vSurface });
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

            TableLayout gadgets_tableLayout = new TableLayout();

            controls.Content = TableLayout.AutoSized(gadgets_tableLayout, centered: true);

            gadgets_tableLayout.Rows.Add(new TableRow());
            TableCell gtc0 = new TableCell();
            gadgets_tableLayout.Rows[0].Cells.Add(gtc0);

            // Upper section needs a nested layout

            upperGadgets_panel = new Panel();
            gtc0.Control = TableLayout.AutoSized(upperGadgets_panel, centered: true);

            upperGadgets_table = new TableLayout();
            upperGadgets_panel.Content = upperGadgets_table;

            upperGadgets_table.Rows.Add(new TableRow());
            TableCell u_gtc0 = new TableCell();

            upperGadgets_table.Rows[0].Cells.Add(u_gtc0);
            setupOmitLayerCheckboxes(u_gtc0);
            upperGadgets_table.Rows[0].Cells.Add(new TableCell() { Control = null });

            upperGadgets_table.Rows.Add(new TableRow());
            TableCell u_gtc1 = new TableCell();
            upperGadgets_table.Rows[1].Cells.Add(u_gtc1);
            setupBGLayerCheckboxes(u_gtc1);
            upperGadgets_table.Rows[1].Cells.Add(new TableCell() { Control = null });

            // Lower section

            gadgets_tableLayout.Rows.Add(new TableRow());
            TableCell gtc2 = new TableCell();
            setupComment(gtc2);
            gadgets_tableLayout.Rows[gadgets_tableLayout.Rows.Count - 1].Cells.Add(gtc2);
            gadgets_tableLayout.Rows[gadgets_tableLayout.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            // Buttons
            gadgets_tableLayout.Rows.Add(new TableRow());
            TableCell gtc3 = new TableCell();
            setup_buttons(gtc3);
            gadgets_tableLayout.Rows[gadgets_tableLayout.Rows.Count - 1].Cells.Add(gtc3);
            gadgets_tableLayout.Rows[gadgets_tableLayout.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            // Configure checkboxes and zoom gadgets.
            gadgets_tableLayout.Rows.Add(new TableRow());
            TableCell gtc4 = new TableCell();
            viewport_gadgets(gtc4);
            gadgets_tableLayout.Rows[gadgets_tableLayout.Rows.Count - 1].Cells.Add(gtc4);
            gadgets_tableLayout.Rows[gadgets_tableLayout.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            gadgets_tableLayout.Rows[gadgets_tableLayout.Rows.Count - 1].ScaleHeight = true;

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
            do2DLayerUI_exp(0, updateUI: true);
            addAllUIHandlers();
            commonVars.setHashes();

            tabControl_main.DragEnter += dragEvent;
            tabControl_main.DragOver += dragEvent;

            tabControl_main.DragDrop += dragAndDrop;
        }

        enum layerLookUpOrder { ICV, OCV, SCDU, TCDU, XOL, YOL, HTPV, HTNV, VTPV, VTNV, WOB, LWR, LWR2 }

        void setDefaultIndices()
        {
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                comboBox_geoEqtn_Op[i].SelectedIndex = 0;
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_2Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_2Layer[i].SelectedIndex = 0;
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_4Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_4Layer[i].SelectedIndex = 0;
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_8Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_8Layer[i].SelectedIndex = 0;
            }

            comboBox_calcModes.SelectedIndex = 0;

            comboBox_RNG.SelectedIndex = 0;

            comboBox_implantRNG.SelectedIndex = 0;

            try
            {
                experimental_listBox_layers.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
        }

        void zoomChanged(object sender, EventArgs e)
        {
            pZoomChanged();
        }

        void pZoomChanged()
        {
            if (openGLErrorReported)
            {
                return;
            }
            if (globalUIFrozen)
            {
                return;
            }

            double zoomValue = 1.0f;
            zoomValue = Convert.ToDouble(num_viewportZoom.Value);
            if (zoomValue < 0.0001)
            {
                zoomValue = 0.0001;
            }

            viewPort.ovpSettings.setZoomFactor((float)(1.0f / zoomValue));

            updateViewport();
        }

        void posChanged(object sender, EventArgs e)
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

        void moveCamera(float x, float y)
        {
            viewPort.ovpSettings.setCameraPos(x, y);
            updateViewport();
        }

        void resetViewPorts()
        {
            for (int i = 0; i < mcVPSettings.Length; i++)
            {
                mcVPSettings[i].fullReset();
            }
            otkVPSettings_implant.fullReset();
        }

        void viewportUpdateHost()
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

        void createVPContextMenu()
        {
            if (viewPort.dragging)
            {
                return;
            }
            // Single viewport now mandates regeneration of the context menu each time, to allow for entry screening.
            vp_menu = new ContextMenu();

            int mainIndex = getMainSelectedIndex();
            int subIndex = getSubTabSelectedIndex();
            int layerIndex = getSelectedLayerIndex();

            int itemIndex = 0;
            int svgIndex = 1;
            int layoutIndex = 2;
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Reset" });
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
            displayOptionsSubItemIndex++;

            if (mainIndex != (int)CommonVars.upperTabNames.Implant)
            {
                if (viewPort.ovpSettings.isLocked())
                {
                    vp_menu.Items.Add(new ButtonMenuItem { Text = "Thaw" });
                }
                else
                {
                    vp_menu.Items.Add(new ButtonMenuItem { Text = "Freeze" });
                }
                freezeThawIndex = itemIndex;
                vp_menu.Items[itemIndex].Click += delegate
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
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Zoom Extents" });
            vp_menu.Items[itemIndex].Click += delegate
            {
                viewPort.zoomExtents();
            };
            itemIndex++;
            vp_menu.Items.AddSeparator();
            itemIndex++;
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Zoom In" });
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
            zoomInSubItemIndex++;

            vp_menu.Items.AddSeparator();
            itemIndex++;

            vp_menu.Items.Add(new ButtonMenuItem { Text = "Zoom Out" });
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
            zoomOutSubItemIndex++;

            if (mainIndex != (int)CommonVars.upperTabNames.Implant)
            {
                if (viewPort.ovpSettings.isLocked())
                {
                    vp_menu.Items[freezeThawIndex].Text = "Thaw";
                }
                else
                {
                    vp_menu.Items[freezeThawIndex].Text = "Freeze";
                }

                if (viewPort.savedLocation_valid)
                {
                    vp_menu.Items[loadBookmarkIndex].Enabled = true;
                }
                else
                {
                    vp_menu.Items[loadBookmarkIndex].Enabled = false;
                }
            }

            svgIndex = itemIndex + 1;

            vp_menu.Items.AddSeparator();

            svgIndex = itemIndex + 1;
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Save SVG" });
            vp_menu.Items[svgIndex].Click += delegate
            {
                saveViewportSVG_File();
            };

            layoutIndex = svgIndex + 1;
            vp_menu.Items.Add(new ButtonMenuItem { Text = "Save Layout" });
            vp_menu.Items[layoutIndex].Click += delegate
            {
                if ((mainIndex == (int)CommonVars.upperTabNames.twoD) && (subIndex == (int)CommonVars.twoDTabNames.layer))
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

        void setup_tabs()
        {
            tabPage2_table = new TableLayout();
            tabPage2 = new TabPage();
            tabPage2.Text = "2D Calculation";
            tabPage2.Content = tabPage2_table;

            tabControl_main.Pages.Add(tabPage2);

            Scrollable tabPage_implant_scrollable = new Scrollable();
            tabPage_implant_table = new TableLayout();
            tabPage_implant_scrollable.Content = tabPage_implant_table;

            tabPage_implant = new TabPage();
            tabPage_implant.Text = "Implant";

            tabPage_implant.Content = tabPage_implant_scrollable;

            tabControl_main.Pages.Add(tabPage_implant);

            tab_1DCalc = new TabPage();
            tab_1DCalc.Text = "1D Calculation";
            tabControl_main.Pages.Add(tab_1DCalc);

            tabPage_utilities_table = new TableLayout();
            Scrollable tabPage_utilities_scrollable = new Scrollable();
            tabPage_utilities_scrollable.Content = tabPage_utilities_table;

            tabPage_utilities = new TabPage();
            tabPage_utilities.Text = "Utils";

            tabPage_utilities.Content = tabPage_utilities_scrollable;

            tabControl_main.Pages.Add(tabPage_utilities);

            tabPage2_table.Rows.Add(new TableRow());

            tabControl_2D_simsettings = new TabControl();
            tabPage2_table.Rows[tabPage2_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = tabControl_2D_simsettings });

            Scrollable tabPage_2D_Settings_scrollable = new Scrollable();
            tabPage_2D_Settings_table = new TableLayout();
            tabPage_2D_Settings_scrollable.Content = tabPage_2D_Settings_table;

            twoD_LayerUISetup();

            tabPage_2D_Settings = new TabPage();
            tabPage_2D_Settings.Text = "Settings";
            tabPage_2D_Settings.Content = tabPage_2D_Settings_scrollable;
            tabControl_2D_simsettings.Pages.Add(tabPage_2D_Settings);

            Scrollable tabPage_2D_DOE_scrollable = new Scrollable();
            tabPage_2D_DOE_table = new TableLayout();
            tabPage_2D_DOE_scrollable.Content = tabPage_2D_DOE_table;

            tabPage_2D_DOE = new TabPage();
            tabPage_2D_DOE.Text = "DOE";

            tabPage_2D_DOE.Content = tabPage_2D_DOE_scrollable;

            tabControl_2D_simsettings.Pages.Add(tabPage_2D_DOE);


            tabPage_2D_PASearch_scrollable = new Scrollable();
            tabPage_2D_PASearch_table = new TableLayout();
            tabPage_2D_PASearch_scrollable.Content = tabPage_2D_PASearch_table;

            tabPage_2D_PASearch = new TabPage();
            tabPage_2D_PASearch.Text = "PA Search";

            tabPage_2D_PASearch.Content = tabPage_2D_PASearch_scrollable;

            tabControl_2D_simsettings.Pages.Add(tabPage_2D_PASearch);
        }

        void setup_layout()
        {
            // mainPanel is tab UI.
            Panel mainPanel = new Panel();
            mainPanel.Size = new Size(920, 800); // force the UI out to contain the panel. Hope this will be redundant eventually with the table UI.
            mainPanel.Content = mainTable;
            // rightPanel will take viewport and controls.
            Panel rightPanel = new Panel();
            Panel mainSplitter = new Panel();
            mainSplitter.Content = new Splitter
            {
                Orientation = Orientation.Horizontal,
                FixedPanel = SplitterFixedPanel.Panel1,
                Panel1 = mainPanel,
                Panel2 = rightPanel,
            };

            // Controls
            controls = new Scrollable();

            rightPanel.Content = new Splitter
            {
                Orientation = Orientation.Vertical,
                FixedPanel = SplitterFixedPanel.Panel1,
                Panel1 = controls,
                Panel2 = vp,
            };

            // We need an additional level here to take the status label and progress bar in the lower portion of the UI.
            Panel outerPanel_upper = new Panel();
            outerPanel_upper.Content = mainSplitter;
            Panel outerPanel_lower = new Panel();
            TableLayout outerPanel_lower_content = new TableLayout();
            TableRow oplc_row0 = new TableRow();
            outerPanel_lower_content.Rows.Add(oplc_row0);
            outerPanel_lower.Content = outerPanel_lower_content;
            Panel outerPanel = new Panel();
            outerPanel.Content = new Splitter
            {
                Orientation = Orientation.Vertical,
                FixedPanel = SplitterFixedPanel.Panel2,
                Panel1 = outerPanel_upper,
                Panel2 = outerPanel_lower,
            };

            // Assign the outer panel to the contents.
            Content = outerPanel;

            mainTable.Rows.Add(new TableRow());

            tabControl_main = new TabControl();
            mainTable.Rows[0].Cells.Add(new TableCell() { Control = tabControl_main });

            // Stick our progress indicator and status label in here.
            statusReadout = new Label();
            statusReadout.Text = "Welcome";
            statusReadout.TextAlignment = TextAlignment.Center;
            oplc_row0.Cells.Add(new TableCell() { Control = statusReadout, ScaleWidth = true });

            statusProgressBar = new ProgressBar();
            statusProgressBar.Value = 0;
            statusProgressBar.Size = new Size(100, label_Height);
            oplc_row0.Cells.Add(new TableCell() { Control = statusProgressBar, ScaleWidth = false });
        }

        void setupComment(TableCell tc)
        {
            Panel p = new Panel();
            commentBox = new RichTextArea();
            p.Content = commentBox;
            tc.Control = TableLayout.AutoSized(p, centered: true);
            commentBox.Wrap = true;
            commentBox.ReadOnly = false;
            commentBox.LostFocus += commentChanged;
            setSize(commentBox, commentBoxWidth, commentBoxHeight);
        }

        void launchHelp(object sender, EventArgs e)
        {
            // errorReporter.showMessage_OK(helpPath, "Info");
            if (helpAvailable)
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo(@helpPath)
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
        }

        void commands()
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
                    new ButtonMenuItem { Text = "&Edit", Items = { copyLayer, pasteLayer, clearLayer } },
                },
                /*ApplicationItems = {
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
				},*/
                QuitItem = quitCommand,
                HelpItems = {
                    helpCommand
                },
                AboutItem = aboutCommand
            };

            helpCommand.Enabled = helpAvailable;
        }

        void setup_buttons(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = TableLayout.AutoSized(p, centered: true);

            TableLayout buttons_table = new TableLayout();
            p.Content = buttons_table;
            buttons_table.Rows.Add(new TableRow());

            btn_singleCPU = new Button();
            btn_singleCPU.Text = "Single\r\nCPU";
            setSize(btn_singleCPU, simButtonWidth, simButtonHeight);
            btn_singleCPU.Click += monteCarloSingleThreadEventHandler;
            // buttons_table.Rows[0].Cells.Add(new TableCell() { Control = btn_singleCPU });

            btn_multiCPU = new Button();
            btn_multiCPU.Text = "Multi\r\nCPU";
            setSize(btn_multiCPU, simButtonWidth, simButtonHeight);
            btn_multiCPU.Click += monteCarloMultipleThreadEventHandler;
            buttons_table.Rows[0].Cells.Add(new TableCell() { Control = btn_multiCPU });

            btn_Cancel = new Button();
            btn_Cancel.Text = "Cancel";
            setSize(btn_Cancel, simButtonWidth, simButtonHeight);
            btn_Cancel.Click += btnCancel;
            buttons_table.Rows[0].Cells.Add(new TableCell() { Control = btn_Cancel });

            btn_STOP = new Button();
            btn_STOP.Text = "STOP";
            setSize(btn_STOP, simButtonWidth, simButtonHeight);
            btn_STOP.Click += btnSTOP;
            buttons_table.Rows[0].Cells.Add(new TableCell() { Control = btn_STOP });
        }

        void viewport_gadgets(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = p; // TableLayout.AutoSized(p, centered: true);

            TableLayout viewport_gadgets_tl = new TableLayout();
            p.Content = viewport_gadgets_tl;
            viewport_gadgets_tl.Rows.Add(new TableRow());

            lbl_simPreviewZoom = new Label();
            lbl_simPreviewZoom.Text = "Zoom";
            viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_simPreviewZoom });

            num_viewportZoom = new NumericStepper();
            num_viewportZoom.DecimalPlaces = 2;
            num_viewportZoom.Increment = 0.01;
            num_viewportZoom.Value = 1.0;
            num_viewportZoom.MinValue = 0.001;
            setSize(num_viewportZoom, 55, num_Height);
            num_viewportZoom.LostFocus += zoomChanged;
            viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_viewportZoom) });

            viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell() { Control = null, ScaleWidth = true }); // padding

            lbl_viewportPos = new Label();
            lbl_viewportPos.Text = "Position";
            viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_viewportPos });

            num_viewportX = new NumericStepper();
            num_viewportX.DecimalPlaces = 2;
            num_viewportX.Increment = 0.01;
            num_viewportX.Value = 0;
            setSize(num_viewportX, 75, num_Height);
            num_viewportX.LostFocus += posChanged;
            viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_viewportX) });

            num_viewportY = new NumericStepper();
            num_viewportY.DecimalPlaces = 2;
            num_viewportY.Increment = 0.01;
            num_viewportY.Value = 0;
            setSize(num_viewportY, 75, num_Height);
            num_viewportY.LostFocus += posChanged;
            viewport_gadgets_tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_viewportY) });
        }

        void viewport_settings()
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
            }
            mcVPSettings[mcVPSettings.Length - 2].drawDrawn(true);
            mcVPSettings[mcVPSettings.Length - 1].drawDrawn(true);

            otkVPSettings_implant = new OVPSettings(45.0f, 45.0f);
            otkVPSettings_implant.setBaseZoom(0.25f);
            otkVPSettings_implant.drawDrawn(true);
            otkVPSettings_implant.drawFilled(commonVars.getOpenGLProp(CommonVars.properties_gl.fill));
            otkVPSettings_implant.drawPoints(commonVars.getOpenGLProp(CommonVars.properties_gl.points));
            otkVPSettings_implant.minorGridColor = Color.FromArgb(commonVars.getColors().minor_Color.toArgb());
            otkVPSettings_implant.majorGridColor = Color.FromArgb(commonVars.getColors().major_Color.toArgb());
        }

        void setupOmitLayerCheckboxes(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = TableLayout.AutoSized(p, centered: true);

            omitLayerBox = new GroupBox();

            p.Content = omitLayerBox;
            omitLayerBox.Text = "Omit layers (Boolean)";

            TableLayout omitLayerBox_table = new TableLayout();
            omitLayerBox.Content = omitLayerBox_table;

            omitLayerBox_table.Rows.Add(new TableRow());
            omitLayerBox_table.Rows.Add(new TableRow());

            checkBox_omit_lyr = new CheckBox[CentralProperties.maxLayersForMC];
            int rowIndex = 0;
            for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
            {
                // Don't add a button for our current layer
                checkBox_omit_lyr[cb] = new CheckBox();
                checkBox_omit_lyr[cb].Text = (cb + 1).ToString();

                TableCell tc0 = new TableCell();
                tc0.Control = checkBox_omit_lyr[cb];
                omitLayerBox_table.Rows[rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if ((cb + 1) == CentralProperties.maxLayersForMC / 2)
                {
                    rowIndex++;
                }
            }
        }

        void updateOmitLayerCheckboxes()
        {
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                checkBox_omit_lyr[i].CheckedChanged -= omitLayerCheckboxChanged;
                checkBox_omit_lyr[i].Checked = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.omit) == 1;
                checkBox_omit_lyr[i].CheckedChanged += omitLayerCheckboxChanged;
            }
        }

        void setupBGLayerCheckboxes(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = TableLayout.AutoSized(p, centered: true);

            bgLayerBox = new GroupBox();
            p.Content = bgLayerBox;

            bgLayerBox.Text = "Background layers";

            bgLayerBox_table = new TableLayout();
            bgLayerBox.Content = bgLayerBox_table;
            bgLayerBox_table.Rows.Add(new TableRow());
            bgLayerBox_table.Rows.Add(new TableRow());

            checkBox_bg_lyr = new CheckBox[CentralProperties.maxLayersForMC];

            int rowIndex = 0;
            for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
            {
                // Don't add a button for our current layer
                checkBox_bg_lyr[cb] = new CheckBox();
                checkBox_bg_lyr[cb].Text = (cb + 1).ToString();

                TableCell tc0 = new TableCell();
                tc0.Control = checkBox_bg_lyr[cb];
                bgLayerBox_table.Rows[rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if ((cb + 1) == CentralProperties.maxLayersForMC / 2)
                {
                    rowIndex++;
                }
            }
        }

        void setup_simPreviewBox()
        {
            simPreviewBox = new GroupBox();
            simPreviewBox.Text = "Simulation Elements";

            TableLayout simPreviewBox_table = new TableLayout();
            simPreviewBox.Content = simPreviewBox_table;
            simPreviewBox_table.Rows.Add(new TableRow());

            checkBox_displayShapes = new CheckBox();
            checkBox_displayShapes.Text = "Display input shapes for each case";
            simPreviewBox_table.Rows[0].Cells.Add(new TableCell() { Control = checkBox_displayShapes });
            checkBox_displayShapes.Checked = true;

            checkBox_displayResults = new CheckBox();
            checkBox_displayResults.Text = "Display results for each case";
            simPreviewBox_table.Rows[0].Cells.Add(new TableCell() { Control = checkBox_displayResults });

            simPreviewBox_table.Rows[0].Cells.Add(new TableCell() { Control = null });

            checkBox_displayResults.Checked = true;
        }

        void setSize(Button _control, int width, int height)
        {
            _control.Width = width;
            _control.Height = height;
        }

        void setSize(TextArea _control, int width, int height)
        {
            _control.Width = width;
            _control.Height = height;
        }

        void setSize(RichTextArea _control, int width, int height)
        {
            _control.Width = width;
            _control.Height = height;
        }

        void setSize(CommonControl _control, int width, int height)
        {
            _control.Width = width;
        }

        void setSize(GroupBox _control, int width, int height)
        {
            _control.Width = width;
            _control.Height = height;
        }
    }
}