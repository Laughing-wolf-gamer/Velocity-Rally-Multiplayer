namespace Best.SocketIO
{
    [Best.HTTP.Shared.PlatformSupport.IL2CPP.Preserve]
    public class Error
    {
        [Best.HTTP.Shared.PlatformSupport.IL2CPP.Preserve] 
        public string message;

        public Error() { }

        public Error(string msg)
        {
            this.message = msg;
        }

        public override string ToString()
        {
            return this.message;
        }
    }
}
