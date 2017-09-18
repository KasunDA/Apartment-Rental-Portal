using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using System.IO;
using System.Text;
using VDS.RDF.Update;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;
using VDS.RDF.Writing.Contexts;
using VDS.RDF.Storage;
using VDS.RDF.Parsing.Handlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VDS.RDF.Parsing.Contexts;

namespace BootstrapBundleProject.Controllers
{
    public class GraphDBController : Controller
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

        public ActionResult GraphDbImportData()
        {
            try
            {
                //First connect to a store, in this example we use Sesame
                SesameHttpProtocolConnector sesame = new SesameHttpProtocolConnector("http://localhost:7200", "MyData", "admin", "admin");

                //Create a Graph and fill it with data we want to save
                Graph g = new Graph();
                string fileLink = @"D:\acads\Spring 17\semantic web\lab\SW-03-17\BootstrapBundleProject\data\log.ttl";

                ViewBag.FileName = "File " + fileLink + " has been saved to the GraphDB";
                g.LoadFromFile(fileLink);
                //UriLoader.Load(g, new Uri(fileLink));
                //Set its BaseUri property to the URI we want to save it as
                g.BaseUri = new Uri(fileLink);

                //Now save it to the store
                if (!sesame.IsReadOnly)
                {
                    sesame.SaveGraph(g);
                }
                else
                {
                    return Content("Store is read-only");
                }

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return View();
        }

        public ActionResult GraphDbShowData()
        {
            try
            {

                // First connect to a store, in this example we use AllegroGraph

                AllegroGraphConnector store = new AllegroGraphConnector("http://localhost:7200", "MyData", "admin", "admin");

                //Make a SPARQL Query against the store
                Object results = store.Query(@"prefix apt: <http://schema.org/Apartment#>
                                                prefix plc: <http://schema.org/Place#>
prefix thg: <http://schema.org/Thing#>
prefix ra: <http://schema.org/RentAction#>
SELECT ?Description ?Address ?Number_Of_Rooms ?Occupancy ?Owner_Name ?Telephone
                                WHERE
                                { ?apartment apt:occupancy ?Occupancy.
                                  ?apartment thg:description ?Description.
                                  ?apartment plc:address ?Address.
                                  ?apartment apt:numberOfRooms ?Number_Of_Rooms.
                                  ?apartment ra:landlord ?Owner_Name.
                                  ?apartment plc:telephone ?Telephone.
                                }");


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

    }
}