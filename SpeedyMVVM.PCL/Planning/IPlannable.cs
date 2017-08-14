using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Planning
{
    /// <summary>
    /// Define a plannable object
    /// </summary>
    public interface IPlannable
    {
        DateTime PlannedDate { get; set; }
    }
}
