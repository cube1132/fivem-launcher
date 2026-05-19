using System;
using System.Collections.Generic;
using IdleTapGame.Economy;
using IdleTapGame.Gameplay;
using IdleTapGame.Save;
using UnityEngine;

namespace IdleTapGame.Core
{
    /// <summary>
    /// Owns the economy, drives the passive-income tick, persistence and
    /// offline progress. Survives scene loads as a singleton.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public CurrencyManager Currency { get; private set; }
        public TapController Tap { get; private set; }
        public IReadOnlyList<Generator> Generators => _generators;
        public OfflineResult PendingOffline { get; private set; }

        /// <summary>Raised when something the UI cares about changed.</summary>
        public event Action StateChanged;

        private readonly List<Generator> _generators = new List<Generator>();
        private const float AutosaveInterval = 15f;
        private float _autosaveTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Currency = new CurrencyManager();
            Tap = new TapController();
            _generators.AddRange(GeneratorCatalog.CreateDefault());

            LoadGame();
        }

        private void Update()
        {
            double perSecond = GetTotalProductionPerSecond();
            if (perSecond > 0d)
                Currency.Add(perSecond * Time.deltaTime);

            _autosaveTimer += Time.deltaTime;
            if (_autosaveTimer >= AutosaveInterval)
            {
                _autosaveTimer = 0f;
                SaveGame();
            }
        }

        public double GetTotalProductionPerSecond()
        {
            double total = 0d;
            for (int i = 0; i < _generators.Count; i++)
                total += _generators[i].ProductionPerSecond;
            return total;
        }

        public void HandleTap()
        {
            Currency.Add(Tap.TapPower);
            StateChanged?.Invoke();
        }

        public bool TryBuyGenerator(Generator generator)
        {
            if (generator == null) return false;
            if (!Currency.TrySpend(generator.NextCost)) return false;

            generator.Owned++;
            StateChanged?.Invoke();
            return true;
        }

        public bool TryUpgradeTap()
        {
            if (!Currency.TrySpend(Tap.UpgradeCost)) return false;

            Tap.Upgrade();
            StateChanged?.Invoke();
            return true;
        }

        public void AcknowledgeOffline()
        {
            PendingOffline = default;
        }

        private void LoadGame()
        {
            SaveData data = SaveSystem.Load();
            if (data == null)
            {
                Currency.SetGold(0d);
                return;
            }

            Currency.SetGold(data.Gold);
            Tap.SetLevel(data.TapLevel);

            if (data.Generators != null)
            {
                foreach (GeneratorSave gs in data.Generators)
                {
                    Generator g = _generators.Find(x => x.Id == gs.Id);
                    if (g != null)
                        g.Owned = gs.Owned;
                }
            }

            PendingOffline = OfflineProgress.Calculate(
                data.LastSaveUnixSeconds, GetTotalProductionPerSecond());

            if (PendingOffline.HasEarnings)
                Currency.Add(PendingOffline.Earned);
        }

        private void SaveGame()
        {
            var data = new SaveData
            {
                Gold = Currency.Gold,
                TapLevel = Tap.Level
            };

            foreach (Generator g in _generators)
                data.Generators.Add(new GeneratorSave { Id = g.Id, Owned = g.Owned });

            SaveSystem.Save(data);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
