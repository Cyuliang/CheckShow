using log4net;
using log4net.Config;
using System.Reflection;

namespace CheckShow
{
    class Lognet
    {
        public static readonly ILog Log= LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Lognet()
        {
            XmlConfigurator.Configure();
            //ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //log.Error("Error message", new Exception("Error message generated"));
        }
    }
}
