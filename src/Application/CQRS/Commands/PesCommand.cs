using Application.DTOs;
using Core.Exceptions;
using Core.Extensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Commands
{
    public abstract class PesCommand<TRequest> : IRequestHandler<TRequest, CommandResponse> where TRequest : IRequest<CommandResponse>
    {
        public async Task<CommandResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            CommandResponse response;
            try
            {
                bool authorized = await Authorize(request, cancellationToken);
                if (authorized)
                {
                    await Run(request, cancellationToken);
                    response = CommandResponse.CreateSuccessfull(SuccessMessage);
                }
                else
                {
                    response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.Unathorized);
                }
            }
            catch (Exception exc)
            {
                if (exc.IsExpected())
                {
                    response = CommandResponse.CreateUnsuccessfull(exc.Message);
                }
                else
                {
                    response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.ServerError);
                }
            }
            finally
            {
                await CleanUp(request, cancellationToken);
            }
            return response;
        }

        public abstract Task Run(TRequest request, CancellationToken token);
        public abstract string SuccessMessage { get; }

        /// <summary>
        /// Returns validity of request.
        /// If not implemented, then all requests are valid
        /// </summary>
        public virtual Task<bool> Authorize(TRequest request, CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public virtual Task CleanUp(TRequest request, CancellationToken token)
        {
            return Task.CompletedTask;
        }

    }
}
