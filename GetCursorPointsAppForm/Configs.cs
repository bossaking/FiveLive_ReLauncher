using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetCursorPointsAppForm
{
    public class Configs
    {

        public string path { get; set; }

        public int waitLauncher { get; set; }

        public int waitGame { get; set; }

        public string login { get; set; }

        public string password { get; set; }

        public DateTime relaunchTime { get; set; }

        public Configs(string path, int waitLauncher, int waitGame, string login, string password, DateTime relaunchTime)
        {
            this.path = path;
            this.waitLauncher = waitLauncher;
            this.waitGame = waitGame;
            this.login = login;
            this.password = password;
            this.relaunchTime = relaunchTime;
        }
    }
}
