namespace TCPLibrary
{
    public enum MessageType
    {
        Broadcast,
        ServerOnly,
        File,
        Connected,
        Disconnected,
        UpdateUsername,
        ScreenShare,
        MouseEvent
    }

    public class Packet280
    {
        public MessageType ContentType { get; set; }
        public string? Payload { get; set; }
        public string? Username { get; set; }
        public int? Room {  get; set; }
    }
}