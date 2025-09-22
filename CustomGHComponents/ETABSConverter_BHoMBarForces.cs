using BH.oM;
using BH.oM.Base;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using Grasshopper.Kernel;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



namespace CustomGHComponents
{

    public class ETABSConverter_BHoMBarForces : GH_Component
    {
        /* 01. CONSTRUCTOR **************************************************************************************************************************/
        public ETABSConverter_BHoMBarForces() : base("ETABS to BHoM Bar Forces", "ETABS to BHoM Bar Forces", "C# component converting raw textual etabs forces into bhom bar forces objects.", "BH Components", "BHoM Data Adapter") {}
        
        /* 02. SET UNIQUE COMPONENT ID **************************************************************************************************************/
        public override Guid ComponentGuid => new Guid("EA367C3A-A240-49AE-94CC-70CE94D2619A");

        /* 03. SET COMPONENT ICON *******************************************************************************************************************/
        protected override Bitmap Icon
        {
            get
            {
                // Fix: Access the embedded resource directly
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("CustomGHComponents.Resources.DB_Etabs_to_BHoM_24x24px.png"))
                {
                    if (stream != null)
                    {
                        return new Bitmap(stream);
                    }
                }
                return null;
            }
        }

        /* 04. SET UP INPUT PARAMETERS *************************************************************************************************************/
        /* uniqueName (list of strings) - Unique Name
         * outputCase (list of strings) - Output Case Name
         * modeNumber (int) - Mode Number
         * timeStep (list of doubles) - Time Step
         * station (list of doubles) - Absolute Station
         * divisions (int) - Number of bar divisions for forces integration
         * p [KN] (list of doubles) - Axial Force [KN]
         * v2 [KN] (list of doubles) - Major Shear Force [KN]
         * v3 [KN] (list of doubles) - Minor Shear Force [KN]
         * t [KNm] (list of doubles) - Torsion [KNm]
         * m2 [KNm] (list of doubles) - Minor Bending Moment [KNm]
         * m3 [KNm] (list of doubles) - Major Bending Moment [KNm] */
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("uniqueName", "uniqueName","Unique Name", GH_ParamAccess.list);
            pManager.AddTextParameter("outputCase","outputCase", "Output Case Name", GH_ParamAccess.list);
            pManager.AddIntegerParameter("modeNumber","modeNumber", "Mode Number", GH_ParamAccess.item);
            pManager.AddNumberParameter("timeStep","timeStep", "Time Step", GH_ParamAccess.list);
            pManager.AddNumberParameter("station","station","Absolute Station", GH_ParamAccess.list);
            pManager.AddIntegerParameter("divisions","divisions","Number of bar divisions for forces integration", GH_ParamAccess.item);
            pManager.AddNumberParameter("p [KN]", "p [KN]", "Axial Force [KN]", GH_ParamAccess.list);
            pManager.AddNumberParameter("v2 [KN]", "v2 [KN]", "Major Shear Force [KN]", GH_ParamAccess.list);
            pManager.AddNumberParameter("v3 [KN]", "v3 [KN]", "Minor Shear Force [KN]", GH_ParamAccess.list);
            pManager.AddNumberParameter("t [KNm]", "t [KNm]", "Torsion [KNm]", GH_ParamAccess.list);
            pManager.AddNumberParameter("m2 [KNm]", "m2 [KNm]", "Minor Bending Moment [KNm]", GH_ParamAccess.list);
            pManager.AddNumberParameter("m3 [KNm]", "m3 [KNm]", "Major Bending Moment [KNm]", GH_ParamAccess.list);
        }
  
        /* 05. SET UP OUTPUT PARAMETERS ************************************************************************************************************/
        /* bhom objs(list of bhom bar force objects) - BHoM Bar Forces. */
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("bhom objs", "bhom objs", "BHoM Bar Forces.", GH_ParamAccess.list);
        }


        /* 06. MAIN ALGORITHM *********************************************************************************************************************/
        /* - Create list of bhom bar force objects from input raw etabs forces data. */
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> uniqueName=new List<string>(), outputCase = new List<string>();
            int modeNumber = 0, divisions = 0;
            List<double> timeStep = new List<double>(), station = new List<double>();
            List<double> p = new List<double>(), v2 = new List<double>(), v3 = new List<double>();
            List<double> t = new List<double>(), m2 = new List<double>(), m3 = new List<double>();

            if ((!DA.GetDataList(0, uniqueName)) || (!DA.GetDataList(1, outputCase)) || (!DA.GetData(2, ref modeNumber)) ||
                (!DA.GetDataList(3, timeStep)) || (!DA.GetDataList(4, station)) || (!DA.GetData(5, ref divisions)) ||
                (!DA.GetDataList(6, p)) || (!DA.GetDataList(7, v2)) || (!DA.GetDataList(8, v3)) ||
                (!DA.GetDataList(9, t)) || (!DA.GetDataList(10, m2)) || (!DA.GetDataList(11, m3))) return;

            List<BH.oM.Structure.Results.BarForce> output = new List<BH.oM.Structure.Results.BarForce>();

            try
            {
                for (int i = 0; i < uniqueName.Count; i++)
                {
                    BH.oM.Structure.Results.BarForce bf = new BH.oM.Structure.Results.BarForce(uniqueName[i],outputCase[i],modeNumber,
                        timeStep[i],station[i],divisions,
                        Math.Round(p[i]*1000,1), Math.Round(v3[i] * 1000, 1), Math.Round(v2[i] * 1000, 1),
                        Math.Round(t[i] * 1000, 1), Math.Round(m3[i] * 1000, 1), Math.Round(m2[i] * 1000, 1)*(-1));
                    output.Add(bf);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            DA.SetDataList(0, output);
            return;
        }
    }

}
