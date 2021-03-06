/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using System.IO;

public class LuaBaseBehaviour : MonoBehaviour
{
    public string luaScriptName;
    //public TextAsset luaScript;

    internal static LuaEnv lua_Env = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaOnEnable;
    private Action luaStart;
    private Action luaFixedUpdate;
    private Action luaUpdate;
    private Action luaLateUpdate;
    private Action luaOnDisable;
    private Action luaOnDestroy;



    private LuaTable scriptEnv;


    //自定义Load
      byte[] CustomMyLoader(ref string flie)
    {
        string fliePath = "";
        if (Application.platform == RuntimePlatform.WindowsEditor)//win 编辑器
        {
            fliePath = Application.dataPath + "/Script/lua/" + flie + ".lua.txt";
        }
        else//原生
        {
            fliePath = Application.persistentDataPath + "/Script/lua/" + flie + ".lua.txt";
        }

        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(fliePath));
    }

    void Awake()
    {
        scriptEnv = lua_Env.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = lua_Env.NewTable();
        meta.Set("__index", lua_Env.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);

        lua_Env.AddLoader(CustomMyLoader);


        string flieName = string.Format("require'{0}'", luaScriptName);// + luaScriptName;
        lua_Env.DoString(flieName, luaScriptName, scriptEnv);
        //luaEnv.DoString(luaScript.text, "LuaTestScript", scriptEnv);
        //XluaCommon.Instance.DoString(luaScriptName, luaScriptName,scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("OnEnable", out luaOnEnable);
        scriptEnv.Get("Start", out luaStart);
        scriptEnv.Get("FixedUpdate", out luaFixedUpdate);
        scriptEnv.Get("Update", out luaUpdate);
        scriptEnv.Get("LateUpdate", out luaLateUpdate);
        scriptEnv.Get("OnDisable", out luaOnDisable);
        scriptEnv.Get("onDestroy", out luaOnDestroy);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

   

    void OnEnable()
    {
        if (luaOnEnable != null)
        {
            luaOnEnable();
        }

    }

    private void OnDisable()
    {
        if (luaOnDisable != null)
        {
            luaOnDisable();
        }
    }
    void FixedUpdate()
    {
        if (luaFixedUpdate != null)
        {
            luaFixedUpdate();
        }
    }


    private void LateUpdate()
    {
        if (luaLateUpdate != null)
        {
            luaLateUpdate();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        if (Time.time - LuaBaseBehaviour.lastGCTime > GCInterval)
        {
            lua_Env.Tick();
            LuaBaseBehaviour.lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaOnDisable = null;
        luaLateUpdate = null;
        luaUpdate = null;
        luaStart = null;
        luaOnEnable = null;
        luaFixedUpdate = null;
        scriptEnv.Dispose();

    }
}

