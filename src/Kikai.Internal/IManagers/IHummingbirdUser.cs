using Kikai.Internal.Contracts.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kikai.Internal.IManagers
{
    public interface IHummingbirdUser
    {
        HBUserPermissionObject GetUserPermissions(int studyId);
    }
}
