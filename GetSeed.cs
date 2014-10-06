using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MazeMasters
{
    public partial class GetSeed : Form
    {
        public int seed;

        public GetSeed()
        {
            InitializeComponent();
        }

        private void SeedGo_Click(object sender, EventArgs e)
        {
            try
            {
                seed = int.Parse(this.SeedValue.Text);
            }
            catch
            {
                seed = 0;
            }

            this.Close();
        }
    }
}
