using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, UserName: {1}", Id, UserName);
        }
    }
}
