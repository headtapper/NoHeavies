using System;
using System.Linq;
using Oxide.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("NoHeavies", "headtapper", "1.0.0")]
    [Description("Block heavy scientist spawns at oilrigs.")]
    public class NoHeavies : RustPlugin
    {

        #region Configuration

        private PluginConfig config;

        private class PluginConfig
        {
            [JsonProperty(PropertyName = "Block Small Oilrig Heavies")]
            public bool BlockSmallOilrigHeavies { get; set; }

            [JsonProperty(PropertyName = "Block Large Oilrig Heavies")]
            public bool BlockLargeOilrigHeavies { get; set; }
        }

        private PluginConfig GetDefaultConfigValues()
        {
            return new PluginConfig
            {
                BlockSmallOilrigHeavies = true,
                BlockLargeOilrigHeavies = true
            };
        }

        private void SaveConfig() => Config.WriteObject(config, true);

        protected override void LoadDefaultConfig()
        {
            config = GetDefaultConfigValues();
            SaveConfig();
        }

        protected override void LoadConfig()
        {
            try
            {
                base.LoadConfig();
                config = Config.ReadObject<PluginConfig>();
                SaveConfig();
            }
            catch (Exception ex)
            {
                LoadDefaultConfig();
            }
        }

        #endregion

        #region Hooks

        private void OnEntitySpawned(BaseEntity entity)
        {
            if (!config.BlockSmallOilrigHeavies && !config.BlockLargeOilrigHeavies) return;
            if (entity == null) return;
            if (IsHeavyScientist(entity))
            {
                timer.Once((float)10.0f, () =>
                {
                    if (entity == null || entity.IsDestroyed) return;
                    if (config.BlockSmallOilrigHeavies && IsEntityNearMonument(entity, "oilrig_2"))
                        entity.Kill();
                    if (config.BlockLargeOilrigHeavies && IsEntityNearMonument(entity, "oilrig_1"))
                        entity.Kill();
                });
            }  
        }

        #endregion

        #region Functions

        private bool IsEntityNearMonument(BaseEntity entity, string monumentName)
        {
            if (entity == null) return false;
            MonumentInfo monument = TerrainMeta.Path.Monuments.Find(m => m.name.Contains(monumentName, StringComparison.OrdinalIgnoreCase));
            if (monument == null) return false;
            return Vector3.Distance(entity.transform.position, monument.transform.position) < 600f;
        }

        private bool IsHeavyScientist(BaseEntity entity) => entity.ShortPrefabName.Contains("scientistnpc_heavy");

        #endregion
    }
}
