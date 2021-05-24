using Application.DTOs;
using Core.Exceptions;
using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands
{
    public static class CommandRunner
    {
        /// <summary>
        /// This method would run a command and return proper response
        /// </summary>
        /// <param name="command"> Command to run</param>
        /// <param name="successMessage"> Message for command being executed successfully</param>
        /// <param name="validator"> Command authorizer </param>
        /// <param name="cleanUp"> Function to get executed after execution of command</param>
        /// <returns></returns>
        public static async Task<CommandResponse> Run(Func<Task> command, string successMessage,
            Func<Task<bool>> validator = null, Func<Task> cleanUp = null)
        {
            CommandResponse response;
            try
            {
                bool authorized = validator is null || await validator() == true;
                if (authorized)
                {
                    await command();
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
                if (cleanUp.IsNotNull())
                {
                    await cleanUp();
                }
            }
            return response;
        }
    }
}
