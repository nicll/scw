using ScwSvc.Procedures.Interfaces;
using ScwSvc.DataAccess.Interfaces;

namespace ScwSvc.Procedures.Impl;

public class UserLogic : IUserLogic
{
    private readonly ISysDbRepository _sysDb;
    private readonly IDynDbRepository _dynDb;
}
