using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarangayApplication
{
    public partial class ExitForms: Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
       (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // height of ellipse
           int nHeightEllipse // width of ellipse
       );
        

        public ExitForms()
        {
            InitializeComponent();
            MainViewForm Forms = new MainViewForm();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
        private void Leavebtn_Click(object sender, EventArgs e)
        {
            var mainView = Application.OpenForms["MainViewForm"];
            if (mainView != null)
                mainView.Close();

            var formApp = Application.OpenForms["FormApplication"];
            if (formApp != null)
                formApp.Close();

            this.Close(); // Close the ExitForms dialog itself.
        }

        
        
        
        
        private void ExitForms_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Staybtn_Click(object sender, EventArgs e)
        {
            this.Close(); // Hides the ExitForms form.
        }

        
    }
}
