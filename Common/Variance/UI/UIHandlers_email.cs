using System;
using System.Globalization;
using EmailNS;
using Error;

namespace Variance;

public partial class MainForm
{
    private bool validateEmailSettings()
    {
        return varianceContext.vc.emailAddress != "" &&
               varianceContext.vc.emailPwd != "" &&
               varianceContext.vc.host != "" &&
               varianceContext.vc.port != "";
    }

    private void emailSettingsChanged(object sender, EventArgs e)
    {
        varianceContext.vc.emailAddress = commonVars.getNonSimulationSettings().emailAddress = text_emailAddress.Text;
        varianceContext.vc.host = commonVars.getNonSimulationSettings().host = text_server.Text;
        varianceContext.vc.emailPwd = commonVars.getNonSimulationSettings().emailPwd = varianceContext.vc.aes.EncryptToString(text_emailPwd.Text);
        varianceContext.vc.port = commonVars.getNonSimulationSettings().port = num_port.Value.ToString(CultureInfo.InvariantCulture);
        varianceContext.vc.ssl = commonVars.getNonSimulationSettings().ssl = (bool)checkBox_SSL.Checked!;

        bool emailOK = validateEmailSettings();
        checkBox_EmailCompletion.Enabled = emailOK;
        checkBox_perJob.Enabled = emailOK;
        button_emailTest.Enabled = emailOK;

        commonVars.getNonSimulationSettings().emailOnCompletion = emailOK;
        commonVars.getNonSimulationSettings().emailPerJob = emailOK;
    }

    private void emailTest(object sender, EventArgs e)
    {
        try
        {
            Email.Send(varianceContext.vc.host, varianceContext.vc.port, varianceContext.vc.ssl, "Variance Email Test", "Testing 1 2 3", varianceContext.vc.emailAddress, varianceContext.vc.aes.DecryptString(varianceContext.vc.emailPwd));
        }
        catch (Exception ex)
        {
            ErrorReporter.showMessage_OK(ex.Message, "Error sending mail");
        }
    }

}