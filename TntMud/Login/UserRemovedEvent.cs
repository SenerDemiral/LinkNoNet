public sealed class UserRemovedEventArgs : EventArgs
{
    public string UserId { get; set; } = string.Empty;
}

public delegate void UserRemovedEventHandler(object sender, UserRemovedEventArgs e);