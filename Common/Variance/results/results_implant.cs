using System.Collections.Generic;
using System.Linq;
using color;
using geoLib;

namespace Variance;

public class Results_implant
{
    private List<PreviewShape> resistShapes; // poly at 0 is the resist contour being evaluated. poly at 1 is the bg poly to complete the shape.

    public List<PreviewShape> getResistShapes()
    {
        return pGetResistShapes();
    }

    private List<PreviewShape> pGetResistShapes()
    {
        return resistShapes;
    }

    public void setResistShapes(Colors colors, GeoLibPointF[] geom, GeoLibPointF[] bgGeom, GeoLibPointF[] shadow, GeoLibPointF[] min_, GeoLibPointF[] max_)
    {
        pSetResistShapes(colors, geom, bgGeom, shadow, min_, max_);
    }

    private void pSetResistShapes(Colors colors, GeoLibPointF[] geom, GeoLibPointF[] bgGeom, GeoLibPointF[] shadow, GeoLibPointF[] min_, GeoLibPointF[] max_)
    {
        resistShapes.Clear();

        resistShapes.Add(new PreviewShape());
        resistShapes[0].addPoints(geom);
        resistShapes[0].setColor(colors.implantResist_Color);

        resistShapes.Add(new PreviewShape());
        resistShapes[1].addPoints(bgGeom);
        resistShapes[1].setColor(colors.implantResist_Color);

        shadowLine = new PreviewShape();
        shadowLine.addPoints(shadow);
        shadowLine.setColor(colors.implantMean_Color);

        minShadowLine = new PreviewShape();
        minShadowLine.addPoints(min_);
        minShadowLine.setColor(colors.implantMin_Color);

        maxShadowLine = new PreviewShape();
        maxShadowLine.addPoints(max_);
        maxShadowLine.setColor(colors.implantMax_Color);

    }

    public void setResistShapes(List<PreviewShape> newShapes)
    {
        pSetResistShapes(newShapes);
    }

    private void pSetResistShapes(List<PreviewShape> newShapes)
    {
        resistShapes = newShapes.ToList();
    }

    public void clear()
    {
        pClear();
    }

    private void pClear()
    {
        resistShapes.Clear();
        shadowLine.clearPoints();
        minShadowLine.clearPoints();
        maxShadowLine.clearPoints();
    }

    public void set(Results_implant newResult)
    {
        pSet(newResult);
    }

    private void pSet(Results_implant newResult)
    {
        resistShapes = newResult.resistShapes.ToList();
        shadowLine.setPoints(newResult.shadowLine.getPoints());
        shadowLine.setColor(newResult.shadowLine.getColor());
        minShadowLine.setPoints(newResult.minShadowLine.getPoints());
        minShadowLine.setColor(newResult.minShadowLine.getColor());
        maxShadowLine.setPoints(newResult.maxShadowLine.getPoints());
        maxShadowLine.setColor(newResult.maxShadowLine.getColor());
        valid = newResult.valid;
    }

    public enum lines { shadow, min, max }

    private PreviewShape shadowLine, minShadowLine, maxShadowLine;

    public PreviewShape getLine(lines l)
    {
        return pGetLine(l);
    }

    private PreviewShape pGetLine(lines l)
    {
        PreviewShape ret = new();
        switch (l)
        {
            case lines.shadow:
                ret = shadowLine;
                break;
            case lines.min:
                ret = minShadowLine;
                break;
            case lines.max:
                ret = maxShadowLine;
                break;
        }

        return ret;
    }

    private string result, min, max;

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

    public string getMin()
    {
        return pGetMin();
    }

    private string pGetMin()
    {
        return min;
    }

    public void setMin(string val)
    {
        pSetMin(val);
    }

    private void pSetMin(string val)
    {
        min = val;
    }

    public string getMax()
    {
        return pGetMax();
    }

    private string pGetMax()
    {
        return max;
    }

    public void setMax(string val)
    {
        pSetMax(val);
    }

    private void pSetMax(string val)
    {
        max = val;
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

    public double resistHeightVar, resistWidthVar, resistCRRVar, tiltVar, twistVar;

    public Results_implant()
    {
        init();
    }

    private void init()
    {
        resistShapes = new List<PreviewShape>();
        shadowLine = new PreviewShape();
        minShadowLine = new PreviewShape();
        maxShadowLine = new PreviewShape();
        valid = false;
    }
}