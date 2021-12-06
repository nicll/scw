using ScwSvc.BusinessLogic.Interfaces;
using ScwSvc.DataAccess.Interfaces;

namespace ScwSvc.BusinessLogic.Impl;

public class UserLogic : IUserLogic
{
    private readonly ISysDbRepository _sysDb;
    private readonly IDynDbRepository _dynDb;
}
