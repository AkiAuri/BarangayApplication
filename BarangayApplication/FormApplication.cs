﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BarangayApplication.Models;
using BarangayApplication.Models.Repositories;

namespace BarangayApplication
{
    public partial class FormApplication : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private int _residentId = 0;

        public FormApplication()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void loadform(object form)
        {
            if (this.MainContainer.Controls.Count > 0)
                this.MainContainer.Controls.RemoveAt(0);
            Form f = form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;

            // Adjust layout for Data form
            if (f is Data dataForm)
            {
                dataForm.AdjustLayoutForFormApplication();
            }

            this.MainContainer.Controls.Add(f);
            this.MainContainer.Tag = f;
            f.Show();
        } 
        private void FormApplication_Load(object sender, EventArgs e) 
        {
            // Ensure the Data form is loaded into MainContainer when FormApplication is opened
            loadform(new Data());
        }
        private void MainContainer_Paint_1(object sender, PaintEventArgs e)
        {
        }
        private string GetPurposeText(Resident resident)
        {
            // Method cleared
            return string.Empty;
        }

        // Collapsed / cleared methods
        private void ClearPurposeFlags(Resident resident) { }
        public void EditResident(Resident resident) { }
        private void SetPurposeFlags(Resident resident, string purpose) { }
        private void Finishbtn_Click_1(object sender, EventArgs e) { }
        private void Cancelbtn_Click_1(object sender, EventArgs e) { }
        private void txtHeight_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtTelCel_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtVoterIdNo_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtSpouseContact_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtRentAmount_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtBorderBedAmount_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtRelativeAge1_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtRelativeAge2_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtRelativeAge3_KeyPress(object sender, KeyPressEventArgs e) { }
        private void txtRelativeAge4_KeyPress(object sender, KeyPressEventArgs e) { }
        private void rBtnOwned_CheckedChanged(object sender, EventArgs e) { }
        private void rBtnRent_CheckedChanged(object sender, EventArgs e) { }
        private void rBtnBorderBed_CheckedChanged(object sender, EventArgs e) { }
        private void EnableDoubleBuffering(Control control) { }
        private void PanelBorder(Panel panel) { }
        
        private void btnResidence_Click(object sender, EventArgs e) { }
        private void btnOccupation_Click(object sender, EventArgs e) { }
        private void btnSpouse_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void lblMname_Click(object sender, EventArgs e) { }
        private void txtHeight_TextChanged(object sender, EventArgs e) { }
        private void panel2_Paint_1(object sender, PaintEventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label18_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void txtLname_TextChanged(object sender, EventArgs e) { }
        private void MainContainer_Paint(object sender, PaintEventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }

        protected override CreateParams CreateParams
        {
            get
            {
                // Method cleared
                return base.CreateParams;
            }
        }

        
    }
}
