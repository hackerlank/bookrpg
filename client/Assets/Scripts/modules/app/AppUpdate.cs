using UnityEngine;
using System.Collections;
using bookrpg.resource;

namespace bookrpg
{
    public class AppUpdate
    {
        private string localVersionUrl;

        private VersionCfgMgr version;
        private VersionCfgMgr localVersion;

        public AppUpdate()
        {
        }

        public void update(string localVersionUrl)
        {
            this.localVersionUrl = localVersionUrl;
            var loader = LoaderMgr.load(localVersionUrl);

            loader.onComplete += ld =>
            {
                localVersion = new VersionCfgMgr();
                if(localVersion.init(ld.text))
                {
                    loadVersion(localVersion.versionAddr);
                }
            };
        }

        private void loadVersion(string url)
        {
            var loader = LoaderMgr.load(url);
            loader.isCheckRedirectError = true;

            loader.onComplete += ld =>
            {
                version = VersionCfgMgr.IT;
                if(version.init(ld.text))
                {
                    doUpdate();
                }
            };
        }

        private void doUpdate()
        {
            if (version.closed)
            {
                Debug.Log(version.closedReason);
                //TODO 弹出公告，退出游戏
                return;
            }

            if (!localVersion.baseVersion.Equals(version.baseVersion))
            {
                reinstall();
                return;
            }

            //TODO 更新资源
            if (!localVersion.lastVersion.Equals(version.lastVersion))
            {
                var loader = LoaderMgr.load(version.resourceTableAddr);
                loader.isCheckRedirectError = true;

                loader.onComplete += ld =>
                {
                    version = VersionCfgMgr.IT;
                    if(version.init(ld.text))
                    {
                        updateResource();
                    }
                };
            }
        }

        private void reinstall()
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

        private void updateResource()
        {

        }
    }
}
