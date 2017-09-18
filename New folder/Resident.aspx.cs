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
            string dir = @"C:\Users\TEMP.DESKTOP-GPV1U9D\Desktop";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
          using (var sw = new StreamWriter(Path.Combine(dir, "log.ttl")))
            {
                sw.WriteLine("# filename: log.ttl");
                sw.WriteLine("");
                sw.WriteLine("@prefix ab: < http://schema.org/Apartment#> .");
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine("ab:" + TextBox1.Text+ " ab:numberOFRooms '"  +TextBox2.Text+"'");
                sw.WriteLine("ab:" + TextBox1.Text + " " + "ab:occupancy '" +TextBox3.Text+"'");
            }
           
          
          //  File.WriteAllText(Path.Combine(dir, "log.ttl"), a);

        }
    }
}