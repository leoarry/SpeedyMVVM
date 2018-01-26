using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    [DataContract]
    public class AppToken
    {
        #region Fields
        [DataMember(Name = nameof(UserName))]
        internal string _UserName;

        [DataMember(Name = nameof(MachineName))]
        internal string _MachineName;

        [DataMember(Name = nameof(OSVersion))]
        internal string _OSVersion;

        [DataMember(Name = nameof(CurrentKey))]
        internal Guid _CurrentKey;

        [DataMember(Name = nameof(IsAuthenticated))]
        internal bool _IsAuthenticated;
        
        [DataMember(Name = nameof(ProxyName))]
        internal string _ProxyName;
        #endregion

        #region Properties
        public string UserName
        {
            get { return _UserName; }
            private set { _UserName = value; }
        }
        
        public string MachineName
        {
            get { return _MachineName; }
            private set { _MachineName = value; }
        }
        
        public string OSVersion
        {
            get { return _OSVersion; }
            private set { _OSVersion = value; }
        }
        
        public Guid CurrentKey
        {
            get { return _CurrentKey; }
            private set { _CurrentKey = value; }
        }
        
        public bool IsAuthenticated
        {
            get { return _IsAuthenticated; }
            private set { _IsAuthenticated = value; }
        }
        
        public string ProxyName
        {
            get { return _ProxyName; }
            private set { _ProxyName = value; }
        }
        #endregion

        #region Methods        
        public void ValidateToken(string proxyName, Guid tokenKey)
        {
            _ProxyName = proxyName;
            _CurrentKey = tokenKey;
            _IsAuthenticated = true;
        }
        #endregion

        #region Constructors
        public AppToken(string userName = "", string machineName = "", string osVersion = "")
        {
            UserName = userName;
            MachineName = machineName;
            OSVersion = osVersion;
        }        
        #endregion
    }
}
