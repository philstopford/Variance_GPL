using System;

namespace Variance
{
    [Serializable]
    public class EntropyImplantSettings
    {
        public enum properties_d { w, wV, h, hV, cRR, cV, tilt, tiltV, twist, twistV }

        string comment;
        double resistWidth;
        double resistWidthVar;
        double resistHeight_postDevelop;
        double resistHeight_postDevelopVar;
        double resistCRR;
        double resistCRRVar;
        double tiltAngle;
        double tiltAngleVar;
        double twistAngle;
        double twistAngleVar;

        static double default_resistWidth = 0.0;
        static double default_resistWidthVar = 0.0;
        static double default_resistHeight_postDevelop = 0.0;
        static double default_resistHeight_postDevelopVar = 0.0;
        static double default_resistCRR = 0.0;
        static double default_resistCRRVar = 0.0;
        static double default_tilt = 0.0;
        static double default_tiltVar = 0.0;
        static double default_twist = 0.0;
        static double default_twistVar = 0.0;

        public EntropyImplantSettings()
        {
            pEntropyImplantSettings();
        }

        void pEntropyImplantSettings()
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

        void pSetComment(string text)
        {
            comment = text;
        }

        public string getComment()
        {
            return pGetComment();
        }

        string pGetComment()
        {
            return comment;
        }

        public void setDouble(properties_d p, double val)
        {
            pSetDouble(p, val);
        }

        void pSetDouble(properties_d p, double val)
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

        void pDefaultDouble(properties_d p)
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

        double pGetDouble(properties_d p)
        {
            double ret = 0;
            switch (p)
            {
                case properties_d.w:
                    ret = resistWidth;
                    break;
                case properties_d.wV:
                    ret = resistWidthVar;
                    break;
                case properties_d.h:
                    ret = resistHeight_postDevelop;
                    break;
                case properties_d.hV:
                    ret = resistHeight_postDevelopVar;
                    break;
                case properties_d.cRR:
                    ret = resistCRR;
                    break;
                case properties_d.cV:
                    ret = resistCRRVar;
                    break;
                case properties_d.tilt:
                    ret = tiltAngle;
                    break;
                case properties_d.tiltV:
                    ret = tiltAngleVar;
                    break;
                case properties_d.twist:
                    ret = twistAngle;
                    break;
                case properties_d.twistV:
                    ret = twistAngleVar;
                    break;
            }
            return ret;
        }

        public double getDefaultDouble(properties_d p)
        {
            return pGetDefaultDouble(p);
        }

        double pGetDefaultDouble(properties_d p)
        {
            double ret = 0;
            switch (p)
            {
                case properties_d.w:
                    ret = default_resistWidth;
                    break;
                case properties_d.wV:
                    ret = default_resistWidthVar;
                    break;
                case properties_d.h:
                    ret = default_resistHeight_postDevelop;
                    break;
                case properties_d.hV:
                    ret = default_resistHeight_postDevelopVar;
                    break;
                case properties_d.cRR:
                    ret = default_resistCRR;
                    break;
                case properties_d.cV:
                    ret = default_resistCRRVar;
                    break;
                case properties_d.tilt:
                    ret = default_tilt;
                    break;
                case properties_d.tiltV:
                    ret = default_tiltVar;
                    break;
                case properties_d.twist:
                    ret = default_twist;
                    break;
                case properties_d.twistV:
                    ret = default_twistVar;
                    break;
            }
            return ret;
        }
    }
}
