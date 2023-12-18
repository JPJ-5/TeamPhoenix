using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Security.Contracts;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security.Contract;

namespace TeamPhoenix.MusiCali.Security
{


    public class Authorization
    {
        private readonly IAuthorizer _authorizer;

        public Authorization(IAuthorizer authorizer)
        {
            _authorizer = authorizer ?? throw new ArgumentNullException(nameof(authorizer));
        }

    }

}
