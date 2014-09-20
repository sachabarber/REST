using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, AccountNumber: {1}, SortCode: {2}", 
                Id, AccountNumber, SortCode);
        }
    }
}
