using System;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using BH.oM.Structure.Results;


namespace CustomGHComponents.GH_Types
{
    public class GH_BarForce : GH_Goo<BarForce>
    {
        public GH_BarForce() : base() { }

        public GH_BarForce(BarForce barForce) : base(barForce) { }

        public override IGH_Goo Duplicate()
        {
            return new GH_BarForce(Value);
        }

        public override bool IsValid => Value != null;

        public override string TypeName => "BarForce";

        public override string TypeDescription => "Wrapper for BHoM BarForce object";

        public override string ToString()
        {
            if (Value == null) return "Null BarForce";
            return $"BarForce | Name: {Value.ObjectId}, Axial: {Value.FX:F2} kN, M3: {Value.MY:F2} kNm";
        }
    }
}
