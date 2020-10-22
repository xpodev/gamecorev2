using System.Net;


namespace GameCore.Mdp
{
    class MdpRemoteServer
    {
        private WebClient WebClient
        {
            get;
            set;
        }

        public MdpRemoteServer()
        {
            WebClient = new WebClient();
            WebClient.Headers.Add(MdpApi.Header.MdpVersion, MdpInfo.MdpApiVersion.ToString());
        }

        public string GetFilesList(string host, string dirname)
        {
            return WebClient.DownloadString(MdpApi.Resources.GetFilesListUrl(host, dirname));
        }
    }
}
