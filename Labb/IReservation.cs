using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb
{
    public interface IReservation
    {
        string Name { get; set; }
        int People { get; set; }
        string Date { get; set; }
        string Time { get; set; }
        string Table { get; set; }
    }
}
