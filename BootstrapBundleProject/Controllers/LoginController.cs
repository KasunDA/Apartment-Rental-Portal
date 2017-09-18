using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace BootstrapBundleProject.Controllers
{
    public class LoginController : Controller
    {
        public static DataTable SparqlResultSetToDataTable(SparqlResultSet results)
        {


            DataTable table = new DataTable();
            DataRow row;

            if (results is SparqlResultSet)
            {
                SparqlResultSet rset = (SparqlResultSet)results;

                foreach (String var in rset.Variables)
                {
                    table.Columns.Add(new DataColumn(var));
                }

                foreach (SparqlResult r in rset)
                {
                    row = table.NewRow();

                    foreach (String var in r.Variables)
                    {
                        if (r.HasValue(var) && r[var] != null)
                        {
                            INode n = r[var];
                            switch (n.NodeType)
                            {
                                case NodeType.Literal:
                                    row[var] = ((ILiteralNode)n).Value;
                                    break;
                                case NodeType.Uri:
                                    Uri u = ((IUriNode)n).Uri;
                                    if (!u.Fragment.Equals(String.Empty))
                                    {
                                        row[var] = u.Fragment.Substring(1);
                                    }
                                    else
                                    {
                                        row[var] = u.AbsoluteUri;
                                    }
                                    break;
                                default:
                                    row[var] = n.ToString();
                                    break;
                            }
                        }
                    }
                    table.Rows.Add(row);
                }
            }

            return table;

        }
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Renter()
        {
            try
            {

                //Read in a webpage that may contain some RDFa
                Graph g = new Graph();
                //g.LoadFromFile(@"D:\acads\Spring 17\semantic web\lab\SW-03-17\BootstrapBundleProject\data\ars_renter01.ttl");
                g.LoadFromFile(@"D:\acads\Spring 17\semantic web\lab\SW-03-17\BootstrapBundleProject\data\log.ttl");

                string theQuery = String.Empty;
                theQuery += @"prefix apt: <http://schema.org/Apartment#>";
                theQuery += @"prefix plc: <http://schema.org/Place#>";
                theQuery += @"prefix thg: <http://schema.org/Thing#>";
                theQuery += @"prefix ra: <http://schema.org/RentAction#>";
                string query = @"
                                SELECT ?Description ?Address ?Number_Of_Rooms ?Occupancy ?Owner_Name ?Telephone
                                WHERE
                                { ?apartment apt:occupancy ?Occupancy.
                                  ?apartment thg:description ?Description.
                                  ?apartment plc:address ?Address.
                                  ?apartment apt:numberOfRooms ?Number_Of_Rooms.
                                  ?apartment ra:landlord ?Owner_Name.
                                  ?apartment plc:telephone ?Telephone.
                                }";

                query = theQuery + query;

                //Query the data with SPARQL
                Object results = g.ExecuteQuery(query);
                if (results is SparqlResultSet)
                {
                    //Dump Results to Console
                    SparqlResultSet resultSet = (SparqlResultSet)results;
                    ViewData["Count"] = resultSet.Count().ToString();
                    DataTable dt = SparqlResultSetToDataTable(resultSet);
                    return View(dt);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return View();
        }

        public ActionResult Owner()
        {
            return View();
        }

        public ActionResult Success()
        {
            return View();
        }
        
    }
}