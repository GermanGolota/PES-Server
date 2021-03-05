namespace Application.DTOs.UpdateMessages
{
    public static class UpdateMessageFactory
    {
        public static MessageCreationUpdateMessage CreateMessageCreatedUpdate(string username, string text)
        {
            return new MessageCreationUpdateMessage
            {
                ActionType = "messageCreation",
                Text = text,
                Username = username
            };
        }

        public static MessageDeletionUpdateMessage CreateMessageDeletedUpdate(string username)
        {
            return new MessageDeletionUpdateMessage
            {
                ActionType = "messageDeletion",
                Username = username
            };
        }
        public static MessageEditUpdateMessage CreateMessageEditedUpdate(string username, string newText)
        {
            return new MessageEditUpdateMessage
            {
                ActionType = "messageEdit",
                Username = username,
                NewText = newText
            };
        }
        public static UserJoinedChatUpdateMessage CreateUserJoinedUpdate(string username)
        {
            return new UserJoinedChatUpdateMessage
            {
                ActionType = "userJoinedChat",
                Username = username,
            };
        }
        public static UserLeftChatUpdateMessage CreateUserLeftdUpdate(string username)
        {
            return new UserLeftChatUpdateMessage
            {
                ActionType = "userJoinedChat",
                Username = username,
            };
        }

        public static UserPromotedToAdminUpdateMessage CreateUserPromotedUpdate(string username)
        {
            return new UserPromotedToAdminUpdateMessage
            {
                ActionType = "userPromotedToAdmin",
                Username = username
            };
        }
    }
}
