using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clipper2Lib;

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

    private List<PathsD> previewShapes;

    public List<PathsD> getPreviewShapes()
    {
        return pGetPreviewShapes();
    }

    private List<PathsD> pGetPreviewShapes()
    {
        return previewShapes;
    }

    public PathsD getLayerPreviewShapes(int layer)
    {
        return pGetLayerPreviewShapes(layer);
    }

    private PathsD pGetLayerPreviewShapes(int layer)
    {
        return previewShapes[layer];
    }

    public PathD getLayerPreviewShapePoly(int layer, int poly)
    {
        return pGetLayerPreviewShapePoly(layer, poly);
    }

    private PathD pGetLayerPreviewShapePoly(int layer, int poly)
    {
        return previewShapes[layer][poly];
    }

    private PathsD points;

    public PathsD getPoints()
    {
        return pGetPoints();
    }

    private PathsD pGetPoints()
    {
        return points;
    }

    public PathD getPoints(int index)
    {
        return pGetPoints(index);
    }

    private PathD pGetPoints(int index)
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
        previewShapes = new List<PathsD>();
        points = new () {Helper.initedPathD(1)};
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

    public void updatePreview(List<PreviewShape> simShapes_, List<PathsD> previewShapes_, PathsD points_, string resultText_)
    {
        pUpdatePreview(simShapes_, previewShapes_, points_, resultText_);
    }

    private void pUpdatePreview(List<PreviewShape> simShapes_, List<PathsD> previewShapes_, PathsD points_, string resultText_)
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

    private void updatePreview(List<PreviewShape> simShapes_, List<PathsD> previewShapes_, PathsD points_)
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
            points = new (points_);
        }
        catch (Exception)
        {
            // Doesn't matter.
        }
    }
}