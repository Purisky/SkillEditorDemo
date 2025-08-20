using Leopotam.EcsLite;
using SkillEditorDemo.Utility;
using System;
using System.Linq;

namespace SkillEditorDemo.Model
{
    public class Unit
    {
        protected const TrigType Aft = STrigType.Aft;
        public EcsPackedEntity Entity;
        public string ID;
        public string Name;
        public static Unit Get(int entity) => entity.Get<UnitCmp>().Unit;
        public static Unit Get(EcsPackedEntity entity)
        {
            if (entity.Gen == EcsWorld.Inst.GetEntityGen(entity.Id))
            {
                return Get(entity.Id);
            }
            return null;
        }
        public UnitData Data => IData<UnitData>.Get(ID);
        public StatHandler StatHandler;
        public BuffHandler BuffHandler;
        public SkillHandler SkillHandler;


        public HealthPoint HP;
        public ShieldPoint SP;
        public Mana Mana;

        public bool Dead;
        public int DeadTick = -1;
        public int Faction;

        public Unit(int entity,string id, int faction)
        {
            Entity = EcsWorld.Inst.PackEntity(entity);
            Faction = faction;
            ID = id;
            StatHandler = new StatHandler(this);
            BuffHandler = new BuffHandler(this);
            SkillHandler = new SkillHandler(this);
            foreach (var stat in Data.Stats)
            {
                if (stat.Key.IsStat())
                {
                    StatHandler.AddStat(stat.Key, stat.Value);
                }
                else
                {
                    StatHandler[stat.Key] = stat.Value;
                }
            }
            if (Data.Buffs != null && Data.Buffs.Any())
            {
                for (int i = 0; i < Data.Buffs.Count; i++)
                {
                    BuffInfo info = Data.Buffs[i];
                    BuffHandler.InitAdd(info.ID, info.Level, info.Degree,info.Param );
                }
            }
            if (Data.Skills != null && Data.Skills.Any())
            {
                for (int i = 0; i < Data.Skills.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Data.Skills[i].ID))
                    {
                        SkillHandler.AddSkill(i, Data.Skills[i].ID, Data.Skills[i].Level);
                    }
                }
            }
            HP = new HealthPoint(this);
            SP = new ShieldPoint(this);
            Mana = new Mana(this);
        }




        public void TakeDmg(int trigCount, CombatCache cache, Unit from)
        {
            int old = HP.Value;
            //Dmged
            cache[CombatCacheType.TotalDmg]*= StatHandler.GetDmgMod(cache.DmgType, true);
            DmgTrig();
            Debug.Log($"{this} TakeDmg HP[{old} -> {HP.Value}]");
            if (HP.Value <= 0)
            {
                TryDie(trigCount, cache, from);
            }
            void DmgTrig()
            {
                if (!Trig(trigCount, TrigType.Dmg.ed(), cache, from)) { return; }
                {
                    //Def
                    float def = MathF.Max(StatHandler[StatType.Def] - from?.StatHandler[StatType.DefIgnore] ?? 0, 0);
                    float defMod = 100 / (100 + def);
                    cache[CombatCacheType.DefDmg] = cache[CombatCacheType.TotalDmg] * (1 - defMod);
                    cache[CombatCacheType.LeftDmg] = cache[CombatCacheType.TotalDmg] * defMod;
                    //ShieldPoint
                    if (!SP.TrigCost(trigCount, (int)cache[CombatCacheType.LeftDmg], cache, from)) { return; }
                    //HealthPoint
                    if (!HP.TrigCost(trigCount, (int)cache[CombatCacheType.LeftDmg], cache, from)) { return; }
                }
                if (!Trig(trigCount, Aft | TrigType.Dmg.ed(), cache, from)) { return; }
            }
        }


        public void TryDie(int trigCount, CombatCache cache, Unit from)
        {
            if (!Trig(trigCount, TrigType.Kill.ed(), cache, from))//Stop die
            {
                HP.Value = 1;
                return;
            }
            Die();
            Trig(trigCount, Aft | TrigType.Kill.ed(), cache, from);
        }
        public void Die()
        {
            Dead = true;
            DeadTick = Time.Tick;
        }

        public bool Trig(int trigCount, TrigType type, CombatCache cache, Unit from = null) => BuffHandler.Trig(trigCount, type, cache, from);
        public bool HitCheck(int trigCount, Unit from, CombatCache cache)//Dodge/Accurate/DodgeIgnore
        {
            if (!cache.Dodge_able) { return true; }
            float dodge = StatHandler[StatType.Dodge];
            float accurate = from?.StatHandler[StatType.Accurate] ?? 1;
            float dodgeIgnore = from?.StatHandler[StatType.DodgeIgnore] ?? 0;
            accurate *= 1 - (dodge * (1 - dodgeIgnore));
            if (accurate < FixRandom.Get01())//Dodge
            {
                if (!Trig(trigCount, TrigType.Dodge, cache, from)) { return true; }//Deny dodge check result
                Trig(trigCount, Aft | TrigType.Dodge, cache, from);
                return false;
            }
            return true;
        }
        public bool CritCheck(int trigCount, Unit from, CombatCache cache)
        {
            if (!cache.Crit_able) { return true; }
            float critChance = from?.StatHandler[StatType.Crit] ?? 0;
            float critResistance = StatHandler[StatType.CritResist];
            critChance -= critResistance;
            if (critChance < FixRandom.Get01()) { return true; }
            if (!Trig(trigCount, TrigType.Crit.ed(), cache, from)) { return false; }
            float critDmgMod = from?.StatHandler[StatType.CritDmgMod] ?? Stat.GetDefault(StatType.CritDmgMod);
            critDmgMod = CountCritDmg(critDmgMod, StatHandler[StatType.CritDmgModResist]);
            cache[CombatCacheType.TotalDmg] *= critDmgMod;
            cache.IsCrit = true;
            if (!Trig(trigCount, Aft | TrigType.Crit.ed(), cache, from)) { return false; }
            return true;
            static float CountCritDmg(float critDmg, float resist) => Math.Max(1, critDmg - resist);
        }

        public void Update()
        {
            HP?.Update();
            SP?.Update();
            Mana?.Update();
            SkillHandler.Update();
        }




        public override string ToString()
        {
            string name = string.IsNullOrEmpty(Name) ? "Unit" : Name;
            return $"{name}({ID}[{Entity.Id}])";
        }

    }
}
