using System.Collections.Generic;
using geoLib;

namespace Variance;

public class NonSimulationSettings
{
    public string version { get; private set; }

    public string host { get; set; }
    public string port { get; set; }
    public string emailAddress { get; set; }
    public string emailPwd { get; set; }
    public bool emailOnCompletion { get; set; }
    public bool emailPerJob { get; set; }
    public bool ssl { get; set; }
    public List<List<GeoLibPointF[]>> extractedTile { get; set; } // put this here because we don't want to track this directly.

    public NonSimulationSettings(string _version)
    {
        pNonSimulationSettings(_version);
    }

    private void pNonSimulationSettings(string _version)
    {
        version = _version;
        extractedTile = new List<List<GeoLibPointF[]>>(); // to hold extracted tile List<PointF[]> for each layer.
        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            extractedTile.Add(new List<GeoLibPointF[]>());
        }
    }
}