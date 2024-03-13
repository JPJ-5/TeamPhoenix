using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Contract
{
    public interface ISqlDAO
    {
        Result ExecuteSql(string sql);

    }
}
