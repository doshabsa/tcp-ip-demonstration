using Newtonsoft.Json;

namespace TCPLibrary
{
    public class FileTransferObject
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }

        public string JsonSerialized()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
