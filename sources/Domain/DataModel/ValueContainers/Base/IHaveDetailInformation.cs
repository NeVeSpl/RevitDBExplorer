using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal interface IHaveDetailInformation
    {
        string DetailInformationText { get; }
    }
}
