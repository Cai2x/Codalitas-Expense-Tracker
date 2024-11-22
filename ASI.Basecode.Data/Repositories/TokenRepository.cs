using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class TokenRepository : BaseRepository, ITokenRepository
    {
        public TokenRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        public IQueryable<Token> RetrieveTokens()
        {
            return this.GetDbSet<Token>();
        }
        public void AddToken(Token token)
        {
            this.GetDbSet<Token>().Add(token);
            UnitOfWork.SaveChanges();
        }
        public void DeleteToken(Token token)
        {
            this.GetDbSet<Token>().Remove(token);
            UnitOfWork.SaveChanges();
        }
    }
}
