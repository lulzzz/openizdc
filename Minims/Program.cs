﻿using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minims
{
    class Program
    {
        // Trusted certificates
        private static List<String> s_trustedCerts = new List<string>();

        static void Main(string[] args)
        {

            String[] directory = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MINIMS"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MINIMS")
            };

            foreach (var dir in directory)
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

            // Token validator
            TokenValidationManager.SymmetricKeyValidationCallback += (o, k, i) =>
            {
                return MessageBox.Show(String.Format("Trust issuer {0} with symmetric key?", i), "Token Validation Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
            };
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, error) =>
            {
                if (certificate == null || chain == null)
                    return false;
                else
                {
                    var valid = s_trustedCerts.Contains(certificate.Subject);
                    if (!valid && (chain.ChainStatus.Length > 0 || error != SslPolicyErrors.None))
                        if (MessageBox.Show(String.Format("The remote certificate is not trusted. The error was {0}. The certificate is: \r\n{1}\r\nWould you like to temporarily trust this certificate?", error, certificate.Subject), "Certificate Error", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                            return false;
                        else
                            s_trustedCerts.Add(certificate.Subject);

                    return true;
                    //isValid &= chain.ChainStatus.Length == 0;
                }
            };

            // Start up!!!
            var consoleArgs = new ParameterParser<ConsoleParameters>().Parse(args);

            if (!MiniApplicationContext.Start(consoleArgs))
            {
                MiniApplicationContext.StartTemporary(consoleArgs);
                // Forward
                Process pi = Process.Start("http://127.0.0.1:9200/org.openiz.core/views/settings/index.html");
            }
            else
            {
                var appletConfig = XamarinApplicationContext.Current.Configuration.GetSection<AppletConfigurationSection>();
                Process pi = Process.Start("http://127.0.0.1:9200/org.openiz.core/index.html#/");

            }
            Console.WriteLine("Press [Enter] key to close...");
            Console.ReadLine();
        }
    }
}