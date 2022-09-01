using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using gds;
using geoCoreLib;
using geoLib;
using oasis;
using VeldridEto;

namespace Variance;

public partial class MainForm
{
    private void applyLocationToViewports(PointF location)
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

    private void setViewportCamera(int index, double[] parameters)
    {
        mcVPSettings[index].setCameraPos((float)parameters[0], (float)parameters[1]);
        mcVPSettings[index].setZoomFactor((float)parameters[2]);
    }

    private double[] getViewportCamera(int index)
    {
        double x = mcVPSettings[index].getCameraX();
        double y = mcVPSettings[index].getCameraY();
        double zoom = mcVPSettings[index].getZoomFactor();
        return new[] { x, y, zoom };
    }

    private void saveViewportSVG_File()
    {
        SaveFileDialog sfd = new()
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

    private static void saveViewportSVG(ref OVPSettings vpSettings, string svgFileName)
    {
        SVGBuilder.SVGBuilder svg = new();

        // The polygons in the viewport are stored flipped due to drawing convention. We need to flip them here for SVG to match drawn viewport.

        // polys
        foreach (ovp_Poly t in vpSettings.polyList)
        {
            svg.style.brushClr = UIHelper.colorToMyColor(t.color);
            svg.style.penClr = UIHelper.colorToMyColor(t.color);
            GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(t.poly);
            int count = temp.Length;
#if !SVGSINGLETHREADED
            Parallel.For(0, count, pt =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if !SVGSINGLETHREADED
            );
#endif
            svg.AddPolygons(temp);
        }

        foreach (ovp_Poly t in vpSettings.bgPolyList)
        {
            svg.style.brushClr = UIHelper.colorToMyColor(t.color);
            svg.style.penClr = UIHelper.colorToMyColor(t.color);
            GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(t.poly);
            int count = temp.Length;
#if !SVGSINGLETHREADED
            Parallel.For(0, count, pt =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if !SVGSINGLETHREADED
            );
#endif
            svg.AddPolygons(temp);
        }

        // lines
        foreach (ovp_Poly t in vpSettings.lineList)
        {
            svg.style.brushClr = UIHelper.colorToMyColor(t.color);
            svg.style.penClr = UIHelper.colorToMyColor(t.color);
            GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(t.poly);
            int count = temp.Length;
#if !SVGSINGLETHREADED
            Parallel.For(0, count, pt =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if !SVGSINGLETHREADED
            );
#endif
            svg.AddPolygons(temp);
        }

        svg.SaveToFile(svgFileName);
    }

    private async void saveViewportLayout_File()
    {
        SaveFileDialog sfd = new()
        {
            Title = "Enter file to save",
            Filters =
            {
                new FileFilter("GDS file", "*.gds"),
                new FileFilter("OAS file", "*.oas")
            }
        };
        if (sfd.ShowDialog(ParentWindow) != DialogResult.Ok)
        {
            return;
        }

        string filename = sfd.FileName;
        string[] tokens = filename.Split(new[] { '.' });
        string ext = tokens[^1].ToUpper();

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
            else if (getSubTabSelectedIndex() == (int)CommonVars.twoDTabNames.settings || getSubTabSelectedIndex() == (int)CommonVars.twoDTabNames.paSearch)
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

        await Application.Instance.InvokeAsync(() =>
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
        await Application.Instance.InvokeAsync(() =>
        {
            statusProgressBar.Indeterminate = false;
        });
    }

    private void saveViewportLayout(ref OVPSettings vpSettings, bool useLineList, bool useBGPolys, int type, string layoutFileName)
    {
        const int scale = 100; // for 0.01 nm resolution
        GeoCore g = new();
        g.reset();
        GCDrawingfield drawing_ = new("")
        {
            accyear = (short) DateTime.Now.Year,
            accmonth = (short) DateTime.Now.Month,
            accday = (short) DateTime.Now.Day,
            acchour = (short) DateTime.Now.Hour,
            accmin = (short) DateTime.Now.Minute,
            accsec = (short) DateTime.Now.Second,
            modyear = (short) DateTime.Now.Year,
            modmonth = (short) DateTime.Now.Month,
            modday = (short) DateTime.Now.Day,
            modhour = (short) DateTime.Now.Hour,
            modmin = (short) DateTime.Now.Minute,
            modsec = (short) DateTime.Now.Second,
            databaseunits = 1000 * scale,
            userunits = 0.001 / scale,
            libname = "variance"
        };

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

        List<int> layers = new();

        for (int poly = 0; poly < vpSettings.polyList.Count; poly++)
        {
            GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.polyList[poly].poly);
            int count = temp.Length;
#if !SVGSINGLETHREADED
            Parallel.For(0, count, pt =>
#else
                for (int pt = 0; pt < count; pt++)
#endif
                {
                    temp[pt].Y = -temp[pt].Y;
                }
#if !SVGSINGLETHREADED
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
            if (polyLength <= 2)
            {
                continue;
            }

            {
                GeoLibPoint[] ePoly = new GeoLibPoint[polyLength];
#if !SVGSINGLETHREADED
                Parallel.For(0, polyLength, pt =>
#else
                    for (int pt = 0; pt < polyLength; pt++)
#endif
                    {
                        // Flip Y coordinate to align with the way the geometry is stored for the viewport.
                        ePoly[pt] = new GeoLibPoint((int)(temp[pt].X * scale), (int)(-temp[pt].Y * scale));
                    }
#if !SVGSINGLETHREADED
                );
#endif
                gcell_root.addPolygon(ePoly, layerIndex + 1, 0); // layer is 1-index based for output, so need to offset value accordingly.
            }
        }

        if (useLineList)
        {
            for (int line = 0; line < vpSettings.lineList.Count; line++)
            {
                GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.lineList[line].poly);
                int count = temp.Length;
#if !SVGSINGLETHREADED
                Parallel.For(0, count, pt =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        temp[pt].Y = -temp[pt].Y;
                    }
#if !SVGSINGLETHREADED
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
#if !SVGSINGLETHREADED
                Parallel.For(0, count, pt =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        // Flip Y coordinate to align with the way the geometry is stored for the viewport.
                        ePoly[pt] = new GeoLibPoint((int)(temp[pt].X * scale), (int)(-temp[pt].Y * scale));
                    }
#if !SVGSINGLETHREADED
                );
#endif
                gcell_root.addPath(ePoly, layerIndex + 1, 0);
            }
        }

        if (useBGPolys)
        {
            for (int poly = 0; poly < vpSettings.bgPolyList.Count; poly++)
            {
                GeoLibPointF[] temp = UIHelper.pointFArrayTomyPointFArray(vpSettings.bgPolyList[poly].poly);
                int count = temp.Length;
#if !SVGSINGLETHREADED
                Parallel.For(0, count, pt =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        temp[pt].Y = -temp[pt].Y;
                    }
#if !SVGSINGLETHREADED
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
#if !SVGSINGLETHREADED
                Parallel.For(0, count, pt =>
#else
                    for (int pt = 0; pt < count; pt++)
#endif
                    {
                        // Flip Y coordinate to align with the way the geometry is stored for the viewport.
                        ePoly[pt] = new GeoLibPoint((int)(temp[pt].X * scale), (int)(-temp[pt].Y * scale));
                    }
#if !SVGSINGLETHREADED
                );
#endif
                gcell_root.addPolygon(ePoly, layerIndex + 1, 0);
            }
        }

        g.setDrawing(drawing_);
        g.setValid(true);

        switch (type)
        {
            case (int)GeoCore.fileType.gds:
                gdsWriter gw = new(g, layoutFileName);
                gw.save();
                break;
            case (int)GeoCore.fileType.oasis:
                oasWriter ow = new(g, layoutFileName);
                ow.save();
                break;

        }
    }
}