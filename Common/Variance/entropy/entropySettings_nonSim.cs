namespace Variance;

public class EntropySettings_nonSim
{
    private int externalType { get; set; }
    private int generateExternal { get; set; }
    private int externalCriteria { get; set; }
    private int generateCSV { get; set; }

    private int extCritCondition1 { get; set; }
    private decimal extCritValue1 { get; set; }
    private int extCritCondition2 { get; set; }
    private decimal extCritValue2 { get; set; }
    private int extCritCondition3 { get; set; }
    private decimal extCritValue3 { get; set; }
    private int extCritCondition4 { get; set; }
    private decimal extCritValue4 { get; set; }

    private const int default_externalType = (int)CommonVars.external_Type.svg;
    private const int default_generateExternal = 0;
    private const int default_generateCSV = 0;

    private int greedyMode { get; set; }
    private string comment { get; set; }
    private string paSearchComment { get; set; }

    private const string default_comment = "";
    private const int default_greedyMode = 1;

    private int displayResults { get; set; }
    private int displayShape { get; set; }

    private const int default_displayResults = 1;
    private const int default_displayShape = 1;

    public enum properties_d { externalCritCond1, externalCritCond2, externalCritCond3, externalCritCond4 }

    public decimal getDecimal(properties_d p)
    {
        return pGetDecimal(p);
    }

    private decimal pGetDecimal(properties_d p)
    {
        decimal ret = p switch
        {
            properties_d.externalCritCond1 => extCritValue1,
            properties_d.externalCritCond2 => extCritValue2,
            properties_d.externalCritCond3 => extCritValue3,
            properties_d.externalCritCond4 => extCritValue4,
            _ => 0
        };

        return ret;
    }

    public void setDecimal(properties_d p, decimal val)
    {
        pSetDecimal(p, val);
    }

    private void pSetDecimal(properties_d p, decimal val)
    {
        switch (p)
        {
            case properties_d.externalCritCond1:
                extCritValue1 = val;
                break;
            case properties_d.externalCritCond2:
                extCritValue2 = val;
                break;
            case properties_d.externalCritCond3:
                extCritValue3 = val;
                break;
            case properties_d.externalCritCond4:
                extCritValue4 = val;
                break;
        }
    }

    public void defaultDecimal(properties_d p)
    {
        pDefaultDecimal(p);
    }

    private void pDefaultDecimal(properties_d p)
    {
        switch (p)
        {
            case properties_d.externalCritCond1:
                extCritValue1 = 0;
                break;
            case properties_d.externalCritCond2:
                extCritValue2 = 0;
                break;
            case properties_d.externalCritCond3:
                extCritValue3 = 0;
                break;
            case properties_d.externalCritCond4:
                extCritValue4 = 0;
                break;
        }
    }

    public static decimal getDefaultDecimal(properties_d p)
    {
        return pGetDefaultDecimal(p);
    }

    private static decimal pGetDefaultDecimal(properties_d p)
    {
        decimal ret = p switch
        {
            properties_d.externalCritCond1 or properties_d.externalCritCond2 or properties_d.externalCritCond3
                or properties_d.externalCritCond4 => 0,
            _ => 0
        };

        return ret;
    }

    public enum properties_i { external, externalType, externalCriteria, externalCritCond1, externalCritCond2, externalCritCond3, externalCritCond4, csv, greedy, results, shape }

    public int getInt(properties_i p)
    {
        return pGetInt(p);
    }

    private int pGetInt(properties_i p)
    {
        int ret = p switch
        {
            properties_i.csv => generateCSV,
            properties_i.external => generateExternal,
            properties_i.externalType => externalType,
            properties_i.externalCriteria => externalCriteria,
            properties_i.externalCritCond1 => extCritCondition1,
            properties_i.externalCritCond2 => extCritCondition2,
            properties_i.externalCritCond3 => extCritCondition3,
            properties_i.externalCritCond4 => extCritCondition4,
            properties_i.greedy => greedyMode,
            properties_i.results => displayResults,
            properties_i.shape => displayShape,
            _ => 0
        };

        return ret;
    }

    public void setInt(properties_i p, int val)
    {
        pSetInt(p, val);
    }

    private void pSetInt(properties_i p, int val)
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
            case properties_i.externalCriteria:
                externalCriteria = val;
                break;
            case properties_i.externalCritCond1:
                extCritCondition1 = val;
                break;
            case properties_i.externalCritCond2:
                extCritCondition2 = val;
                break;
            case properties_i.externalCritCond3:
                extCritCondition3 = val;
                break;
            case properties_i.externalCritCond4:
                extCritCondition4 = val;
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

    public void defaultInt(properties_i p)
    {
        pDefaultInt(p);
    }

    private void pDefaultInt(properties_i p)
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
            case properties_i.externalCriteria:
                externalCriteria = 0;
                break;
            case properties_i.externalCritCond1:
                extCritCondition1 = 0;
                break;
            case properties_i.externalCritCond2:
                extCritCondition2 = 0;
                break;
            case properties_i.externalCritCond3:
                extCritCondition3 = 0;
                break;
            case properties_i.externalCritCond4:
                extCritCondition4 = 0;
                break;
            case properties_i.externalType:
                externalType = default_externalType;
                break;
        }
    }

    public static int getDefaultInt(properties_i p)
    {
        return pGetDefaultInt(p);
    }

    private static int pGetDefaultInt(properties_i p)
    {
        int ret = p switch
        {
            properties_i.csv => default_generateCSV,
            properties_i.external => default_generateExternal,
            properties_i.externalType => default_externalType,
            properties_i.externalCriteria or properties_i.externalCritCond1 or properties_i.externalCritCond2
                or properties_i.externalCritCond3 or properties_i.externalCritCond4 => 0,
            properties_i.greedy => default_greedyMode,
            properties_i.results => default_displayResults,
            properties_i.shape => default_displayShape,
            _ => 0
        };

        return ret;
    }

    public enum properties_s { comment, paComment }

    public string getString(properties_s p)
    {
        return pGetString(p);
    }

    private string pGetString(properties_s p)
    {
        string ret = p switch
        {
            properties_s.comment => comment,
            properties_s.paComment => paSearchComment,
            _ => ""
        };
        return ret;
    }

    public void setString(properties_s p, string val)
    {
        pSetString(p, val);
    }

    private void pSetString(properties_s p, string val)
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

    private void pDefaultString(properties_s p)
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

    private void pEntropySettings_nonSim()
    {
        displayResults = default_displayResults;
        displayShape = default_displayShape;

        comment = default_comment;
        paSearchComment = default_comment;
        greedyMode = default_greedyMode;
        generateExternal = default_generateExternal;
        externalType = default_externalType;
        externalCriteria = 0;
        extCritCondition1 = 0;
        extCritCondition2 = 0;
        extCritCondition3 = 0;
        extCritCondition4 = 0;
        generateCSV = default_generateCSV;
    }
}