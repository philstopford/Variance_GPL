using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eto.Forms;
using gds;
using geoCoreLib;
using geoLib;
using geoWrangler;
using oasis;

namespace Variance;

public partial class MainForm
{
    private void exportActiveLayerToLayout(object sender, EventArgs e)
    {
        exportActiveLayerToLayout();
    }

    private async void exportActiveLayerToLayout()
    {
        // Need to request output file location and name.
        SaveFileDialog sfd = new()
        {
            Title = "Enter file to save",
            Filters =
            {
                new FileFilter("GDS file", "*.gds", ".gdsii"),
                new FileFilter("GDS file, GZIP compressed", "*.gds.gz", "*.gdsii.gz"),
                new FileFilter("OAS file", "*.oas", "*.oasis"),
                new FileFilter("OAS file. GZIP compressed", "*.oas.gz", "*.oasis.gz")
            }
        };
        if (sfd.ShowDialog(ParentWindow) != DialogResult.Ok)
        {
            return;
        }

        string filename = sfd.FileName;
        string[] tokens = filename.Split(new[] { '.' });
        string ext = tokens[^1].ToUpper();

        int type = -1;
        switch (ext)
        {
            case "GDS":
            case "GZ" when tokens[^2].ToUpper() == "GDS":
            case "GDSII":
            case "GZ" when tokens[^2].ToUpper() == "GDSII":
                type = (int)GeoCore.fileType.gds;
                break;
            case "OAS":
            case "GZ" when tokens[^2].ToUpper() == "OAS":
            case "OASIS":
            case "GZ" when tokens[^2].ToUpper() == "OASIS":
                type = (int)GeoCore.fileType.oasis;
                break;
        }

        if (type == -1)
        {
            return;
        }

        await Application.Instance.InvokeAsync(() =>
        {
            statusProgressBar.Indeterminate = true;
        });
        try
        {
            await Task.Run(() =>
            {
                toGeoCore(type, filename);
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

    private object exportLock = new();

    private void toGeoCore(int type, string file)
    {
        int layerIndex = getSelectedLayerIndex();

        Monitor.Enter(exportLock);
        Stopwatch sw = new();
        try
        {
            sw.Reset();
            sw.Start();
            Application.Instance.Invoke(() =>
            {
                updateStatusLine("Saving");
                startIndeterminateProgress();
            });
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

            // Register layer names with geoCore. Need to compensate the 1-index for the layer registration.
            g.addLayerName("L" + (layerIndex + 1) + "D0", commonVars.getLayerSettings(layerIndex).getString(EntropyLayerSettings.properties_s.name));

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
            gcell_root.cellName = commonVars.getLayerSettings(layerIndex).getString(EntropyLayerSettings.properties_s.name);

            // Let's get our geometry for the layer.
            // We can't use the viewport data here because it might be tessellated, so we need to evaluate the contours for the layer.
            List<PreviewShape> previewShapes = generate_shapes(layerIndex);

            const double progress = 0;
            Application.Instance.Invoke(() =>
            {
                updateProgressBar(progress);
            });

            foreach (PreviewShape t in previewShapes)
            {
                List<GeoLibPointF[]> polys = t.getPoints();
                for (int poly = 0; poly < polys.Count; poly++)
                {
                    // No drawn polygons desired.
                    if (t.getDrawnPoly(poly))
                    {
                        continue;
                    }

                    GeoLibPoint[] ePoly = GeoWrangler.resize_to_int(polys[poly], scale);

                    gcell_root.addPolygon(ePoly.ToArray(), layerIndex + 1, 0); // layer is 1-index based for output, so need to offset value accordingly.
                }
            }

            g.setDrawing(drawing_);
            g.setValid(true);

            switch (type)
            {
                case (int)GeoCore.fileType.gds:
                    gdsWriter gw = new(g, file)
                    {
                        statusUpdateUI = updateStatusLine,
                        progressUpdateUI = updateProgressBar
                    };
                    gw.save();
                    break;
                case (int)GeoCore.fileType.oasis:
                    oasWriter ow = new(g, file)
                    {
                        statusUpdateUI = updateStatusLine,
                        progressUpdateUI = updateProgressBar
                    };
                    ow.save();
                    break;
            }
        }
        finally
        {
            sw.Stop();
            Application.Instance.Invoke(() =>
            {
                updateStatusLine("Done in " + sw.Elapsed.TotalSeconds.ToString("0.00") + " s.");
            });
            Monitor.Exit(exportLock);
        }

    }

}