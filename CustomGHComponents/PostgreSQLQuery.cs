using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Npgsql;


namespace CustomGHComponents
{

    public class PostgreSQLQuery : GH_Component
    {
        public PostgreSQLQuery() : base("PostgreSQL Query", "PostgreSQL Query", "C# component using the Npgsql library to extract data from a PostgreSQL Database based on an input SQL Query.", "BH Components", "SQL") {}

        public override Guid ComponentGuid => new Guid("64E69758-4275-4147-AF7D-7B8DAD5A5E75");

        protected override Bitmap Icon
        {
            get
            {
                // Fix: Access the embedded resource directly
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("CustomGHComponents.Resources.PostgreSQL_24x24px.png"))
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
            pManager.AddTextParameter("connString", "connString", "Connection String Host=...;Port=...;Username=...;Password:...;Database=...", GH_ParamAccess.item);
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
                NpgsqlConnection connection = new NpgsqlConnection(connString);

                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(query, connection);
                NpgsqlDataReader reader = cmd.ExecuteReader();

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
