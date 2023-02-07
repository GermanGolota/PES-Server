using System;
using System.Threading.Tasks;
using Application.DTOs;
using Core.Exceptions;
using Core.Extensions;
using MediatR;

namespace Application.CQRS.Commands;

public static class CommandRunner
{
    /// <summary>
    ///     This method would run a command and return proper response
    /// </summary>
    /// <typeparam name="TCommand"> Type of mediatr command to use</typeparam>
    /// <param name="commandObj"> Command object from Mediatr </param>
    /// <param name="commandHandler"> Command to run</param>
    /// <param name="successMessage"> Message for command being executed successfully</param>
    /// <param name="validator"> Command authorizer </param>
    /// <param name="cleanUp"> Function to get executed after execution of command</param>
    /// <returns> Response to command</returns>
    public static async Task<CommandResponse> Run<TCommand>(TCommand commandObj, Func<TCommand, Task> commandHandler,
        string successMessage,
        Func<TCommand, Task<bool>> validator = null, Func<TCommand, Task> cleanUp = null)
        where TCommand : IRequest<CommandResponse>
    {
        CommandResponse response;
        try
        {
            bool authorized = validator is null || await validator(commandObj);
            if (authorized)
            {
                await commandHandler(commandObj);
                response = CommandResponse.CreateSuccessfull(successMessage);
            }
            else
            {
                response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.Unathorized);
            }
        }
        catch (Exception exc)
        {
            if (exc.IsExpected())
                response = CommandResponse.CreateUnsuccessfull(exc.Message);
            else
                response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.ServerError);
        }
        finally
        {
            if (cleanUp.IsNotNull()) await cleanUp(commandObj);
        }

        return response;
    }
}