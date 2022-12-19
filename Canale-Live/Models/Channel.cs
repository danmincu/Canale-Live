namespace Canale_Live.Models
{
    public class ChannelModel
    {
        /// <summary>
        /// Base uri to set the vlc player
        /// </summary>
        public string? HostUrl { get; set; }

        /// <summary>
        /// string used in the paths to identify the channel path
        /// </summary>
        public string? ChannelId { get; set; }

        /// <summary>
        /// Public name for the channel
        /// </summary>
        public string? ChannelName { get; set; }
    }
}