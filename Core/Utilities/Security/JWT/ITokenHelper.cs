using System.Collections.Generic;
using Core.Entities.Concrete;

namespace Core.Utilities.Security.JWT
{
    public interface ITokenHelper
    {
        //Token üretecek mekanizma
        AccessToken CreateToken(User user, List<OperationClaim> operationClaims);
    }
}