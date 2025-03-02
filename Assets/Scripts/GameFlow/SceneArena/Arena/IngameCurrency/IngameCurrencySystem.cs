using UnityEngine;
using System.Collections.Generic;


namespace PinataMasters
{
    public class IngameCurrencySystem : MonoBehaviour
    {
        #region Nested types

        public struct InitializationConfig
        {
            public Shooter collector;
            public Vector3 gravity;
            public Vector3 borders;
        }

        #endregion



        #region Fields

        public static readonly ResourceGameObject<IngameCurrencySystem> Prefab = new ResourceGameObject<IngameCurrencySystem>("Game/Game/IngameCurrency/IngameCurrencySystem");

        [SerializeField] IngameCurrencyCooldownsHandler.Settings[] cooldownsHandlersSettings;

        InitializationConfig configuration;

        HashSet<IngameCurrency> ingameCurrencies = new HashSet<IngameCurrency>();

        HashSet<IngameCurrency> addedIngameCurrencies = new HashSet<IngameCurrency>();
        HashSet<IngameCurrency> removedIngameCurrencies = new HashSet<IngameCurrency>();

        Dictionary<IngameCurrencyType, IngameCurrencyCooldownsHandler> cooldownHandlersByType = new Dictionary<IngameCurrencyType, IngameCurrencyCooldownsHandler>();
        Dictionary<IngameCurrencyType, float> currenciesPricesByType = new Dictionary<IngameCurrencyType, float>();

        #endregion



        #region Properties

        public bool IsVibrationAllow { get; set; }

        #endregion



        #region Unity lifecycle

        void Awake()
        {
            for (int idx = 0; idx < cooldownsHandlersSettings.Length; idx++)
            {
                cooldownHandlersByType.Add(cooldownsHandlersSettings[idx].currencyType, new IngameCurrencyCooldownsHandler(cooldownsHandlersSettings[idx]));
            }
        }


        void Update()
        {
            foreach (IngameCurrencyCooldownsHandler cooldownHandler in cooldownHandlersByType.Values)
            {
                cooldownHandler.CustomUpdate(Time.deltaTime);
            }
        }


        void FixedUpdate()
        {
            foreach (IngameCurrency currency in ingameCurrencies)
            {
                currency.UpdateTransform(Time.fixedDeltaTime, configuration.borders, configuration.gravity);

                if (configuration.collector != null)
                {
                    configuration.collector.Rect.center = configuration.collector.transform.position + Vector3.Scale(configuration.collector.Offset, configuration.collector.transform.localScale);
                }

                if (configuration.collector != null && configuration.collector.Rect.Overlaps(currency.Rect))
                {
                    currency.Collision();
                    configuration.collector.Collision(this, currency);
                }
            }

            ingameCurrencies.ExceptWith(removedIngameCurrencies);
            removedIngameCurrencies.Clear();

            ingameCurrencies.UnionWith(addedIngameCurrencies);
            addedIngameCurrencies.Clear();
        }

        #endregion



        #region Public methods

        public void Configure(InitializationConfig configuration)
        {
            this.configuration = configuration;
        }


        public void AddIngameCurrency(IngameCurrency currency)
        {
            addedIngameCurrencies.Add(currency);
        }


        public void RemoveIngameCurrency(IngameCurrency currency)
        {
            removedIngameCurrencies.Add(currency);
        }


        public void TrySpawnCollectText(IngameCurrency currency, Vector3 position)
        {
            cooldownHandlersByType[currency.CurrencyType].TrySpawnIngameOfferText(() =>
            {
                float currencyPrice = currency.Price;

                if (Mathf.Approximately(currencyPrice, 0f))
                {
                    if (currenciesPricesByType.ContainsKey(currency.CurrencyType))
                    {
                        currencyPrice = currenciesPricesByType[currency.CurrencyType];
                    }
                }
                else
                {
                    if (!currenciesPricesByType.ContainsKey(currency.CurrencyType))
                    {
                        currenciesPricesByType.Add(currency.CurrencyType, currencyPrice);
                    }
                    else
                    {
                        currenciesPricesByType[currency.CurrencyType] = currencyPrice;
                    }
                }

                currency.SpawnCollectText(currencyPrice, position);
            });
        }


        public void TryPlaySound(IngameCurrency currency)
        {
            cooldownHandlersByType[currency.CurrencyType].TryPlayIngameOfferSound(() =>
            {
                currency.PlayCollectSound();
            });
        }


        public void TryPlayVibrationEffect(IngameCurrency currency)
        {
            cooldownHandlersByType[currency.CurrencyType].TryPlayIngameOfferVibration(() =>
            {
                currency.PlayVibrationEffect();
            });
        }


        public float GetIngameCurrencyPrice(IngameCurrencyType currencyType)
        {
            float price = 0f;

            foreach (IngameCurrency ingameCurrency in ingameCurrencies)
            {
                if (ingameCurrency.CurrencyType == currencyType)
                {
                    price += ingameCurrency.Price;
                }
            }

            return price;
        }


        public void ResetIngameCurrenciesPrice()
        {
            foreach (IngameCurrency ingameCurrency in ingameCurrencies)
            {
                ingameCurrency.Price = 0f;
            }
        }


        public void ResetIngameCurrencyPrice(IngameCurrencyType currencyType)
        {
            foreach (IngameCurrency ingameCurrency in ingameCurrencies)
            {
                if (ingameCurrency.CurrencyType == currencyType)
                {
                    ingameCurrency.Price = 0f;
                }
            }
        }


        public void ForceStartAutodestroyForIngameCurrency(IngameCurrencyType currencyType)
        {
            foreach (IngameCurrency ingameCurrency in ingameCurrencies)
            {
                if (ingameCurrency.CurrencyType == currencyType)
                {
                    ingameCurrency.ForceStartAutodestroy();
                }
            }
        }

        #endregion
    }
}
