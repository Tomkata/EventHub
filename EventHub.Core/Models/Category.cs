using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Models
{
    public class Category
    {
        public Category()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

    }
}

