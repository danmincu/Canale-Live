// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

var channels = ChannelBuilder.ChannelParser.Channels();
StringBuilder stringBuilder = new StringBuilder();
foreach (var channel in channels)
{
    stringBuilder.AppendLine($@"""{channel.Key}"":""{channel.Value}"",");
    //Console.WriteLine($"{channel.Key}:{channel.Value}");
}

System.IO.File.WriteAllText("channels.json", stringBuilder.ToString());


