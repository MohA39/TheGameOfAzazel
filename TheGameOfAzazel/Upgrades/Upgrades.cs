using System;

namespace TheGameOfAzazel
{
    public static class Upgrades
    {

        public static Upgrade Health, Shield, Light, Speed, Dash, FireRate, FirePower, Luck, StartAllies, CoinMagnet;

        public static void Initalize()
        {
            Health = new Upgrade()
            {
                Name = "health",
                Level = SaveManager.GetValueInt("health"),
                InitialValue = 100,
                Unit = "hp",
                OffsetterFunction = (level) => { return GetHealthOffset(level); }
            };
            Health.Value = Health.InitialValue + Health.OffsetterFunction(null);
            // Health.NextValue = Health.InitialValue + Health.OffsetterFunction(Health.Level + 1);

            Shield = new Upgrade()
            {
                Name = "shield",
                Level = SaveManager.GetValueInt("shield"),
                InitialValue = 0,
                OffsetterFunction = (level) => { return GetShield(level); }
            };
            Shield.Value = Shield.OffsetterFunction(null);
            // Shield.NextValue = GetShield(Shield.Level + 1);

            Light = new Upgrade()
            {
                Name = "light",
                Level = SaveManager.GetValueInt("light"),
                InitialValue = 100,
                OffsetterFunction = (level) => { return GetLightOffset(level); }
            };
            Light.Value = Light.InitialValue + Light.OffsetterFunction(null);
            // Light.NextValue = Light.InitialValue + GetLightOffset(Light.Level + 1);


            Speed = new Upgrade()
            {
                Name = "speed",
                Level = SaveManager.GetValueInt("speed"),
                InitialValue = 50,
                Unit = "m/s",
                OffsetterFunction = (level) => { return GetSpeedOffset(level); }
            };
            Speed.Value = Speed.InitialValue + Speed.OffsetterFunction(null);
            //Speed.NextValue = Speed.InitialValue + GetSpeedOffset(Speed.Level + 1);

            Dash = new Upgrade()
            {
                Name = "dash",
                Level = SaveManager.GetValueInt("dash"),
                InitialValue = 100,
                Unit = "m",
                OffsetterFunction = (level) => { return GetDashOffset(level); }
            };
            Dash.Value = Dash.InitialValue + Dash.OffsetterFunction(null);
            //Dash.NextValue = Dash.InitialValue + GetDashOffset(Dash.Level + 1);

            FireRate = new Upgrade()
            {
                Name = "firerate",
                Level = SaveManager.GetValueInt("firerate"),
                InitialValue = 1,
                Unit = "rps",
                OffsetterFunction = (level) => { return GetFireRateOffset(level); }
            };
            FireRate.Value = FireRate.InitialValue + FireRate.OffsetterFunction(null);
            // FireRate.NextValue = FireRate.InitialValue + GetFireRateOffset(FireRate.Level + 1);

            FirePower = new Upgrade()
            {
                Name = "firepower",
                Level = SaveManager.GetValueInt("firepower"),
                InitialValue = 10,
                Unit = "dmg",
                OffsetterFunction = (level) => { return GetFirePowerOffset(level); }
            };
            FirePower.Value = FirePower.InitialValue + FirePower.OffsetterFunction(null);
            //FirePower.NextValue = FirePower.InitialValue + GetFirePowerOffset(FirePower.Level + 1);

            Luck = new Upgrade()
            {
                Name = "luck",
                Level = SaveManager.GetValueInt("luck"),
                InitialValue = 0,
                Unit = "%",
                OffsetterFunction = (level) => { return GetLuckOffset(level); }
            };
            Luck.Value = Luck.InitialValue + Luck.OffsetterFunction(null);
            //Luck.NextValue = Luck.InitialValue + GetLuckOffset(Luck.Level + 1);
            StartAllies = new Upgrade()
            {
                Name = "startallies",
                Level = SaveManager.GetValueInt("startallies"),
                InitialValue = 0,
                OffsetterFunction = (level) => { return GetStartingAlliesOffset(level); }
            };
            StartAllies.Value = StartAllies.InitialValue + StartAllies.OffsetterFunction(null);
            //StartAllies.NextValue = StartAllies.InitialValue + GetStartingAlliesOffset(StartAllies.Level + 1);

            CoinMagnet = new Upgrade()
            {
                Name = "coinmagnet",
                Level = SaveManager.GetValueInt("coinmagnet"),
                InitialValue = 0,
                OffsetterFunction = (level) => { return GetCoinMagnetOffset(level); }
            };
            CoinMagnet.Value = CoinMagnet.InitialValue + CoinMagnet.OffsetterFunction(null);
            //CoinMagnet.NextValue = CoinMagnet.InitialValue + GetCoinMagnetOffset(StartAllies.Level + 1);
        }

        public static float GetHealthOffset(int? level = null)
        {
            float Multiplyer = 10;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multiplyer;
            }
            return (Health.Level - 1) * Multiplyer;
        }

        public static float GetShield(int? level = null)
        {
            float Multipler = 1f;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multipler;
            }
            return (Shield.Level - 1) * Multipler;
        }
        public static float GetLightOffset(int? level = null)
        {
            float Multipler = 15;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multipler;
            }
            return (Light.Level - 1) * Multipler;
        }

        public static float GetSpeedOffset(int? level = null)
        {
            float Multiplyer = 3;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multiplyer;
            }
            return (Speed.Level - 1) * Multiplyer;
        }

        public static float GetDashOffset(int? level = null)
        {
            float Multiplyer = 15;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multiplyer;
            }
            return (Dash.Level - 1) * Multiplyer;
        }

        public static float GetFireRateOffset(int? level = null)
        {
            if (level.HasValue)
            {
                return (float)-Math.Abs((FireRate.InitialValue - Math.Pow(0.93, (level.Value - 1))));
            }

            return (float)-Math.Abs((FireRate.InitialValue - Math.Pow(0.93, (FireRate.Level - 1))));
        }

        public static float GetFirePowerOffset(int? level = null)
        {
            float Multiplyer = 1f;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multiplyer;
            }
            return (FirePower.Level - 1) * Multiplyer;
        }

        public static float GetLuckOffset(int? level = null)
        {
            float Multiplyer = 6f;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multiplyer;
            }
            return (Luck.Level - 1) * Multiplyer;
        }

        public static float GetStartingAlliesOffset(int? level = null)
        {
            float Multiplyer = 1f;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multiplyer;
            }
            return (StartAllies.Level - 1) * Multiplyer;
        }

        public static float GetCoinMagnetOffset(int? level = null)
        {
            float Multiplyer = 25f;
            if (level.HasValue)
            {
                return (level.Value - 1) * Multiplyer;
            }
            return (CoinMagnet.Level - 1) * Multiplyer;
        }

        public static void LevelUp(Upgrade upgrade)
        {
            upgrade.Level++;
            SaveManager.SetValue(upgrade.Name, upgrade.Level);
            upgrade.Value = upgrade.InitialValue + upgrade.OffsetterFunction(null);
        }
    }
}
