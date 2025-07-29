using System;

namespace Variance;

[Serializable]
public class EntropyImplantSettings
{
    public enum properties_d { w, wV, h, hV, cRR, cV, tilt, tiltV, twist, twistV }

    private string comment;
    private double resistWidth;
    private double resistWidthVar;
    private double resistHeight_postDevelop;
    private double resistHeight_postDevelopVar;
    private double resistCRR;
    private double resistCRRVar;
    private double tiltAngle;
    private double tiltAngleVar;
    private double twistAngle;
    private double twistAngleVar;

    private const double default_resistWidth = 0.0;
    private const double default_resistWidthVar = 0.0;
    private const double default_resistHeight_postDevelop = 0.0;
    private const double default_resistHeight_postDevelopVar = 0.0;
    private const double default_resistCRR = 0.0;
    private const double default_resistCRRVar = 0.0;
    private const double default_tilt = 0.0;
    private const double default_tiltVar = 0.0;
    private const double default_twist = 0.0;
    private const double default_twistVar = 0.0;

    public EntropyImplantSettings()
    {
        pEntropyImplantSettings();
    }

    private void pEntropyImplantSettings()
    {
        comment = "";
        resistWidth = default_resistWidth;
        resistHeight_postDevelop = default_resistHeight_postDevelop;
        resistCRR = default_resistCRR;
        tiltAngle = default_tilt;
        twistAngle = default_twist;
        resistWidthVar = default_resistWidthVar;
        resistHeight_postDevelopVar = default_resistHeight_postDevelopVar;
        resistCRRVar = default_resistCRRVar;
        tiltAngleVar = default_tiltVar;
        twistAngleVar = default_twistVar;
    }

    public void setComment(string text)
    {
        pSetComment(text);
    }

    private void pSetComment(string text)
    {
        comment = text;
    }

    public string getComment()
    {
        return pGetComment();
    }

    private string pGetComment()
    {
        return comment;
    }

    public void setDouble(properties_d p, double val)
    {
        pSetDouble(p, val);
    }

    private void pSetDouble(properties_d p, double val)
    {
        switch (p)
        {
            case properties_d.w:
                resistWidth = val;
                break;
            case properties_d.wV:
                resistWidthVar = val;
                break;
            case properties_d.h:
                resistHeight_postDevelop = val;
                break;
            case properties_d.hV:
                resistHeight_postDevelopVar = val;
                break;
            case properties_d.cRR:
                resistCRR = val;
                break;
            case properties_d.cV:
                resistCRRVar = val;
                break;
            case properties_d.tilt:
                tiltAngle = val;
                break;
            case properties_d.tiltV:
                tiltAngleVar = val;
                break;
            case properties_d.twist:
                twistAngle = val;
                break;
            case properties_d.twistV:
                twistAngleVar = val;
                break;
        }
    }

    public void defaultDouble(properties_d p)
    {
        pDefaultDouble(p);
    }

    private void pDefaultDouble(properties_d p)
    {
        switch (p)
        {
            case properties_d.w:
                resistWidth = default_resistWidth;
                break;
            case properties_d.wV:
                resistWidthVar = default_resistWidthVar;
                break;
            case properties_d.h:
                resistHeight_postDevelop = default_resistHeight_postDevelop;
                break;
            case properties_d.hV:
                resistHeight_postDevelopVar = default_resistHeight_postDevelopVar;
                break;
            case properties_d.cRR:
                resistCRR = default_resistCRR;
                break;
            case properties_d.cV:
                resistCRRVar = default_resistCRRVar;
                break;
            case properties_d.tilt:
                tiltAngle = default_tilt;
                break;
            case properties_d.tiltV:
                tiltAngleVar = default_tiltVar;
                break;
            case properties_d.twist:
                twistAngle = default_twist;
                break;
            case properties_d.twistV:
                twistAngleVar = default_twistVar;
                break;
        }
    }

    public double getDouble(properties_d p)
    {
        return pGetDouble(p);
    }

    private double pGetDouble(properties_d p)
    {
        double ret = p switch
        {
            properties_d.w => resistWidth,
            properties_d.wV => resistWidthVar,
            properties_d.h => resistHeight_postDevelop,
            properties_d.hV => resistHeight_postDevelopVar,
            properties_d.cRR => resistCRR,
            properties_d.cV => resistCRRVar,
            properties_d.tilt => tiltAngle,
            properties_d.tiltV => tiltAngleVar,
            properties_d.twist => twistAngle,
            properties_d.twistV => twistAngleVar,
            _ => 0
        };
        return ret;
    }

    public static double getDefaultDouble(properties_d p)
    {
        return pGetDefaultDouble(p);
    }

    private static double pGetDefaultDouble(properties_d p)
    {
        double ret = p switch
        {
            properties_d.w => default_resistWidth,
            properties_d.wV => default_resistWidthVar,
            properties_d.h => default_resistHeight_postDevelop,
            properties_d.hV => default_resistHeight_postDevelopVar,
            properties_d.cRR => default_resistCRR,
            properties_d.cV => default_resistCRRVar,
            properties_d.tilt => default_tilt,
            properties_d.tiltV => default_tiltVar,
            properties_d.twist => default_twist,
            properties_d.twistV => default_twistVar,
            _ => 0
        };
        return ret;
    }
}