namespace Application.DTOs;

public class CommandResponse
{
    public bool Successfull { get; set; }
    public string ResultMessage { get; set; }

    public static CommandResponse CreateSuccessfull(string message)
    {
        return new CommandResponse
        {
            Successfull = true,
            ResultMessage = message
        };
    }

    public static CommandResponse CreateUnsuccessfull(string message)
    {
        return new CommandResponse
        {
            Successfull = false,
            ResultMessage = message
        };
    }
}