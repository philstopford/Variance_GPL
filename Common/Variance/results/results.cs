using System.Collections.Generic;
using System.Linq;
using Clipper2Lib;

namespace Variance;

/// <summary>
/// Container class for simulation results including geometric shapes and calculated values.
/// Stores both preview shapes for real-time visualization and final computed results.
/// Used throughout the Monte Carlo simulation system for result storage and analysis.
/// </summary>
public class Results
{
    /// <summary>Collection of geometric paths for preview/visualization purposes</summary>
    private List<PathsD> previewShapes;
    
    /// <summary>Collection of processed shapes with layer index information</summary>
    private List<PreviewShape> simShapes;

    /// <summary>
    /// Gets the layer index associated with a specific shape.
    /// Used to determine which simulation layer a shape belongs to.
    /// </summary>
    /// <param name="shapeIndex">Index of the shape in the collection</param>
    /// <returns>Layer index for the specified shape</returns>
    public int getLayerIndex(int shapeIndex)
    {
        return pGetLayerIndex(shapeIndex);
    }

    /// <summary>
    /// Private implementation of layer index getter with bounds checking.
    /// </summary>
    /// <param name="shapeIndex">Index of the shape to query</param>
    /// <returns>Layer index for the shape</returns>
    private int pGetLayerIndex(int shapeIndex)
    {
        return simShapes[shapeIndex].getIndex();
    }
    
    /// <summary>
    /// Gets the collection of preview shapes for visualization.
    /// These shapes are used for real-time display during simulation runs.
    /// </summary>
    /// <returns>List of geometric paths representing preview shapes</returns>
    public List<PathsD> getPreviewShapes()
    {
        return pGetPreviewShapes();
    }

    /// <summary>
    /// Private implementation of preview shapes getter.
    /// </summary>
    /// <returns>Current preview shapes collection</returns>
    private List<PathsD> pGetPreviewShapes()
    {
        return previewShapes;
    }

    /// <summary>
    /// Clears all preview shapes from the collection.
    /// Used to reset visualization state between simulation runs.
    /// </summary>
    public void clearPreviewShapes()
    {
        pClearPreviewShapes();
    }

    /// <summary>
    /// Private implementation of preview shapes clearing.
    /// </summary>
    private void pClearPreviewShapes()
    {
        previewShapes.Clear();
    }

    /// <summary>
    /// Sets the preview shapes collection to a new set of geometric paths.
    /// Creates a defensive copy to prevent external modification.
    /// </summary>
    /// <param name="newPreviewShapes">New collection of preview shapes to set</param>
    public void setPreviewShapes(List<PathsD> newPreviewShapes)
    {
        pSetPreviewShapes(newPreviewShapes);
    }

    /// <summary>
    /// Private implementation of preview shapes setter with optimized copying.
    /// Uses more efficient copying for better performance.
    /// </summary>
    /// <param name="newPreviewShapes">New shapes to copy into the preview collection</param>
    private void pSetPreviewShapes(List<PathsD> newPreviewShapes)
    {
        // Optimize: Pre-allocate with known capacity and use AddRange
        previewShapes = new List<PathsD>(newPreviewShapes.Count);
        previewShapes.AddRange(newPreviewShapes);
    }

    public List<PreviewShape> getSimShapes()
    {
        return pGetSimShapes();
    }

    private List<PreviewShape> pGetSimShapes()
    {
        return simShapes;
    }

    public void setSimShapes(List<PreviewShape> newSimShapes)
    {
        pSetSimShapes(newSimShapes);
    }

    private void pSetSimShapes(List<PreviewShape> newSimShapes)
    {
        // Optimize: Pre-allocate with known capacity and use AddRange
        simShapes = new List<PreviewShape>(newSimShapes.Count);
        simShapes.AddRange(newSimShapes);
    }

    public void clearSimShapes()
    {
        pClearSimShapes();
    }

    private void pClearSimShapes()
    {
        simShapes.Clear();
    }

    private string result;

    public string getResult()
    {
        return pGetResult();
    }

    private string pGetResult()
    {
        return result;
    }

    public void setResult(string val)
    {
        pSetResult(val);
    }

    private void pSetResult(string val)
    {
        result = val;
    }

    private PathsD points; // result points.

    public PathsD getPoints()
    {
        return pGetPoints();
    }

    private PathsD pGetPoints()
    {
        return points;
    }

    public void setPoints(PathsD newPoints)
    {
        pSetPoints(newPoints);
    }

    private void pSetPoints(PathsD newPoints)
    {
        points = new PathsD(newPoints);
    }

    public void clearPoints()
    {
        pClearPoints();
    }

    private void pClearPoints()
    {
        points.Clear();
    }

    public enum fields_d { svar, tvar, lwr, lwr2, htip, vtip, icv, ocv, olx, oly, wob }

    private double[] CDUSVar;
    private double[] CDUTVar;
    private double[] LWRVar;
    private double[] LWR2Var;
    private double[] horTipBiasVar;
    private double[] verTipBiasVar;
    private double[] iCVar;
    private double[] oCVar;
    private double[] overlayX;
    private double[] overlayY;
    private double[] wobbleVar;

    public double[] getFields(fields_d f)
    {
        return pGetFields(f);
    }

    private double[] pGetFields(fields_d f)
    {
        double[] ret = f switch
        {
            fields_d.svar => CDUSVar,
            fields_d.tvar => CDUTVar,
            fields_d.lwr => LWRVar,
            fields_d.lwr2 => LWR2Var,
            fields_d.htip => horTipBiasVar,
            fields_d.vtip => verTipBiasVar,
            fields_d.icv => iCVar,
            fields_d.ocv => oCVar,
            fields_d.olx => overlayX,
            fields_d.oly => overlayY,
            fields_d.wob => wobbleVar,
            _ => []
        };

        return ret;
    }

    public double getField(fields_d f, int index)
    {
        return pGetField(f, index);
    }

    private double pGetField(fields_d f, int index)
    {
        double[] t = pGetFields(f);
        return t[index];
    }

    public void setFields(fields_d f, double[] val)
    {
        pSetFields(f, val);
    }

    private void pSetFields(fields_d f, double[] val)
    {
        switch (f)
        {
            case fields_d.svar:
                CDUSVar = val;
                break;
            case fields_d.tvar:
                CDUTVar = val;
                break;
            case fields_d.lwr:
                LWRVar = val;
                break;
            case fields_d.lwr2:
                LWR2Var = val;
                break;
            case fields_d.htip:
                horTipBiasVar = val;
                break;
            case fields_d.vtip:
                verTipBiasVar = val;
                break;
            case fields_d.icv:
                iCVar = val;
                break;
            case fields_d.ocv:
                oCVar = val;
                break;
            case fields_d.olx:
                overlayX = val;
                break;
            case fields_d.oly:
                overlayY = val;
                break;
            case fields_d.wob:
                wobbleVar = val;
                break;
        }
    }

    public void setField(fields_d f, int index, double val)
    {
        pSetField(f, index, val);
    }

    private void pSetField(fields_d f, int index, double val)
    {
        double[] t = pGetFields(f);
        t[index] = val;
    }

    public enum fields_i { lwrs, lwr2s }

    private int[] LWRSeed;
    private int[] LWR2Seed;

    public int[] getSeeds(fields_i f)
    {
        return pGetSeeds(f);
    }

    private int[] pGetSeeds(fields_i f)
    {
        int[] ret = f switch
        {
            fields_i.lwrs => LWRSeed,
            fields_i.lwr2s => LWR2Seed,
            _ => []
        };

        return ret;
    }

    public int getSeed(fields_i f, int index)
    {
        return pGetSeed(f, index);
    }

    private int pGetSeed(fields_i f, int index)
    {
        int[] t = pGetSeeds(f);
        return t[index];
    }

    public void setSeeds(fields_i f, int[] val)
    {
        pSetSeeds(f, val);
    }

    private void pSetSeeds(fields_i f, int[] val)
    {
        switch (f)
        {
            case fields_i.lwrs:
                LWRSeed = val;
                break;
            case fields_i.lwr2s:
                LWR2Seed = val;
                break;
        }
    }


    private bool valid;

    public void setValid(bool val)
    {
        pSetValid(val);
    }

    private void pSetValid(bool val)
    {
        valid = val;
    }

    public bool isValid()
    {
        return pIsValid();
    }

    private bool pIsValid()
    {
        return valid;
    }

    public Results()
    {
        init();
    }

    private void init()
    {
        LWRSeed = [];
        LWR2Seed = [];

        previewShapes = [];
        valid = false;
    }

    public void genPreviewShapes()
    {
        pGenPreviewShapes();
    }

    private void pGenPreviewShapes()
    {
        previewShapes.Clear();
        for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
        {
            previewShapes.Add(new PathsD(simShapes[layer].getPoints()));
        }
    }
}