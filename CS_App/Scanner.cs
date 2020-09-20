using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_App
{
    class Scanner
    {
        public Scanner(List<List<string>> policies)
        {
            using (SamServer server = new SamServer(null, SamServer.SERVER_ACCESS_MASK.SAM_SERVER_ENUMERATE_DOMAINS | SamServer.SERVER_ACCESS_MASK.SAM_SERVER_LOOKUP_DOMAIN))
            {
                foreach (string domain in server.EnumerateDomains())
                {
                    Console.WriteLine("domain: " + domain);

                    var sid = server.GetDomainSid(domain);
                    Console.WriteLine(" sid: " + sid);

                    var pi = server.GetDomainPasswordInformation(sid);
                    Console.WriteLine(" MaxPasswordAge: " + pi.MaxPasswordAge);
                    Console.WriteLine(" MinPasswordAge: " + pi.MinPasswordAge);
                    Console.WriteLine(" MinPasswordLength: " + pi.MinPasswordLength);
                    Console.WriteLine(" PasswordHistoryLength: " + pi.PasswordHistoryLength);
                    Console.WriteLine(" PasswordProperties: " + pi.PasswordProperties);
                }
            }
        }
    }
}
