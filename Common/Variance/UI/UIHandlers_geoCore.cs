using Eto.Forms;
using geoCoreLib;
using geoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Variance
{
    public partial class MainForm : Form
    {
        void exportActiveLayerToLayout(object sender, EventArgs e)
        {
            exportActiveLayerToLayout();
        }

        async void exportActiveLayerToLayout()
        {
            // Need to request output file location and name.
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

                int type = (int)geoCoreLib.GeoCore.fileType.gds;

                if (ext == "OAS")
                {
                    type = (int)geoCoreLib.GeoCore.fileType.oasis;
                }

                Application.Instance.Invoke(() =>
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
                Application.Instance.Invoke(() =>
                {
                    statusProgressBar.Indeterminate = false;
                });
            }
        }

        object exportLock = new object();
        void toGeoCore(int type, string file)
        {
            int layerIndex = getSelectedLayerIndex();

            Monitor.Enter(exportLock);
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Reset();
                sw.Start();
                Application.Instance.Invoke(() =>
                {
                    updateStatusLine("Saving");
                    startIndeterminateProgress();
                });
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

                // Register layer names with geoCore. Need to compensate the 1-index for the layer registration.
                g.addLayerName("L" + (layerIndex + 1).ToString() + "D0", commonVars.getLayerSettings(layerIndex).getString(EntropyLayerSettings.properties_s.name));

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

                // Set to 1 to avoid problems if there are fewer than 100 patterns.
                int updateInterval = Math.Max(1, previewShapes.Count);
                double progress = 0;
                Application.Instance.Invoke(() =>
                {
                    updateProgressBar(progress);
                });

                for (int i = 0; i < previewShapes.Count; i++)
                {
                    List<GeoLibPointF[]> polys = previewShapes[i].getPoints();
                    for (int poly = 0; poly < polys.Count; poly++)
                    {
                        // No drawn polygons desired.
                        if (!previewShapes[i].getDrawnPoly(poly))
                        {
                            int length = polys[poly].Length;
                            GeoLibPoint[] ePoly = new GeoLibPoint[length];
                            Parallel.For(0, length, (pt) =>
                            // for (int pt = 0; pt < length; pt++)
                            {
                                ePoly[pt] = new GeoLibPoint((int)(polys[poly][pt].X * scale), (int)(polys[poly][pt].Y * scale));
                            });

                            gcell_root.addPolygon(ePoly.ToArray(), layerIndex + 1, 0); // layer is 1-index based for output, so need to offset value accordingly.
                        }
                    }
                }

                g.setDrawing(drawing_);
                g.setValid(true);

                switch (type)
                {
                    case (int)GeoCore.fileType.gds:
                        gds.gdsWriter gw = new gds.gdsWriter(g, file);
                        gw.statusUpdateUI = updateStatusLine;
                        gw.progressUpdateUI = updateProgressBar;
                        gw.save();
                        break;
                    case (int)GeoCore.fileType.oasis:
                        oasis.oasWriter ow = new oasis.oasWriter(g, file);
                        ow.statusUpdateUI = updateStatusLine;
                        ow.progressUpdateUI = updateProgressBar;
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
}
