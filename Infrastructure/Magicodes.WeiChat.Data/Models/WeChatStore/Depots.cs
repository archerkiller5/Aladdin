using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.WeChatStore
{
   public class Depots
    {
        public int Id { get; set; }
        public int Manager_Id { get; set; }
        public string Depot_Name { get; set; }
        public string Depot_Address { get; set; }
        public string Depot_Telephone { get; set; }
        public int School_Id { get; set; }
        public string Director { get; set; }
    }
}
