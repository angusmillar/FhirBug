﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bug.Data.Repository.Base;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System.Threading.Tasks;
using Bug.Common.Enums;
//using System.Data.Entity;

namespace Bug.Data.Repository
{
  public class HttpStatusCodeRepository : Repository<HttpStatusCode>, IHttpStatusCodeRepository
  {
    public HttpStatusCodeRepository(AppDbContext context)
      : base(context) { }

    public async Task<HttpStatusCode?> GetByCode(System.Net.HttpStatusCode httpStatusCode)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.Code == httpStatusCode.ToString());
    }

  }
}

