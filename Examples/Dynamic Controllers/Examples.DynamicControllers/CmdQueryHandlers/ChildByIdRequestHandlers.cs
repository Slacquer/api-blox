using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.AspNetCore.Contracts;
using APIBlox.AspNetCore.Types;
using APIBlox.NetCore.Attributes;
using APIBlox.NetCore.Contracts;
using Examples.Resources;

namespace Examples.CmdQueryHandlers
{
    [InjectableService]
    internal class ChildByIdRequestHandlers :
        IQueryHandler<ChildByIdRequest, HandlerResponse>
    {
        public Task<HandlerResponse> HandleAsync(ChildByIdRequest query, CancellationToken cancellationToken)
        {
            var ret = new HandlerResponse
            {
                Result = new List<ChildResponse>
                {
                    new ChildResponse
                    {
                        Age = 5,
                        FirstName = "Sebastian",
                        Id = 1,
                        LastName = "Booth",
                        Parents = new Collection<ParentResponse>
                        {
                            new ParentResponse
                            {
                                Age = 29,
                                LastName = "Booth",
                                FirstName = "Britani"
                            }
                        }
                    }
                }
            };

            //ret.CreateError().SetErrorToBadRequest("OMG!");

            return Task.FromResult(ret);
        }
    }
}
