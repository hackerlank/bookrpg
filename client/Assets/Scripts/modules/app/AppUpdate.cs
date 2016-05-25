using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using bookrpg.core;
using bookrpg.resource;
using bookrpg.utils;

namespace bookrpg
{
    public class AppUpdate
    {
        private string persistentDataPath;
        private string localVersionPath;
        private string localResourceTablePath;

        private string resourceTableText;
        private IResourceTable resourceTable;
        private IResourceTable localResourceTable;

        private string versionText;
        public VersionCfgMgr version;
        private VersionCfgMgr localVersion;

        public bool forceUpdateResource;

        private BKAction onComplete;

        public AppUpdate()
        {
            this.persistentDataPath = AppConfig.persistentDataPath;
            this.localVersionPath = AppConfig.localVersionPath;
            this.localResourceTablePath = AppConfig.localResourceTablePath;
        }

        public void Update(BKAction onComplete = null)
        {
            this.onComplete = onComplete;

            var loader = LoaderMgr.Load(localVersionPath);
            loader.isCheckRedirectError = true;

            loader.onComplete += ld =>
            {
                if(ld.hasError)
                {
                    //TODO 
                    return;
                }

                localVersion = new VersionCfgMgr();
                if (localVersion.Init(ld.text))
                {
                    LoadVersion(localVersion.versionAddr);
                }
            };
        }

        private void LoadVersion(string path)
        {
            versionText = Util.Load(path);
            version = VersionCfgMgr.IT;

            if (versionText != null && version.Init(versionText))
            {
                DoUpdate();
            }
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

            LoaderMgr.baseUrl = version.updateAddr;
            LoaderMgr.backupBaseUrl = version.updateAddr2;
                
            //TODO 更新资源
            if (forceUpdateResource || !localVersion.lastVersion.Equals(version.lastVersion))
            {
                var bl = LoaderMgr.LoadBatch();
                bl.AddLoader(localResourceTablePath);
                bl.AddLoader(version.resourceTableAddr);
                bl.isCheckRedirectError = true;

                bl.onComplete += ld =>
                {
                    var localTxt = bl.GetLoader(localResourceTablePath).text;
                    resourceTableText = bl.GetLoader(version.resourceTableAddr).text;
                    localResourceTable = new ResourceTableImpl();
                    resourceTable = new ResourceTableImpl();

                    if (localResourceTable.Deserialize(localTxt) &&
                        resourceTable.Deserialize(resourceTableText))
                    {
                        UpdateResource();
                    }
                };
            }
        }

        private void Reinstall()
        {
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
            var curTable = localResourceTable.resourcePackList;
            var newTable = resourceTable.resourcePackList;

            var bl = LoaderMgr.LoadBatch();
            bl.isCheckRedirectError = true;

            foreach (var item in newTable)
            {
                if (!curTable.ContainsKey(item.Key) ||
                    curTable[item.Key].version != item.Value.version)
                {
                    var loader = bl.AddLoader(item.Value.targetFile, 
                                     item.Value.version, 
                                     item.Value.size);
                    loader.customData = item;
                }
            }

            bl.onOneComplete += (obj) =>
            {
                var item = (KeyValuePair<string, IResourcePack>)obj.customData;
                if (obj.hasError)
                {
                    //TODO  更新失败
                    bl.Dispose();
                    return;
                }

                if (curTable.ContainsKey(item.Key))
                {
                    curTable[item.Key] = item.Value;
                } else
                {
                    curTable.Add(item);
                }

                if(!Util.Save(persistentDataPath + "/" + item.Value.targetFile, obj.bytes) ||
                    !Util.Save(localResourceTablePath, localResourceTable.Serialize()))
                {
                    //TODO  更新失败
                    bl.Dispose();
                }

                obj.Dispose();
            };

            bl.onComplete += (obj) => {
                bl.Dispose();
                if(bl.errorCount > 0)
                {
                    return;
                }

                if(Util.Save(localResourceTablePath, resourceTableText) &&
                    Util.Save(localVersionPath, versionText))
                {
                    if(onComplete != null)
                    {
                        onComplete.Invoke();
                    }
                }
                else{
                    //TODO tip
                }
            };
        }
    }
}
