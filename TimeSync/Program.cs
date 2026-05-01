using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace TimeSync
{
    internal class Program
    {
        public static int Retry { get; set; } = 2;
        public static string NtpServer { get; set; } = "ntp.aliyun.com";

        private static int Retried = 0;


        static async Task<int> Main(string[] args)
        {
            try
            {
                //获得参数
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--retry" && i + 1 < args.Length)
                        Retry = int.Parse(args[++i]);
                    else if (args[i] == "--ntp-server" && i + 1 < args.Length)
                        NtpServer = args[++i];
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex);
                return 1;
            }

        RETRY:
            try
            {
                await StartGetNtpTime();
            }
            catch (Exception ex)
            {
                if (Retry > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Retried++;
                    Retry--;
                    Console.WriteLine($"在与NTP服务器通信时遇到错误: {ex.Message} \n    {ex}");
                    Console.ResetColor();
                    await Task.Delay(2000);
                    Console.WriteLine($"开始重试，次数: {Retried}  剩余次数: {Retry}");
                    goto RETRY;//这种情形下最简做法就是goto，别骂了
                }
                else
                {
                    ErrorReport(new Exception("重试次数用尽，均无法与Ntp服务器通信。程序终止"));
                    return 0;
                }
            }

            return 0;
        }

        static async Task StartGetNtpTime()
        {
            Console.WriteLine($"尝试连接NTP服务器: {NtpServer} ...");

            //尝试获得时间信息
            var result = await GetNtpTime(NtpServer);

            Console.WriteLine($"NTP服务器回应: {result.ToString("yyyy.MM.dd HH:mm:ss.fff")}");
            Console.WriteLine($"尝试设置系统时间...");

            TimeSetter.SetTime(result);

            Console.WriteLine("成功！");

            Console.Write("点击任意键退出...");
            Console.ReadKey();
        }

        static void ErrorReport(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"程序遇到错误 {ex.Message} :\n    {ex}");
            Console.ResetColor();

            Console.Write("点击任意键退出...");
            Console.ReadKey();
        }


        public static async Task<DateTime> GetNtpTime(string ntpServer)
        {
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // NTP version 3, Client mode

            using var udpClient = new UdpClient();

            var addresses = await Dns.GetHostAddressesAsync(ntpServer);
            var ipEndPoint = new IPEndPoint(addresses[0], 123);

            await udpClient.SendAsync(ntpData, ntpData.Length, ipEndPoint);

            var result = await udpClient.ReceiveAsync();
            byte[] response = result.Buffer;

            uint intPart = BinaryPrimitives.ReadUInt32BigEndian(response.AsSpan(40, 4));
            uint fractPart = BinaryPrimitives.ReadUInt32BigEndian(response.AsSpan(44, 4));
            long milliseconds = (long)intPart * 1000 + ((long)fractPart * 1000 / 0x100000000L);

            var networkDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);

            return networkDateTime.ToLocalTime();
        }
    }
}
