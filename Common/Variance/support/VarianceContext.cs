using System.Collections.Generic;
using color;
using keys;

namespace Variance;

public class VarianceContext
{
    // This is intended to hold application context settings to allow for cleaner handling of
    // commandline options related to headless mode, XML file loading from the command line, viewport switches

    public object previewLock;
    public object implantPreviewLock;
    public bool implantMode { get; set; }
    public string xmlFileArg { get; private set; }
    public int numberOfThreads { get; private set; }
    public object referenceUI { get; set; }
    public string host { get; set; }
    public string port { get; set; }
    public bool ssl { get; set; }
    public bool perJob { get; set; }
    public bool completion { get; set; }
    public string emailAddress { get; set; }
    public string emailPwd { get; set; }
    public int openGLZoomFactor { get; set; }
    public double FGOpacity { get; set; }
    public double BGOpacity { get; set; }
    public bool AA { get; set; }
    public bool FilledPolygons { get; set; }
    public bool drawPoints { get; set; }

    public bool expandUI { get; set; }

    public Colors colors { get; private set; }
    public bool layerPreviewDOETile { get; set; }
    public bool geoCoreCDVariation { get; set; }
    public int HTCount { get; private set; }
    public List<string> rngMappingEquations { get; private set; }
    public string licenseLocation { get; set; }
    public byte[] _certPubicKeyData { get; set; }
    public bool friendlyNumber { get; set; }

    // License data.
    public SimpleAES aes { get; set; }
    public string licenceName { get; set; }
    public string licenceType { get; set; }
    public string licenceExpiration { get; set; }

    public VarianceContext(bool implantMode_, string xmlFileArg_, int numberOfThreads_,
        int HTCount, string refName = "Variance")
    {
        makeContext(implantMode_, xmlFileArg_, numberOfThreads_, HTCount, refName);
    }

    private void makeContext(bool implantMode_, string xmlFileArg_, int numberOfThreads_,
        int HTCount_, string refName)
    {
        previewLock = new object();
        implantPreviewLock = new object();
        implantMode = implantMode_;
        xmlFileArg = xmlFileArg_;
        numberOfThreads = numberOfThreads_;
        emailAddress = "";
        emailPwd = "";
        host = "";
        port = "587";
        ssl = true;
        perJob = false;
        completion = false;
        openGLZoomFactor = 1;
        FilledPolygons = false;
        drawPoints = false;
        expandUI = true;
        AA = true;
        FGOpacity = 0.7;
        BGOpacity = 0.5;
        colors = new Colors();
        layerPreviewDOETile = false;
        geoCoreCDVariation = false;
        HTCount = HTCount_;
        rngMappingEquations = new List<string>();
        friendlyNumber = false;
        expandUI = false;

        string _msg = string.Empty;

        aes = new SimpleAES(Arrays.nameKey, Arrays.nameVector);
        licenceExpiration = "";
        licenceType = "advanced_permanent";
        licenceName = "GPLv3";
    }
}