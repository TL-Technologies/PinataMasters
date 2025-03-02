using Modules.General.Obsolete;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


namespace PinataMasters
{
    public class IngameCurrencySpawner : MonoBehaviour
    {
        #region Nested types

        public enum Type
        {
            Speed,
            Time
        }

        #endregion



        #region Fields

        const int MaxIngameCurrencyObjectsCount = 200;

        [FormerlySerializedAs("coinPrefab")]
        [SerializeField] IngameCurrency ingameCurrencyPrefab;
        [SerializeField] float minSpawnAngle = 30f;
        [SerializeField] float maxSpawnAngle = -30f;
        [SerializeField] float spawnImpulse = 1f;
        [SerializeField] Rect spawnRect;


        IngameCurrencySystem ingameCurrencySystem;

        private ObjectPool poolForObject;

        #endregion



        #region Properties

        IngameCurrencySystem IngameCurrencySystem => ingameCurrencySystem ?? (ingameCurrencySystem = IngameCurrencySystem.Prefab.Instance);

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            PoolManager poolManager = PoolManager.Instance;
            poolForObject = poolManager.PoolForObject(ingameCurrencyPrefab.gameObject);
        }

        #endregion



        #region Public methods

        public void SpawnIngameCurrency(uint amount, float price, Type type, float typeValue, bool isAutodestroyEnabled = true, System.Action callback = null)
        {
            StartCoroutine(Spawn(amount, price, type, typeValue, isAutodestroyEnabled, callback));
        }

        #endregion



        #region Private methods

        IEnumerator Spawn(uint amount, float price, Type type, float typeValue, bool isAutodestroyEnabled, System.Action callback)
        {
            amount = (uint)Mathf.Min(amount, (type == Type.Speed) ? MaxIngameCurrencyObjectsCount : Mathf.RoundToInt(MaxIngameCurrencyObjectsCount * typeValue));
            float speed = (type == Type.Speed) ? typeValue : amount / typeValue;
            float naminal = price / amount;
            float currencyObjects = 0f;
            float stashCurrencyObjects = 0f;

            while (amount > 0f)
            {
                yield return null;

                currencyObjects = speed * (Time.deltaTime + stashCurrencyObjects);

                if (currencyObjects < 1f)
                {
                    stashCurrencyObjects += Time.deltaTime;
                    continue;
                }
                stashCurrencyObjects = 0f;

                currencyObjects = amount < currencyObjects ? amount : currencyObjects;
                amount -= (uint)Mathf.Round(currencyObjects);

                for (int i = 0; i < (uint)Mathf.Round(currencyObjects); i++)
                {
                    poolForObject.Pop((ingameCurrencyObject) =>
                    {
                        ingameCurrencyObject.transform.position =
                           new Vector3(transform.position.x + spawnRect.position.x + Random.Range(0f, spawnRect.width),
                                       transform.position.y + spawnRect.position.y + Random.Range(0f, spawnRect.height),
                                       ingameCurrencyPrefab.transform.position.z);

                        Vector3 direction = Quaternion.AngleAxis(Random.Range(minSpawnAngle, maxSpawnAngle), Vector3.forward) * Vector3.up;

                        IngameCurrency currency = ingameCurrencyObject.GetComponent<IngameCurrency>();
                        currency.Init(IngameCurrencySystem, naminal, direction.normalized * spawnImpulse, isAutodestroyEnabled);

                        //ingameCurrencyObject.GetComponent<Coin>().Init(naminal, direction.normalized * spawnImpulse);
                    });
                }


            }

            callback?.Invoke();
        }

        #endregion



        #region Editor methods

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector2 pinataPosition = transform.position;
            Gizmos.DrawLine(pinataPosition + new Vector2(spawnRect.xMin, spawnRect.yMin), pinataPosition + new Vector2(spawnRect.xMax, spawnRect.yMin));
            Gizmos.DrawLine(pinataPosition + new Vector2(spawnRect.xMax, spawnRect.yMin), pinataPosition + new Vector2(spawnRect.xMax, spawnRect.yMax));
            Gizmos.DrawLine(pinataPosition + new Vector2(spawnRect.xMax, spawnRect.yMax), pinataPosition + new Vector2(spawnRect.xMin, spawnRect.yMax));
            Gizmos.DrawLine(pinataPosition + new Vector2(spawnRect.xMin, spawnRect.yMax), pinataPosition + new Vector2(spawnRect.xMin, spawnRect.yMin));


            Vector2 leftPoint = new Vector2(pinataPosition.x + spawnRect.xMin, pinataPosition.y + spawnRect.yMin);
            Gizmos.DrawLine(leftPoint, leftPoint + (Vector2)(Quaternion.AngleAxis(minSpawnAngle, Vector3.forward) * Vector3.up * 2f));

            Vector2 rightPoint = new Vector2(pinataPosition.x + spawnRect.xMax, pinataPosition.y + spawnRect.yMin);
            Gizmos.DrawLine(rightPoint, rightPoint + (Vector2)(Quaternion.AngleAxis(maxSpawnAngle, Vector3.forward) * Vector3.up * 2f));
        }

        #endregion
    }
}
