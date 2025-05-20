using System;
using System.Windows.Forms;
using BarangayApplication.Properties;

namespace BarangayApplication
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        // Event handler for the ExitButton click event.
        private void ExitButton_Click(object sender, EventArgs e)
        {
            
        }

        // Event handler for label1 click event (likely a logout label).
        private void label1_Click(object sender, EventArgs e)
        {
            LogoutConfirm logout = new LogoutConfirm(); // Creates a new instance of the LogoutConfirm form.

            logout.ShowDialog(); // Shows the Logout form modally, requiring user interaction before proceeding.
        }

        // Event handler for pictureBox6 click event (likely a logout picture).
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            LogoutConfirm logout = new LogoutConfirm(); // Creates a new instance of the LogoutConfirm form.

            logout.ShowDialog(); // Shows the Logout form modally.
        }

        // Method to load a form into the MainPanel.
        private void loadform(object form)
        {
            // Check if there's already a form in the MainPanel.
            if (this.MainPanel.Controls.Count > 0)
                this.MainPanel.Controls.RemoveAt(0); // Remove the existing form.

            Form f = form as Form; // Cast the object to a Form.
            f.TopLevel = false; // Set the form as a child control, not a top-level window.
            f.Dock = DockStyle.Fill; // Make the form fill the MainPanel.
            this.MainPanel.Controls.Add(f); // Add the form to the MainPanel.
            this.MainPanel.Tag = f; // Store the form in the MainPanel's Tag property (optional, for later reference).
            f.Show(); // Display the form.
        }

        // Event handler for the Overview button click event.
        private void button1_Click(object sender, EventArgs e)
        {
            loadform(new Overview()); // Load the Overview form into the MainPanel.
        }

        // Event handler for the Data button click event.
        private void button2_Click(object sender, EventArgs e)
        {
            loadform(new Data()); // Load the Data form into the MainPanel.
        }

        // Event handler for the Archive button click event.
        private void button3_Click(object sender, EventArgs e)
        {
            loadform(new Archive()); // Load the Archive form into the MainPanel.
        }

        // Event handler for the Logbook button click event.
        private void button4_Click(object sender, EventArgs e)
        {
            loadform(new Logbook()); // Load the Logbook form into the MainPanel.
        }
        private void settingsLogo_Click(object sender, EventArgs e)
        {
            loadform(new Settings()); // Load the Settings form into the MainPanel.
        }
        private void settings_Click(object sender, EventArgs e)
        {
            loadform(new Settings()); // Load the Settings form into the MainPanel.
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {

        }

        private void button2_MouseHover(object sender, EventArgs e)
        {

        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {

        }

        private void button3_MouseHover(object sender, EventArgs e)
        {

        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {

        }

        private void button4_MouseHover(object sender, EventArgs e)
        {

        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {

        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            // Automatically load the Overview form into the MainPanel when MainMenu is opened
            loadform(new Overview());
        }

        private void Sidebar_Paint(object sender, PaintEventArgs e)
        {

        }

        

        
        private void Overview_Click(object sender, EventArgs e)
        {
          
        }

        private void Overviewpic_Click(object sender, EventArgs e)
        {
          
        }

        private void Data_Click(object sender, EventArgs e)
        {
            
        }

        private void Datapic_Click(object sender, EventArgs e)
        {
          
        }

        private void Archive_Click(object sender, EventArgs e)
        {
           
        }

        private void Archivepic_Click(object sender, EventArgs e)
        {
           
        }

        private void Logbook_Click(object sender, EventArgs e)
        {
            
        }

        private void Logbookpic_Click(object sender, EventArgs e)
        {
            
        }

        
    }
}