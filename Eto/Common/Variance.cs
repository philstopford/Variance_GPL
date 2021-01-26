using Eto;
using Eto.Forms;
using System;
using System.ComponentModel;
using Error;

namespace Variance
{
    public class VarianceApplication : Application
    {
        bool doPrompts; // pass this as a reference to allow UI to decide whether prompts are shown or not.
        VarianceContextGUI varianceContext;
        public VarianceApplication(Platform platform, VarianceContextGUI vContext) : base(platform)
        {
            /*
            if (DateTime.Now > new DateTime(2021, 02, 28))
            {
                ErrorReporter.showMessage_OK("Contact phil.stopford@gmail.com", "Build expired!");
                Quit();
            }
            */
            varianceContext = vContext;
        }

        protected override void OnInitialized(EventArgs e)
        {
            MainForm = new MainForm(ref doPrompts, varianceContext);
            base.OnInitialized(e);
            MainForm.Show();
        }

        protected override void OnTerminating(CancelEventArgs e)
        {
            base.OnTerminating(e);

            if (doPrompts)
            {
                var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
                if (result == DialogResult.No)
                    e.Cancel = true;
            }
        }
    }
}
