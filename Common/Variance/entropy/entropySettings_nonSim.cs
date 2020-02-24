using System;

namespace Variance
{
    public class EntropySettings_nonSim
    {
        Int32 externalType { get; set; }
        Int32 generateExternal { get; set; }
        Int32 generateCSV { get; set; }

        static Int32 default_externalType = (Int32)CommonVars.external_Type.svg;
        static Int32 default_generateExternal = 0;
        static Int32 default_generateCSV = 0;

        Int32 greedyMode { get; set; }
        string comment { get; set; }
        string paSearchComment { get; set; }

        static string default_comment = "";
        static Int32 default_greedyMode = 1;

        Int32 displayResults { get; set; }
        Int32 displayShape { get; set; }

        static Int32 default_displayResults = 1;
        static Int32 default_displayShape = 1;

        public enum properties_i { external, externalType, csv, greedy, results, shape }

        public Int32 getValue(properties_i p)
        {
            return pGetValue(p);
        }

        Int32 pGetValue(properties_i p)
        {
            Int32 ret = 0;
            switch (p)
            {
                case properties_i.csv:
                    ret = generateCSV;
                    break;
                case properties_i.external:
                    ret = generateExternal;
                    break;
                case properties_i.externalType:
                    ret = externalType;
                    break;
                case properties_i.greedy:
                    ret = greedyMode;
                    break;
                case properties_i.results:
                    ret = displayResults;
                    break;
                case properties_i.shape:
                    ret = displayShape;
                    break;
            }

            return ret;
        }

        public void setValue(properties_i p, int val)
        {
            pSetValue(p, val);
        }

        void pSetValue(properties_i p, int val)
        {
            switch (p)
            {
                case properties_i.csv:
                    generateCSV = val;
                    break;
                case properties_i.external:
                    generateExternal = val;
                    break;
                case properties_i.externalType:
                    externalType = val;
                    break;
                case properties_i.greedy:
                    greedyMode = val;
                    break;
                case properties_i.results:
                    displayResults = val;
                    break;
                case properties_i.shape:
                    displayShape = val;
                    break;
            }
        }

        public void defaultValue(properties_i p)
        {
            pDefaultValue(p);
        }

        void pDefaultValue(properties_i p)
        {
            switch (p)
            {
                case properties_i.csv:
                    generateCSV = default_generateCSV;
                    break;
                case properties_i.greedy:
                    greedyMode = default_greedyMode;
                    break;
                case properties_i.results:
                    displayResults = default_displayResults;
                    break;
                case properties_i.shape:
                    displayShape = default_displayShape;
                    break;
                case properties_i.external:
                    generateExternal = default_generateExternal;
                    break;
                case properties_i.externalType:
                    externalType = default_externalType;
                    break;
            }
        }

        public Int32 getDefaultValue(properties_i p)
        {
            return pGetDefaultValue(p);
        }

        Int32 pGetDefaultValue(properties_i p)
        {
            Int32 ret = 0;
            switch (p)
            {
                case properties_i.csv:
                    ret = default_generateCSV;
                    break;
                case properties_i.external:
                    ret = default_generateExternal;
                    break;
                case properties_i.externalType:
                    ret = default_externalType;
                    break;
                case properties_i.greedy:
                    ret = default_greedyMode;
                    break;
                case properties_i.results:
                    ret = default_displayResults;
                    break;
                case properties_i.shape:
                    ret = default_displayShape;
                    break;
            }

            return ret;
        }

        public enum properties_s { comment, paComment }

        public string getString(properties_s p)
        {
            return pGetString(p);
        }

        string pGetString(properties_s p)
        {
            string ret = "";
            switch (p)
            {
                case properties_s.comment:
                    ret = comment;
                    break;
                case properties_s.paComment:
                    ret = paSearchComment;
                    break;
            }
            return ret;
        }

        public void setString(properties_s p, string val)
        {
            pSetString(p, val);
        }

        void pSetString(properties_s p, string val)
        {
            switch (p)
            {
                case properties_s.comment:
                    comment = val;
                    break;
                case properties_s.paComment:
                    paSearchComment = val;
                    break;
            }
        }

        public void defaultString(properties_s p)
        {
            pDefaultString(p);
        }

        void pDefaultString(properties_s p)
        {
            switch (p)
            {
                case properties_s.comment:
                    comment = default_comment;
                    break;
                case properties_s.paComment:
                    paSearchComment = default_comment;
                    break;
            }
        }

        public EntropySettings_nonSim()
        {
            pEntropySettings_nonSim();
        }

        void pEntropySettings_nonSim()
        {
            displayResults = default_displayResults;
            displayShape = default_displayShape;

            comment = default_comment;
            paSearchComment = default_comment;
            greedyMode = default_greedyMode;
            generateExternal = default_generateExternal;
            externalType = default_externalType;
            generateCSV = default_generateCSV;
        }
    }
}