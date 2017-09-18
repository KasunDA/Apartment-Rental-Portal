using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BootstrapBundleProject.Views.Resident
{
    
    public partial class Resident : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
           // FileStream F = new FileStream("a.ttl", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            string dir = @"D:\acads\Spring 17\semantic web\lab\SW-03-17\BootstrapBundleProject\data";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(Path.Combine(dir, "log.ttl")))
            {
                using (var sw = new StreamWriter(Path.Combine(dir, "log.ttl")))
                {
                    sw.WriteLine("# filename: log.ttl");
                    sw.WriteLine("");
                    sw.WriteLine("@prefix apt: < http://schema.org/Apartment#>.");
                    sw.WriteLine("@prefix plc: <http://schema.org/Place#>.");
                    sw.WriteLine("@prefix thg: <http://schema.org/Thing#>.");
                    sw.WriteLine("@prefix ra: <http://schema.org/RentAction#>.");
                    sw.WriteLine("@prefix d: <http://learningsparql.com/ns/data#>.");
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine("d:" + TextBox6.Text + " thg:description '" + TextBox1.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " plc:address '" + TextBox2.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " apt:occupancy '" + TextBox3.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " apt:numberOfRooms '" + TextBox4.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " ra:landlord '" + TextBox5.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " plc:telephone '" + TextBox6.Text + "'.");
                    sw.WriteLine("");

                }
            }
            else
            {
                using (FileStream fs = new FileStream(Path.Combine(dir, "log.ttl"), FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("d:" + TextBox6.Text + " thg:description '" + TextBox1.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " plc:address '" + TextBox2.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " apt:occupancy '" + TextBox3.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " apt:numberOfRooms '" + TextBox4.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " ra:landlord '" + TextBox5.Text + "'.");
                    sw.WriteLine("d:" + TextBox6.Text + " plc:telephone '" + TextBox6.Text + "'.");
                    sw.WriteLine("");
                }
            }
            
            Response.Redirect("http://localhost:49899/Login/success");
            //  File.WriteAllText(Path.Combine(dir, "log.ttl"), a);

        }
    }
}