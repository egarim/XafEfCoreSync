using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XafEfCoreSync.Module.BusinessObjects
{
    [DefaultClassOptions()]
    public class Blog : BaseObject
    {
        public Blog()
        {
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Key, Column(Order = 0)]
        //public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }


        public virtual ObservableCollection<Post> Posts { get; set; }
    }
}