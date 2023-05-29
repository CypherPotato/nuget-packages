using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptonite.ECDH.Core
{
    internal struct GroupElementP1
    {
        public FieldElement X;
        public FieldElement Y;
        public FieldElement Z;
        public FieldElement T;
    }

    internal struct GroupElementP2
    {
        public FieldElement X;
        public FieldElement Y;
        public FieldElement Z;
    }

    internal struct GroupElementP3
    {
        public FieldElement X;
        public FieldElement Y;
        public FieldElement Z;
        public FieldElement T;
    }

    internal struct GroupElementP4
    {
        public FieldElement YplusX;
        public FieldElement YminusX;
        public FieldElement XY2D;

        public GroupElementP4(FieldElement yplusX, FieldElement yminusX, FieldElement xy2d)
        {
            YplusX = yplusX;
            YminusX = yminusX;
            XY2D = xy2d;
        }
    }
}
