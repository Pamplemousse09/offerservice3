using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.Internal.Contracts.Objects
{
    public class HBUserPermissionObject
    {
        public List<HBUserRightsItemObject> UserRightsList { get; set; }

        public string HtmlHead { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

    }
}
