using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.ViewModels.Common
{
    public interface IEventFormViewModel
    {
        public IEnumerable<DropdownOptionModel> Categories { get; set; }
        public IEnumerable<DropdownOptionModel> Locations { get; set; }
    }
}
