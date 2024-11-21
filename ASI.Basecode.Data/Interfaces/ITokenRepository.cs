using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITokenRepository
    {
        IQueryable<Token> RetrieveTokens();
        void AddToken(Token token);
        void DeleteToken(Token token);
    }
}
