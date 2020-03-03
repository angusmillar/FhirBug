﻿using Bug.Common.Enums;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Service.TableService
{
  public class MethodTableService : IMethodTableService
  {
    private readonly IMethodRepository IMethodRepository;
    private readonly IMethodCache IMethodCache;
    public MethodTableService(IMethodRepository IMethodRepository, IMethodCache IMethodCache)
    {
      this.IMethodRepository = IMethodRepository;
      this.IMethodCache = IMethodCache;
    }

    public async Task<Method> GetSetMethod(HttpVerb httpVerb)
    {
      Method? Method = await IMethodCache.GetAsync(httpVerb);
      if (Method is null)
      {
        Method = new Method() { HttpVerb = httpVerb, Code = httpVerb.GetCode() };
        IMethodRepository.Add(Method);
        await IMethodRepository.SaveChangesAsync();
        await IMethodCache.SetAsync(Method);
      }
      return Method;
    }
  }
}