using DevExpress.Persistent.BaseImpl.EF;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XafEfCoreSync.Module.BusinessObjects
{
    public class Post : BaseObject
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Key, Column(Order = 0)]
        //public virtual Guid Id { get; set; }
        public Post()
        {
        }
        public virtual string Title { get; set; }

        public virtual Blog Blog { get; set; }
    }
}