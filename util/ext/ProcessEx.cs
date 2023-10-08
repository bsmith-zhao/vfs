using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.ext
{
    public static class ProcessEx
    {
        public static int runCmd(this string cmd, string args, 
            Action<Process> before, 
            Action<string> stdout, 
            Action<string> stderr, 
            Action<Process> after)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = args;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;

                if (null != stdout)
                {
                    p.StartInfo.RedirectStandardOutput = true;
                    p.OutputDataReceived += (s, e) =>
                    {
                        if (null != e.Data)
                            stdout(e.Data);
                    };
                }

                if (null != stderr)
                {
                    p.StartInfo.RedirectStandardError = true;
                    p.ErrorDataReceived += (s, e) =>
                    {
                        if (null != e.Data)
                            stderr(e.Data);
                    };
                }

                before?.Invoke(p);

                p.Start();

                if (null != stdout)
                    p.BeginOutputReadLine();

                if (null != stderr)
                    p.BeginErrorReadLine();

                p.WaitForExit();

                after?.Invoke(p);

                return p.ExitCode;
            }
        }
    }
}
