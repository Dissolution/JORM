using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JORM.Querying.SqlBuilding
{
    public interface ISqlRenderable
    {
        void Render(Source source, StringBuilder text);
    }
}
