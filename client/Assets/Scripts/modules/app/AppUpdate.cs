using UnityEngine;
using System.Collections;
using bookrpg.resource;

namespace bookrpg
{
    public class AppUpdate
    {
        private string localVersionUrl;
        private string localResourceTableUrl;

        private IResourceTable resourceTable;
        private IResourceTable localResourceTable;

        private VersionCfgMgr version;
        private VersionCfgMgr localVersion;

        public AppUpdate()
        {
            this.localVersionUrl = AppConfig.localVersionUrl;
            this.localResourceTableUrl = AppConfig.localResourceTableUrl;
        }

        public void Update()
        {
            var loader = LoaderMgr.Load(localVersionUrl);

            loader.onComplete += ld =>
            {
                localVersion = new VersionCfgMgr();
                if(localVersion.Init(ld.text))
                {
                    LoadVersion(localVersion.versionAddr);
                }
            };
        }

        private void LoadVersion(string url)
        {
            var loader = LoaderMgr.Load(url);
            loader.isCheckRedirectError = true;

            loader.onComplete += ld =>
            {
                version = VersionCfgMgr.IT;
                if(version.Init(ld.text))
                {
                    DoUpdate();
                }
            };
        }

        private void DoUpdate()
        {
            if (version.closed)
            {
                Debug.Log(version.closedReason);
                //TODO 弹出公告，退出游戏
                return;
            }

            if (!localVersion.baseVersion.Equals(version.baseVersion))
            {
                Reinstall();
                return;
            }

            //TODO 更新资源
            if (!localVersion.lastVersion.Equals(version.lastVersion))
            {
                var bl = LoaderMgr.LoadBatch();
                bl.AddLoader(localResourceTableUrl);
                bl.AddLoader(version.resourceTableAddr);
                bl.isCheckRedirectError = true;

                bl.onComplete += ld =>
                {
                    var localTxt = bl.GetLoader(localResourceTableUrl).text;
                    var txt = bl.GetLoader(version.resourceTableAddr).text;
                    localResourceTable = new ResourceTableImpl();
                    resourceTable = new ResourceTableImpl();
                    if(localResourceTable.Deserialize(localTxt) && 
                        resourceTable.Deserialize(txt))
                    {
                        UpdateResource();
                    }
                };
            }
        }

        private void Reinstall()
        {
            if (version == null)
            {
                return;
            }
            //TODO 需要安装升级
            switch (version.installMethod)
            {
                case "directDownload":
                    break;
                case "openBrowser":
                    break;
                case "openAppStore":
                    break;
            }
        }

        private void UpdateResource()
        {

        }
    }
}
