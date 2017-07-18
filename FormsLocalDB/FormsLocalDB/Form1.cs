using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;

namespace FormsLocalDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var customers = db.GetCollection<Customer>("customers");

                // Create your new customer instance
                var customer = new Customer
                {
                   // CustomerId=Convert.ToInt64(Guid.NewGuid()),
                    Name = textBox1.Text,
                    AccountId = textBox2.Text,
                    Region = textBox3.Text,
                    Product = textBox4.Text,
                    Supplier = textBox5.Text,
                    Team = textBox6.Text
                };

                // Insert new customer document (Id will be auto-incremented)
                customers.Insert(customer);
                

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListCustomers obj1 = new ListCustomers();
            obj1.Show();
        }
    }
}
