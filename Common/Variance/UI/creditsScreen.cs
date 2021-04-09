using System;
using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;
using resources;

namespace Variance
{
    public class CreditsScreen : Form
    {
        // public System.Diagnostics.Process p = new System.Diagnostics.Process();

        RichTextArea textBox_credits;

        public CreditsScreen(Form parent, string textToDisplay)
        {
            Title = CentralProperties.productName + " " + CentralProperties.version;
            TableLayout content = new TableLayout();
            Content = content;

            Size = new Size(600, 430);

            Panel imageHolder = new Panel();
            ImageView image = new ImageView();
            image.Image = images.mcImage();
            imageHolder.Size = image.Image.Size;
            imageHolder.Content = image;
            content.Rows.Add(new TableRow());
            content.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(imageHolder, centered: true) });

            content.Rows.Add(new TableRow());
            textBox_credits = new RichTextArea();
            try
            {
                textBox_credits.Font = SystemFonts.Default(13 * 0.66f);
            }
            catch (Exception)
            {

            }
            textBox_credits.Size = new Size(550, 260);
            textBox_credits.Wrap = true;
            textBox_credits.ReadOnly = true;
            textBox_credits.Text = textToDisplay;
            textBox_credits.CaretIndex = 0;

            content.Rows[1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(textBox_credits, centered: true) });

            Resizable = false;
            Maximizable = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        public override void Close()
        {
            Visible = false;
        }

        /*
		private void linkClicked(object sender, EventArgs e)
		{
			// Call Process.Start method to open a browser
			// with link text as URL.
			p = System.Diagnostics.Process.Start("IExplore.exe", e.LinkText);
		}
		*/
    }
}
