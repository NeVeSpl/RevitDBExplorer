using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitDBExplorer.Domain.DataModel.Accessors
{
    internal class ExecuteResultCollection
    {
        public List<(object Arg, object Result)> Results { get; } = new List<(object, object)> ();
        public string Label { get; init; }
        public string Param_0_Name { get; init; }


        public static ExecuteResultCollection Create<T>(string param_0_Name)
        {            
            return new ExecuteResultCollection()
            {
                Param_0_Name = param_0_Name,
                Label = $"[{typeof(T).GetCSharpName()}]",
            };
        }


        public void Add<TParam0Type, TReturnType>(TParam0Type arg, TReturnType result)
        {
            Results.Add ((arg, result));
        }
    }
}