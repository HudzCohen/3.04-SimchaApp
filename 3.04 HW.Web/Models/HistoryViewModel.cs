using _3._04_HW.Data;

namespace _3._04_HW.Web.Models
{
    public class HistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public List<Transaction> Transactions { get; set; }
        public decimal Balance { get; set; }
    }
}
