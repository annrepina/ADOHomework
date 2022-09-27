using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework
{
    public class Order
    {
        public Order()
        {
            _usersId = new List<int>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int Summ { get; set; }

        public DateTime DateTime { get; set; }

        private List<int> _usersId;

        public void AddId(int id)
        {
            _usersId.Add(id);
        }
    }
}
