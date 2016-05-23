﻿///
/// You can modify this file for your purpose 
///

using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using bookrpg.config;
using UnityEngine;

namespace bookrpg 
{
    public class VersionCfgMgr : VersionCfgMgrCE 
    {
        private static VersionCfgMgr instance;

        public static VersionCfgMgr IT
        {
            get
            {
                if(instance == null){
                    instance = new VersionCfgMgr();
                }
                return instance;
            }
        }

        public VersionCfgMgr() : base()
        {
            //your code
        }

        public override bool init(string text, string format=null)
        {
            if (base.init(text, format))
            {
                //your code
                return true;
            }

            return false;
        }
        
    }

    public class VersionCfg : VersionCfgCE 
    {
        ///parse form txt 
        public override bool parseFrom(IConfigParser parser)
        {
            if (base.parseFrom(parser))
            {
                //your code
                return true;
            }

            return false;
        }
    }
}
