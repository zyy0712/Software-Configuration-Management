using System;
using System.IO;
using System.Text;
using System.Web;

namespace Gzl.CommonComponent
{
    /// <summary>
    /// 简单的文件访问/修改日志，用于记录读取和修改操作。
    /// 日志文件位于 ~/Log/access.log
    /// </summary>
    public static class AccessLogger
    {
        private static string GetLogPath()
        {
            try
            {
                string p = HttpContext.Current.Server.MapPath("~/Log/access.log");
                string dir = Path.GetDirectoryName(p);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return p;
            }
            catch
            {
                // 退回到应用程序基目录的 Log 子目录
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string fallback = Path.Combine(baseDir, "Log", "access.log");
                string d = Path.GetDirectoryName(fallback);
                if (!Directory.Exists(d)) Directory.CreateDirectory(d);
                return fallback;
            }
        }

        public static void LogAction(string user, string page, string action)
        {
            try
            {
                string path = GetLogPath();
                string line = string.Format("{0:yyyy-MM-dd HH:mm:ss}\tUser:{1}\tPage:{2}\tAction:{3}", DateTime.Now, user ?? "unknown", page ?? "unknown", action ?? "");
                File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }

        // 读取最近若干行，返回 HTML 安全字符串（换行替换为 <br/>）
        public static string ReadRecentLines(int count)
        {
            try
            {
                string path = GetLogPath();
                if (!File.Exists(path)) return "";
                var lines = File.ReadAllLines(path, Encoding.UTF8);
                int start = Math.Max(0, lines.Length - count);
                StringBuilder sb = new StringBuilder();
                for (int i = start; i < lines.Length; i++)
                {
                    sb.Append(HttpUtility.HtmlEncode(lines[i]));
                    sb.Append("<br/>");
                }
                return sb.ToString();
            }
            catch { return ""; }
        }
    }
}
