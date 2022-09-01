using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using geoLib;

namespace Variance;

public class SimulationPreview
{
    private VarianceContext varianceContext;

    private List<PreviewShape> simShapes;
    public List<PreviewShape> getSimShapes()
    {
        return pGetSimShapes();
    }

    private List<PreviewShape> pGetSimShapes()
    {
        return simShapes;
    }

    private List<List<GeoLibPointF[]>> previewShapes;

    public List<List<GeoLibPointF[]>> getPreviewShapes()
    {
        return pGetPreviewShapes();
    }

    private List<List<GeoLibPointF[]>> pGetPreviewShapes()
    {
        return previewShapes;
    }

    public List<GeoLibPointF[]> getLayerPreviewShapes(int layer)
    {
        return pGetLayerPreviewShapes(layer);
    }

    private List<GeoLibPointF[]> pGetLayerPreviewShapes(int layer)
    {
        return previewShapes[layer];
    }

    public GeoLibPointF[] getLayerPreviewShapePoly(int layer, int poly)
    {
        return pGetLayerPreviewShapePoly(layer, poly);
    }

    private GeoLibPointF[] pGetLayerPreviewShapePoly(int layer, int poly)
    {
        return previewShapes[layer][poly];
    }

    private List<GeoLibPointF[]> points;

    public List<GeoLibPointF[]> getPoints()
    {
        return pGetPoints();
    }

    private List<GeoLibPointF[]> pGetPoints()
    {
        return points;
    }

    public GeoLibPointF[] getPoints(int index)
    {
        return pGetPoints(index);
    }

    private GeoLibPointF[] pGetPoints(int index)
    {
        return points[index];
    }

    private string resultText;

    public string getResult()
    {
        return pGetResult();
    }

    private string pGetResult()
    {
        return resultText;
    }

    public SimulationPreview(ref VarianceContext varianceContext)
    {
        pSimulationPreview(ref varianceContext);
    }

    private void pSimulationPreview(ref VarianceContext _varianceContext)
    {
        varianceContext = _varianceContext;
        simShapes = new List<PreviewShape>();
        previewShapes = new List<List<GeoLibPointF[]>>();
        points = new List<GeoLibPointF[]> {new GeoLibPointF[1]};
    }

    private void updatePreview(List<PreviewShape> simShapes_)
    {
        try
        {
            simShapes = simShapes_.ToList();
        }
        catch (Exception)
        {
            // Doesn't matter.
        }
    }

    public void updatePreview(string resultText_)
    {
        pUpdatePreview(resultText_);
    }

    private void pUpdatePreview(string resultText_)
    {
        resultText = resultText_;
    }

    public void updatePreview(SimResultPackage resultPackage)
    {
        pUpdatePreview(resultPackage);
    }

    private void pUpdatePreview(SimResultPackage resultPackage)
    {
        if (Monitor.IsEntered(varianceContext.previewLock))
        {
            updatePreview(resultPackage.getPreviewResult().getSimShapes(), resultPackage.getPreviewResult().getPreviewShapes(),
                resultPackage.getPreviewResult().getPoints(), resultPackage.getMeanAndStdDev());
        }
    }

    public void updatePreview(List<PreviewShape> simShapes_, List<List<GeoLibPointF[]>> previewShapes_, List<GeoLibPointF[]> points_, string resultText_)
    {
        pUpdatePreview(simShapes_, previewShapes_, points_, resultText_);
    }

    private void pUpdatePreview(List<PreviewShape> simShapes_, List<List<GeoLibPointF[]>> previewShapes_, List<GeoLibPointF[]> points_, string resultText_)
    {
        try
        {
            updatePreview(simShapes_, previewShapes_, points_);
        }
        catch (Exception)
        {
            // Doesn't matter.
        }
        try
        {
            updatePreview(resultText_);
        }
        catch (Exception)
        {
            // Doesn't matter.
        }
    }

    private void updatePreview(List<PreviewShape> simShapes_, List<List<GeoLibPointF[]>> previewShapes_, List<GeoLibPointF[]> points_)
    {
        updatePreview(simShapes_);
        try
        {
            previewShapes = previewShapes_.ToList();
        }
        catch (Exception)
        {
            // Doesn't matter.
        }
        try
        {
            points = points_.ToList();
        }
        catch (Exception)
        {
            // Doesn't matter.
        }
    }
}