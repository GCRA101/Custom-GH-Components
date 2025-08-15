using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Grasshopper.Kernel;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomGHComponents
{
    public class AccessSQLQuery : GH_Component
    {
        public AccessSQLQuery() : base("AccessSQL Query", "AccessSQL Query", "C# component using the System.Data.OleDb library to extract data from a Microsoft Access SQL Database based on an input SQL Query.", "BH Components", "SQL") { }

        public override Guid ComponentGuid => new Guid("A05CA127-8511-402F-BE23-CFADAFC2B191");

        protected override Bitmap Icon
        {
            get
            {
                // Fix: Access the embedded resource directly
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("CustomGHComponents.Resources.MicrosoftAccess_24x24px.png"))
                {
                    if (stream != null)
                    {
                        return new Bitmap(stream);
                    }
                }
                return null;
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("connString", "connString", "Connection String: Provider=Microsoft.ACE.OLEDB.12.0; Data Source =c:\\...\\filename.accdb;", GH_ParamAccess.item);
            pManager.AddTextParameter("query", "query", "SQL Query", GH_ParamAccess.item);
            pManager.AddBooleanParameter("trigger", "trigger", "Execute the SQL Query", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("data", "data", "String output of the SQL Query.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string connString = "";
            string query = "";
            bool trigger = true;

            if ((!DA.GetData(0, ref connString)) || (!DA.GetData(1, ref query)) || (!DA.GetData(2, ref trigger))) return;

            List<string> output = new List<string>();

            try
            {
                OleDbConnection connection = new OleDbConnection(connString);

                connection.Open();
                OleDbCommand cmd = new OleDbCommand(query, connection);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var row = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                        row += reader.GetValue(i).ToString() + (i < reader.FieldCount - 1 ? ", " : "");
                    output.Add(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            DA.SetData(0, output);
            return;

        }
    }
}
