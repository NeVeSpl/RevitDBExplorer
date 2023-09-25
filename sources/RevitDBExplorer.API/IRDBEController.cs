using System.Collections.Generic;

namespace RevitDBExplorer.API
{
    public interface IRDBEController
    {
        void Snoop(object document, IEnumerable<object> elements);
    }
}