using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3._04_HW.Data
{
    public class Deposit
    {
        public int ContributorId { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
    }
}
