using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsLocalDB
{
    public partial class ListCustomers : Form
    {
        public ListCustomers()
        {
            InitializeComponent();
            PrepCustomers();
        }
        internal void PrepCustomers()
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                var customers = db.GetCollection<Customer>("customers");
              //  var results = customers.Find(x=> x.Name.StartsWith(""));
                var results = customers.FindAll();
                dataGridView1.DataSource = results.ToList();
               
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                var customers = db.GetCollection<Customer>("customers");
                if (textBox6.Text.Length != 0)
                {
                    var results = customers.Find(x => x.AccountId.Equals(textBox6.Text) || x.Name.StartsWith(textBox6.Text) || x.CustomerId==Convert.ToInt64(textBox6.Text));
                    // var results = customers.Find(x => x.AccountId.Equals(textBox6.Text));
                    dataGridView1.DataSource = results.ToList();
                }
                else
                {
                    var results = customers.FindAll();
                    dataGridView1.DataSource = results.ToList();
                }
                 
            }
        }
    }
}
