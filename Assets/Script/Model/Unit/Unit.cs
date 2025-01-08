using SkillEditorDemo.Utility;
using System;

namespace SkillEditorDemo.Model
{
    public class Unit : IIndexData<Unit, UnitData>
    {
        protected const TrigType Aft = STrigType.Aft;
        public int Index { get; set; }
        public string ID { get; set; }
        public static Unit Get(int id) => IIndex<Unit>.Get(id);
        public UnitData Data => IData<UnitData>.Get(ID);
        public StatHandler StatHandler;
        public BuffHandler BuffHandler;
        public SkillHandler SkillHandler;


        public HealthPoint HP;
        public ShieldPoint SP;
        public Mana Mana;

        public bool Dead;
        public int DeadTick = -1;


        public void TakeDmg(int trigCount, CombatCache cache, Unit from)
        {
            //Dmged
            cache[CombatCacheType.TotalDmg]*= StatHandler.GetDmgMod(cache.DmgType, true);
            DmgTrig();
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

        }






    }
}
