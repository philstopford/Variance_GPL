using Eto.Drawing;
using Eto.Forms;
using VeldridEto;
using geoCoreLib;
using geoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Variance
{
    public partial class MainForm
    {
        void applyLocationToViewports(PointF location)
        {
            Application.Instance.Invoke(() =>
            {
                int count = mcVPSettings.Length;
                for (int i = 0; i < count; i++)
                {
                    mcVPSettings[i].setCameraPos(location.X, location.Y);
                }
            });
        }

        void setViewportCamera(int index, double[] parameters)
        {
            mcVPSettings[index].setCameraPos((float)parameters[0], (float)parameters[1]);
            mcVPSettings[index].setZoomFactor((float)parameters[2]);
        }

        double[] getViewportCamera(int index)
        {
            double x = mcVPSettings[index].getCameraX();
            double y = mcVPSettings[index].getCameraY();
            double zoom = mcVPSettings[index].getZoomFactor();
            return new double[] { x, y, zoom };
        }

        void saveViewportSVG_File()
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Enter file to save",
                Filters =
                            {
                                new FileFilter("SVG Files (*.svg)", ".svg")
                            }
            };
            if (sfd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                saveViewportSVG(ref viewPort.ovpSettings, sfd.FileName);
            }
        }

        void saveViewportSVG(ref OVPSettings vpSettings, string svgFileName)
        {
            SVGBuilder svg = new SVGBuilder();

            // The polygons in the viewport are stored flipped due to drawing convention. We need to flip them here for SVG to match drawn viewport.

            // polys
            for (Int32 poly = 0; poly < vpSettings.polyList.Count(); poly++)
            {
                svg.style.brushClr = UIHelper.colorToMyColor(vpSettings.polyList[poly].color);
                svg.style.penClr = UIHelper.colorToMyColor(vpSettings.polyList[poly].color);
                GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.polyList[poly].poly);
                int count = temp.Length;
#if SVGTHREADED
                Parallel.For(0, count, (pt) =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if SVGTHREADED
                );
#endif
                svg.AddPolygons(temp);
            }

            for (Int32 poly = 0; poly < vpSettings.bgPolyList.Count(); poly++)
            {
                svg.style.brushClr = UIHelper.colorToMyColor(vpSettings.bgPolyList[poly].color);
                svg.style.penClr = UIHelper.colorToMyColor(vpSettings.bgPolyList[poly].color);
                GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.bgPolyList[poly].poly);
                int count = temp.Length;
#if SVGTHREADED
                Parallel.For(0, count, (pt) =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if SVGTHREADED
                );
#endif
                svg.AddPolygons(temp);
            }

            // lines
            for (Int32 line = 0; line < vpSettings.lineList.Count(); line++)
            {
                svg.style.brushClr = UIHelper.colorToMyColor(vpSettings.lineList[line].color);
                svg.style.penClr = UIHelper.colorToMyColor(vpSettings.lineList[line].color);
                GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.lineList[line].poly);
                int count = temp.Length;
#if SVGTHREADED
                Parallel.For(0, count, (pt) =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if SVGTHREADED
                );
#endif
                svg.AddPolygons(temp);
            }

            svg.SaveToFile(svgFileName);
        }

        async void saveViewportLayout_File()
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Enter file to save",
                Filters =
                        {
                            new FileFilter("GDS file", "*.gds"),
                            new FileFilter("OAS file", "*.oas")
                        }
            };
            if (sfd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                string filename = sfd.FileName;
                string[] tokens = filename.Split(new char[] { '.' });
                string ext = tokens[tokens.Length - 1].ToUpper();

                int type = (int)GeoCore.fileType.gds;

                if (ext == "OAS")
                {
                    type = (int)GeoCore.fileType.oasis;
                }

                bool useLines = false;
                bool useBGPolys = false;
                if (getMainSelectedIndex() == (int)CommonVars.upperTabNames.twoD)
                {
                    if (getSubTabSelectedIndex() == (int)CommonVars.twoDTabNames.DOE)
                    {
                        useLines = true;
                        useBGPolys = true;
                    }
                    else if ((getSubTabSelectedIndex() == (int)CommonVars.twoDTabNames.settings) || (getSubTabSelectedIndex() == (int)CommonVars.twoDTabNames.paSearch))
                    {
                        useLines = true;
                    }
                }
                else
                {
                    if (getMainSelectedIndex() == (int)CommonVars.upperTabNames.Implant)
                    {
                        useLines = true;
                        useBGPolys = true;
                    }
                }

                Application.Instance.Invoke(() =>
                {
                    statusProgressBar.Indeterminate = true;
                });
                try
                {
                    await Task.Run(() =>
                    {
                        saveViewportLayout(ref viewPort.ovpSettings, useLines, useBGPolys, type, sfd.FileName);
                    });
                }
                catch (Exception)
                {
                    // Handle any task cancelled exception without crashing the tool. The cancellation may occur due to close of the tool whilst evaluation is underway.
                }
                Application.Instance.Invoke(() =>
                {
                    statusProgressBar.Indeterminate = false;
                });
            }
        }

        void saveViewportLayout(ref OVPSettings vpSettings, bool useLineList, bool useBGPolys, int type, string layoutFileName)
        {
            int scale = 100; // for 0.01 nm resolution
            GeoCore g = new GeoCore();
            g.reset();
            GCDrawingfield drawing_ = new GCDrawingfield("");
            drawing_.accyear = (short)DateTime.Now.Year;
            drawing_.accmonth = (short)DateTime.Now.Month;
            drawing_.accday = (short)DateTime.Now.Day;
            drawing_.acchour = (short)DateTime.Now.Hour;
            drawing_.accmin = (short)DateTime.Now.Minute;
            drawing_.accsec = (short)DateTime.Now.Second;
            drawing_.modyear = (short)DateTime.Now.Year;
            drawing_.modmonth = (short)DateTime.Now.Month;
            drawing_.modday = (short)DateTime.Now.Day;
            drawing_.modhour = (short)DateTime.Now.Hour;
            drawing_.modmin = (short)DateTime.Now.Minute;
            drawing_.modsec = (short)DateTime.Now.Second;
            drawing_.databaseunits = 1000 * scale;
            drawing_.userunits = 0.001 / scale;
            drawing_.libname = "variance";

            GCCell gcell_root = drawing_.addCell();
            gcell_root.accyear = (short)DateTime.Now.Year;
            gcell_root.accmonth = (short)DateTime.Now.Month;
            gcell_root.accday = (short)DateTime.Now.Day;
            gcell_root.acchour = (short)DateTime.Now.Hour;
            gcell_root.accmin = (short)DateTime.Now.Minute;
            gcell_root.accsec = (short)DateTime.Now.Second;
            gcell_root.modyear = (short)DateTime.Now.Year;
            gcell_root.modmonth = (short)DateTime.Now.Month;
            gcell_root.modday = (short)DateTime.Now.Day;
            gcell_root.modhour = (short)DateTime.Now.Hour;
            gcell_root.modmin = (short)DateTime.Now.Minute;
            gcell_root.modsec = (short)DateTime.Now.Second;
            gcell_root.cellName = "viewport";

            List<Int32> layers = new List<int>();

            for (Int32 poly = 0; poly < vpSettings.polyList.Count(); poly++)
            {
                GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.polyList[poly].poly);
                int count = temp.Length;
#if SVGTHREADED
                Parallel.For(0, count, (pt) =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if SVGTHREADED
                );
#endif
                int layerIndex = vpSettings.polySourceIndex[poly];
                if (layers.IndexOf(layerIndex) == -1)
                {
                    layers.Add(layerIndex);
                    // Register layer names with geoCore. Need to compensate the 1-index for the layer registration.
                    if (layerIndex < CentralProperties.maxLayersForMC)
                    {
                        g.addLayerName("L" + (layerIndex + 1) + "D0", commonVars.getLayerSettings(layerIndex).getString(EntropyLayerSettings.properties_s.name));
                    }
                    else
                    {
                        g.addLayerName("L" + (layerIndex + 1) + "D0", "result" + (layerIndex - CentralProperties.maxLayersForMC));
                    }
                }

                int polyLength = temp.Length;
                if (polyLength > 2)
                {
                    GeoLibPoint[] ePoly = new GeoLibPoint[polyLength];
#if SVGTHREADED
                    Parallel.For(0, polyLength, (pt) =>
#else
                    for (int pt = 0; pt < polyLength; pt++)
#endif
                    {
                        // Flip Y coordinate to align with the way the geometry is stored for the viewport.
                        ePoly[pt] = new GeoLibPoint((int)(temp[pt].X * scale), (int)(-temp[pt].Y * scale));
                    }
#if SVGTHREADED
                    );
#endif
                    gcell_root.addPolygon(ePoly.ToArray(), layerIndex + 1, 0); // layer is 1-index based for output, so need to offset value accordingly.
                }
            }

            if (useLineList)
            {
                for (Int32 line = 0; line < vpSettings.lineList.Count(); line++)
                {
                    GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.lineList[line].poly);
                    int count = temp.Length;
#if SVGTHREADED
                    Parallel.For(0, count, (pt) =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        temp[pt].Y = -temp[pt].Y;
                    }
#if SVGTHREADED
                    );
#endif
                    int layerIndex = vpSettings.lineSourceIndex[line];
                    if (layers.IndexOf(layerIndex) == -1)
                    {
                        layers.Add(layerIndex);
                        // Register layer names with geoCore. Need to compensate the 1-index for the layer registration.
                        if (layerIndex < CentralProperties.maxLayersForMC)
                        {
                            g.addLayerName("L" + (layerIndex + 1) + "D0", commonVars.getLayerSettings(layerIndex).getString(EntropyLayerSettings.properties_s.name));
                        }
                        else
                        {
                            g.addLayerName("L" + (layerIndex + 1) + "D0", "result" + (layerIndex - CentralProperties.maxLayersForMC));
                        }
                    }

                    GeoLibPoint[] ePoly = new GeoLibPoint[count];
#if SVGTHREADED
                    Parallel.For(0, count, (pt) =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        // Flip Y coordinate to align with the way the geometry is stored for the viewport.
                        ePoly[pt] = new GeoLibPoint((int)(temp[pt].X * scale), (int)(-temp[pt].Y * scale));
                    }
#if SVGTHREADED
                    );
#endif
                    gcell_root.addPath(ePoly.ToArray(), layerIndex + 1, 0);
                }
            }

            if (useBGPolys)
            {
                for (Int32 poly = 0; poly < vpSettings.bgPolyList.Count(); poly++)
                {
                    GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.bgPolyList[poly].poly);
                    int count = temp.Length;
#if SVGTHREADED
                    Parallel.For(0, count, (pt) =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        temp[pt].Y = -temp[pt].Y;
                    }
#if SVGTHREADED
                    );
#endif
                    int layerIndex = vpSettings.bgPolySourceIndex[poly];
                    if (layers.IndexOf(layerIndex) == -1)
                    {
                        layers.Add(layerIndex);
                        // Register layer names with geoCore. Need to compensate the 1-index for the layer registration.
                        if (layerIndex < CentralProperties.maxLayersForMC)
                        {
                            g.addLayerName("L" + (layerIndex + 1) + "D0", commonVars.getLayerSettings(layerIndex).getString(EntropyLayerSettings.properties_s.name));
                        }
                        else
                        {
                            g.addLayerName("L" + (layerIndex + 1) + "D0", "result" + (layerIndex - CentralProperties.maxLayersForMC));
                        }
                    }

                    GeoLibPoint[] ePoly = new GeoLibPoint[count];
#if SVGTHREADED
                    Parallel.For(0, count, (pt) =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        // Flip Y coordinate to align with the way the geometry is stored for the viewport.
                        ePoly[pt] = new GeoLibPoint((int)(temp[pt].X * scale), (int)(-temp[pt].Y * scale));
                    }
#if SVGTHREADED
                    );
#endif
                    gcell_root.addPolygon(ePoly.ToArray(), layerIndex + 1, 0);
                }
            }

            g.setDrawing(drawing_);
            g.setValid(true);

            switch (type)
            {
                case (int)GeoCore.fileType.gds:
                    gds.gdsWriter gw = new gds.gdsWriter(g, layoutFileName);
                    gw.save();
                    break;
                case (int)GeoCore.fileType.oasis:
                    oasis.oasWriter ow = new oasis.oasWriter(g, layoutFileName);
                    ow.save();
                    break;

            }
        }
    }
}