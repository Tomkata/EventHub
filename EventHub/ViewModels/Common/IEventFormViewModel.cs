using EventHub.Core.Common;
using EventHub.Web.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Web.ViewModels.Common
{
    public interface IEventFormViewModel
    {
        public IEnumerable<DropdownOptionModel> Categories { get; set; }
        public IEnumerable<DropdownOptionModel> Locations { get; set; }
    }
}
