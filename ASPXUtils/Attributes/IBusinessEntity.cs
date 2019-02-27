using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ASPXUtils
{
    public interface INameValue
    {
        int ID { get; set; }
        string Name { get; set; }
    }
    public interface IBusinessEntity 
    {
        int ID { get; set; }
        int Insert();
        void Update();
        void Delete(); 
    }

    public abstract class BusinessEntity {
        public int ID { get; set; } 
        public abstract int Insert();
        public abstract void Update();
        public abstract void Delete();  
    }
}
