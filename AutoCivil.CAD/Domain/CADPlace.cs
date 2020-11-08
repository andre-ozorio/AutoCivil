using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCivil.CAD.Domain
{
    public class CADPlace
    {
        public CADPlace(string name)
        {
            Name = name;
            Shapes = new List<CADShape>();
        }

        public string Name { get; set; }
        public CADShape OriginShape { get; set; }
        public List<CADShape> Shapes { get; set; }
        public CADPlace LeftPlace { get; set; }
        public CADPlace RightPlace { get; set; }
        public CADPlace TopPlace { get; set; }
        public CADPlace BottomPlace { get; set; }        
    }
}
