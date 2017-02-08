using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Magicodes.Shop.Models
{
    public class HelloWorlda
    {

        public string hello { get  ; set ; }
        [Key]
        public int id { get; set; }

        public HelloWorlda()
        {
            id = 3;
            
            hello = "adaddddd";
        }
    }
}