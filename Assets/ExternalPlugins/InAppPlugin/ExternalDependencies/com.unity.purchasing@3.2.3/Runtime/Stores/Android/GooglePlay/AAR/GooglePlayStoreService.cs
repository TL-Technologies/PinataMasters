using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Interfaces;
using UnityEngine.Purchasing.Models;

namespace UnityEngine.Purchasing
{
    class GooglePlayStoreService : IGooglePlayStoreService
    {        
        class RetrieveProductsData
        {
            public ReadOnlyCollection<ProductDefinition> products;
            public Action<List<ProductDescription>> onProductsReceived;
            public Action onRetrieveProductFailed;
        }

        GoogleBillingConnectionState m_GoogleConnectionState = GoogleBillingConnectionState.Disconnected;
        RetrieveProductsData retrieveProductsData;
        
        IGoogleBillingClient m_BillingClient;
        IBillingClientStateListener m_BillingClientStateListener;
        IQuerySkuDetailsService m_QuerySkuDetailsService;
        Queue<ProductDescriptionQuery> m_ProductsToQuery = new Queue<ProductDescriptionQuery>();
        Queue<Action<List<GooglePurchase>>> m_OnPurchaseSucceededQueue = new Queue<Action<List<GooglePurchase>>>();
        IGooglePurchaseService m_GooglePurchaseService;
        IGoogleFinishTransactionService m_GoogleFinishTransactionService;
        IGoogleQueryPurchasesService m_GoogleQueryPurchasesService;
        IGooglePriceChangeService m_GooglePriceChangeService;
        IGoogleLastKnownProductService m_GoogleLastKnownProductService;

        internal GooglePlayStoreService(
            IGoogleBillingClient billingClient,
            IQuerySkuDetailsService querySkuDetailsService,
            IGooglePurchaseService purchaseService,
            IGoogleFinishTransactionService finishTransactionService,
            IGoogleQueryPurchasesService queryPurchasesService,
            IBillingClientStateListener billingClientStateListener,
            IGooglePriceChangeService priceChangeService,
            IGoogleLastKnownProductService lastKnownProductService)
        {
            m_BillingClient = billingClient;
            m_QuerySkuDetailsService = querySkuDetailsService;
            m_GooglePurchaseService = purchaseService;
            m_GoogleFinishTransactionService = finishTransactionService;
            m_GoogleQueryPurchasesService = queryPurchasesService;
            m_GooglePriceChangeService = priceChangeService;
            m_GoogleLastKnownProductService = lastKnownProductService;
            m_BillingClientStateListener = billingClientStateListener;

            InitConnectionWithGooglePlay();
        }

        void InitConnectionWithGooglePlay()
        {
            m_BillingClientStateListener.RegisterOnConnected(OnConnected);
            m_BillingClientStateListener.RegisterOnDisconnected(OnDisconnected);

            StartConnection();
        }

        void StartConnection()
        {
            m_GoogleConnectionState = GoogleBillingConnectionState.Connecting;
            m_BillingClient.StartConnection(m_BillingClientStateListener);
        }

        public void ResumeConnection()
        {
            if (m_GoogleConnectionState == GoogleBillingConnectionState.Disconnected)
            {
                StartConnection();
            }
        }

        void OnConnected()
        {
            m_GoogleConnectionState = GoogleBillingConnectionState.Connected;
            DequeueQueryProducts();
            DequeueFetchPurchases();
            TryReretrieveProducts();
        }

        void DequeueQueryProducts()
        {
            var productsFailedToDequeue = new Queue<ProductDescriptionQuery>();
            var stop = false;

            while (m_ProductsToQuery.Count > 0 && !stop)
            {
                switch (m_GoogleConnectionState)
                {
                    case GoogleBillingConnectionState.Connected:
                    {
                        var productDescriptionQuery = m_ProductsToQuery.Dequeue();
                        m_QuerySkuDetailsService.QueryAsyncSku(productDescriptionQuery.products, productDescriptionQuery.onProductsReceived);
                        break;
                    }
                    case GoogleBillingConnectionState.Disconnected:
                    {
                        var productDescriptionQuery = m_ProductsToQuery.Dequeue();
                        productDescriptionQuery.onRetrieveProductsFailed();

                        productsFailedToDequeue.Enqueue(productDescriptionQuery);
                        break;
                    }
                    case GoogleBillingConnectionState.Connecting:
                    {
                        stop = true;
                        break;
                    }
                    default:
                    {
                        Debug.LogErrorFormat("GooglePlayStoreService state ({0}) unrecognized, cannot process ProductDescriptionQuery",
                            m_GoogleConnectionState);
                        stop = true;
                        break;
                    }
                }
            }

            foreach (var product in productsFailedToDequeue)
            {
                m_ProductsToQuery.Enqueue(product);
            }
        }

        void DequeueFetchPurchases()
        {
            while (m_OnPurchaseSucceededQueue.Count > 0)
            {
                var onPurchaseSucceed = m_OnPurchaseSucceededQueue.Dequeue();
                FetchPurchases(onPurchaseSucceed);
            }
        }

        void OnDisconnected()
        {
            m_GoogleConnectionState = GoogleBillingConnectionState.Disconnected;
            DequeueQueryProducts();
            TryReretrieveProducts();
        }
        
        private void TryReretrieveProducts()
        {
            if (retrieveProductsData != null)
            {
                RetrieveProducts(retrieveProductsData.products, retrieveProductsData.onProductsReceived, retrieveProductsData.onRetrieveProductFailed);
                retrieveProductsData = null;
            }
        }

        public void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products, Action<List<ProductDescription>> onProductsReceived, Action onRetrieveProductFailed)
        {
            if (m_GoogleConnectionState == GoogleBillingConnectionState.Connected)
            {
                m_QuerySkuDetailsService.QueryAsyncSku(products, onProductsReceived);
            }
            else
            {
                if (m_GoogleConnectionState == GoogleBillingConnectionState.Disconnected)
                {
                    if (retrieveProductsData == null)
                    {
                        retrieveProductsData = new RetrieveProductsData()
                        {
                            products = products,
                            onProductsReceived = onProductsReceived,
                            onRetrieveProductFailed = onRetrieveProductFailed
                        };
                        StartConnection();
                        return;
                    }
                    onRetrieveProductFailed();
                }
                m_ProductsToQuery.Enqueue(new ProductDescriptionQuery(products, onProductsReceived, onRetrieveProductFailed));
            }
        }

        public void Purchase(ProductDefinition product)
        {
            Purchase(product, null, GooglePlayProrationMode.k_NullProrationMode);
        }

        public void Purchase(ProductDefinition product, Product oldProduct, int desiredProrationMode)
        {
            m_GoogleLastKnownProductService.SetLastKnownProductId(product.storeSpecificId);
            m_GoogleLastKnownProductService.SetLastKnownProrationMode(desiredProrationMode);
            m_GooglePurchaseService.Purchase(product, oldProduct, desiredProrationMode);
        }

        public void FinishTransaction(ProductDefinition product, string purchaseToken, Action<ProductDefinition, GooglePurchase, GoogleBillingResult, string> onConsume, Action<ProductDefinition, GooglePurchase, GoogleBillingResult> onAcknowledge)
        {
            m_GoogleFinishTransactionService.FinishTransaction(product, purchaseToken, onConsume, onAcknowledge);
        }

        public void FetchPurchases(Action<List<GooglePurchase>> onQueryPurchaseSucceed)
        {
            if (m_GoogleConnectionState == GoogleBillingConnectionState.Connected)
            {
                m_GoogleQueryPurchasesService.QueryPurchases(onQueryPurchaseSucceed);
            }
            else
            {
                m_OnPurchaseSucceededQueue.Enqueue(onQueryPurchaseSucceed);
            }
        }

        public void SetObfuscatedAccountId(string obfuscatedAccountId)
        {
            m_BillingClient.SetObfuscationAccountId(obfuscatedAccountId);
        }

        public void SetObfuscatedProfileId(string obfuscatedProfileId)
        {
            m_BillingClient.SetObfuscationProfileId(obfuscatedProfileId);
        }

        public void EndConnection()
        {
            m_GoogleConnectionState = GoogleBillingConnectionState.Disconnected;
            m_BillingClient.EndConnection();
        }

        public void ConfirmSubscriptionPriceChange(ProductDefinition product, Action<GoogleBillingResult> onPriceChangeAction)
        {
            m_GooglePriceChangeService.PriceChange(product, onPriceChangeAction);
        }
    }
}
