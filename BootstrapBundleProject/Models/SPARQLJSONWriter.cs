using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using VDS.RDF.Query;

namespace VDS.RDF.Writing
{
    /// <summary>
    /// Class for saving SPARQL Result Sets to the SPARQL Results JSON Format
    /// </summary>
    public class SPARQLJSONWriter : ISPARQLResultsWriter
    {
        /// <summary>
        /// Saves the Result Set to the given File in the SPARQL Results XML Format
        /// </summary>
        /// <param name="results">Result Set to save</param>
        /// <param name="filename">File to save to</param>
        public void Save(SPARQLResultSet results, String filename)
        {
            StreamWriter output = new StreamWriter(filename, false, Encoding.UTF8);
            this.Save(results, output);
        }

        /// <summary>
        /// Saves the Result Set to the given Stream in the SPARQL Results XML Format
        /// </summary>
        /// <param name="results">Result Set to save</param>
        /// <param name="output">Stream to save to</param>
        public void Save(SPARQLResultSet results, StreamWriter output)
        {
            try
            {
                this.GenerateOutput(results, output);
                output.Close();
            }
            catch
            {
                try
                {
                    output.Close();
                }
                catch
                {
                    //No Catch Actions
                }
                throw;
            }
        }

        /// <summary>
        /// Internal method which generates the SPARQL Query Results JSON output
        /// </summary>
        /// <param name="results">Result Set to save</param>
        /// <param name="output">Stream to save to</param>
        private void GenerateOutput(SPARQLResultSet results, StreamWriter output)
        {
            JsonTextWriter writer = new JsonTextWriter(output);
            writer.Formatting = Formatting.Indented;

            //Start a JSON Object for the Result Set
            writer.WriteStartObject();

            //Create the Head Object
            writer.WritePropertyName("head");
            writer.WriteStartObject();

            if (results.Variables.Count() > 0)
            {
                //SELECT query results

                //Create the Variables Object
                writer.WritePropertyName("vars");
                writer.WriteStartArray();
                foreach (String var in results.Variables)
                {
                    writer.WriteValue(var);
                }
                writer.WriteEndArray();

                //End Head Object
                writer.WriteEndObject();

                //Create the Result Object
                writer.WritePropertyName("results");
                writer.WriteStartObject();
                writer.WritePropertyName("bindings");
                writer.WriteStartArray();

                foreach (SPARQLResult result in results)
                {
                    //Create a Binding Object
                    writer.WriteStartObject();
                    foreach (String var in results.Variables)
                    {
                        if (!result.HasValue(var)) continue; //No output for unbound variables

                        INode value = result.Value(var);
                        if (value == null) continue;

                        //Create an Object for the Variable
                        writer.WritePropertyName(var);
                        writer.WriteStartObject();
                        writer.WritePropertyName("type");

                        switch (value.NodeType)
                        {
                            case NodeType.Blank:
                                //Blank Node
                                writer.WriteValue("bnode");
                                writer.WritePropertyName("value");
                                String id = ((BlankNode)value).InternalID;
                                id = id.Substring(id.IndexOf(':') + 1);
                                writer.WriteValue(id);
                                break;

                            case NodeType.GraphLiteral:
                                //Error
                                throw new RDFException("Result Sets which contain Graph Literal Nodes cannot be serialized in the SPARQL Query Results JSON Format");

                            case NodeType.Literal:
                                //Literal
                                LiteralNode lit = (LiteralNode)value;
                                if (lit.DataType != null)
                                {
                                    writer.WriteValue("typed-literal");
                                }
                                else
                                {
                                    writer.WriteValue("literal");
                                }
                                writer.WritePropertyName("value");

                                writer.WriteValue(lit.Value);
                                if (!lit.Language.Equals(String.Empty))
                                {
                                    writer.WritePropertyName("lang");
                                    writer.WriteValue(lit.Language);
                                }
                                else if (lit.DataType != null)
                                {
                                    writer.WritePropertyName("datatype");
                                    writer.WriteValue(lit.DataType.ToString());
                                }
                                break;

                            case NodeType.URI:
                                //URI
                                writer.WriteValue("uri");
                                writer.WritePropertyName("value");
                                writer.WriteValue(value.ToString());
                                break;

                            default:
                                throw new RDFException("Result Sets which contain Nodes of unknown Type cannot be serialized in the SPARQL Query Results JSON Format");
                        }

                        //End the Variable Object
                        writer.WriteEndObject();
                    }
                    //End the Binding Object
                    writer.WriteEndObject();
                }

                //End Result Object
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            else
            {
                //ASK query result

                //Set an empty JSON Object in the Head
                writer.WriteEndObject();

                //Create a Boolean Property
                writer.WritePropertyName("boolean");
                writer.WriteValue(results.Result);
            }

            //End the JSON Object for the Result Set
            writer.WriteEndObject();

        }
    }
}
