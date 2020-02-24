using System;
using Veldrid;

namespace Variance
{
    public class VarianceContextGUI
    {
        public GraphicsBackend backend { get; set; }
        public VarianceContext vc;

        public VarianceContextGUI(bool implantMode_, string xmlFileArg_, int numberOfThreads_,
                             Int32 HTCount, GraphicsBackend backend_, string refName = "Variance")
        {
            vc = new VarianceContext(implantMode_, xmlFileArg_, numberOfThreads_, HTCount, refName);
            backend = backend_;
        }
    }
}
